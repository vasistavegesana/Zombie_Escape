using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class ZombieController2D : MonoBehaviour
    {
        [SerializeField] private float chaseRange = 7f;
        [SerializeField] private float moveSpeed = 2.25f;
        [SerializeField] private float attackRange = 0.8f;
        [SerializeField] private float attackCooldown = 1.1f;
        [SerializeField] private int health = 3;

        private Rigidbody2D body;
        private Transform player;
        private float nextAttackTime;

        public void Configure(float speed)
        {
            moveSpeed = speed;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Start()
        {
            PlayerController2D controller = FindFirstObjectByType<PlayerController2D>();
            if (controller != null)
            {
                player = controller.transform;
            }
        }

        private void FixedUpdate()
        {
            if (player == null || GameManager.Instance == null || GameManager.Instance.IsGameOver)
            {
                return;
            }

            Vector2 offset = (Vector2)player.position - body.position;
            float distance = offset.magnitude;

            // Deliverable #2 uses an intentionally simple placeholder chase.
            // A* pathfinding and the full state machine are reserved for the final build.
            if (distance <= chaseRange && distance > attackRange)
            {
                Vector2 step = offset.normalized * moveSpeed * Time.fixedDeltaTime;
                body.MovePosition(body.position + step);
                transform.up = offset.normalized;
            }

            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                GameManager.Instance.DamagePlayer(1);
            }
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                GameManager.Instance.RecordZombieDefeat();
                Destroy(gameObject);
                return;
            }

            GameManager.Instance.ShowMessage($"Zombie hit. {health} health remaining.", 1.2f);
        }
    }
}
