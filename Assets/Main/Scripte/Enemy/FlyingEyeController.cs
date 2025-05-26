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
    public GameObject magicParticles;
    public GameObject chargingWavePrefab;

    [Header("Cible")]
    public Transform target;

    [Header("Rayon")]
    public float rotationLerpSpeed = 5f;
    public float lengthLerpSpeed = 5f;
    public float targetRayLength = 100f;

    [Header("Rotation de base")]
    public float baseRotationOffset = 180f;

    private Vector3 startPos;
    private Vector3 wanderTarget;
    private float nextWanderTime = 0f;
    private float lastAttackTime = -999f;
    private Rigidbody2D rb;
    private GameObject activeRay = null;

    private Quaternion currentRayRotation;

    void Start()
    {

        target = FindAnyObjectByType<PlayerActionMove>().transform;
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        PickNewWanderTarget();
    }

    void Update()
    {
        MoveWander();

        if (Vector3.Distance(transform.position, target.position) <= detectionRadius)
        {
            TryAttack();
        }

        if (activeRay != null)
        {
            activeRay.transform.position = transform.position;
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

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + baseRotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
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
        if (Time.time - lastAttackTime >= attackCooldown && activeRay == null)
        {
            lastAttackTime = Time.time;
            StartCoroutine(ChargeAndFire());
        }
    }

    IEnumerator ChargeAndFire()
    {
        if (target == null) yield break;

        LookAtTarget();

        GameObject wave = null;
        float chargeTime = 2.5f;

        if (chargingWavePrefab != null)
        {
            wave = Instantiate(chargingWavePrefab, transform.position, Quaternion.identity);
            wave.transform.localScale = Vector3.one * 2f;
            wave.transform.SetParent(transform);

            // Tourne pendant 2.5s (on garde iTween ici)
            iTween.RotateBy(wave, iTween.Hash("z", 2f, "time", chargeTime, "easetype", "linear"));

            // Déplacement subtil
            iTween.MoveBy(wave, iTween.Hash(
                "x", 0.05f, "y", 0.05f,
                "time", chargeTime,
                "islocal", true,
                "easetype", "easeInOutSine"
            ));

            // Shrink manuellement
            Vector3 initialScale = wave.transform.localScale;
            float elapsed = 0f;

            while (elapsed < chargeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / chargeTime;
                wave.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, Mathf.Pow(t, 2)); // easeIn
                yield return null;
            }

            Destroy(wave);
        }
        else
        {
            yield return new WaitForSeconds(chargeTime);
        }

        FireRay();
    }

    void LookAtTarget()
    {
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;

            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float randomOffset = Random.Range(-5f, 5f);
            float finalAngle = baseAngle + randomOffset + baseRotationOffset;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, finalAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    void FireRay()
    {
        if (rayPrefab != null && target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float randomOffset = Random.Range(15f, 30f);
            angle += randomOffset;

            Quaternion rayRotation = Quaternion.Euler(0f, 0f, angle);

            activeRay = Instantiate(rayPrefab, transform.position, rayRotation);
            activeRay.transform.parent = transform;
        }
        else
        {
            Debug.LogWarning("rayPrefab ou target est null !");
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
