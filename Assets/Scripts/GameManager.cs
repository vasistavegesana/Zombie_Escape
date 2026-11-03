using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombieEscape
{
    // Centralizes the prototype's inventory, health, feedback, and win/lose state.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int maximumHealth = 5;

        private readonly HashSet<string> collectedKeys = new HashSet<string>();
        private int currentHealth;
        private int supplies;
        private int zombiesDefeated;
        private string message = "Find the red key, unlock the security door, and reach the exit.";
        private float messageUntil;
        private bool gameOver;
        private bool won;

        public bool IsGameOver => gameOver;
        public int CurrentHealth => currentHealth;
        public int MaximumHealth => maximumHealth;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            currentHealth = maximumHealth;
            messageUntil = Time.time + 8f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                string screenshotPath = Path.Combine(Application.persistentDataPath, "ZombieEscapePrototype.png");
                ScreenCapture.CaptureScreenshot(screenshotPath);
                ShowMessage("Prototype screenshot saved.", 2f);
            }

            if (gameOver && Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void Collect(PickupType type, string itemId, int amount)
        {
            if (gameOver)
            {
                return;
            }

            switch (type)
            {
                case PickupType.Key:
                    collectedKeys.Add(itemId);
                    ShowMessage("Security key collected. The red door can now be opened.");
                    break;
                case PickupType.Health:
                    int previousHealth = currentHealth;
                    currentHealth = Mathf.Min(maximumHealth, currentHealth + amount);
                    ShowMessage($"Health restored: {previousHealth} -> {currentHealth}.");
                    break;
                case PickupType.Supplies:
                    supplies += amount;
                    ShowMessage($"Supplies collected. Total supplies: {supplies}.");
                    break;
            }
        }

        public bool HasKey(string keyId)
        {
            return collectedKeys.Contains(keyId);
        }

        public void DamagePlayer(int amount)
        {
            if (gameOver)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);
            ShowMessage($"A zombie hit you. Health remaining: {currentHealth}.");

            if (currentHealth == 0)
            {
                gameOver = true;
                won = false;
            }
        }

        public void RecordZombieDefeat()
        {
            zombiesDefeated++;
            ShowMessage($"Zombie defeated. Total defeated: {zombiesDefeated}.");
        }

        public void Win()
        {
            if (gameOver)
            {
                return;
            }

            gameOver = true;
            won = true;
        }

        public void ShowMessage(string text, float seconds = 3f)
        {
            message = text;
            messageUntil = Time.time + seconds;
        }

        private void OnGUI()
        {
            GUIStyle panel = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 18,
                normal = { textColor = Color.white }
            };

            GUIStyle title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            string keyStatus = HasKey("ExitKey") ? "COLLECTED" : "MISSING";
            GUI.Box(new Rect(16, 16, 340, 122),
                $"ZOMBIE ESCAPE\nHealth: {currentHealth}/{maximumHealth}\nExit key: {keyStatus}\nSupplies: {supplies}   Zombies defeated: {zombiesDefeated}", panel);

            GUI.Box(new Rect(Screen.width - 320, 16, 304, 122),
                "CONTROLS\nMove: WASD / Arrow Keys\nAttack: Space\nRestart after finish: R", panel);

            if (!gameOver && Time.time < messageUntil)
            {
                GUI.Box(new Rect(Screen.width / 2f - 310, Screen.height - 68, 620, 48), message, panel);
            }

            if (!gameOver)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.82f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);
            GUI.color = previous;

            string result = won
                ? "YOU ESCAPED\nThe prototype gameplay loop is complete."
                : "YOU WERE OVERRUN\nUse supplies carefully and try another route.";
            GUI.Label(new Rect(Screen.width / 2f - 360, Screen.height / 2f - 90, 720, 120), result, title);
            GUI.Label(new Rect(Screen.width / 2f - 180, Screen.height / 2f + 45, 360, 40),
                "Press R to restart", title);
        }
    }
}
