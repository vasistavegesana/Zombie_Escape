using UnityEngine;

namespace ZombieEscape
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LockedDoor2D : MonoBehaviour
    {
        [SerializeField] private string requiredKey = "ExitKey";
        private bool opened;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (opened || collision.collider.GetComponent<PlayerController2D>() == null)
            {
                return;
            }

            if (GameManager.Instance.HasKey(requiredKey))
            {
                opened = true;
                GameManager.Instance.ShowMessage("Security door unlocked.");
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.8f, 0.35f, 0.28f);
            }
            else
            {
                GameManager.Instance.ShowMessage("The security door is locked. Find the red key.");
            }
        }
    }
}
