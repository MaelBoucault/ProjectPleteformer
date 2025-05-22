using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BardeScripte : MonoBehaviour
{
    public float soinRange = 5f;
    public float healAmount = 5f;
    public float healInterval = 1f;
    public GameObject healEffectPrefab;

    public GameObject Aurra;
    public GameObject BigAurra;

    private SpriteRenderer SpriteRendererAurra;
    private bool isHealing = false;

    private void Start()
    {
        BigAurra.SetActive(false);
        SpriteRendererAurra = Aurra.GetComponent<SpriteRenderer>();
        SetAuraAlpha(0.5f); // Valeur par défaut
        StartCoroutine(HealingLoop());
    }

    IEnumerator HealingLoop()
    {
        while (true)
        {
            bool healedSomeone = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soinRange);

            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Ennemies"))
                {
                    EnnemieHealth ennemi = col.GetComponent<EnnemieHealth>();
                    if (ennemi != null && ennemi.health < ennemi.maxHealth)
                    {
                        // Appliquer le soin
                        ennemi.UpdateHealth(healAmount);
                        healedSomeone = true;

                        // FX de soin
                        if (healEffectPrefab != null)
                        {
                            Instantiate(healEffectPrefab, col.transform.position, Quaternion.identity);
                        }
                    }
                }
            }

            // Affichage des auras
            if (healedSomeone)
            {
                if (!isHealing)
                {
                    SetAuraAlpha(1f);
                    BigAurra.SetActive(true);
                    isHealing = true;
                }
            }
            else
            {
                if (isHealing)
                {
                    SetAuraAlpha(0.5f);
                    BigAurra.SetActive(false);
                    isHealing = false;
                }
            }

            yield return new WaitForSeconds(healInterval);
        }
    }

    void SetAuraAlpha(float alpha)
    {
        if (SpriteRendererAurra != null)
        {
            Color c = SpriteRendererAurra.color;
            c.a = alpha;
            SpriteRendererAurra.color = c;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, soinRange);
    }
}
