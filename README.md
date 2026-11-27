# Carrera Hybrid — Windows reconstruction

Unity `6000.0.84f1` Windows x64 reconstruction based on the Android APK-derived archive (`com.sturmkind.carrerahybrid`, v1.2.6).

This revision contains a newly authored UI navigation prototype for Startup, Login, registration, password recovery, Menu, Garage, Career, Coaching, Settings, Lobby, Race, Pause and Race Result. It includes local practice-session controls and a Windows x64 GitHub Actions build workflow.

**Fidelity note:** the UI is an approximation informed by decompiled type/scene metadata, not the original serialized Android UI. The original Unity project and C# sources were not present in the APK archive. Online account services, multiplayer, real-car BLE control, firmware update, original 3D assets and original race physics are not represented as working features.

## Open and build

1. Install Unity Hub and Unity `6000.0.84f1` with Windows Build Support (IL2CPP).
2. Open this directory in Unity Hub.
3. Open `Assets/Scenes/Startup.unity` and press Play.
4. Build for `PC, Mac & Linux Standalone` → `Windows` → `x86_64`.

## Controls

- `W` / `Up Arrow`: accelerate in the local practice prototype.
- `S` / `Down Arrow`: brake.
- `Esc`: pause.
- In-race Pause and Finish controls are also available on screen.

## GitHub Actions and Unity Personal

- `.github/workflows/build-windows.yml` builds a Windows x64 player.
- Unity Personal is activated through Unity Hub. For GameCI, copy the Hub-created `Unity_lic.ulf` contents into the `UNITY_LICENSE` GitHub secret and configure `UNITY_EMAIL` and `UNITY_PASSWORD`.
- The ALF request workflow is legacy and is **not** the Personal activation method.

Follow [Documentation/UNITY-LICENSE-SETUP.md](Documentation/UNITY-LICENSE-SETUP.md). Do not commit license files or credentials.

## Asset recovery

1. Export the Android Unity data with AssetRipper into a separate project folder.
2. Stage the exported `Assets` directory without overwriting existing work:
   `powershell -ExecutionPolicy Bypass -File .\Tools\Stage-AssetRipperExport.ps1 -AssetRipperExport "C:\Path\To\AssetRipperExport"`
3. Open Unity and wait for import.
4. Run **Carrera Hybrid → Recovery → Generate Asset Inventory**.

See [Documentation/ASSET-RECOVERY.md](Documentation/ASSET-RECOVERY.md) for details. The staging script refuses to overwrite an existing RecoveredAndroid folder.

## Screen reconstruction order

Startup → Login → Menu/Garage → Lobby → Race → Race Result. Each screen should be completed with its reachable dialogs, loading/error states and navigation paths before moving on.

See [Documentation/STATUS.md](Documentation/STATUS.md) for verified findings, limitations and remaining fidelity work.
