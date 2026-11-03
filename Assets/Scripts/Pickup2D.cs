using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Pickup2D : MonoBehaviour
    {
        [SerializeField] private PickupType pickupType;
        [SerializeField] private string itemId = "Item";
        [SerializeField] private int amount = 1;

        public void Configure(PickupType type, string id, int pickupAmount)
        {
            pickupType = type;
            itemId = id;
            amount = pickupAmount;
        }

        private void Awake()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        private void Update()
        {
            transform.Rotate(0f, 0f, 75f * Time.deltaTime);
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
