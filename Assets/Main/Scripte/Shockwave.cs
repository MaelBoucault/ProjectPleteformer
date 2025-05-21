using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float damage = 10f;
    public float radius = 5f;
    public float lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Ennemies"))
            {
                enemy.GetComponent<EnnemieHealth>().UpdateHealth(-damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
