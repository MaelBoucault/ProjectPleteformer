using UnityEngine;
using System.Collections;

public class ZoneDeSoin : MonoBehaviour
{
    public float healAmount = 5f;
    public float healInterval = 1f;
    public float duration = 5f;

    private void Start()
    {
        StartCoroutine(HealRoutine());
    }

    IEnumerator HealRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 3f); 

            foreach (Collider2D col in enemies)
            {
                if (col.CompareTag("Ennemies"))
                {
                    EnnemieHealth ennemi = col.GetComponent<EnnemieHealth>();
                    if (ennemi != null)
                    {
                        ennemi.UpdateHealth(healAmount);
                    }
                }
            }

            yield return new WaitForSeconds(healInterval);
            timer += healInterval;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
