using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI2D : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float stopChaseRange = 8f;

    public float idleTime = 1f;
    private float idleTimer = 0f;
    private bool isIdling = false;

    public float attackRange = 1f;
    public LayerMask playerLayer;
    public float dashForce = 8f;
    public float dashDuration = 0.2f;

    private Transform currentTarget;
    internal bool chasing = false;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    public GameObject Aurra;

    Animator animator;
    internal Vector2 direction = Vector2.zero;
    internal bool Attack;

    void Start()
    {
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB;
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            chasing = true;
            Aurra.SetActive(true);
        }
        else if (distanceToPlayer > stopChaseRange)
        {
            chasing = false;
            Aurra.SetActive(false);
        }

        if (!Attack)
        {
            if (chasing)
            {
                MoveTowards(player.position);
            }
            else
            {
                Patrol();
            }
        }

        CheckAttack();
        ApplySquashAndStretch();
    }

    void Patrol()
    {
        if (isIdling)
        {
            idleTimer -= Time.fixedDeltaTime;
            if (idleTimer <= 0f)
            {
                isIdling = false;
                currentTarget = currentTarget == pointA ? pointB : pointA;
            }
            return;
        }

        MoveTowards(currentTarget.position);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            isIdling = true;
            idleTimer = idleTime;
            animator.SetFloat("Speed", 0);
            rb.linearVelocity = new Vector3(0, 0, 0);
        }
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = new Vector2(target.x, currentPosition.y);
        direction = (targetPosition - currentPosition).normalized;

        animator.SetFloat("Speed", Math.Abs(direction.x));

        Vector2 desiredVelocity = direction * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, 0.1f);

        FlipSprite(direction.x);
    }

    void CheckAttack()
    {
        if (chasing)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (hit != null && !Attack)
            {
                StartCoroutine(DashTowardsPlayer());
            }
        }
    }

    System.Collections.IEnumerator DashTowardsPlayer()
    {
        Attack = true;
        animator.SetTrigger("Attack");

        Vector3 targetPos = player.position;
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", new Vector3(targetPos.x, transform.position.y, transform.position.z),
            "speed", dashForce,
            "easetype", iTween.EaseType.easeInOutSine,
            "time", dashDuration
        ));

        yield return new WaitForSeconds(dashDuration + 0.1f);

        Attack = false;
    }

    void FlipSprite(float directionX)
    {
        if (Mathf.Abs(directionX) > 0.01f)
        {
            Vector3 scale = originalScale;
            scale.x = directionX > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void ApplySquashAndStretch()
    {
        float moveSpeed = rb.linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(moveSpeed / speed);

        float stretchX = Mathf.Lerp(1f, 1.2f, speedFactor);
        float squashY = Mathf.Lerp(1f, 0.85f, speedFactor);

        Vector3 targetScale = new Vector3(stretchX * Mathf.Sign(transform.localScale.x), squashY, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.1f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
