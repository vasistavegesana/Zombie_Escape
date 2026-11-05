using UnityEngine;

namespace ZombieEscape
{
    // Builds two complete levels from original code-generated shapes.
    public class PrototypeBootstrap : MonoBehaviour
    {
        private Sprite squareSprite;
        private GameObject levelRoot;
        private GameManager gameManager;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            squareSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f), 1f);

            CreateCamera();
            gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
            gameManager.Configure(this);
            BuildLevel(1);
            gameManager.PrepareMainMenu();
        }

        public void BuildLevel(int levelNumber)
        {
            if (levelRoot != null)
            {
                levelRoot.SetActive(false);
                Destroy(levelRoot);
            }

            levelRoot = new GameObject($"Level {levelNumber}");
            CreateBoundaries(levelNumber);

            if (levelNumber == 2)
            {
                CreateLevelTwoEnvironment();
                CreateLevelTwoGameplay();
            }
            else
            {
                CreateLevelOneEnvironment();
                CreateLevelOneGameplay();
            }
        }

        private void CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera gameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 8.2f;
            gameCamera.backgroundColor = new Color(0.012f, 0.018f, 0.028f);
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void CreateBoundaries(int levelNumber)
        {
            Color floor = levelNumber == 1
                ? new Color(0.09f, 0.12f, 0.15f)
                : new Color(0.075f, 0.085f, 0.12f);
            Color wall = levelNumber == 1
                ? new Color(0.25f, 0.31f, 0.36f)
                : new Color(0.27f, 0.22f, 0.34f);

            CreateVisual("Floor", Vector2.zero, new Vector2(26f, 14f), floor, -10);
            CreateVisual("Floor Accent Top", new Vector2(0f, 5.9f), new Vector2(25.5f, 0.13f),
                levelNumber == 1 ? new Color(0.08f, 0.5f, 0.52f) : new Color(0.46f, 0.2f, 0.58f), -8);
            CreateVisual("Floor Accent Bottom", new Vector2(0f, -5.9f), new Vector2(25.5f, 0.13f),
                levelNumber == 1 ? new Color(0.08f, 0.5f, 0.52f) : new Color(0.46f, 0.2f, 0.58f), -8);

            CreateWall("North Wall", new Vector2(0f, 7f), new Vector2(26.6f, 0.5f), wall);
            CreateWall("South Wall", new Vector2(0f, -7f), new Vector2(26.6f, 0.5f), wall);
            CreateWall("West Wall", new Vector2(-13f, 0f), new Vector2(0.5f, 14.5f), wall);
            CreateWall("East Wall", new Vector2(13f, 0f), new Vector2(0.5f, 14.5f), wall);
        }

        private void CreateLevelOneEnvironment()
        {
            Color wall = new Color(0.25f, 0.31f, 0.36f);
            Color furniture = new Color(0.35f, 0.22f, 0.14f);
            Color glow = new Color(0.08f, 0.4f, 0.42f, 0.42f);

            CreateWall("Office Divider Lower", new Vector2(-3f, -4f), new Vector2(0.45f, 6f), wall);
            CreateWall("Office Divider Upper", new Vector2(-3f, 4f), new Vector2(0.45f, 6f), wall);
            CreateWall("Reception Desk", new Vector2(-9f, 1.8f), new Vector2(2.8f, 0.8f), furniture);
            CreateWall("File Cabinet", new Vector2(-7.2f, -2.5f), new Vector2(0.8f, 2.4f), furniture);
            CreateWall("Shelves A", new Vector2(2.2f, 3.5f), new Vector2(3.8f, 0.7f), furniture);
            CreateWall("Shelves B", new Vector2(5.8f, -3.8f), new Vector2(0.7f, 3.2f), furniture);
            CreateWall("Barricade", new Vector2(8.2f, 1f), new Vector2(3.2f, 0.65f), furniture);
            CreateVisual("Emergency Light A", new Vector2(-10.8f, 5.8f), new Vector2(2.2f, 0.22f), glow, -4);
            CreateVisual("Emergency Light B", new Vector2(9.8f, -5.8f), new Vector2(2.2f, 0.22f), glow, -4);

            CreateLabel("LEVEL 1  -  ABANDONED OFFICES", new Vector2(0f, 6.25f), new Color(0.35f, 0.9f, 0.92f), 30);
            CreateLabel("START", new Vector2(-10.5f, -5.9f), Color.cyan, 42);
            CreateLabel("KEY", new Vector2(-9.2f, 5.7f), new Color(1f, 0.35f, 0.25f), 42);
            CreateLabel("LOCKED SECURITY DOOR", new Vector2(-3f, 1.1f), new Color(1f, 0.45f, 0.35f), 34);
            CreateLabel("EXIT", new Vector2(10.5f, 5.8f), new Color(0.35f, 1f, 0.45f), 42);
        }

        private void CreateLevelOneGameplay()
        {
            CreatePlayer(new Vector2(-10.5f, -5f));
            CreateDoor(new Vector2(-3f, 0f), new Vector2(0.65f, 2f));
            CreatePickup("Security Key", new Vector2(-9.2f, 5f), PickupType.Key, "ExitKey", 1,
                new Color(1f, 0.17f, 0.12f));
            CreatePickup("Health Pack", new Vector2(-5f, -5.3f), PickupType.Health, "Health", 1,
                new Color(0.18f, 1f, 0.35f));
            CreatePickup("Supplies", new Vector2(3f, -4.8f), PickupType.Supplies, "Supplies", 1,
                new Color(1f, 0.75f, 0.12f));
            CreatePickup("Supplies 2", new Vector2(9f, -2f), PickupType.Supplies, "Supplies", 1,
                new Color(1f, 0.75f, 0.12f));

            CreateZombie("Office Zombie", new Vector2(-5.3f, 3.7f), 1.9f, new Color(0.55f, 0.95f, 0.25f));
            CreateZombie("Hall Zombie", new Vector2(1.5f, -1.5f), 2.2f, new Color(0.45f, 0.85f, 0.2f));
            CreateZombie("Fast Zombie", new Vector2(8.2f, 3.3f), 2.8f, new Color(0.75f, 1f, 0.25f));
            CreateExit(new Vector2(11.2f, 5.2f));
        }

        private void CreateLevelTwoEnvironment()
        {
            Color wall = new Color(0.27f, 0.22f, 0.34f);
            Color crate = new Color(0.32f, 0.24f, 0.17f);
            Color hazard = new Color(0.75f, 0.48f, 0.08f);

            CreateWall("Bay Divider Lower", new Vector2(3f, -4.5f), new Vector2(0.45f, 5f), wall);
            CreateWall("Bay Divider Upper", new Vector2(3f, 3.5f), new Vector2(0.45f, 7f), wall);
            CreateWall("Cargo Stack A", new Vector2(-8.3f, 1.4f), new Vector2(3.3f, 0.85f), crate);
            CreateWall("Cargo Stack B", new Vector2(-5.3f, -3.6f), new Vector2(0.9f, 3.3f), crate);
            CreateWall("Cargo Stack C", new Vector2(-0.5f, 2.8f), new Vector2(3.4f, 0.8f), crate);
            CreateWall("Cargo Stack D", new Vector2(7.2f, 2.2f), new Vector2(0.8f, 3.2f), crate);
            CreateWall("Cargo Stack E", new Vector2(8.5f, -3.1f), new Vector2(3.5f, 0.8f), crate);
            CreateWall("Hazard Barrier A", new Vector2(-9f, -1.3f), new Vector2(2.4f, 0.28f), hazard);
            CreateWall("Hazard Barrier B", new Vector2(6.5f, 5f), new Vector2(2.4f, 0.28f), hazard);

            CreateLabel("LEVEL 2  -  NIGHT LOADING BAY", new Vector2(0f, 6.25f), new Color(0.84f, 0.45f, 1f), 30);
            CreateLabel("START", new Vector2(-10.5f, 5.8f), new Color(0.65f, 0.4f, 1f), 42);
            CreateLabel("KEY", new Vector2(-0.5f, -5.8f), new Color(1f, 0.35f, 0.25f), 42);
            CreateLabel("FINAL SECURITY DOOR", new Vector2(3f, 0.3f), new Color(1f, 0.45f, 0.35f), 32);
            CreateLabel("FINAL EXIT", new Vector2(10.5f, -5.8f), new Color(0.35f, 1f, 0.45f), 42);
        }

        private void CreateLevelTwoGameplay()
        {
            CreatePlayer(new Vector2(-10.5f, 5f));
            CreateDoor(new Vector2(3f, -1f), new Vector2(0.65f, 2f));
            CreatePickup("Final Security Key", new Vector2(-0.5f, -5f), PickupType.Key, "ExitKey", 1,
                new Color(1f, 0.12f, 0.2f));
            CreatePickup("Emergency Health", new Vector2(-10.5f, -4.8f), PickupType.Health, "Health", 1,
                new Color(0.18f, 1f, 0.35f));
            CreatePickup("Bay Supplies", new Vector2(5.4f, 5.2f), PickupType.Supplies, "Supplies", 2,
                new Color(1f, 0.75f, 0.12f));

            CreateZombie("Dock Zombie A", new Vector2(-7f, 4f), 2.25f, new Color(0.55f, 0.92f, 0.25f));
            CreateZombie("Dock Zombie B", new Vector2(-2.5f, 0f), 2.35f, new Color(0.45f, 0.85f, 0.2f));
            CreateZombie("Dock Zombie C", new Vector2(-1.8f, -4.3f), 2.5f, new Color(0.65f, 0.95f, 0.25f));
            CreateZombie("Dock Zombie D", new Vector2(6f, -1f), 2.6f, new Color(0.58f, 0.9f, 0.18f));
            CreateZombie("Dock Runner", new Vector2(10f, 3.7f), 3f, new Color(0.82f, 1f, 0.25f));
            CreateExit(new Vector2(11.2f, -5.2f));
        }

        private void CreatePlayer(Vector2 position)
        {
            GameObject player = CreateActor("Player", position, new Vector2(0.75f, 1f),
                new Color(0.12f, 0.78f, 1f), 10);
            PlayerController2D controller = player.AddComponent<PlayerController2D>();
            if (gameManager != null)
            {
                controller.ApplyCharacter(gameManager.SelectedCharacter);
            }
        }

        private void CreateDoor(Vector2 position, Vector2 size)
        {
            GameObject door = CreateVisual("Locked Security Door", position, size,
                new Color(0.9f, 0.13f, 0.12f), 5);
            door.AddComponent<BoxCollider2D>();
            door.AddComponent<LockedDoor2D>();
        }

        private void CreateExit(Vector2 position)
        {
            GameObject exit = CreateVisual("Escape Zone", position, new Vector2(2f, 2.2f),
                new Color(0.1f, 0.85f, 0.28f, 0.7f), 1);
            exit.AddComponent<BoxCollider2D>();
            exit.AddComponent<ExitZone2D>();
        }

        private GameObject CreateVisual(string objectName, Vector2 position, Vector2 size, Color color, int order)
        {
            GameObject created = new GameObject(objectName);
            created.transform.SetParent(levelRoot.transform);
            created.transform.position = position;
            created.transform.localScale = size;
            SpriteRenderer renderer = created.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = order;
            return created;
        }

        private GameObject CreateActor(string objectName, Vector2 position, Vector2 size, Color color, int order)
        {
            GameObject actor = CreateVisual(objectName, position, size, color, order);
            actor.AddComponent<Rigidbody2D>();
            actor.AddComponent<CircleCollider2D>();
            return actor;
        }

        private void CreateWall(string objectName, Vector2 position, Vector2 size, Color color)
        {
            GameObject wall = CreateVisual(objectName, position, size, color, 0);
            wall.AddComponent<BoxCollider2D>();
        }

        private void CreatePickup(string objectName, Vector2 position, PickupType type, string itemId, int amount, Color color)
        {
            GameObject pickup = CreateVisual(objectName, position, new Vector2(0.55f, 0.55f), color, 8);
            pickup.AddComponent<CircleCollider2D>();
            Pickup2D component = pickup.AddComponent<Pickup2D>();
            component.Configure(type, itemId, amount);
        }

        private void CreateZombie(string objectName, Vector2 position, float speed, Color color)
        {
            GameObject zombie = CreateActor(objectName, position, new Vector2(0.82f, 1.05f), color, 9);
            ZombieController2D controller = zombie.AddComponent<ZombieController2D>();
            controller.Configure(speed);
            if (speed > 2.7f)
            {
                zombie.transform.localScale *= 0.84f;
            }
        }

        private void CreateLabel(string text, Vector2 position, Color color, int fontSize)
        {
            GameObject labelObject = new GameObject($"Label {text}");
            labelObject.transform.SetParent(levelRoot.transform);
            labelObject.transform.position = new Vector3(position.x, position.y, -0.2f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = text;
            label.fontSize = fontSize;
            label.characterSize = 0.08f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = color;
            labelObject.GetComponent<MeshRenderer>().sortingOrder = 20;
        }
    }
}
