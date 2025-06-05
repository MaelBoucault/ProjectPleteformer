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

    [Header("Effets Visuels")]
    public GameObject Aurra;
    public GameObject BigAurra;

    [Header("Références de Déplacement")]
    [Tooltip("Transform du joueur pour garder une distance.")]
    public Transform player;

    [Tooltip("Distance à garder du joueur/ennemi pendant le soin.")]
    public float distanceFromEnemy = 25f;

    [Tooltip("Vitesse de déplacement du barde.")]
    public float moveSpeed = 5f;

    [Space(10)]
    [Header("Paramètres d'Attaque Joueur")]
    [Tooltip("Distance minimale et maximale pour la téléportation autour du joueur.")]
    public Vector2 teleportDistanceRange = new Vector2(8f, 12f);
    [Tooltip("Temps minimum et maximum entre les actions d'attaque (mouvement/tir).")]
    public Vector2 attackActionIntervalRange = new Vector2(0.5f, 1.5f);
    [Tooltip("Nombre de tirs par salve quand il attaque le joueur.")]
    public int shotsPerVolley = 1;


    // Privés
    private SpriteRenderer SpriteRendererAurra;
    private bool isHealing = false;

    private Transform currentTarget;
    private EnnemieHealth currentEnemyHealth;

    private bool isTeleporting = false;
    private bool isEvading = false;

    private bool attackingPlayerMode = false;
    private Coroutine attackRoutineInstance; // Pour contrôler la coroutine d'attaque

    private bool isExploding = false;

    private EnemyAudioController audioController;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        BigAurra.SetActive(false);
        SpriteRendererAurra = Aurra.GetComponent<SpriteRenderer>();
        SetAuraAlpha(0.5f);
        StartCoroutine(HealingLoop());

        audioController = GetComponent<EnemyAudioController>();
        if (audioController == null)
        {
            Debug.LogError("BardeScripte: EnemyAudioController manquant sur ce GameObject !");
        }
    }

    private void Update()
    {
        if (isTeleporting || isEvading) return; // Ne pas bouger si on tp ou évite

        if (currentTarget != null)
        {
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
        else // Pas de cible de soin, et pas en mode attaque (sinon l'attaque gère le mouvement)
        {
            // Évitement du joueur quand il est proche
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (!attackingPlayerMode && distanceToPlayer < 15f) 
            {
                Vector3 evadeDirection = (transform.position - player.position).normalized;
                Vector3 evadeTarget = transform.position + evadeDirection * 10f;

                isEvading = true; 
                iTween.MoveTo(gameObject, iTween.Hash(
                    "position", evadeTarget,
                    "time", 0.3f,
                    "easetype", iTween.EaseType.easeOutSine,
                    "oncomplete", "EndEvade"
                ));
                audioController?.PlayBardeFuit();
            }
        }
    }

    IEnumerator HealingLoop()
    {
        while (true)
        {
            // Étape 1: Trouver une cible de soin
            bool foundHealTarget = false;
            currentTarget = null;
            currentEnemyHealth = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soinRange);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Ennemies") && col.gameObject != gameObject)
                {
                    EnnemieHealth ennemi = col.GetComponent<EnnemieHealth>();
                    if (ennemi != null && ennemi.health < ennemi.maxHealth)
                    {
                        currentTarget = col.transform;
                        currentEnemyHealth = ennemi;
                        foundHealTarget = true;
                        break;
                    }
                }
            }

            // Étape 2: Gérer les modes (Soin vs Attaque Joueur)
            if (foundHealTarget)
            {
                // Si on était en mode attaque, on l'arrête
                if (attackingPlayerMode)
                {
                    attackingPlayerMode = false;
                    if (attackRoutineInstance != null)
                    {
                        StopCoroutine(attackRoutineInstance);
                        attackRoutineInstance = null;
                    }
                }

                // Gérer l'aura de soin
                if (!isHealing)
                {
                    SetAuraAlpha(1f);
                    BigAurra.SetActive(true);
                    isHealing = true;
                }

                // Si on est bien positionné, appliquer le soin
                Vector3 directionAwayFromPlayer = (currentTarget.position - player.position).normalized;
                Vector3 idealHealPosition = currentTarget.position + directionAwayFromPlayer * distanceFromEnemy;
                if (Vector3.Distance(transform.position, idealHealPosition) <= 0.5f)
                {
                    currentEnemyHealth.UpdateHealth(healAmount);
                    audioController?.PlayBardeHeal();

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
                            "time", 0.15f,
                            "easetype", iTween.EaseType.easeOutBack
                        ));
                    }
                }
            }
            else // Pas de cible de soin trouvée
            {
                if (isHealing)
                {
                    SetAuraAlpha(0.5f);
                    BigAurra.SetActive(false);
                    isHealing = false;
                }

                // Si plus aucun ennemi n'est en vie du tout, passer en mode attaque joueur
                if (!AreThereEnemiesLeft() && !attackingPlayerMode)
                {
                    attackingPlayerMode = true;
                    attackRoutineInstance = StartCoroutine(HandlePlayerAttackPhase());
                }
            }

            yield return new WaitForSeconds(healInterval);
        }
    }

    IEnumerator HandlePlayerAttackPhase()
    {
        while (attackingPlayerMode && !AreThereEnemiesLeft())
        {
            if (isExploding) yield break;

            // Décide de l'action: téléporter ou bouger autour
            float actionRoll = Random.Range(0f, 1f);

            if (actionRoll < 0.6f) // 60% de chance de téléporter
            {
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(teleportDistanceRange.x, teleportDistanceRange.y);
                Vector3 teleportPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                yield return StartCoroutine(TeleportToPosition(teleportPos));
            }
            else // 40% de chance de faire un mouvement autour
            {
                float angle = Random.Range(0f, 360f);
                Vector3 orbitalTarget = player.position + (Vector3)(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * Random.Range(teleportDistanceRange.x, teleportDistanceRange.y));

                iTween.MoveTo(gameObject, iTween.Hash(
                    "position", orbitalTarget,
                    "time", Random.Range(0.8f, 1.5f),
                    "easetype", iTween.EaseType.easeInOutSine
                ));
                yield return new WaitForSeconds(Random.Range(0.8f, 1.5f)); // Attendre la fin du mouvement
            }

            // Tirer des projectiles
            for (int i = 0; i < shotsPerVolley; i++)
            {
                if (healProjectilePrefab != null)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    float angleOffset = Random.Range(-30f, 30f);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;

                    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                    GameObject proj = Instantiate(healProjectilePrefab, transform.position, rotation);
                    HealProjectile projectile = proj.GetComponent<HealProjectile>();
                    if (projectile != null)
                    {
                        Vector3 target = player.position + (Vector3)(Random.insideUnitCircle * 1f);
                    }
                    audioController?.PlayBardeAttack();

                    iTween.ScaleFrom(proj, iTween.Hash(
                        "scale", Vector3.zero,
                        "time", 0.15f,
                        "easetype", iTween.EaseType.easeOutBack
                    ));
                }
                yield return new WaitForSeconds(Random.Range(0.1f, 0.4f)); 
            }

            if (AreThereEnemiesLeft())
            {
                attackingPlayerMode = false;
                if (attackRoutineInstance != null)
                {
                    StopCoroutine(attackRoutineInstance);
                    attackRoutineInstance = null;
                }
                yield break;
            }

            yield return new WaitForSeconds(Random.Range(attackActionIntervalRange.x, attackActionIntervalRange.y)); // Attendre avant la prochaine action
        }

        attackingPlayerMode = false;
        attackRoutineInstance = null;
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
        audioController?.PlayBardeTp();

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.zero,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeInCirc
        ));

        yield return new WaitForSeconds(0.2f);

        transform.position = targetPosition;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.one,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeOutBack
        ));

        yield return new WaitForSeconds(0.2f);

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
        isTeleporting = true; 
        StopAllCoroutines(); 
    }
}