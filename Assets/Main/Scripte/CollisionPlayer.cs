using UnityEngine;

public class CollisionPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fall")
        {
            gameObject.GetComponent<PlayerHealth>().UpdateHealth(-gameObject.GetComponent<PlayerHealth>().MaxHealth);
        }

    }
}
