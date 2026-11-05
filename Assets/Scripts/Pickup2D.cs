using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Pickup2D : MonoBehaviour
    {
        [SerializeField] private PickupType pickupType;
        [SerializeField] private string itemId = "Item";
        [SerializeField] private int amount = 1;
        private Vector3 baseScale;
        private Vector3 basePosition;

        public void Configure(PickupType type, string id, int pickupAmount)
        {
            pickupType = type;
            itemId = id;
            amount = pickupAmount;
        }

        private void Awake()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
            baseScale = transform.localScale;
            basePosition = transform.position;
        }

        private void Update()
        {
            float wave = (Mathf.Sin(Time.time * 3.8f) + 1f) * 0.5f;
            transform.position = basePosition + Vector3.up * Mathf.Lerp(-0.06f, 0.1f, wave);
            transform.localScale = baseScale * Mathf.Lerp(0.96f, 1.05f, wave);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-4f, 4f, wave));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController2D>() == null || GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.Collect(pickupType, itemId, amount);
            Destroy(gameObject);
        }
    }
}
