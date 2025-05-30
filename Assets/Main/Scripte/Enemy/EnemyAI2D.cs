using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI2D : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;
    public float stopChaseRange = 8f;

    public float attackRange = 1f;
    public LayerMask playerLayer;

    public float groundCheckDistance = 0.2f;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public LayerMask platformLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;
    private bool facingRight = true;

    internal bool chasing = false;
    internal bool Attack;
    internal Vector2 direction = Vector2.zero;
    public GameObject Aurra;
    public Transform player;


    private Transform lastPlatform;
    private Vector2 lastSafePosition;
    void Start()
    {
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            RespawnToLastPlatform();
        }

        transform.localScale = new Vector3(0.5f, 0.5f, 1);

        if (player != null)
        {
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // L'enfant est AU-DESSUS
                {
                    lastPlatform = collision.transform;
                    lastSafePosition = transform.position;
                    break;
                }
            }
        }
    }
    void Patrol()
    {
        bool isGroundedLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckDistance, platformLayer);
        bool isGroundedRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckDistance, platformLayer);

        bool wallLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, 0.1f, platformLayer);
        bool wallRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, 0.1f, platformLayer);

        if ((facingRight && (!isGroundedRight || wallRight)) ||
            (!facingRight && (!isGroundedLeft || wallLeft)))
        {
            facingRight = !facingRight;
        }

        direction = new Vector2(facingRight ? 1f : -1f, 0f);
        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        HandleFlip(direction.x);
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = new Vector2(target.x, currentPosition.y);
        direction = (targetPosition - currentPosition).normalized;

        bool isGroundedLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckDistance, platformLayer);
        bool isGroundedRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckDistance, platformLayer);

        bool wallLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, 0.1f, platformLayer);
        bool wallRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, 0.1f, platformLayer);

        if ((facingRight && (!isGroundedRight || wallRight)) ||
            (!facingRight && (!isGroundedLeft || wallLeft)))
        {
            facingRight = !facingRight;
        }

        direction = new Vector2(facingRight ? 1f : -1f, 0f);
        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        HandleFlip(direction.x);
    }

    void CheckAttack()
    {
        if (chasing)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (hit != null && !Attack)
            {
                Attack = true;
                animator.SetTrigger("Attacking");
            }
        }
    }
    void RespawnToLastPlatform()
    {
        rb.linearVelocity = Vector2.zero;

        if (lastPlatform != null)
        {
            Vector3 platformTop = lastPlatform.position + Vector3.up * 1f;
            transform.position = platformTop;
        }
        else
        {
            transform.position = lastSafePosition;
        }
    }

    void HandleFlip(float directionX)
    {
        if (directionX < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= 1;
            transform.localScale = scale;
        }
        if (directionX > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        if (groundCheckLeft != null)
            Gizmos.DrawLine(groundCheckLeft.position, groundCheckLeft.position + Vector3.down * groundCheckDistance);
        if (groundCheckRight != null)
            Gizmos.DrawLine(groundCheckRight.position, groundCheckRight.position + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.yellow;
        if (wallCheckLeft != null)
            Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * 0.2f);
        if (wallCheckRight != null)
            Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * 0.2f);
    }
}
