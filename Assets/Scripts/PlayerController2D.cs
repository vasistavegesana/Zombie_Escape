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
        private SpriteRenderer spriteRenderer;
        private Vector2 movement;
        private Vector2 facing = Vector2.right;
        private Vector3 baseScale;
        private float nextAttackTime;
        private Sprite survivorSprite;
        private Sprite scoutSprite;
        private Sprite guardianSprite;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void ConfigureArt(Sprite survivor, Sprite scout, Sprite guardian)
        {
            survivorSprite = survivor;
            scoutSprite = scout;
            guardianSprite = guardian;
        }

        public void ApplyCharacter(CharacterType characterType)
        {
            transform.localScale = baseScale;

            switch (characterType)
            {
                case CharacterType.Scout:
                    moveSpeed = 6.8f;
                    attackRadius = 0.65f;
                    transform.localScale = new Vector3(baseScale.x * 0.78f, baseScale.y * 1.08f, 1f);
                    spriteRenderer.sprite = scoutSprite != null ? scoutSprite : survivorSprite;
                    spriteRenderer.color = Color.white;
                    break;
                case CharacterType.Guardian:
                    moveSpeed = 4.8f;
                    attackRadius = 1.05f;
                    transform.localScale = new Vector3(baseScale.x * 1.25f, baseScale.y * 1.05f, 1f);
                    spriteRenderer.sprite = guardianSprite != null ? guardianSprite : survivorSprite;
                    spriteRenderer.color = Color.white;
                    break;
                default:
                    moveSpeed = 5.5f;
                    attackRadius = 0.75f;
                    spriteRenderer.sprite = survivorSprite != null ? survivorSprite : spriteRenderer.sprite;
                    spriteRenderer.color = Color.white;
                    break;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
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
