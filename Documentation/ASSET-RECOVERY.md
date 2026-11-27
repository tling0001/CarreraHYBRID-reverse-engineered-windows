# Recovering original Carrera Hybrid assets

The Android package is a compiled Unity IL2CPP build. This repository's current UI is a desktop reconstruction, not the original serialized UI. The APK archive does contain Unity serialized assets, Addressables and scene-level Visual Scripting data. It does not contain the editable Unity project or original C# source.

## 1. Prepare the Android archive on Windows

The supplied archive is named carrerahybrid-decompiled.zip. Place it in a convenient folder on your Windows PC.

Open PowerShell in a copy of this repository and run:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\Tools\Prepare-CarreraUnityData.ps1 -ZipPath "C:\path\to\carrerahybrid-decompiled.zip" -Destination "C:\CarreraHybrid-Reconstruction"
```

The script:
- extracts the archive into a raw folder;
- reassembles Unity .splitN serialized asset parts in numeric order;
- preserves all original split parts;
- refuses to overwrite an existing extraction;
- checks for Unity's globalgamemanagers file;
- creates a SHA256SUMS.txt manifest.

Expected locations:
- Unity Data: C:\CarreraHybrid-Reconstruction\raw\base-main\resources\assets\bin\Data
- Addressables: C:\CarreraHybrid-Reconstruction\raw\base-main\resources\assets\aa\Android

## 2. Export with AssetRipper

1. Download the official Windows x64 build from https://github.com/AssetRipper/AssetRipper/releases/latest.
2. Extract and run AssetRipper.
3. Load the Unity Data folder above. If the Addressables bundles are not automatically discovered, inspect/load the Android Addressables folder separately.
4. Export the reconstructed Unity project to a new folder, for example C:\CarreraHybrid-AssetRipper-Export.
5. Do not export over the raw extraction or over this Windows project.
6. Keep the AssetRipper log and note any missing scripts, unreadable assets, shader errors or failed bundles.

AssetRipper reconstructs serialized assets; it cannot restore the original source code or guarantee perfect recovery of every scene/prefab.

## 3. Stage the export into this Windows project

After AssetRipper finishes, the export should contain an Assets folder. From this repository root, run:

```powershell
.\Tools\Stage-AssetRipperExport.ps1 -AssetRipperExport "C:\CarreraHybrid-AssetRipper-Export"
```

This copies the exported Assets contents into Assets/RecoveredAndroid while preserving .meta files. It refuses to overwrite an existing staging folder. It does not copy ProjectSettings or replace the current scenes/scripts.

Open the project in Unity 6000.0.84f1 and wait for asset import to finish.

## 4. Generate the recovered scene and UI inventory

In Unity, select **Carrera Hybrid → Recovery → Generate Asset Inventory**.

The report is written to Documentation/Asset-Recovery-Inventory.md. It includes:
- asset counts and key Unity asset paths;
- likely car, track, and UI assets based on names;
- scene hierarchy and component types;
- UI image sprite references;
- TMP and legacy UI text plus font references;
- animation controllers and missing-script indicators.

The inventory tool is read-only. It does not edit imported assets or scenes.

## 5. Screen reconstruction order

Work through complete screens in this order. Do not mark a screen complete until its reachable dialogs, validation, loading/error states and navigation paths have been tested.

1. **Startup** — splash, initialization, loading, first-run and failure transitions.
2. **Login** — sign-in, guest/offline, registration, password recovery, validation and service errors.
3. **Menu and Garage** — navigation, car showroom/collection, career, coaching, settings and dialogs.
4. **Lobby** — player list, connection states, car setup, track selection, race configuration and readiness.
5. **Race** — original vehicle and track, camera, driving physics, HUD, flags, pause, finish and connection-loss behavior.
6. **Race Result** — rankings, progression, experience, rewards, persistence and navigation.

Use recovered scene/prefab layouts and sprites where available. Reuse original fonts and animation clips. Preserve imported originals and make Windows-specific shader/material changes in duplicated assets.

## 6. Known recovery limits

- No original Unity scene YAML, prefab hierarchy source, original C# assemblies, ProjectSettings or source project was supplied.
- Android IL2CPP metadata provides type names and method metadata, not a complete recoverable C# implementation.
- Visual Scripting graph data exists in scene-level serialized files and should be inspected after export.
- Android Addressables may require platform-specific shader/material and asset handling for Windows.
- The recovered BLE bridge constants are not the complete Carrera drive protocol. Do not enable real car control until packet fields, notifications, connection lifecycle, failsafe behavior and DFU are reconstructed and tested on physical hardware.
- The current local practice race is not the original physics, multiplayer or progression implementation.
