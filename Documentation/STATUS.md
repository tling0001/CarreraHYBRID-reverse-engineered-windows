# Carrera Hybrid Windows reconstruction status

## Verified Android source basis
- Package: com.sturmkind.carrerahybrid, version 1.2.6 / versionCode 284.
- Unity version: 6000.0.84f1; IL2CPP backend.
- Original scene names: Startup, Login, Menu, Lobby, Race, Race Result.
- APK contains Unity serialized assets, Addressables and scene-level Visual Scripting data, but not the editable Unity project, original C# source, ProjectSettings or original scene YAML.
- Android BLE bridge facts: Nordic UART write UUID 6E400002-B5A3-F393-E0A9-E50E24DCCA9E, 20-byte drive packet, 0xAF header, 50 ms send interval and 500 ms stale-target timeout. These do not define the complete drive protocol.

## Implemented in the Windows reconstruction
- Unity 6000.0.84f1 project targeting Windows x64.
- Runtime-created dark motorsport UI for Startup, Login, registration, password recovery, Main Menu, Garage, Career, Coaching, Settings, Lobby, Race, Pause and Race Result.
- Local navigation, form validation, guest/offline entry, track selection, lap count, practice timer/speed HUD, keyboard acceleration/braking, pause/resume/restart/finish and results navigation.
- Desktop settings for fullscreen/windowed mode and 60/120 FPS target.
- Explicit non-success status messages where backend authentication, account recovery, online career, original 3D assets or BLE functionality are not implemented.
- GitHub Actions Windows x64 build using GameCI Unity Builder v4.
- Unity Personal licensing instructions aligned with the current GameCI v4 Personal workflow: Hub-created Unity_lic.ulf plus UNITY_LICENSE, UNITY_EMAIL and UNITY_PASSWORD secrets.
- Android archive preparation script that reassembles split Unity asset files without deleting original parts.
- Safe AssetRipper export staging script that copies exported Assets into Assets/RecoveredAndroid without overwriting an existing destination.
- Unity Editor asset audit that inventories sprites, textures, meshes, materials, fonts, animation controllers, scenes, UI text/sprite references, scene hierarchy and missing scripts.

## Fidelity and functional limitations
The current screens are newly authored approximations based on Android scene names, type metadata and recovered bridge behavior. They are not pixel-identical reproductions. Original scene layouts, prefabs, typography, sprites, animations, 3D car/track assets, Visual Scripting graphs and localized strings have not yet been imported and compared.

The local Race view is a UI/handling prototype, not original physics, track simulation, multiplayer or car control. It does not send BLE packets. Login, registration and password recovery validate locally and do not transmit credentials or claim server success. Online multiplayer, career persistence, rewards and firmware updates remain unimplemented.

## Screen-by-screen implementation plan
1. **Startup:** inspect the recovered Startup scene hierarchy, splash assets, logos, fonts, loading states, initialization graph and transitions. Rebuild the screen and every reachable initialization/error path, then compare against Android screenshots.
2. **Login:** recover the original login/guest/register/forgot-password panels, typography, icons, field validation, keyboard/focus behavior, loading and service-error states. Implement only verified backend behavior.
3. **Menu and Garage:** recover navigation hierarchy, car collection/showroom prefabs, car rotation/camera, vehicle stats, buttons, career/coaching/settings entries and modal dialogs.
4. **Lobby:** recover track/car selection assets, tuning controls, driver list, connection/readiness states, race configuration and lobby transitions.
5. **Race:** recover vehicle and track meshes, materials, physics-related components, camera rigs, HUD, flags, race events, pause/finish and network-loss states. Replace practice-only movement only after original physics behavior is understood.
6. **Race Result:** recover standings, ranking, experience/progression, rewards, persistence, animations and navigation back to menu/lobby.

## Required input for original-asset reconstruction
The AssetRipper export is not yet present in this repository. Run Tools/Prepare-CarreraUnityData.ps1 on the APK archive, export the Unity Data and Addressables with AssetRipper, then run Tools/Stage-AssetRipperExport.ps1. Open the project in Unity 6000.0.84f1 and run **Carrera Hybrid → Recovery → Generate Asset Inventory**. The resulting Documentation/Asset-Recovery-Inventory.md and the staged RecoveredAndroid assets are the basis for the next actual screen port.

## Build and licensing
Open with Unity 6000.0.84f1 and build StandaloneWindows64. Configure the Unity Personal secrets described in UNITY-LICENSE-SETUP.md. A successful CI build verifies the current reconstruction prototype, not full Android feature parity.
