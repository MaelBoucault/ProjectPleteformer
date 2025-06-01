using UnityEngine;
using System.Collections;

public class EnnemieHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public bool invincibility;
    public float invincibilityTime = 0.3f;

    SpriteRenderer EnnemieRenderer;

    

    public Vector3 amount = new Vector3(0.5f, 0.2f, 0);
    public float time = 0.5f;

    public GameObject GameObjectSpriterenderer;

    public bool isBarde = false;

    private EnemyAudioController enemyAudioController;

    private void Start()
    {
        enemyAudioController = GetComponent<EnemyAudioController>();
        if (enemyAudioController == null)
        {
            Debug.LogError("EnnemieHealth: EnemyAudioController not found on this GameObject! Add it to the prefab.");
        }

        if (gameObject.GetComponent<SpriteRenderer>())
        {
            EnnemieRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        else if (GameObjectSpriterenderer != null)
        {
            EnnemieRenderer = GameObjectSpriterenderer.GetComponent<SpriteRenderer>();
        }
        health = maxHealth;
    }

    public void UpdateHealth(float Amount)
    {
        if (!invincibility)
        {
            health += Amount;

            if (Amount < 0)
            {
                if (enemyAudioController != null)
                {
                    enemyAudioController.PlayHitSound();
                }

                if (GetComponent<iTween>())
                {
                    iTween.PunchScale(gameObject, iTween.Hash(
                        "amount", amount,
                        "time", time));
                }

                invincibility = true;
                StartCoroutine(InvincibilityCoroutine());
            }
        }

        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            if (!isBarde)
            {
                Dead();
            }
            else
            {
                if (enemyAudioController != null)
                {
                    enemyAudioController.PlayBardeFuit();
                }
                Debug.Log("Barde has fled!");
                Destroy(gameObject, 1f);
            }
        }
    }

    void Dead()
    {
        if (enemyAudioController != null)
        {
            enemyAudioController.PlayDieSound();
        }
        Destroy(gameObject);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincibility = true;

        float elapsedTime = 0f;
        Color originalColor = EnnemieRenderer != null ? EnnemieRenderer.color : Color.white;
        Color transparentColor = originalColor;
        transparentColor.a = .5f;

        while (elapsedTime < invincibilityTime)
        {
            if (EnnemieRenderer != null)
            {
                float lerp = Mathf.PingPong(Time.time * 5, 1);
                EnnemieRenderer.color = Color.Lerp(originalColor, transparentColor, lerp);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (EnnemieRenderer != null)
        {
            EnnemieRenderer.color = originalColor;
        }

        invincibility = false;
    }
}