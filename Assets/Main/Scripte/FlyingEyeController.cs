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

    private float baseRotationOffset = 180f; // L'offset pour que l'œil regarde à gauche par défaut

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

        // Déplacement
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);

        // Rotation : faire en sorte que l'œil regarde dans la direction du déplacement
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
            LookAtTarget();  // Faire en sorte que l'œil regarde le joueur avant de tirer
            FireRay();
        }
    }

    void LookAtTarget()
    {
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            // Calculer l'angle nécessaire pour que l'œil regarde la cible
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + baseRotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            // Rotation lissée
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    void FireRay()
    {
        if (rayPrefab != null && target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rayRotation = Quaternion.Euler(0f, 0f, angle);

            activeRay = Instantiate(rayPrefab, transform.position, rayRotation);
            activeRay.transform.parent = transform;

            if (magicParticles != null)
                Instantiate(magicParticles, transform.position, Quaternion.identity);

            currentRayRotation = rayRotation;
            currentRayLength = 0f;

            //StartCoroutine(DestroyRayAfterSeconds(3f));
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
