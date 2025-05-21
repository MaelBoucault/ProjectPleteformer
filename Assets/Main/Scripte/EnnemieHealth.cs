using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering;
using System.Collections;


public class EnnemieHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public bool invincibility;
    public float invincibilityTime = 0.3f;

    public GameObject EnnemiePrefab;
    SpriteRenderer EnnemieRenderer;

    private void Start()
    {
        EnnemieRenderer = GetComponent<SpriteRenderer>();
    }


    public void UpdateHealth( float Amount)
    {
        if (!invincibility)
        {
            health += Amount;
            invincibility = true;
            StartCoroutine(InvincibilityCoroutine());
        }

        if (health >= maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            health = 0;
            Dead();
        }
    }

    void Dead()
    {
        Destroy(EnnemiePrefab);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincibility = true;

        float elapsedTime = 0f;
        Color originalColor = EnnemieRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsedTime < invincibilityTime)
        {
            float lerp = Mathf.PingPong(Time.time * 5, 1);
            EnnemieRenderer.color = Color.Lerp(originalColor, transparentColor, lerp);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EnnemieRenderer.color = originalColor;

        invincibility = false;
    }
}
