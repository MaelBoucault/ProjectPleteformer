using UnityEngine;
using System.Collections;
public class EnnemieHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public bool invincibility;
    public float invincibilityTime = 0.3f;

    public GameObject EnnemiePrefab;
    SpriteRenderer EnnemieRenderer;

    [Header("Itween")]
    public Vector3 amount = new Vector3(0.5f, 0.2f, 0);
    public float time = 0.5f;

    public GameObject GameObjectSpriterenderer;

    [Header("Spécifique au barde")]
    public bool isBarde = false; // 👈 Ajout de ce bool

    private void Start()
    {
        if (gameObject.GetComponent<SpriteRenderer>())
        {
            EnnemieRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        else if (GameObjectSpriterenderer != null) 
        {
            EnnemieRenderer = GameObjectSpriterenderer.GetComponent<SpriteRenderer>();
        }
        
    }

    public void UpdateHealth(float Amount)
    {
        if (!invincibility)
        {
            health += Amount;
            if (Amount < 0)
            {
                float randomTime = Random.Range(time - 0.5f, time + 0.5f);
                iTween.PunchScale(gameObject, iTween.Hash(
                    "amount", amount,
                    "time", time));
            }
            invincibility = true;
            StartCoroutine(InvincibilityCoroutine());
        }

        if (health >= maxHealth) health = maxHealth;
        if (health <= 0) health = 0;

        if (health <= 0)
        {
            if (!isBarde) // 👈 NE PAS détruire si c’est le barde
            {
                Dead();
            }
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
