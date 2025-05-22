using UnityEngine;
using System.Collections;
using System;

public class EnfantScript : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float speed = 15f;
    public float pauseDuration = 5f;
    public GameObject Aurra;

    private Vector2 direction;
    private Vector2 targetPosition;
    private Vector2 lastSafePosition;

    private bool isChasing = false;
    private bool isPaused = false;

    private float chaseTimer = 0f;
    private float maxChaseDuration = 6f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool hasDamaged = false;
    private Transform lastPlatform;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        lastSafePosition = transform.position;
        lastPlatform = null;
    }

    void Update()
    {
        // Respawn s'il tombe trop bas
        if (transform.position.y < -10f)
        {
            Debug.Log("L'enfant est tombé. Respawn.");
            RespawnToLastPlatform();
        }

        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isChasing)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                targetPosition = player.position;
                isChasing = true;
                chaseTimer = 0f;
            }
        }
        else
        {
            chaseTimer += Time.deltaTime;

            Vector2 currentPosition = rb.position;
            direction = (targetPosition - currentPosition).normalized;

            rb.linearVelocity = direction * speed;
            Aurra.SetActive(true);

            bool isDash = Mathf.Abs(player.position.y - transform.position.y) > 0.5f;
            animator.SetBool("Dash", isDash);
            animator.SetFloat("Speed", isDash ? 0 : Mathf.Abs(direction.x));

            spriteRenderer.flipX = direction.x < 0;

            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                animator.SetFloat("Speed", 0);
                animator.SetBool("Dash", false);
                Aurra.SetActive(false);
                StartCoroutine(PauseAtPosition());
            }
            else if (chaseTimer >= maxChaseDuration)
            {
                animator.SetFloat("Speed", 0);
                animator.SetBool("Dash", false);
                Aurra.SetActive(false);
                StartCoroutine(PauseAtPosition());
            }
        }
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

        if (collision.gameObject.tag == "Player" && !hasDamaged)
        {
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(-15,gameObject.transform.position);
            hasDamaged = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasDamaged = false;
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

        isChasing = false;
        isPaused = true;
        Aurra.SetActive(false);
        animator.SetFloat("Speed", 0);
        animator.SetBool("Dash", false);
        StartCoroutine(PauseAtPosition());
    }

    IEnumerator PauseAtPosition()
    {
        isPaused = true;
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}
