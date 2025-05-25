using UnityEngine;
using System.Collections;

public class BardeScripte : MonoBehaviour
{
    [Header("Paramètres de Soin")]
    [Tooltip("Rayon dans lequel le barde peut détecter les ennemis à soigner.")]
    public float soinRange = 5f;

    [Tooltip("Quantité de points de vie restaurés à chaque soin.")]
    public float healAmount = 5f;

    [Tooltip("Temps entre deux soins (en secondes).")]
    public float healInterval = 1f;

    [Tooltip("Projectile de soin à instancier.")]
    public GameObject healProjectilePrefab;

    [Space(10)]
    [Header("Effets Visuels")]
    public GameObject Aurra;
    public GameObject BigAurra;

    [Space(10)]
    [Header("Références de Déplacement")]
    [Tooltip("Transform du joueur pour garder une distance.")]
    public Transform player;

    [Tooltip("Distance à garder du joueur/ennemi pendant le soin.")]
    public float distanceFromEnemy = 25f;

    [Tooltip("Vitesse de déplacement du barde.")]
    public float moveSpeed = 5f;

    // Privés
    private SpriteRenderer SpriteRendererAurra;
    private bool isHealing = false;

    private Transform currentTarget;
    private EnnemieHealth currentEnemyHealth;

    private bool isTeleporting = false;
    private bool isEvading = false;

    private bool attackingPlayerMode = false;

    private bool isExploding = false;



    private void Start()
    {
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        BigAurra.SetActive(false);
        SpriteRendererAurra = Aurra.GetComponent<SpriteRenderer>();
        SetAuraAlpha(0.5f);
        StartCoroutine(HealingLoop());
    }

    private void Update()
    {
        if (isTeleporting) return;

        if (currentTarget != null)
        {
            // 🔄 Comportement de soin
            Vector3 directionAwayFromPlayer = (currentTarget.position - player.position).normalized;
            Vector3 targetPosition = currentTarget.position + directionAwayFromPlayer * distanceFromEnemy;

            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > 20f)
            {
                StartCoroutine(TeleportToPosition(targetPosition));
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (!attackingPlayerMode && distanceToPlayer < 15f && !isEvading)

            {
                Vector3 evadeDirection = (transform.position - player.position).normalized;
                Vector3 evadeTarget = transform.position + evadeDirection * 10f;

                isEvading = true;
                iTween.MoveTo(gameObject, iTween.Hash(
                    "position", evadeTarget,
                    "time", 0.4f,
                    "easetype", iTween.EaseType.easeOutQuad,
                    "oncomplete", "EndEvade"
                ));
            }
        }
    }

    IEnumerator HealingLoop()
    {
        while (true)
        {
            bool healedSomeone = false;
            currentTarget = null;
            currentEnemyHealth = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soinRange);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Ennemies"))
                {
                    EnnemieHealth ennemi = col.GetComponent<EnnemieHealth>();
                    if (ennemi != null && ennemi.health < ennemi.maxHealth)
                    {
                        currentTarget = col.transform;
                        currentEnemyHealth = ennemi;
                        break;
                    }
                }
            }

            if (currentTarget != null && currentEnemyHealth != null)
            {
                Vector3 directionAwayFromPlayer = (currentTarget.position - player.position).normalized;
                Vector3 idealHealPosition = currentTarget.position + directionAwayFromPlayer * distanceFromEnemy;

                if (Vector3.Distance(transform.position, idealHealPosition) <= 0.5f)
                {
                    currentEnemyHealth.UpdateHealth(healAmount);
                    healedSomeone = true;

                    if (healProjectilePrefab != null)
                    {
                        Vector3 direction = (currentTarget.position - transform.position).normalized;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                        GameObject proj = Instantiate(healProjectilePrefab, transform.position, rotation);
                        HealProjectile projectile = proj.GetComponent<HealProjectile>();
                        if (projectile != null)
                        {
                            projectile.SetTarget(currentTarget.position, currentTarget);
                        }

                        iTween.ScaleFrom(proj, iTween.Hash(
                            "scale", Vector3.zero,
                            "time", 0.2f,
                            "easetype", iTween.EaseType.easeOutBack
                        ));
                    }
                }
            }

            // Aura
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

                // Si plus aucun ennemi n'est vivant
                if (!AreThereEnemiesLeft() && !attackingPlayerMode)
                {
                    attackingPlayerMode = true;
                    StartCoroutine(AttackPlayerRoutine());
                }
            }

            yield return new WaitForSeconds(healInterval);
        }
    }

    IEnumerator AttackPlayerRoutine()
    {
        if (isTeleporting || isEvading || isExploding) yield break;

        Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(5f, 8f);
        Vector3 randomPosition = player.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", randomPosition,
            "time", Random.Range(0.5f, 1.2f),
            "easetype", iTween.EaseType.easeOutQuad
        ));

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        if (healProjectilePrefab != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            float angleOffset = Random.Range(-15f, 15f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;

            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject proj = Instantiate(healProjectilePrefab, transform.position, rotation);
            HealProjectile projectile = proj.GetComponent<HealProjectile>();
            if (projectile != null)
            {
                Vector3 target = player.position + (Vector3)(Random.insideUnitCircle * 0.5f);
                projectile.SetTarget(target, player);
            }

            iTween.ScaleFrom(proj, iTween.Hash(
                "scale", Vector3.zero,
                "time", 0.2f,
                "easetype", iTween.EaseType.easeOutBack
            ));
        }

        yield return new WaitForSeconds(Random.Range(2f, 4f));

        if (!AreThereEnemiesLeft())
        {
            StartCoroutine(AttackPlayerRoutine());
        }
        if (AreThereEnemiesLeft())
        {
            attackingPlayerMode = false;
        }
        else
        {
            StartCoroutine(AttackPlayerRoutine());
        }
    }

    bool AreThereEnemiesLeft()
    {
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Ennemies");
        return ennemis.Length > 0;
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

    IEnumerator TeleportToPosition(Vector3 targetPosition)
    {
        isTeleporting = true;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.zero,
            "time", 0.3f,
            "easetype", iTween.EaseType.easeInBack
        ));

        yield return new WaitForSeconds(0.3f);

        transform.position = targetPosition;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.one,
            "time", 0.3f,
            "easetype", iTween.EaseType.easeOutBack
        ));

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
    }

    private void EndEvade()
    {
        isEvading = false;
    }

    public void OnStartExplosion()
    {
        isExploding = true;
        attackingPlayerMode = false;
        isTeleporting = true ;
        StopAllCoroutines(); // Stoppe les routines offensives
    }
}
