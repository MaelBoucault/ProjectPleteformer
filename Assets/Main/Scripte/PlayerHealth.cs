using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    private Rigidbody2D rb;

    private Color originalColor;

    [Header("Health")]
    public float CurrentHealth;
    public float MaxHealth = 100f;

    [Header("Health Slider")]
    public Slider SliderHealth;

    [Header("Manager Health")]

    public bool invincibility = false;
    public float invincibilityTime = 1f;
    SpriteRenderer playerRenderer;

    [Header("Itween")]
    public Vector3 amount;
    public float time;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = MaxHealth;
        playerRenderer = GetComponent<SpriteRenderer>();
        originalColor = playerRenderer.color;
    }

    private void Update()
    {
        if (!invincibility) playerRenderer.color = originalColor;

        SliderHealth.value = (CurrentHealth/MaxHealth)*100;
    }

    public void UpdateHealth(float Amount, Vector3 positionEnemy)
    {
        if (!invincibility)
        {
            CurrentHealth += Amount;

            if (Amount < 0)
            {
                float punchAmount = Mathf.Abs(Amount) * 0.2f;
                if (punchAmount >= 1f) punchAmount = 1f;

                float randomTime = Random.Range(time - 0.5f, time + 0.5f);

                iTween.PunchScale(gameObject, iTween.Hash(
                    "amount", new Vector3(punchAmount, punchAmount, 0),
                    "time", randomTime
                ));

                Vector3 knockbackDir = (transform.position - positionEnemy).normalized;
                float knockbackForce = Mathf.Abs(Amount);
                //rb.linearVelocity = Vector3.zero;
                rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            invincibility = true;
            StartCoroutine(InvincibilityCoroutine());
        }

        // Clamp entre 0 et max
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Death();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Piege") )
        {
            UpdateHealth(-20, collision.transform.position);
        }
    }

    void Death()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincibility = true;

        float elapsedTime = 0f;
        originalColor = playerRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsedTime < invincibilityTime)
        {
            float lerp = Mathf.PingPong(Time.time * 5, 1);
            playerRenderer.color = Color.Lerp(originalColor, transparentColor, lerp);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        invincibility = false;

        playerRenderer.color = originalColor;
    }

}
