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

## GitHub Actions

- `.github/workflows/build-windows.yml` builds a Windows x64 player.
- `.github/workflows/unity-activation-file.yml` is a manually triggered legacy ALF request workflow. Its artifact is a request file, not a license.

Read [Documentation/UNITY-LICENSE-SETUP.md](Documentation/UNITY-LICENSE-SETUP.md) before configuring license secrets. Unity's current manual activation rules depend on license type; Unity Personal activation should be performed through Unity Hub when manual ALF activation is unsupported.

## Asset recovery

Read [Documentation/ASSET-RECOVERY.md](Documentation/ASSET-RECOVERY.md) to export the Android Unity data with AssetRipper, stage the recovered assets safely, and generate an inventory from the Unity Editor using **Carrera Hybrid → Recovery → Generate Asset Inventory**.

See [Documentation/STATUS.md](Documentation/STATUS.md) for verified reverse-engineering findings, limitations and the screen-by-screen fidelity gates.
