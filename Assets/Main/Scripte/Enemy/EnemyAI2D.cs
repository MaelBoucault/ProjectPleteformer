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
    public Transform groundCheck; // Renommé de groundCheckRight
    public Transform wallCheck;   // Renommé de wallCheckRight
    public LayerMask platformLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

    internal bool chasing = false;
    internal bool Attack;
    internal Vector2 direction = Vector2.zero;
    public GameObject Aurra;
    public Transform player;

    private Transform lastPlatform;
    private Vector2 lastSafePosition;

    private EnemyAudioController enemyAudioController;

    void Start()
    {
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        enemyAudioController = GetComponent<EnemyAudioController>();
        if (enemyAudioController == null)
        {
            Debug.LogError("EnemyAI2D: EnemyAudioController not found on this GameObject! Add it to the prefab.");
        }

        lastSafePosition = transform.position;
    }

    void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            RespawnToLastPlatform();
        }

        transform.localScale = new Vector3(0.5f * Mathf.Sign(transform.localScale.x), 0.5f, 1);

        bool wasChasing = chasing;

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

        if (chasing && !wasChasing && enemyAudioController != null)
        {
            enemyAudioController.PlayOgreGrr();
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
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);
        }

        CheckAttack();
        ApplySquashAndStretch();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Scriptmorve>())
        {
            speed--;
        }
        if (collision.gameObject.layer == 3)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    lastPlatform = collision.transform;
                    lastSafePosition = transform.position;
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Scriptmorve>())
        {
            // speed++;
        }
    }

    void Patrol()
    {
        // Décalage du point de vérification en fonction de la direction de déplacement
        Vector3 currentGroundCheckPosition = groundCheck.position;
        Vector3 currentWallCheckPosition = wallCheck.position;
        Vector2 checkDirection = Vector2.zero;

        if (facingRight)
        {
            checkDirection = Vector2.right;
        }
        else
        {
            checkDirection = Vector2.left;
        }

        bool isGroundedAhead = Physics2D.Raycast(currentGroundCheckPosition, Vector2.down, groundCheckDistance, platformLayer);
        bool wallAhead = Physics2D.Raycast(currentWallCheckPosition, checkDirection, 0.1f, platformLayer);

        if (!isGroundedAhead || wallAhead)
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
        float desiredDirectionX = Mathf.Sign(target.x - transform.position.x);

        Vector3 currentGroundCheckPosition = groundCheck.position;
        Vector3 currentWallCheckPosition = wallCheck.position;
        Vector2 checkDirection = Vector2.zero;

        if (desiredDirectionX > 0)
        {
            checkDirection = Vector2.right;
        }
        else
        {
            checkDirection = Vector2.left;
        }

        bool hasGroundAhead = Physics2D.Raycast(currentGroundCheckPosition, Vector2.down, groundCheckDistance, platformLayer);
        bool wallAhead = Physics2D.Raycast(currentWallCheckPosition, checkDirection, 0.1f, platformLayer);

        bool canMoveForward = hasGroundAhead && !wallAhead;

        if (canMoveForward)
        {
            direction = new Vector2(desiredDirectionX, 0f);
            animator.SetFloat("Speed", Mathf.Abs(direction.x));
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
            HandleFlip(desiredDirectionX);
        }
        else
        {
            direction = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            HandleFlip(desiredDirectionX);
        }
    }

    void CheckAttack()
    {
        if (chasing)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (hit != null && !Attack)
            {
                Attack = true;
                if (player.position.x < transform.position.x)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x);
                    transform.localScale = scale;
                }
                else
                {
                    Vector3 scale = transform.localScale;
                    scale.x = -Mathf.Abs(scale.x);
                    transform.localScale = scale;
                }

                animator.SetTrigger("Attacking");
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void RespawnToLastPlatform()
    {
        rb.linearVelocity = Vector2.zero;

        if (lastPlatform != null)
        {
            Vector3 platformTop = lastPlatform.position;
            platformTop.y += 1f;
            transform.position = platformTop;
        }
        else
        {
            transform.position = lastSafePosition;
        }
        Debug.Log("Respawned to last safe position.");
    }

    void HandleFlip(float directionX)
    {
        if (directionX < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        if (directionX > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
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
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.yellow;
        if (wallCheck != null)
        {
            // Dessine le wallCheck dans les deux directions pour visualisation
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * 0.2f);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * 0.2f);
        }

        Gizmos.color = Color.magenta;
        if (groundCheck != null)
        {
            // Dessine les raycasts de "sol devant" pour visualisation
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}