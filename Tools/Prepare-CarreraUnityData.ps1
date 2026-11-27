[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ZipPath,
    [string]$Destination = (Join-Path (Get-Location) 'CarreraHybrid-Reconstruction'),
    [switch]$SkipExtraction
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$ZipPath = (Resolve-Path -LiteralPath $ZipPath).Path
$Destination = [System.IO.Path]::GetFullPath($Destination)
$RawRoot = Join-Path $Destination 'raw'

if (-not $SkipExtraction) {
    if (Test-Path -LiteralPath $RawRoot) {
        throw "Raw extraction folder already exists: $RawRoot. Choose a new Destination or move the existing folder first."
    }
    New-Item -ItemType Directory -Path $RawRoot -Force | Out-Null
    Expand-Archive -LiteralPath $ZipPath -DestinationPath $RawRoot
}
elseif (-not (Test-Path -LiteralPath $RawRoot)) {
    throw "-SkipExtraction was specified, but the raw folder does not exist: $RawRoot"
}

# Reassemble Unity split files such as sharedassets0.assets.split0.
# Original split parts are preserved.
$splitFiles = Get-ChildItem -LiteralPath $RawRoot -Recurse -File | Where-Object {
    $_.Name -match '^(?<base>.+)\.split(?<index>\d+)$'
}
$groups = $splitFiles | Group-Object {
    $match = [regex]::Match($_.Name, '^(?<base>.+)\.split(?<index>\d+)$')
    Join-Path $_.DirectoryName $match.Groups['base'].Value
}

foreach ($group in $groups) {
    $outputPath = $group.Name
    $parts = @($group.Group | Sort-Object {
        [int]([regex]::Match($_.Name, '\.split(?<index>\d+)$').Groups['index'].Value)
    })
    if (Test-Path -LiteralPath $outputPath) {
        throw "Refusing to overwrite existing assembled file: $outputPath"
    }

    $expectedIndex = 0
    $outputStream = [System.IO.File]::Open($outputPath, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)
    try {
        $buffer = New-Object byte[] (1024 * 1024)
        foreach ($part in $parts) {
            $match = [regex]::Match($part.Name, '\.split(?<index>\d+)$')
            $index = [int]$match.Groups['index'].Value
            if ($index -ne $expectedIndex) {
                throw "Split sequence gap for '$outputPath': expected part $expectedIndex but found $index."
            }
            $inputStream = [System.IO.File]::OpenRead($part.FullName)
            try {
                while (($read = $inputStream.Read($buffer, 0, $buffer.Length)) -gt 0) {
                    $outputStream.Write($buffer, 0, $read)
                }
            }
            finally { $inputStream.Dispose() }
            $expectedIndex++
        }
    }
    catch {
        $outputStream.Dispose()
        if (Test-Path -LiteralPath $outputPath) { Remove-Item -LiteralPath $outputPath -Force }
        throw
    }
    finally {
        if ($outputStream) { $outputStream.Dispose() }
    }
    Write-Host ("Assembled {0} from {1} parts ({2:N0} bytes)" -f $outputPath, $expectedIndex, (Get-Item -LiteralPath $outputPath).Length)
}

$dataRoot = Join-Path $RawRoot 'base-main\resources\assets\bin\Data'
if (-not (Test-Path -LiteralPath (Join-Path $dataRoot 'globalgamemanagers'))) {
    throw "Unity globalgamemanagers was not found at the expected path: $dataRoot"
}

$hashManifest = Join-Path $Destination 'SHA256SUMS.txt'
Get-ChildItem -LiteralPath $RawRoot -Recurse -File | ForEach-Object {
    $hash = Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256
    '{0} *{1}' -f $hash.Hash, $_.FullName.Substring($RawRoot.Length + 1)
} | Set-Content -LiteralPath $hashManifest -Encoding UTF8

Write-Host ''
Write-Host 'Preparation complete.' -ForegroundColor Green
Write-Host "Unity Data folder: $dataRoot"
Write-Host "Addressables folder: $(Join-Path $RawRoot 'base-main\resources\assets\aa\Android')"
Write-Host "Hash manifest: $hashManifest"
Write-Host ''
Write-Host 'Next: open the Unity Data folder and Addressables bundles with AssetRipper. Keep the raw split files unchanged.'
