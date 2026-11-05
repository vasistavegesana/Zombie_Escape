using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZombieEscape
{
    // Owns the menu, character selection, level progression, health, and HUD.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int maximumHealth = 3;

        private readonly HashSet<string> collectedKeys = new HashSet<string>();
        private PrototypeBootstrap bootstrap;
        private GamePhase phase = GamePhase.MainMenu;
        private CharacterType selectedCharacter = CharacterType.Survivor;
        private int currentHealth;
        private int currentLevel = 1;
        private int supplies;
        private int zombiesDefeated;
        private string message = "Find the security key and reach the exit.";
        private float messageUntil;
        private float damageFlashUntil;
        private Font displayFont;
        private Font bodyFont;
        private Font monoFont;

        public bool IsGameplayActive => phase == GamePhase.Playing;
        public bool IsGameOver => phase == GamePhase.Won || phase == GamePhase.Lost;
        public CharacterType SelectedCharacter => selectedCharacter;
        public int CurrentHealth => currentHealth;
        public int MaximumHealth => maximumHealth;
        public int CurrentLevel => currentLevel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            currentHealth = maximumHealth;
            InitializeFonts();
        }

        private void InitializeFonts()
        {
            // Dynamic system fonts keep this code-generated project self-contained while
            // giving the interface a much stronger identity than Unity's default typeface.
            displayFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Avenir Next Condensed", "Arial Narrow", "Helvetica Neue" }, 64);
            bodyFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Avenir Next", "Helvetica Neue", "Arial" }, 32);
            monoFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Menlo", "SF Mono", "Courier New" }, 24);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Configure(PrototypeBootstrap levelBuilder)
        {
            bootstrap = levelBuilder;
        }

        public void PrepareMainMenu()
        {
            phase = GamePhase.MainMenu;
            Time.timeScale = 0f;
            ApplySelectedCharacter();
        }

        private void Update()
        {
            if (phase == GamePhase.MainMenu)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(CharacterType.Survivor);
                if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(CharacterType.Scout);
                if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(CharacterType.Guardian);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) StartGame();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                string screenshotPath = Path.Combine(Application.persistentDataPath, "ZombieEscape.png");
                ScreenCapture.CaptureScreenshot(screenshotPath);
                ShowMessage("Screenshot saved.", 2f);
            }

            if (IsGameOver && Input.GetKeyDown(KeyCode.R))
            {
                ReturnToMenu();
            }
        }

        public void SelectCharacter(CharacterType characterType)
        {
            selectedCharacter = characterType;
            ApplySelectedCharacter();
        }

        private void ApplySelectedCharacter()
        {
            PlayerController2D player = FindFirstObjectByType<PlayerController2D>();
            if (player != null)
            {
                player.ApplyCharacter(selectedCharacter);
            }
        }

        public void StartGame()
        {
            currentLevel = 1;
            currentHealth = maximumHealth;
            supplies = 0;
            zombiesDefeated = 0;
            collectedKeys.Clear();
            phase = GamePhase.Playing;
            Time.timeScale = 1f;
            bootstrap.BuildLevel(currentLevel);
            ApplySelectedCharacter();
            ShowMessage("LEVEL 1: Find the red key and escape the offices.", 5f);
        }

        public void ReturnToMenu()
        {
            currentLevel = 1;
            currentHealth = maximumHealth;
            supplies = 0;
            zombiesDefeated = 0;
            collectedKeys.Clear();
            bootstrap.BuildLevel(currentLevel);
            PrepareMainMenu();
        }

        public void Collect(PickupType type, string itemId, int amount)
        {
            if (!IsGameplayActive)
            {
                return;
            }

            switch (type)
            {
                case PickupType.Key:
                    collectedKeys.Add(itemId);
                    ShowMessage("SECURITY KEY COLLECTED - the red door is ready.");
                    break;
                case PickupType.Health:
                    int previousHealth = currentHealth;
                    currentHealth = Mathf.Min(maximumHealth, currentHealth + amount);
                    ShowMessage($"Health restored: {previousHealth} -> {currentHealth}.");
                    break;
                case PickupType.Supplies:
                    supplies += amount;
                    ShowMessage($"Supplies collected: {supplies}.");
                    break;
            }
        }

        public bool HasKey(string keyId)
        {
            return collectedKeys.Contains(keyId);
        }

        public void DamagePlayer(int amount)
        {
            if (!IsGameplayActive)
            {
                return;
            }

            // Every completed zombie attack removes exactly one of the three health units.
            currentHealth = Mathf.Max(0, currentHealth - 1);
            damageFlashUntil = Time.unscaledTime + 0.22f;
            ShowMessage($"ZOMBIE HIT! Health remaining: {currentHealth}/{maximumHealth}.", 2.2f);

            if (currentHealth <= 0)
            {
                phase = GamePhase.Lost;
                Time.timeScale = 0f;
            }
        }

        public void RecordZombieDefeat()
        {
            zombiesDefeated++;
            ShowMessage($"Zombie defeated! Total: {zombiesDefeated}.");
        }

        public void ReachExit()
        {
            if (!IsGameplayActive)
            {
                return;
            }

            if (currentLevel == 1)
            {
                currentLevel = 2;
                currentHealth = maximumHealth;
                collectedKeys.Clear();
                bootstrap.BuildLevel(currentLevel);
                ApplySelectedCharacter();
                ShowMessage("LEVEL 2: The loading bay is darker and more dangerous.", 5f);
                return;
            }

            phase = GamePhase.Won;
            Time.timeScale = 0f;
        }

        public void ShowMessage(string text, float seconds = 3f)
        {
            message = text;
            messageUntil = Time.unscaledTime + seconds;
        }

        private string CharacterName()
        {
            switch (selectedCharacter)
            {
                case CharacterType.Scout: return "SCOUT";
                case CharacterType.Guardian: return "GUARDIAN";
                default: return "SURVIVOR";
            }
        }

        private void OnGUI()
        {
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            float offsetX = (Screen.width - 1280f * scale) * 0.5f;
            float offsetY = (Screen.height - 720f * scale) * 0.5f;
            Matrix4x4 previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(new Vector3(offsetX, offsetY, 0f), Quaternion.identity,
                new Vector3(scale, scale, 1f));

            if (phase == GamePhase.MainMenu)
            {
                DrawMainMenu();
            }
            else
            {
                DrawHud();
                if (phase == GamePhase.Won || phase == GamePhase.Lost)
                {
                    DrawResult();
                }
            }

            GUI.matrix = previousMatrix;
        }

        private void DrawMainMenu()
        {
            Color mint = new Color(0.18f, 0.94f, 0.69f);
            DrawBackdrop(mint);

            DrawText(new Rect(68, 43, 400, 24), "NIGHTSHIFT PROTOCOL  //  02", 13,
                new Color(0.48f, 0.62f, 0.64f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(804, 43, 408, 24), "CITY SECTOR 07  •  SIGNAL LOST  •  02:17 AM", 12,
                new Color(0.48f, 0.62f, 0.64f), TextAnchor.MiddleRight, FontStyle.Normal, monoFont);

            DrawTextShadow(new Rect(68, 90, 800, 96), "ZOMBIE", 76, Color.white,
                TextAnchor.MiddleLeft, displayFont, 4f);
            DrawTextShadow(new Rect(68, 155, 800, 96), "ESCAPE", 76, mint,
                TextAnchor.MiddleLeft, displayFont, 4f);
            FillRect(new Rect(72, 249, 610, 2), new Color(mint.r, mint.g, mint.b, 0.72f));
            DrawText(new Rect(72, 258, 610, 28), "TWO STAGES. THREE HEARTS. ONE WAY OUT.", 17,
                new Color(0.72f, 0.8f, 0.82f), TextAnchor.MiddleLeft, FontStyle.Bold, bodyFont);

            DrawPanel(new Rect(760, 88, 452, 170), mint, 0.78f);
            DrawText(new Rect(790, 112, 390, 24), "MISSION BRIEF", 14, mint,
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(790, 145, 382, 72),
                "Recover the security key. Break through the infected offices and loading bay. Reach extraction alive.",
                20, new Color(0.9f, 0.94f, 0.95f), TextAnchor.UpperLeft, FontStyle.Normal, bodyFont);
            DrawText(new Rect(790, 219, 382, 20), "THREAT LEVEL  //  SEVERE", 12,
                new Color(1f, 0.39f, 0.36f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);

            DrawText(new Rect(70, 317, 1140, 30), "SELECT YOUR SURVIVOR", 18, Color.white,
                TextAnchor.MiddleLeft, FontStyle.Bold, displayFont);
            DrawText(new Rect(992, 317, 220, 30), "PRESS  1  /  2  /  3", 11,
                new Color(0.46f, 0.58f, 0.62f), TextAnchor.MiddleRight, FontStyle.Bold, monoFont);

            DrawCharacterCard(new Rect(70, 358, 350, 190), CharacterType.Survivor,
                "01", "SURVIVOR", "BALANCED", "Reliable movement and reach", new Color(0.12f, 0.78f, 1f), 0.68f, 0.66f);
            DrawCharacterCard(new Rect(465, 358, 350, 190), CharacterType.Scout,
                "02", "SCOUT", "AGILE", "Fast movement, precise strike", new Color(0.65f, 0.35f, 1f), 0.94f, 0.52f);
            DrawCharacterCard(new Rect(860, 358, 350, 190), CharacterType.Guardian,
                "03", "GUARDIAN", "HEAVY", "Slower movement, wider strike", new Color(1f, 0.58f, 0.18f), 0.48f, 0.94f);

            Rect startButton = new Rect(70, 586, 480, 72);
            if (DrawButton(startButton, "BEGIN EVACUATION", "ENTER", mint))
            {
                StartGame();
            }

            DrawText(new Rect(585, 586, 625, 27), "FIELD CONTROLS", 11,
                new Color(0.42f, 0.55f, 0.58f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawControlHint(new Rect(585, 619, 176, 39), "WASD", "MOVE");
            DrawControlHint(new Rect(775, 619, 176, 39), "SPACE", "ATTACK");
            DrawControlHint(new Rect(965, 619, 245, 39), "P", "SCREENSHOT");
        }

        private void DrawCharacterCard(Rect rect, CharacterType type, string number, string name,
            string role, string description, Color accent, float speed, float reach)
        {
            bool selected = selectedCharacter == type;
            bool hovered = rect.Contains(Event.current.mousePosition);
            Color panelColor = selected
                ? new Color(accent.r * 0.15f, accent.g * 0.15f, accent.b * 0.15f, 0.98f)
                : hovered ? new Color(0.075f, 0.105f, 0.12f, 0.98f) : new Color(0.045f, 0.065f, 0.078f, 0.96f);

            FillRect(new Rect(rect.x + 5, rect.y + 7, rect.width, rect.height), new Color(0f, 0f, 0f, 0.38f));
            FillRect(rect, panelColor);
            StrokeRect(rect, selected ? accent : new Color(0.16f, 0.22f, 0.25f), selected ? 2f : 1f);
            FillRect(new Rect(rect.x, rect.y, selected ? 86f : 42f, 4f), accent);

            DrawText(new Rect(rect.x + 20, rect.y + 18, 42, 28), number, 14, accent,
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(rect.x + 66, rect.y + 17, rect.width - 86, 31), name, 26, Color.white,
                TextAnchor.MiddleLeft, FontStyle.Bold, displayFont);
            DrawText(new Rect(rect.x + 20, rect.y + 56, rect.width - 40, 20), role, 11,
                new Color(accent.r, accent.g, accent.b, 0.92f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(rect.x + 20, rect.y + 82, rect.width - 40, 26), description, 15,
                new Color(0.72f, 0.79f, 0.81f), TextAnchor.MiddleLeft, FontStyle.Normal, bodyFont);

            DrawStat(new Rect(rect.x + 20, rect.y + 126, 145, 18), "SPEED", speed, accent);
            DrawStat(new Rect(rect.x + 185, rect.y + 126, 145, 18), "REACH", reach, accent);

            if (selected)
            {
                DrawText(new Rect(rect.x + 20, rect.y + 157, rect.width - 40, 20), "●  ACTIVE OPERATIVE", 11,
                    Color.white, TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            }
            else
            {
                DrawText(new Rect(rect.x + 20, rect.y + 157, rect.width - 40, 20), "SELECT OPERATIVE", 10,
                    new Color(0.42f, 0.52f, 0.55f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            }

            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                SelectCharacter(type);
            }
        }

        private void DrawHud()
        {
            Color accent = currentLevel == 1 ? new Color(0.18f, 0.94f, 0.86f) : new Color(0.78f, 0.42f, 1f);

            // Matches the camera's reserved viewport margin. This solid command
            // strip makes the HUD feel intentional and keeps gameplay fully clear.
            FillRect(new Rect(0, 0, 1280, 151), new Color(0.008f, 0.016f, 0.021f, 1f));
            FillRect(new Rect(0, 149, 1280, 2), new Color(accent.r, accent.g, accent.b, 0.5f));

            if (Time.unscaledTime < damageFlashUntil)
            {
                FillRect(new Rect(0, 0, 1280, 720), new Color(0.8f, 0.015f, 0.025f, 0.25f));
                StrokeRect(new Rect(5, 5, 1270, 710), new Color(1f, 0.08f, 0.1f, 0.8f), 5f);
            }

            DrawPanel(new Rect(24, 22, 386, 118), accent, 0.9f);
            DrawText(new Rect(46, 36, 334, 20), $"SECTOR 0{currentLevel}  //  {CharacterName()}", 11, accent,
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(46, 58, 165, 26), "VITAL STATUS", 20, Color.white,
                TextAnchor.MiddleLeft, FontStyle.Bold, displayFont);
            DrawText(new Rect(345, 60, 40, 22), $"{currentHealth}/3", 15,
                currentHealth == 1 ? new Color(1f, 0.28f, 0.25f) : Color.white,
                TextAnchor.MiddleRight, FontStyle.Bold, monoFont);

            for (int i = 0; i < maximumHealth; i++)
            {
                Rect healthRect = new Rect(46 + i * 104, 92, 88, 18);
                bool active = i < currentHealth;
                FillRect(healthRect, active ? new Color(0.94f, 0.15f, 0.22f) : new Color(0.13f, 0.17f, 0.19f));
                FillRect(new Rect(healthRect.x + 4, healthRect.y + 4, healthRect.width - 8, 2),
                    active ? new Color(1f, 0.56f, 0.58f, 0.75f) : new Color(0.28f, 0.32f, 0.34f));
                StrokeRect(healthRect, active ? new Color(1f, 0.3f, 0.35f) : new Color(0.22f, 0.28f, 0.3f), 1f);
            }

            DrawPanel(new Rect(876, 22, 380, 72), accent, 0.9f);
            DrawHudDatum(new Rect(896, 36, 110, 44), "KEYCARD", HasKey("ExitKey") ? "SECURED" : "MISSING",
                HasKey("ExitKey") ? new Color(0.22f, 1f, 0.54f) : new Color(1f, 0.38f, 0.32f));
            DrawHudDatum(new Rect(1020, 36, 92, 44), "SUPPLIES", supplies.ToString("00"), Color.white);
            DrawHudDatum(new Rect(1126, 36, 108, 44), "INFECTED", zombiesDefeated.ToString("00"), Color.white);

            DrawText(new Rect(986, 105, 270, 22), "WASD  MOVE   •   SPACE  ATTACK", 10,
                new Color(0.72f, 0.8f, 0.82f), TextAnchor.MiddleRight, FontStyle.Bold, monoFont);

            if (Time.unscaledTime < messageUntil)
            {
                Rect toast = new Rect(335, 644, 610, 52);
                FillRect(new Rect(toast.x + 4, toast.y + 5, toast.width, toast.height), new Color(0f, 0f, 0f, 0.4f));
                FillRect(toast, new Color(0.025f, 0.045f, 0.052f, 0.97f));
                FillRect(new Rect(toast.x, toast.y, 5, toast.height), accent);
                StrokeRect(toast, new Color(accent.r, accent.g, accent.b, 0.36f), 1f);
                DrawText(new Rect(toast.x + 24, toast.y + 8, toast.width - 48, 36), message.ToUpperInvariant(), 13,
                    Color.white, TextAnchor.MiddleCenter, FontStyle.Bold, monoFont);
            }
        }

        private void DrawResult()
        {
            bool won = phase == GamePhase.Won;
            Color accent = won ? new Color(0.18f, 0.94f, 0.69f) : new Color(1f, 0.22f, 0.28f);
            FillRect(new Rect(0, 0, 1280, 720), new Color(0.008f, 0.014f, 0.018f, 0.94f));

            for (int i = 0; i < 10; i++)
            {
                FillRect(new Rect(0, 32 + i * 70, 1280, 1), new Color(accent.r, accent.g, accent.b, 0.045f));
            }

            DrawPanel(new Rect(300, 94, 680, 532), accent, 0.96f);
            DrawText(new Rect(340, 126, 600, 24), won ? "EVACUATION STATUS  //  CONFIRMED" : "VITAL STATUS  //  TERMINATED",
                12, accent, TextAnchor.MiddleCenter, FontStyle.Bold, monoFont);
            DrawTextShadow(new Rect(330, 169, 620, 100), won ? "EXTRACTED" : "OVERRUN", 65, Color.white,
                TextAnchor.MiddleCenter, displayFont, 3f);
            DrawText(new Rect(360, 273, 560, 48),
                won ? "Both sectors cleared. You made it beyond the quarantine line."
                    : "All three vital units were lost to the infected.",
                18, new Color(0.75f, 0.82f, 0.84f), TextAnchor.MiddleCenter, FontStyle.Normal, bodyFont);

            FillRect(new Rect(370, 344, 540, 1), new Color(accent.r, accent.g, accent.b, 0.35f));
            DrawResultStat(new Rect(380, 365, 150, 64), "SECTOR", currentLevel.ToString("00"), accent);
            DrawResultStat(new Rect(565, 365, 150, 64), "SUPPLIES", supplies.ToString("00"), accent);
            DrawResultStat(new Rect(750, 365, 150, 64), "INFECTED", zombiesDefeated.ToString("00"), accent);

            Rect menuButton = new Rect(410, 469, 460, 68);
            if (DrawButton(menuButton, "RETURN TO COMMAND", "R", accent))
            {
                ReturnToMenu();
            }
            DrawText(new Rect(400, 558, 480, 22), "PRESS R TO RETURN TO THE MAIN MENU", 10,
                new Color(0.42f, 0.55f, 0.58f), TextAnchor.MiddleCenter, FontStyle.Bold, monoFont);
        }

        private void DrawBackdrop(Color accent)
        {
            FillRect(new Rect(0, 0, 1280, 720), new Color(0.012f, 0.022f, 0.027f, 0.99f));
            FillRect(new Rect(0, 0, 18, 720), accent);
            FillRect(new Rect(18, 0, 3, 720), new Color(accent.r, accent.g, accent.b, 0.18f));

            for (int x = 65; x < 1280; x += 64)
            {
                FillRect(new Rect(x, 0, 1, 720), new Color(0.3f, 0.65f, 0.64f, 0.035f));
            }
            for (int y = 40; y < 720; y += 64)
            {
                FillRect(new Rect(0, y, 1280, 1), new Color(0.3f, 0.65f, 0.64f, 0.035f));
            }

            FillRect(new Rect(0, 692, 1280, 28), new Color(0f, 0f, 0f, 0.32f));
        }

        private void DrawPanel(Rect rect, Color accent, float opacity)
        {
            FillRect(new Rect(rect.x + 6, rect.y + 8, rect.width, rect.height), new Color(0f, 0f, 0f, 0.34f));
            FillRect(rect, new Color(0.025f, 0.043f, 0.051f, opacity));
            StrokeRect(rect, new Color(accent.r, accent.g, accent.b, 0.28f), 1f);
            FillRect(new Rect(rect.x, rect.y, 4, 28), accent);
            FillRect(new Rect(rect.x, rect.y, 34, 2), accent);
            FillRect(new Rect(rect.xMax - 18, rect.yMax - 2, 18, 2), new Color(accent.r, accent.g, accent.b, 0.6f));
        }

        private bool DrawButton(Rect rect, string label, string shortcut, Color accent)
        {
            bool hovered = rect.Contains(Event.current.mousePosition);
            FillRect(new Rect(rect.x + 6, rect.y + 7, rect.width, rect.height), new Color(0f, 0f, 0f, 0.46f));
            FillRect(rect, hovered ? Color.Lerp(accent, Color.white, 0.12f) : accent);
            FillRect(new Rect(rect.x, rect.y, 8, rect.height), Color.white);
            DrawText(new Rect(rect.x + 32, rect.y, rect.width - 112, rect.height), label, 21,
                new Color(0.015f, 0.04f, 0.04f), TextAnchor.MiddleLeft, FontStyle.Bold, displayFont);
            DrawText(new Rect(rect.xMax - 82, rect.y, 58, rect.height), $"[{shortcut}]", 12,
                new Color(0.02f, 0.12f, 0.1f), TextAnchor.MiddleRight, FontStyle.Bold, monoFont);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DrawControlHint(Rect rect, string key, string action)
        {
            FillRect(rect, new Color(0.04f, 0.065f, 0.075f, 0.94f));
            StrokeRect(rect, new Color(0.16f, 0.23f, 0.25f), 1f);
            DrawText(new Rect(rect.x + 11, rect.y, 55, rect.height), key, 11, Color.white,
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(rect.x + 65, rect.y, rect.width - 76, rect.height), action, 10,
                new Color(0.48f, 0.62f, 0.65f), TextAnchor.MiddleRight, FontStyle.Bold, monoFont);
        }

        private void DrawStat(Rect rect, string label, float value, Color accent)
        {
            DrawText(new Rect(rect.x, rect.y - 14, rect.width, 14), label, 9,
                new Color(0.48f, 0.58f, 0.61f), TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            FillRect(new Rect(rect.x, rect.y + 5, rect.width, 4), new Color(0.14f, 0.19f, 0.21f));
            FillRect(new Rect(rect.x, rect.y + 5, rect.width * value, 4), accent);
        }

        private void DrawHudDatum(Rect rect, string label, string value, Color color)
        {
            DrawText(new Rect(rect.x, rect.y, rect.width, 15), label, 9, new Color(0.48f, 0.59f, 0.62f),
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
            DrawText(new Rect(rect.x, rect.y + 17, rect.width, 24), value, 15, color,
                TextAnchor.MiddleLeft, FontStyle.Bold, monoFont);
        }

        private void DrawResultStat(Rect rect, string label, string value, Color accent)
        {
            DrawText(new Rect(rect.x, rect.y, rect.width, 20), label, 10,
                new Color(0.48f, 0.6f, 0.63f), TextAnchor.MiddleCenter, FontStyle.Bold, monoFont);
            DrawText(new Rect(rect.x, rect.y + 21, rect.width, 40), value, 28, accent,
                TextAnchor.MiddleCenter, FontStyle.Bold, displayFont);
        }

        private void DrawText(Rect rect, string text, int size, Color color, TextAnchor alignment,
            FontStyle fontStyle, Font font)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                font = font,
                fontSize = size,
                alignment = alignment,
                fontStyle = fontStyle,
                wordWrap = true,
                clipping = TextClipping.Clip,
                padding = new RectOffset(0, 0, 0, 0)
            };
            style.normal.textColor = color;
            GUI.Label(rect, text, style);
        }

        private void DrawTextShadow(Rect rect, string text, int size, Color color, TextAnchor alignment,
            Font font, float shadowOffset)
        {
            Rect shadow = new Rect(rect.x + shadowOffset, rect.y + shadowOffset, rect.width, rect.height);
            DrawText(shadow, text, size, new Color(0f, 0f, 0f, 0.72f), alignment, FontStyle.Bold, font);
            DrawText(rect, text, size, color, alignment, FontStyle.Bold, font);
        }

        private void StrokeRect(Rect rect, Color color, float thickness)
        {
            FillRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            FillRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            FillRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            FillRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void FillRect(Rect rect, Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = oldColor;
        }
    }
}
