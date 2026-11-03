using UnityEngine;

namespace ZombieEscape
{
    // Builds the prototype environment from original, code-generated shapes.
    // This keeps the submission reproducible and avoids reliance on third-party art assets.
    public class PrototypeBootstrap : MonoBehaviour
    {
        private Sprite squareSprite;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            squareSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f), 1f);

            CreateCamera();
            new GameObject("Game Manager").AddComponent<GameManager>();
            CreateEnvironment();
            CreateGameplayObjects();
        }

        private void CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera gameCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 8.2f;
            gameCamera.backgroundColor = new Color(0.025f, 0.03f, 0.04f);
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void CreateEnvironment()
        {
            CreateVisual("Floor", Vector2.zero, new Vector2(26f, 14f), new Color(0.13f, 0.15f, 0.17f), -10);

            Color wallColor = new Color(0.3f, 0.34f, 0.38f);
            CreateWall("North Wall", new Vector2(0f, 7f), new Vector2(26.6f, 0.5f), wallColor);
            CreateWall("South Wall", new Vector2(0f, -7f), new Vector2(26.6f, 0.5f), wallColor);
            CreateWall("West Wall", new Vector2(-13f, 0f), new Vector2(0.5f, 14.5f), wallColor);
            CreateWall("East Wall", new Vector2(13f, 0f), new Vector2(0.5f, 14.5f), wallColor);

            // A full-height divider creates two rooms with one locked passage.
            CreateWall("Divider Lower", new Vector2(-3f, -4f), new Vector2(0.45f, 6f), wallColor);
            CreateWall("Divider Upper", new Vector2(-3f, 4f), new Vector2(0.45f, 6f), wallColor);

            // Furniture creates recognizable hallways and cover without blocking the critical route.
            Color furniture = new Color(0.35f, 0.23f, 0.15f);
            CreateWall("Desk A", new Vector2(-9f, 1.8f), new Vector2(2.8f, 0.8f), furniture);
            CreateWall("Desk B", new Vector2(-7.2f, -2.5f), new Vector2(0.8f, 2.4f), furniture);
            CreateWall("Shelves A", new Vector2(2.2f, 3.5f), new Vector2(3.8f, 0.7f), furniture);
            CreateWall("Shelves B", new Vector2(5.8f, -3.8f), new Vector2(0.7f, 3.2f), furniture);
            CreateWall("Barricade", new Vector2(8.2f, 1f), new Vector2(3.2f, 0.65f), furniture);

            CreateLabel("START", new Vector2(-10.5f, -5.9f), Color.cyan);
            CreateLabel("KEY", new Vector2(-9.2f, 5.7f), new Color(1f, 0.35f, 0.25f));
            CreateLabel("LOCKED SECURITY DOOR", new Vector2(-3f, 1f), new Color(1f, 0.45f, 0.35f));
            CreateLabel("EXIT", new Vector2(10.5f, 5.8f), new Color(0.35f, 1f, 0.45f));
        }

        private void CreateGameplayObjects()
        {
            GameObject player = CreateActor("Player", new Vector2(-10.5f, -5f), new Vector2(0.75f, 1f),
                new Color(0.15f, 0.78f, 1f), 10);
            player.AddComponent<PlayerController2D>();

            GameObject door = CreateVisual("Locked Security Door", new Vector2(-3f, 0f),
                new Vector2(0.65f, 2f), new Color(0.85f, 0.16f, 0.12f), 5);
            door.AddComponent<BoxCollider2D>();
            door.AddComponent<LockedDoor2D>();

            CreatePickup("Security Key", new Vector2(-9.2f, 5f), PickupType.Key, "ExitKey", 1,
                new Color(1f, 0.17f, 0.12f));
            CreatePickup("Health Pack", new Vector2(-5f, -5.3f), PickupType.Health, "Health", 2,
                new Color(0.18f, 1f, 0.35f));
            CreatePickup("Supplies", new Vector2(3f, -4.8f), PickupType.Supplies, "Supplies", 1,
                new Color(1f, 0.75f, 0.12f));
            CreatePickup("Supplies 2", new Vector2(9f, -2f), PickupType.Supplies, "Supplies", 1,
                new Color(1f, 0.75f, 0.12f));

            CreateZombie("Zombie Left", new Vector2(-5.3f, 3.7f), 1.9f, new Color(0.55f, 0.95f, 0.25f));
            CreateZombie("Zombie Hall", new Vector2(1.5f, -1.5f), 2.2f, new Color(0.45f, 0.85f, 0.2f));
            CreateZombie("Fast Zombie", new Vector2(8.2f, 3.3f), 2.8f, new Color(0.75f, 1f, 0.25f));

            GameObject exit = CreateVisual("Escape Zone", new Vector2(11.2f, 5.2f), new Vector2(2f, 2.2f),
                new Color(0.1f, 0.85f, 0.28f, 0.65f), 1);
            exit.AddComponent<BoxCollider2D>();
            exit.AddComponent<ExitZone2D>();
        }

        private GameObject CreateVisual(string objectName, Vector2 position, Vector2 size, Color color, int order)
        {
            GameObject created = new GameObject(objectName);
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

            // The three enemies share the same placeholder behavior; scale distinguishes the faster type.
            if (speed > 2.5f)
            {
                zombie.transform.localScale *= 0.85f;
            }
        }

        private void CreateLabel(string text, Vector2 position, Color color)
        {
            GameObject labelObject = new GameObject($"Label {text}");
            labelObject.transform.position = new Vector3(position.x, position.y, -0.2f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = text;
            label.fontSize = 44;
            label.characterSize = 0.08f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = color;
            labelObject.GetComponent<MeshRenderer>().sortingOrder = 20;
        }
    }
}
