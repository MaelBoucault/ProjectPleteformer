using UnityEngine;

public class ProjectilEnemy : MonoBehaviour
{
    public float AmountDamage;
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(-AmountDamage, gameObject.transform.position);
        }
    }
}
