# Unity CI license setup (Carrera Hybrid)

## 1. Request the GitHub runner activation file

The repository contains a manual workflow at:

`.github/workflows/unity-activation-file.yml`

1. Open the repository on GitHub.
2. Select **Actions**.
3. Select **Request Unity activation file (legacy ALF)**.
4. Select **Run workflow** on the `main` branch.
5. Wait for the run to finish successfully.
6. Open that run and scroll to **Artifacts** at the bottom.
7. Download **Unity-Activation-Request-ALF** and extract the ZIP. It contains a file named similar to `Unity_v6000.0.84f1.alf`.
8. The workflow only uploads the request file as an artifact. It does not print the file contents or any credentials in the logs.

The action used by this workflow is GameCI's legacy `unity-request-activation-file@v2`. The action's own repository now marks it as no longer maintained. It is retained here because it implements the requested ALF artifact flow. See the GameCI activation documentation before relying on it.

## 2. Convert the ALF into a Unity license

Sign in to your Unity account, then open:

https://license.unity3d.com/manual

Upload the extracted `.alf` file and follow the portal instructions. Depending on the subscription/license type and current Unity activation rules, Unity may return a `.ulf` or `.xml` license file.

**Important license-type limitation:** Unity's current manual-activation documentation says manual activation does not support Unity Personal, Unity Pro assigned seats, or floating licenses. It supports assigned seats on Enterprise/Industry and legacy serial-based Pro licenses. If you use Unity Personal, activate Unity through Unity Hub with your Unity ID instead; do not try to bypass the portal's license-type checks. A generated ALF is only a request, not a license.

Unity documentation:
- https://docs.unity.com/en-us/engine/6000.6/manual/get-started/install-and-upgrade/licenses-and-activation/manual-license-activation/manual-activation-hub
- https://docs.unity.com/en-us/engine/6000.6/manual/get-started/install-and-upgrade/licenses-and-activation/manual-license-activation/manual-activation-cmd-win

## 3. Add the license to GitHub Actions

If Unity returns a `.ulf` license file and your license type permits this CI activation method:

1. Open the `.ulf` file in a text editor.
2. Copy its **entire contents** (including XML/license tags).
3. In GitHub, open **Settings → Secrets and variables → Actions → New repository secret**.
4. Create:
   - `UNITY_LICENSE`: complete contents of the `.ulf` file.
   - `UNITY_EMAIL`: the email address for your Unity account.
   - `UNITY_PASSWORD`: your Unity account password, only if required by the selected GameCI activation method.
5. Do not commit the `.ulf` file, passwords, or license material to the repository. Do not paste them into an issue, chat, or workflow log.

The existing `.github/workflows/build-windows.yml` reads these values from GitHub Secrets. It never writes them into the repository.

For a Unity Pro serial-key workflow, GameCI's documentation describes `UNITY_SERIAL`, `UNITY_EMAIL`, and `UNITY_PASSWORD` instead. Follow the method that matches the actual license assigned to your Unity account; do not create secrets for a license type you do not have.

## 4. Run the Windows build

After the required secrets are configured:

1. Open **Actions → Build Carrera Hybrid for Windows**.
2. Select **Run workflow**.
3. Open the run and inspect the **Build Windows x64** step.
4. If it succeeds, download the `CarreraHybrid-Windows-x64-...` artifact.

The build uses Unity `6000.0.84f1`, matching the version identified in the Android APK.

## Troubleshooting

- **No artifact:** the activation workflow failed before the upload step. Open the run log and inspect the activation action error.
- **ALF generated but Unity portal rejects it:** check the license type. An ALF does not grant a license and cannot convert an unsupported license type into a supported one.
- **Build says no valid license:** confirm the full license file contents were copied to `UNITY_LICENSE`, not the filename or a path.
- **Unity Personal:** activate in Unity Hub with your Unity ID. Unity's current documentation says manual ALF activation is not supported for Personal licenses.
- **Unity version changed:** regenerate/revalidate the activation material for the exact Editor version used by CI.
