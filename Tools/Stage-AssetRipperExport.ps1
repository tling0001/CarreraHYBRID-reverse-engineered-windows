param(
    [Parameter(Mandatory = $true)]
    [string]$AssetRipperExport,
    [string]$ProjectRoot = $PSScriptRoot,
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'
$sourceAssets = Join-Path (Resolve-Path $AssetRipperExport) 'Assets'
$projectAssets = Join-Path (Resolve-Path $ProjectRoot) 'Assets'
$destination = Join-Path $projectAssets 'RecoveredAndroid'

if (-not (Test-Path -LiteralPath $sourceAssets -PathType Container)) {
    throw "AssetRipper export must contain an Assets folder: $sourceAssets"
}

if (Test-Path -LiteralPath $destination) {
    throw "Destination already exists: $destination. Move it aside or choose a clean project copy; this script never overwrites staged assets."
}

$files = Get-ChildItem -LiteralPath $sourceAssets -Force -Recurse -File
if ($files.Count -eq 0) { throw 'The AssetRipper Assets folder is empty.' }

Write-Host "Source:      $sourceAssets"
Write-Host "Destination: $destination"
Write-Host "Files:       $($files.Count)"

if ($WhatIf) {
    Write-Host 'WhatIf: no files copied.'
    exit 0
}

New-Item -ItemType Directory -Path $destination | Out-Null
try {
    Get-ChildItem -LiteralPath $sourceAssets -Force | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination $destination -Recurse -Force:$false
    }
} catch {
    Remove-Item -LiteralPath $destination -Recurse -Force -ErrorAction SilentlyContinue
    throw
}

$copied = (Get-ChildItem -LiteralPath $destination -Force -Recurse -File).Count
if ($copied -ne $files.Count) {
    throw "Copy verification failed. Expected $($files.Count) files, found $copied."
}

Write-Host "Staged $copied files under Assets/RecoveredAndroid."
Write-Host 'Open the project in Unity 6000.0.84f1 and wait for import to finish.'
Write-Host 'Then run: Carrera Hybrid > Recovery > Generate Asset Inventory.'
