using UnityEngine;
using System.Collections;

public class BardeScripte : MonoBehaviour
{
    public float soinRange = 5f;
    public float healAmount = 5f;
    public float healInterval = 1f;
    public GameObject healEffectPrefab; // FX à instancier sur les ennemis

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= healInterval)
        {
            SoignerAllies();
            timer = 0f;
        }
    }

    void SoignerAllies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soinRange);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Ennemies"))
            {
                EnnemieHealth ennemi = col.GetComponent<EnnemieHealth>();
                if (ennemi != null && ennemi.health < ennemi.maxHealth)
                {
                    ennemi.UpdateHealth(healAmount);

                    // Particules de soin
                    if (healEffectPrefab != null)
                    {
                        Instantiate(healEffectPrefab, col.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    // Visualiser la zone de soin dans l'éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, soinRange);
    }
}
