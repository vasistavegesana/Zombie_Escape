using UnityEngine;

namespace ZombieEscape
{
    // Keeps the objective visible without covering the keycard art itself.
    public class KeycardBeacon : MonoBehaviour
    {
        private Transform target;
        private SpriteRenderer spriteRenderer;
        private Vector3 baseScale;

        public void Configure(Transform keycard)
        {
            target = keycard;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = target.position;
            float wave = (Mathf.Sin(Time.time * 4.5f) + 1f) * 0.5f;
            transform.localScale = new Vector3(baseScale.x * Mathf.Lerp(0.82f, 1.14f, wave), baseScale.y, 1f);
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0.18f, 0.58f, wave);
            spriteRenderer.color = color;
        }
    }
}
