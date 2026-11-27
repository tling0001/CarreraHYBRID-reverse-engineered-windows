# Carrera Hybrid — Windows reconstruction

Unity `6000.0.84f1` Windows x64 reconstruction based on the Android APK-derived archive (`com.sturmkind.carrerahybrid`, v1.2.6).

This revision contains a working, newly authored UI navigation prototype for Startup, Login, registration, password recovery, Menu, Garage, Career, Coaching, Settings, Lobby, Race, Pause and Race Result. It includes local practice-session controls and an Actions build workflow.

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
`.github/workflows/build-windows.yml` builds a Windows x64 player on pushes, pull requests and manual dispatches. Configure Unity license secrets `UNITY_LICENSE`, `UNITY_EMAIL` and `UNITY_PASSWORD` in repository settings.

See [Documentation/STATUS.md](Documentation/STATUS.md) for verified reverse-engineering findings, limitations and the next fidelity gates.
