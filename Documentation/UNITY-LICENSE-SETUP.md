# Unity Personal license setup for Carrera Hybrid

You do not need Unity Pro, a serial key, or an ALF file for Unity Personal.

## A. Activate Personal on your Windows PC

1. Install/update Unity Hub from https://unity.com/download.
2. Open Unity Hub and sign in with the Unity account you want to use for this project.
3. Open **Settings → Licenses**.
4. If a Personal license is already listed as active, you are activated.
5. If not, select **Add license → Get a free Personal license**.
6. Accept the terms with **Agree and get personal edition license**.

Unity says signing in to Unity Hub automatically activates Personal; the Add license flow is available if it did not. Personal activation and return are Hub-only. See https://docs.unity.com/en-us/hub/manage-license.

## B. Get the .ulf for GitHub Actions

GameCI's current GitHub Actions v4 instructions document using the Personal license created by Unity Hub as a CI secret:

1. In Unity Hub, open **Settings → Licenses**.
2. Select **Add license → Get a free Personal license**, and complete the activation flow even if the license appears to be present already. GameCI notes this ensures the license file is created.
3. In Windows File Explorer, enter this path in the address bar: C:\ProgramData\Unity
4. Locate Unity_lic.ulf. The ProgramData folder may be hidden; enable **View → Show → Hidden items** if necessary.
5. Open Unity_lic.ulf with Notepad and copy its entire contents. Do not upload the file to the repository or paste it into a chat.
6. In GitHub, open **Settings → Secrets and variables → Actions → New repository secret**.
7. Create these three secrets:

| Secret | Value |
|---|---|
| UNITY_LICENSE | Entire contents of Unity_lic.ulf |
| UNITY_EMAIL | Email address of the Unity account used to activate Personal |
| UNITY_PASSWORD | Password for that Unity account |

GameCI v4's current GitHub activation guide documents these steps and the Windows license path: https://game.ci/docs/github/activation/.

**Do not use the .alf workflow for Personal.** The ALF workflow in this repository is retained only as a legacy workflow for license types supported by Unity's manual activation portal. Unity explicitly excludes Personal from manual activation: https://docs.unity.com/en-us/engine/6000.6/manual/get-started/install-and-upgrade/licenses-and-activation/manual-license-activation/manual-activation-guide.

## C. Personal license seat behavior

GameCI's guide says to sign out of Unity Hub after copying the .ulf for CI use. Signing out returns the Personal license on your local machine; the CI runner then activates using the license secret and credentials. If you want to keep using Unity Editor locally at the same time, be aware of Unity's Personal activation/device restrictions and avoid concurrent activations that compete for the same entitlement. See GameCI's current guide for its return-license notes.

## D. Run the Windows build

1. Confirm all three secrets exist (the values themselves must remain private).
2. Open **Actions → Build Carrera Hybrid for Windows**.
3. Select **Run workflow** on main.
4. Inspect the **Build Windows x64 Player** job.
5. If successful, download the Windows x64 build artifact.

The project is pinned to Unity 6000.0.84f1. The CI build uses game-ci/unity-builder@v4.

## Troubleshooting

- **No Unity_lic.ulf:** repeat the Add license → Get a free Personal license flow while signed in to Unity Hub, then check C:\ProgramData\Unity.
- **No Personal option:** confirm you are signed into the intended Unity account and that it is eligible for Unity Personal.
- **License is invalid in CI:** verify that UNITY_LICENSE contains the complete file contents (not a path or filename), and that email/password correspond to the same Unity account.
- **Authentication failure:** check the Unity account password and any account security requirements. Never print secrets into logs.
- **Build fails after activation:** inspect the Unity compilation/build error separately; a valid license only resolves activation.
- **ALF portal says Personal is unsupported:** expected. Do not use the portal for Personal; use Unity Hub and the GameCI Personal workflow above.