# Recovering original Carrera Hybrid assets

The Android package is a compiled Unity IL2CPP build. The repository's current UI is a desktop reconstruction, not the original serialized UI. This workflow is intended to bring the original Unity assets into the Windows project without overwriting the working reconstruction.

## 1. Export with AssetRipper

1. Download the latest Windows x64 build from https://github.com/AssetRipper/AssetRipper/releases/latest.
2. Extract the Android APK data using the supplied preparation script/instructions from the reconstruction package.
3. Open the Unity Data directory in AssetRipper:
   - resources/assets/bin/Data
4. Also inspect the Android Addressables directory:
   - resources/assets/aa/Android
5. Export to a separate folder. Do not export over the raw APK extraction.
6. Open the exported Unity project in Unity 6000.0.84f1 first. Resolve importer errors before copying assets into this Windows project.

AssetRipper's output is a reconstruction of serialized Unity assets. Some components may be missing scripts or have incomplete serialized data because the original C# source and project settings were not included in the APK.

## 2. Preserve and stage the export

Keep the raw extraction and AssetRipper export intact. Copy only the required original assets into this repository under a separate staging directory, for example:

Assets/RecoveredAndroid/

Do not replace existing UI scripts or scenes wholesale. Keep imported originals in their own folders until their dependencies, shaders, materials, and references have been checked.

## 3. Generate an inventory

After opening this project in Unity:

1. Wait for asset import to finish.
2. Select **Carrera Hybrid → Recovery → Generate Asset Inventory**.
3. Unity writes Documentation/Asset-Recovery-Inventory.md.
4. Review the report for scenes, prefabs, sprites, textures, meshes, materials, fonts, animation clips, audio, and likely car/track/UI asset names.

The inventory tool is read-only. It does not edit imported assets or scenes.

## 4. Screen reconstruction order

Use the recovered original content to replace approximations in this order:

1. Startup — splash, loading, bootstrap state, initialization errors and transitions.
2. Login — login, guest/offline, registration, password recovery and validation states.
3. Menu — navigation, car/showroom content, career, coaching, settings and dialogs.
4. Lobby — player list, connection states, car setup, track selection, race configuration and readiness.
5. Race — original vehicle and track, camera, driving physics, HUD, race events, pause and connection loss.
6. Race Result — rankings, progression, experience, rewards, persistence and navigation.

Do not mark a screen complete just because its first view renders. Every reachable subview, popup, error, loading state and navigation path must work before moving to the next screen.

## 5. Current technical gaps

- No original Unity scene YAML, prefab hierarchy, original C# assemblies, ProjectSettings, or source project was supplied.
- Android IL2CPP metadata provides type names and method metadata, not a complete recoverable C# implementation.
- Visual Scripting graph data is present in scene-level serialized files, but the full graphs still need to be exported and inspected.
- Android Addressables bundles may require platform-specific shader/material and asset handling for Windows.
- The recovered BLE bridge constants are not the complete Carrera drive protocol. Do not enable real car control until packet fields, notifications, connection lifecycle, failsafe behavior, and DFU are reconstructed and tested on physical hardware.
- The local practice race remains a prototype, not the original physics, multiplayer, or progression implementation.
