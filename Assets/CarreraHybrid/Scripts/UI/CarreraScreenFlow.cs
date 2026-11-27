using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CarreraHybrid.UI
{
    /// <summary>
    /// Reconstructed desktop UI flow based on the Android APK's recovered scene and presenter
    /// names. This is an approximation, not a recovered copy of the original serialized UI.
    /// Online services and real-car control are deliberately not represented as successful.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public sealed class CarreraScreenFlow : MonoBehaviour
    {
        private const int W = 1920, H = 1080;
        private readonly Color ink = new Color32(12, 15, 19, 255);
        private readonly Color panel = new Color32(24, 29, 35, 255);
        private readonly Color panel2 = new Color32(34, 41, 48, 255);
        private readonly Color orange = new Color32(255, 91, 28, 255);
        private readonly Color muted = new Color32(155, 166, 176, 255);
        private Canvas canvas;
        private Font font;
        private RectTransform body;
        private Text status;
        private string playerName = "Driver";
        private string selectedCar = "Carrera Hybrid";
        private string selectedTrack = "Training Circuit";
        private int laps = 3;
        private int speed;
        private bool braking;
        private bool accelerating;
        private float raceTime;
        private bool raceRunning;
        private int currentLap = 1;
        private bool fullscreen = true;
        private int targetFps = 60;
        private Text speedReadout;
        private Text timerReadout;
        private string loginEmail;
        private string loginPassword;

        private void Start()
        {
            Application.targetFrameRate = 60;
            EnsureEventSystem();
            BuildCanvas();
            ShowStartup();
        }

        private void Update()
        {
            if (raceRunning)
            {
                raceTime += Time.deltaTime;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) accelerating = true;
                else accelerating = false;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) braking = true;
                else braking = false;
                speed = Mathf.Clamp(speed + (accelerating ? 2 : -1) - (braking ? 5 : 0), 0, 220);
                if (speedReadout != null) speedReadout.text = speed.ToString();
                if (timerReadout != null) timerReadout.text = FormatTime(raceTime);
                if (Input.GetKeyDown(KeyCode.Escape)) { raceRunning = false; ShowPause(); }
            }
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private void BuildCanvas()
        {
            var root = new GameObject("Carrera Hybrid UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(W, H); scaler.matchWidthOrHeight = 0.5f;
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private void Clear()
        {
            for (int i = canvas.transform.childCount - 1; i >= 0; i--) Destroy(canvas.transform.GetChild(i).gameObject);
            body = null; status = null;
        }

        private void Base(string section, string subtitle)
        {
            Clear();
            RectTransform root = Rect("Screen", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root.GetComponent<Image>().color = ink;
            RectTransform top = Rect("TopBar", root, new Vector2(0, .88f), Vector2.one, Vector2.zero, Vector2.zero);
            top.GetComponent<Image>().color = new Color32(17, 21, 26, 255);
            Label("CARRERA", top, 44, orange, TextAnchor.MiddleLeft, new Vector2(.035f, 0), new Vector2(.22f, 1));
            Label("HYBRID  /  " + section.ToUpperInvariant(), top, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.25f, 0), new Vector2(.72f, 1));
            Label(subtitle, root, 20, muted, TextAnchor.MiddleLeft, new Vector2(.055f, .81f), new Vector2(.94f, .87f));
            body = Rect("Content", root, new Vector2(.055f, .08f), new Vector2(.945f, .79f), Vector2.zero, Vector2.zero);
            status = Label("", root, 18, new Color32(255, 190, 120, 255), TextAnchor.MiddleLeft,
                new Vector2(.055f, .015f), new Vector2(.94f, .065f));
        }

        private void ShowStartup()
        {
            Base("Startup", "INITIALIZING GAME SYSTEMS");
            Label("CARRERA HYBRID", body, 64, Color.white, TextAnchor.MiddleCenter,
                new Vector2(.1f, .46f), new Vector2(.9f, .68f));
            Label("DRIVE  •  RACE  •  COMPETE", body, 24, orange, TextAnchor.MiddleCenter,
                new Vector2(.1f, .36f), new Vector2(.9f, .46f));
            Label("Windows reconstruction  |  Android release reference: 1.2.6", body, 17, muted,
                TextAnchor.MiddleCenter, new Vector2(.1f, .22f), new Vector2(.9f, .3f));
            StartCoroutine(StartupDelay());
        }

        private IEnumerator StartupDelay()
        {
            yield return new WaitForSeconds(1.4f);
            ShowLogin();
        }

        private void ShowLogin()
        {
            Base("Login", "SIGN IN TO YOUR CARRERA ACCOUNT");
            RectTransform card = Card(body, new Vector2(.25f, .04f), new Vector2(.75f, .98f));
            Label("WELCOME BACK", card, 34, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .78f), new Vector2(.92f, .94f));
            Label("Sign in to continue your racing career", card, 18, muted, TextAnchor.MiddleCenter, new Vector2(.08f, .7f), new Vector2(.92f, .79f));
            InputField email = Field(card, "Email", new Vector2(.1f, .53f), new Vector2(.9f, .65f), false);
            InputField password = Field(card, "Password", new Vector2(.1f, .38f), new Vector2(.9f, .5f), true);
            AddButton(card, "SIGN IN", new Vector2(.1f, .22f), new Vector2(.9f, .34f), orange, () =>
            {
                loginEmail = email.text; loginPassword = password.text;
                SetStatus(string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword)
                    ? "Enter both email and password." : "Online authentication service is not yet connected. Your credentials were not sent.");
            });
            AddButton(card, "CONTINUE OFFLINE", new Vector2(.1f, .15f), new Vector2(.9f, .25f), panel2, () =>
            { playerName = "Guest Driver"; ShowMenu(); });
            AddButton(card, "CREATE ACCOUNT", new Vector2(.1f, .045f), new Vector2(.48f, .13f), panel2, ShowRegistration);
            AddButton(card, "FORGOT PASSWORD", new Vector2(.52f, .045f), new Vector2(.9f, .13f), panel2, ShowPasswordRecovery);
        }

        private void ShowRegistration()
        {
            Base("Registration", "CREATE YOUR CARRERA ACCOUNT");
            RectTransform card = Card(body, new Vector2(.25f, .04f), new Vector2(.75f, .98f));
            Label("NEW DRIVER", card, 32, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .82f), new Vector2(.92f, .95f));
            InputField name = Field(card, "Driver name", new Vector2(.1f, .65f), new Vector2(.9f, .76f), false);
            InputField email = Field(card, "Email", new Vector2(.1f, .49f), new Vector2(.9f, .6f), false);
            InputField password = Field(card, "Password", new Vector2(.1f, .33f), new Vector2(.9f, .44f), true);
            InputField confirm = Field(card, "Confirm password", new Vector2(.1f, .17f), new Vector2(.9f, .28f), true);
            AddButton(card, "CREATE ACCOUNT", new Vector2(.1f, .045f), new Vector2(.55f, .13f), orange, () =>
            {
                if (string.IsNullOrWhiteSpace(name.text) || string.IsNullOrWhiteSpace(email.text) || string.IsNullOrEmpty(password.text))
                    SetStatus("Complete all registration fields.");
                else if (password.text != confirm.text) SetStatus("The passwords do not match.");
                else SetStatus("Registration backend is unavailable. No account was created.");
            });
            AddButton(card, "BACK", new Vector2(.59f, .045f), new Vector2(.9f, .13f), panel2, ShowLogin);
        }

        private void ShowPasswordRecovery()
        {
            Base("Password Recovery", "RESET YOUR ACCOUNT PASSWORD");
            RectTransform card = Card(body, new Vector2(.25f, .15f), new Vector2(.75f, .88f));
            Label("PASSWORD RESET", card, 32, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .7f), new Vector2(.92f, .9f));
            Label("Enter the email address associated with your Carrera account.", card, 18, muted, TextAnchor.MiddleCenter,
                new Vector2(.08f, .55f), new Vector2(.92f, .7f));
            InputField email = Field(card, "Email", new Vector2(.1f, .36f), new Vector2(.9f, .49f), false);
            AddButton(card, "SEND RESET LINK", new Vector2(.1f, .17f), new Vector2(.55f, .3f), orange, () =>
                SetStatus(string.IsNullOrWhiteSpace(email.text) ? "Enter your email address." : "Password recovery backend is unavailable. No email was sent."));
            AddButton(card, "BACK TO LOGIN", new Vector2(.59f, .17f), new Vector2(.9f, .3f), panel2, ShowLogin);
        }

        private void ShowCareer()
        {
            Base("Career", "DRIVER PROGRESSION");
            RectTransform card = Card(body, new Vector2(.12f, .05f), new Vector2(.88f, .96f));
            Label("CAREER PROFILE", card, 30, Color.white, TextAnchor.MiddleLeft, new Vector2(.06f, .78f), new Vector2(.94f, .94f));
            Label("OFFLINE DRIVER", card, 20, orange, TextAnchor.MiddleLeft, new Vector2(.06f, .62f), new Vector2(.94f, .73f));
            Label("Career records, challenges, rewards and saved progression are account-backed in the Android game. They cannot be loaded in this offline reconstruction yet.",
                card, 20, muted, TextAnchor.MiddleLeft, new Vector2(.06f, .34f), new Vector2(.94f, .59f));
            AddButton(card, "BACK TO MENU", new Vector2(.65f, .06f), new Vector2(.94f, .19f), orange, ShowMenu);
        }

        private void ShowCoaching()
        {
            Base("Coaching", "DRIVING TIPS AND CHALLENGES");
            RectTransform card = Card(body, new Vector2(.12f, .05f), new Vector2(.88f, .96f));
            Label("DRIVER COACHING", card, 30, Color.white, TextAnchor.MiddleLeft, new Vector2(.06f, .78f), new Vector2(.94f, .94f));
            Label("Practice smooth steering, controlled braking and consistent throttle. Use W / ↑ to accelerate, S / ↓ to brake and Esc to pause in the current desktop practice prototype.",
                card, 21, muted, TextAnchor.MiddleLeft, new Vector2(.06f, .43f), new Vector2(.94f, .72f));
            Label("Original guided coaching events and challenge progression require the recovered content and game logic.",
                card, 17, muted, TextAnchor.MiddleLeft, new Vector2(.06f, .25f), new Vector2(.94f, .4f));
            AddButton(card, "BACK TO MENU", new Vector2(.65f, .06f), new Vector2(.94f, .19f), orange, ShowMenu);
        }

        private void ShowMenu()
        {
            Base("Main Menu", "WELCOME, " + playerName.ToUpperInvariant());
            RectTransform hero = Card(body, new Vector2(.02f, .42f), new Vector2(.98f, .98f));
            Label("YOUR GARAGE", hero, 22, orange, TextAnchor.MiddleLeft, new Vector2(.04f, .78f), new Vector2(.5f, .94f));
            Label(selectedCar.ToUpperInvariant(), hero, 46, Color.white, TextAnchor.MiddleLeft, new Vector2(.04f, .48f), new Vector2(.62f, .76f));
            Label("READY TO DRIVE", hero, 18, muted, TextAnchor.MiddleLeft, new Vector2(.04f, .36f), new Vector2(.6f, .48f));
            Label("CARRERA\nHYBRID", hero, 42, orange, TextAnchor.MiddleCenter, new Vector2(.67f, .25f), new Vector2(.96f, .75f));
            AddButton(hero, "RACE NOW", new Vector2(.04f, .08f), new Vector2(.28f, .27f), orange, ShowLobby);
            AddButton(hero, "GARAGE", new Vector2(.32f, .08f), new Vector2(.52f, .27f), panel2, ShowGarage);
            AddButton(hero, "CAREER", new Vector2(.56f, .08f), new Vector2(.76f, .27f), panel2, ShowCareer);
            AddButton(hero, "SETTINGS", new Vector2(.8f, .08f), new Vector2(.96f, .27f), panel2, ShowSettings);
            RectTransform lower = Card(body, new Vector2(.02f, .02f), new Vector2(.98f, .37f));
            Label("RACE CENTER", lower, 22, Color.white, TextAnchor.MiddleLeft, new Vector2(.04f, .65f), new Vector2(.5f, .9f));
            Label("Choose a track, configure your session and connect a Carrera car when Windows BLE support is integrated.", lower,
                18, muted, TextAnchor.MiddleLeft, new Vector2(.04f, .25f), new Vector2(.92f, .62f));
            AddButton(lower, "COACHING", new Vector2(.48f, .28f), new Vector2(.7f, .65f), panel2, ShowCoaching);
            AddButton(lower, "OPEN LOBBY", new Vector2(.72f, .28f), new Vector2(.95f, .65f), orange, ShowLobby);
        }

        private void ShowGarage()
        {
            Base("Garage", "YOUR VEHICLES AND DRIVE CONFIGURATION");
            RectTransform left = Card(body, new Vector2(.02f, .05f), new Vector2(.58f, .96f));
            Label("VEHICLE", left, 20, orange, TextAnchor.MiddleLeft, new Vector2(.06f, .84f), new Vector2(.9f, .96f));
            Label(selectedCar.ToUpperInvariant(), left, 44, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .4f), new Vector2(.92f, .72f));
            Label("Vehicle model preview will use the recovered original 3D asset when exported.", left, 17, muted, TextAnchor.MiddleCenter,
                new Vector2(.08f, .22f), new Vector2(.92f, .38f));
            RectTransform right = Card(body, new Vector2(.61f, .05f), new Vector2(.98f, .96f));
            Label("AVAILABLE CONFIGURATION", right, 20, orange, TextAnchor.MiddleLeft, new Vector2(.06f, .82f), new Vector2(.94f, .96f));
            AddButton(right, "CARRERA HYBRID", new Vector2(.06f, .62f), new Vector2(.94f, .76f), panel2, () => { selectedCar = "Carrera Hybrid"; ShowGarage(); });
            AddButton(right, "DRIVE ASSISTS", new Vector2(.06f, .43f), new Vector2(.94f, .57f), panel2, () => SetStatus("Drive-assist tuning will be enabled when the original car setup model is recovered."));
            AddButton(right, "BACK TO MENU", new Vector2(.06f, .08f), new Vector2(.94f, .22f), orange, ShowMenu);
        }

        private void ShowSettings()
        {
            Base("Settings", "GAME AND DESKTOP CONFIGURATION");
            RectTransform card = Card(body, new Vector2(.12f, .04f), new Vector2(.88f, .98f));
            Label("GENERAL", card, 22, orange, TextAnchor.MiddleLeft, new Vector2(.06f, .82f), new Vector2(.94f, .96f));
            Label("Display mode", card, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.07f, .68f), new Vector2(.48f, .79f));
            Label(fullscreen ? "Fullscreen" : "Windowed", card, 18, muted, TextAnchor.MiddleRight, new Vector2(.5f, .68f), new Vector2(.56f, .79f));
            AddButton(card, "TOGGLE DISPLAY MODE", new Vector2(.58f, .67f), new Vector2(.94f, .78f), panel2, () => { fullscreen = !fullscreen; Screen.fullScreen = fullscreen; ShowSettings(); });
            SettingRow(card, "Resolution", Screen.currentResolution.width + " × " + Screen.currentResolution.height, .49f);
            Label("Frame rate", card, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.07f, .32f), new Vector2(.48f, .43f));
            Label(targetFps + " FPS target", card, 18, muted, TextAnchor.MiddleRight, new Vector2(.5f, .32f), new Vector2(.56f, .43f));
            AddButton(card, "TOGGLE 60 / 120 FPS", new Vector2(.58f, .31f), new Vector2(.94f, .42f), panel2, () => { targetFps = targetFps == 60 ? 120 : 60; Application.targetFrameRate = targetFps; ShowSettings(); });
            SettingRow(card, "Input", "Keyboard / Mouse", .16f);
            AddButton(card, "BACK TO MENU", new Vector2(.64f, .02f), new Vector2(.94f, .12f), orange, ShowMenu);
        }

        private void SettingRow(Transform parent, string name, string value, float y)
        {
            Label(name, parent, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.07f, y), new Vector2(.48f, y + .11f));
            Label(value, parent, 18, muted, TextAnchor.MiddleRight, new Vector2(.5f, y), new Vector2(.93f, y + .11f));
        }

        private void ShowLobby()
        {
            Base("Lobby", "RACE SETUP  /  SESSION CONFIGURATION");
            RectTransform setup = Card(body, new Vector2(.02f, .05f), new Vector2(.62f, .96f));
            Label("RACE SETTINGS", setup, 24, orange, TextAnchor.MiddleLeft, new Vector2(.06f, .82f), new Vector2(.94f, .96f));
            Label("TRACK", setup, 18, muted, TextAnchor.MiddleLeft, new Vector2(.07f, .68f), new Vector2(.4f, .78f));
            AddButton(setup, "‹", new Vector2(.07f, .52f), new Vector2(.17f, .65f), panel2, () => CycleTrack(-1));
            Label(selectedTrack.ToUpperInvariant(), setup, 25, Color.white, TextAnchor.MiddleCenter, new Vector2(.19f, .52f), new Vector2(.82f, .65f));
            AddButton(setup, "›", new Vector2(.84f, .52f), new Vector2(.94f, .65f), panel2, () => CycleTrack(1));
            Label("LAPS", setup, 18, muted, TextAnchor.MiddleLeft, new Vector2(.07f, .34f), new Vector2(.4f, .44f));
            AddButton(setup, "−", new Vector2(.07f, .17f), new Vector2(.17f, .3f), panel2, () => { laps = Mathf.Max(1, laps - 1); ShowLobby(); });
            Label(laps.ToString(), setup, 28, Color.white, TextAnchor.MiddleCenter, new Vector2(.19f, .17f), new Vector2(.82f, .3f));
            AddButton(setup, "+", new Vector2(.84f, .17f), new Vector2(.94f, .3f), panel2, () => { laps = Mathf.Min(20, laps + 1); ShowLobby(); });
            RectTransform drivers = Card(body, new Vector2(.65f, .05f), new Vector2(.98f, .96f));
            Label("DRIVERS", drivers, 23, orange, TextAnchor.MiddleLeft, new Vector2(.08f, .82f), new Vector2(.92f, .96f));
            Label(playerName + "  •  LOCAL", drivers, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.08f, .65f), new Vector2(.92f, .76f));
            Label("No online drivers in offline session", drivers, 16, muted, TextAnchor.MiddleLeft, new Vector2(.08f, .52f), new Vector2(.92f, .62f));
            Label("CAR CONNECTION", drivers, 18, orange, TextAnchor.MiddleLeft, new Vector2(.08f, .35f), new Vector2(.92f, .45f));
            Label("Not connected  /  BLE transport unavailable", drivers, 15, muted, TextAnchor.MiddleLeft, new Vector2(.08f, .25f), new Vector2(.92f, .34f));
            AddButton(drivers, "CAR CONNECTION STATUS", new Vector2(.08f, .16f), new Vector2(.92f, .24f), panel2, () => SetStatus("Windows Bluetooth GATT and Carrera drive protocol are not implemented. No car connection was attempted."));
            AddButton(drivers, "START PRACTICE", new Vector2(.08f, .04f), new Vector2(.92f, .14f), orange, () => StartRace());
        }

        private void CycleTrack(int direction)
        {
            string[] tracks = { "Training Circuit", "Sprint Circuit", "Technical Circuit" };
            int index = System.Array.IndexOf(tracks, selectedTrack);
            selectedTrack = tracks[(index + direction + tracks.Length) % tracks.Length]; ShowLobby();
        }

        private void StartRace()
        {
            raceTime = 0; speed = 0; currentLap = 1; raceRunning = true; ShowRace();
        }

        private void ShowRace()
        {
            Base("Race", "PRACTICE SESSION  /  " + selectedTrack.ToUpperInvariant());
            RectTransform track = Card(body, new Vector2(.02f, .05f), new Vector2(.72f, .96f));
            Label("TRACK VIEW", track, 18, muted, TextAnchor.MiddleLeft, new Vector2(.04f, .88f), new Vector2(.4f, .98f));
            // A clear, navigable schematic until the original 3D track assets are imported.
            RectTransform circuit = Rect("Circuit", track, new Vector2(.14f, .2f), new Vector2(.86f, .82f), Vector2.zero, Vector2.zero);
            Image circuitImage = circuit.GetComponent<Image>(); circuitImage.color = new Color32(52, 62, 69, 255);
            RectTransform inner = Rect("CircuitInner", circuit, new Vector2(.18f, .18f), new Vector2(.82f, .82f), Vector2.zero, Vector2.zero);
            inner.GetComponent<Image>().color = panel;
            Label("PRACTICE", circuit, 24, muted, TextAnchor.MiddleCenter, new Vector2(.2f, .4f), new Vector2(.8f, .6f));
            RectTransform hud = Card(body, new Vector2(.75f, .05f), new Vector2(.98f, .96f));
            Label("SPEED", hud, 18, muted, TextAnchor.MiddleCenter, new Vector2(.08f, .84f), new Vector2(.92f, .94f));
            speedReadout = Label("" + speed, hud, 76, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .62f), new Vector2(.92f, .84f));
            Label("KM/H", hud, 16, muted, TextAnchor.MiddleCenter, new Vector2(.08f, .56f), new Vector2(.92f, .64f));
            Label("LAP  " + currentLap + " / " + laps, hud, 22, orange, TextAnchor.MiddleCenter, new Vector2(.08f, .43f), new Vector2(.92f, .53f));
            timerReadout = Label(FormatTime(raceTime), hud, 19, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .34f), new Vector2(.92f, .42f));
            AddButton(hud, "BRAKE", new Vector2(.08f, .2f), new Vector2(.92f, .3f), panel2, () => { speed = Mathf.Max(0, speed - 20); });
            AddButton(hud, "PAUSE", new Vector2(.08f, .06f), new Vector2(.48f, .16f), orange, () => { raceRunning = false; ShowPause(); });
            AddButton(hud, "FINISH", new Vector2(.52f, .06f), new Vector2(.92f, .16f), panel2, FinishRace);
            SetStatus("Keyboard: W / ↑ accelerate   S / ↓ brake   Esc pause. This is a local UI/handling prototype, not the original race simulation.");
        }

        private void ShowPause()
        {
            Base("Race Paused", "SESSION PAUSED");
            RectTransform card = Card(body, new Vector2(.3f, .12f), new Vector2(.7f, .9f));
            Label("PAUSED", card, 42, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .68f), new Vector2(.92f, .9f));
            AddButton(card, "RESUME", new Vector2(.1f, .48f), new Vector2(.9f, .62f), orange, () => { raceRunning = true; ShowRace(); });
            AddButton(card, "RESTART PRACTICE", new Vector2(.1f, .3f), new Vector2(.9f, .44f), panel2, StartRace);
            AddButton(card, "END SESSION", new Vector2(.1f, .12f), new Vector2(.9f, .26f), panel2, FinishRace);
        }

        private void FinishRace()
        {
            raceRunning = false;
            ShowResults();
        }

        private void ShowResults()
        {
            Base("Race Result", "PRACTICE SESSION COMPLETE");
            RectTransform card = Card(body, new Vector2(.2f, .04f), new Vector2(.8f, .98f));
            Label("SESSION COMPLETE", card, 36, Color.white, TextAnchor.MiddleCenter, new Vector2(.08f, .78f), new Vector2(.92f, .93f));
            Label(selectedTrack.ToUpperInvariant(), card, 22, orange, TextAnchor.MiddleCenter, new Vector2(.08f, .68f), new Vector2(.92f, .78f));
            ResultRow(card, "DRIVER", playerName, .55f);
            ResultRow(card, "LAPS CONFIGURED", laps.ToString(), .43f);
            ResultRow(card, "SESSION TIME", FormatTime(raceTime), .31f);
            ResultRow(card, "SESSION TYPE", "LOCAL PRACTICE", .19f);
            AddButton(card, "BACK TO MENU", new Vector2(.08f, .03f), new Vector2(.48f, .14f), panel2, ShowMenu);
            AddButton(card, "RACE AGAIN", new Vector2(.52f, .03f), new Vector2(.92f, .14f), orange, ShowLobby);
        }

        private void ResultRow(Transform parent, string key, string value, float y)
        {
            Label(key, parent, 18, muted, TextAnchor.MiddleLeft, new Vector2(.1f, y), new Vector2(.5f, y + .09f));
            Label(value, parent, 20, Color.white, TextAnchor.MiddleRight, new Vector2(.5f, y), new Vector2(.9f, y + .09f));
        }

        private RectTransform Card(Transform parent, Vector2 min, Vector2 max)
        {
            RectTransform r = Rect("Panel", parent, min, max, Vector2.zero, Vector2.zero);
            r.GetComponent<Image>().color = panel; return r;
        }

        private RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = offsetMin; rt.offsetMax = offsetMax; return rt;
        }

        private Text Label(string value, Transform parent, int size, Color color, TextAnchor align, Vector2 min, Vector2 max)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            Text t = go.GetComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.color = color;
            t.alignment = align; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
            t.resizeTextForBestFit = true; t.resizeTextMinSize = Mathf.Max(12, size - 4); t.resizeTextMaxSize = size;
            return t;
        }

        private Button AddButton(Transform parent, string text, Vector2 min, Vector2 max, Color color, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject("Button - " + text, typeof(RectTransform), typeof(Image), typeof(Button)); go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            Image image = go.GetComponent<Image>(); image.color = color;
            Button b = go.GetComponent<Button>(); b.targetGraphic = image; b.onClick.AddListener(action);
            var colors = b.colors; colors.normalColor = color; colors.highlightedColor = Color.Lerp(color, Color.white, .16f);
            colors.pressedColor = Color.Lerp(color, Color.black, .2f); colors.selectedColor = colors.highlightedColor; b.colors = colors;
            Label(text, go.transform, 20, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            return b;
        }

        private InputField Field(Transform parent, string placeholder, Vector2 min, Vector2 max, bool secret)
        {
            var go = new GameObject("Input - " + placeholder, typeof(RectTransform), typeof(Image), typeof(InputField)); go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = panel2;
            Text text = Label("", go.transform, 20, Color.white, TextAnchor.MiddleLeft, new Vector2(.04f, .05f), new Vector2(.96f, .95f));
            Text hint = Label(placeholder, go.transform, 18, muted, TextAnchor.MiddleLeft, new Vector2(.04f, .05f), new Vector2(.96f, .95f));
            InputField field = go.GetComponent<InputField>(); field.textComponent = text; field.placeholder = hint;
            field.contentType = secret ? InputField.ContentType.Password : InputField.ContentType.EmailAddress;
            return field;
        }

        private void SetStatus(string message) { if (status != null) status.text = message; }
        private static string FormatTime(float seconds)
        {
            int m = Mathf.FloorToInt(seconds / 60f); int s = Mathf.FloorToInt(seconds % 60f);
            int ms = Mathf.FloorToInt((seconds * 100f) % 100f); return m.ToString("00") + ":" + s.ToString("00") + "." + ms.ToString("00");
        }
    }
}
