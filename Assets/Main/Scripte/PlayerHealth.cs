using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PlayerHealth : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 vignetteTargetCenter;
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

    [Header("Post-Process")]
    public Volume postProcessVolume;
    private Vignette vignette;


    [Header("Itween")]
    public Vector3 amount;
    public float time;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = MaxHealth;
        playerRenderer = GetComponent<SpriteRenderer>();
        originalColor = playerRenderer.color;

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }
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

                float vignetteIntensity = Mathf.Clamp01(Mathf.Abs(Amount) / 20f);
                StartCoroutine(ShowPostProcessVignette(vignetteIntensity));


                float shakeIntensity = Mathf.Clamp01(Mathf.Abs(Amount) / 50f);
                float shakeDuration = Mathf.Lerp(0.1f, 0.4f, shakeIntensity);
                CameraShake.Shake(shakeIntensity * 0.2f, shakeDuration);

                float punchAmount = Mathf.Abs(Amount) * 0.5f;
                if (punchAmount >= 2f) punchAmount = 2f;

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

    void OnUpdateVignetteCenter(object val)
    {
        vignette.center.value = (Vector2)val;
    }


    IEnumerator ShowPostProcessVignette(float intensity)
    {
        if (vignette == null) yield break;

        float targetIntensity = Mathf.Lerp(0.4f, 0.8f, intensity);
        vignette.intensity.value = 0f;

        Vector2 originalCenter = vignette.center.value;
        vignetteTargetCenter = originalCenter + new Vector2(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));

        // Shake vers une nouvelle position
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", originalCenter,
            "to", vignetteTargetCenter,
            "time", 0.2f,
            "onupdate", "OnUpdateVignetteCenter",
            "onupdatetarget", gameObject
        ));

        for (float t = 0; t < 1; t += Time.deltaTime * 6)
        {
            vignette.intensity.value = Mathf.Lerp(0f, targetIntensity, t);
            yield return null;
        }

        // Retour vers la position d’origine
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", vignette.center.value,
            "to", originalCenter,
            "time", 0.4f,
            "onupdate", "OnUpdateVignetteCenter",
            "onupdatetarget", gameObject
        ));

        for (float t = 0; t < 1; t += Time.deltaTime * 1.5f)
        {
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, t);
            yield return null;
        }

        vignette.intensity.value = 0f;
        vignette.center.value = originalCenter;
    }


}
