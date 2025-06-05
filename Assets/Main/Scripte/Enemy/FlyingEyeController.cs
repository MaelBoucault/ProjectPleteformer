using UnityEngine;
using System.Collections;

public class FlyingEyeController : MonoBehaviour
{
    [Header("Déplacement")]
    public float wanderRadius = 4f;
    public float moveSpeed = 2f;

    [Header("Détection & Attaque")]
    public float detectionRadius = 6f;
    public float attackCooldown = 2f;

    [Header("Références de Prefabs")]
    public GameObject rayPrefab;
    public GameObject chargingWavePrefab;

    [Header("Cible")]
    public Transform target;

    [Header("Rayon")]
    public float chargeTime = 5f;
    public float rayOffsetRange = 2f;
    public float rayDurationAfterFire = 0.5f;

    [Header("Rotation de l'Oeil")]
    public float rotationLerpSpeed = 5f;
    public float baseRotationOffset = 180f;

    private Vector3 startPos;
    private Vector3 wanderTarget;
    private float nextWanderTime = 0f;
    private float lastAttackTime = -999f;
    private Rigidbody2D rb;
    private GameObject activeRay = null;
    private GameObject activeChargingWave = null;

    private EnemyAudioController enemyAudioController;

    void Start()
    {
        enemyAudioController = GetComponent<EnemyAudioController>();
        PlayerActionMove player = FindAnyObjectByType<PlayerActionMove>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("PlayerActionMove non trouvé dans la scène ! Assurez-vous qu'un joueur existe.");
            enabled = false;
            return;
        }

        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        PickNewWanderTarget();
    }

    void Update()
    {
        MoveWander();

        if (target != null)
        {
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + baseRotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }

        if (target != null && Vector3.Distance(transform.position, target.position) <= detectionRadius)
        {
            TryAttack();
        }
    }

    void MoveWander()
    {
        if (Time.time >= nextWanderTime)
        {
            PickNewWanderTarget();
        }

        Vector3 dir = (wanderTarget - transform.position).normalized;
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    void PickNewWanderTarget()
    {
        float x = Random.Range(-wanderRadius, wanderRadius);
        float y = Random.Range(-wanderRadius, wanderRadius);
        wanderTarget = startPos + new Vector3(x, y, 0f);
        nextWanderTime = Time.time + Random.Range(1.5f, 3f);
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && activeRay == null && activeChargingWave == null)
        {
            lastAttackTime = Time.time;
            enemyAudioController.PlayYeuxCharge();
            StartCoroutine(ChargeAndFire());
        }
    }

    IEnumerator ChargeAndFire()
    {
        if (target == null) yield break;

        Debug.Log("Oeil: Début de la charge...");

        if (chargingWavePrefab != null)
        {
            if (activeChargingWave != null)
            {
                Destroy(activeChargingWave);
                activeChargingWave = null;
            }

            activeChargingWave = Instantiate(chargingWavePrefab, transform.position, Quaternion.identity);
            activeChargingWave.transform.localScale = Vector3.one * 2f;
            activeChargingWave.transform.SetParent(transform);

            iTween.RotateBy(activeChargingWave, iTween.Hash("z", 2f, "time", chargeTime, "easetype", "linear"));
            iTween.MoveBy(activeChargingWave, iTween.Hash(
                "x", 0.05f, "y", 0.05f,
                "time", chargeTime,
                "islocal", true,
                "easetype", "easeInOutSine"
            ));

            Vector3 initialScale = activeChargingWave.transform.localScale;
            float elapsed = 0f;

            while (elapsed < chargeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / chargeTime;
                activeChargingWave.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, Mathf.Pow(t, 2));
                yield return null;
            }

            if (activeChargingWave != null)
            {
                Destroy(activeChargingWave);
                activeChargingWave = null;
            }
        }
        else
        {
            yield return new WaitForSeconds(chargeTime);
        }

        Debug.Log("Oeil: Rayon prêt à tirer !");
        enemyAudioController.PlayYeuxTire();

        if (activeRay != null)
        {
            Destroy(activeRay);
            activeRay = null;
        }

        if (rayPrefab != null && target != null)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-rayOffsetRange, rayOffsetRange),
                Random.Range(-rayOffsetRange, rayOffsetRange),
                0f
            );
            Vector3 finalTargetPosition = target.position + randomOffset;

            activeRay = Instantiate(rayPrefab, transform.position, Quaternion.identity);

            Vector3 rayDirection = (finalTargetPosition - activeRay.transform.position).normalized;
            float angle = Mathf.Atan2(rayDirection.y, rayDirection.x) * Mathf.Rad2Deg + (Random.Range(-20, 20));

            activeRay.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            StartCoroutine(DestroyRayAfterSeconds(rayDurationAfterFire));
        }
        else
        {
            Debug.LogWarning("rayPrefab ou target est null ! Impossible de tirer le rayon.");
        }
    }

    IEnumerator DestroyRayAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (activeRay != null)
        {
            Destroy(activeRay);
            activeRay = null;
        }
    }
}