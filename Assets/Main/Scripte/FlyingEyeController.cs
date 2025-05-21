using UnityEngine;
using System.Collections;

public class FlyingEyeController : MonoBehaviour
{
    public float wanderRadius = 4f;
    public float moveSpeed = 2f;
    public float detectionRadius = 6f;
    public float attackCooldown = 2f;
    public GameObject rayPrefab;
    public GameObject magicParticles;
    public Transform target;

    private Vector3 startPos;
    private Vector3 wanderTarget;
    private float nextWanderTime = 0f;
    private float lastAttackTime = -999f;
    private Rigidbody2D rb;
    private GameObject activeRay = null;

    public float rotationLerpSpeed = 5f;
    public float lengthLerpSpeed = 5f;

    private Quaternion currentRayRotation;
    private float currentRayLength = 0f;
    private float targetRayLength = 100f;

    void Start()
    {
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

        if (activeRay != null && target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float desiredAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion desiredRotation = Quaternion.Euler(0f, 0f, desiredAngle);

            currentRayRotation = Quaternion.Slerp(currentRayRotation, desiredRotation, Time.deltaTime * rotationLerpSpeed);
            activeRay.transform.rotation = currentRayRotation;

            currentRayLength = Mathf.Lerp(currentRayLength, targetRayLength, Time.deltaTime * lengthLerpSpeed);
            activeRay.transform.localScale = new Vector3(currentRayLength, activeRay.transform.localScale.y, 1f);

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
            FireRay();
        }
    }

    void FireRay()
    {
        if (rayPrefab != null && target != null)
        {
            activeRay = Instantiate(rayPrefab, transform.position, Quaternion.identity);
            activeRay.transform.parent = transform;

            if (magicParticles != null)
                Instantiate(magicParticles, transform.position, Quaternion.identity);

            currentRayRotation = activeRay.transform.rotation;
            currentRayLength = 0f;

            StartCoroutine(DestroyRayAfterSeconds(3f)); // <- durée modifiée ici
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
