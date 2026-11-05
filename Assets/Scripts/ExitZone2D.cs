using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ExitZone2D : MonoBehaviour
    {
        [SerializeField] private string requiredKey = "ExitKey";

        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController2D>() == null)
            {
                return;
            }

            if (GameManager.Instance.HasKey(requiredKey))
            {
                GameManager.Instance.ReachExit();
            }
            else
            {
                GameManager.Instance.ShowMessage("The exit scanner requires the security key.");
            }
        }
    }
}
