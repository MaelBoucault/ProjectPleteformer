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

    private Vector2 targetPosition;
    private bool isChasing = false;
    private bool isPaused = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 direction;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Math.Abs(direction.x));

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
            }
        }
        else
        {
            Vector2 currentPosition = rb.position;
            direction = (targetPosition - currentPosition).normalized;

            // Applique la vitesse
            rb.linearVelocity = direction * speed;
            animator.SetFloat("Speed", 1);
            Aurra.SetActive(true);

            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;

            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                animator.SetFloat("Speed", 0);
                Aurra.SetActive(false);
                StartCoroutine(PauseAtPosition());
            }
        }
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
