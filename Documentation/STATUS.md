# Carrera Hybrid Windows reconstruction status

## Verified source basis
- Android package `com.sturmkind.carrerahybrid`, version 1.2.6 / versionCode 284.
- Unity `6000.0.84f1`, IL2CPP backend.
- Six recovered original scene names: Startup, Login, Menu, Lobby, Race, Race Result.
- APK includes Unity serialized assets, Addressables and scene-level Visual Scripting data, but not the editable Unity project, original C# source, ProjectSettings or original scene YAML.
- Android Java BLE bridge facts: Nordic UART write UUID `6E400002-B5A3-F393-E0A9-E50E24DCCA9E`, 20-byte drive packet, `0xAF` header, 50 ms send interval and 500 ms stale-target timeout. These do not define the complete drive protocol.

## Implemented in this source revision
- Unity 6000.0.84f1 project targeting Windows x64.
- Runtime-created dark motorsport UI with Startup, Login, registration, password recovery, Main Menu, Garage, Career, Coaching, Settings, Lobby, Race, Pause and Race Result views.
- Navigation between views, form validation, offline guest entry, track selection, lap count, local practice timer/speed HUD, keyboard acceleration/braking, pause/resume/restart/finish and results navigation.
- Desktop settings for fullscreen/windowed mode and 60/120 FPS target.
- Explicit status messages where backend authentication, account recovery, online career, original 3D assets or BLE functionality are not implemented.
- GitHub Actions Windows x64 build workflow.
- Manually triggered legacy GameCI ALF request workflow that uploads the activation request as a downloadable artifact; the ALF is not itself a license.
- Unity Editor asset inventory tool at Carrera Hybrid → Recovery → Generate Asset Inventory. It reports imported textures, sprites, meshes, materials, fonts, animation clips, prefabs, scenes and likely car/track/UI paths without modifying source assets.

## Fidelity and functional limitations
This is a newly authored approximation based on recovered Android scene names, class/type names and bridge behavior. It is not the original Android UI, and no pixel-identical claim is made. Exact layouts, typography, sprites, animations, prefabs, 3D car/track models, Visual Scripting graphs and localized strings must be imported/reconstructed from an AssetRipper export and compared against Android screenshots.

The local Race view is a UI/handling prototype, not the original physics, track simulation, multiplayer or car-control implementation. It does not send BLE packets. Login, registration and password recovery validate input locally but never claim server success or transmit credentials. Online multiplayer, career persistence, rewards and firmware updates remain unimplemented.

## Next integration work
1. Import an AssetRipper export and run the asset inventory tool; map the six original scene hierarchies, prefabs, UI sprites, fonts, materials, animations and Visual Scripting graphs.
2. Replace the approximate Startup screen with the recovered original content and verify every reachable state before proceeding to Login; then continue in order through Menu, Lobby, Race and Race Result.
3. Recover backend endpoint/request/authentication contracts and port them without inventing responses.
4. Recover the complete BLE packet/notification protocol and implement Windows GATT + Nordic DFU; test on physical Carrera hardware before enabling car control.
5. Replace the practice prototype with original track, vehicle, physics, HUD, race session, multiplayer and results logic.

## Build
Open with Unity `6000.0.84f1` and build `StandaloneWindows64`. GitHub Actions uses `game-ci/unity-builder@v4`. Configure the license secrets described in [UNITY-LICENSE-SETUP.md](UNITY-LICENSE-SETUP.md), using the method appropriate to your actual Unity license type. Unity Personal licenses should be activated through Unity Hub when manual ALF activation is unsupported. A successful CI build verifies the reconstruction prototype, not full feature parity.
