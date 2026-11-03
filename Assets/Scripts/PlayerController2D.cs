using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.5f;
        [SerializeField] private float attackDistance = 0.9f;
        [SerializeField] private float attackRadius = 0.75f;
        [SerializeField] private float attackCooldown = 0.35f;

        private Rigidbody2D body;
        private Vector2 movement;
        private Vector2 facing = Vector2.right;
        private float nextAttackTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            {
                movement = Vector2.zero;
                return;
            }

            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movement = Vector2.ClampMagnitude(movement, 1f);

            if (movement.sqrMagnitude > 0.01f)
            {
                facing = movement.normalized;
                transform.up = facing;
            }

            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        private void FixedUpdate()
        {
            if (movement.sqrMagnitude > 0f)
            {
                body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
        }

        private void Attack()
        {
            nextAttackTime = Time.time + attackCooldown;
            Vector2 center = body.position + facing * attackDistance;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius);
            bool hitZombie = false;

            foreach (Collider2D hit in hits)
            {
                ZombieController2D zombie = hit.GetComponent<ZombieController2D>();
                if (zombie != null)
                {
                    zombie.TakeDamage(1);
                    hitZombie = true;
                }
            }

            if (!hitZombie)
            {
                GameManager.Instance.ShowMessage("Your attack missed.", 1.1f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere((Vector2)transform.position + facing * attackDistance, attackRadius);
        }
    }
}
