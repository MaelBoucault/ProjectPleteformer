using UnityEngine;
using System.Collections;

public class MagicRay : MonoBehaviour
{
    public float damage = 2f;
    public float damageInterval = 0.5f;
    public float lifeTime = 30f;

    private bool playerInside = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerInside = true;
                StartCoroutine(DealDamageOverTime());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerHealth = null;
            StopAllCoroutines();
        }
    }

    IEnumerator DealDamageOverTime()
    {
        while (playerInside && playerHealth != null)
        {
            playerHealth.UpdateHealth(-damage, new Vector3(0, 0, 0) );
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
