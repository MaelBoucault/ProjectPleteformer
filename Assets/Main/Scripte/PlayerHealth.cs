using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{

    [Header("Health")]
    public float CurrentHealth;
    public float MaxHealth = 100f;

    [Header("Health Slider")]
    public Slider SliderHealth;

    [Header("Manager Health")]

    public bool invincibility = false;
    public float invincibilityTime = 1f;
    SpriteRenderer playerRenderer;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        SliderHealth.value = (CurrentHealth/MaxHealth)*100;
    }

    public void UpdateHealth(float Amount)
    {
        if (!invincibility)
        {
            CurrentHealth += Amount;
            StartCoroutine(InvincibilityCoroutine());
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Piege") )
        {
            UpdateHealth(-20);
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
        Color originalColor = playerRenderer.color;
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
