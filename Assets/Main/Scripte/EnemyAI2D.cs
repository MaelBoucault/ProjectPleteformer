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

    private Transform currentTarget;
    internal bool chasing = false;
    private Rigidbody2D rb;
    private Vector3 originalScale;

    Animator animator;

    internal bool Attack;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB;
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            chasing = true;
        }
        else if (distanceToPlayer > stopChaseRange)
        {
            chasing = false;
        }

        if (chasing)
        {
            MoveTowards(player.position);
        }
        else
        {
            Patrol();
        }

        ApplySquashAndStretch();
    }

    void Patrol()
    {
        MoveTowards(currentTarget.position);


        if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }
    }

    void MoveTowards(Vector2 target)
    {
        if (!Attack)
        {
            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = new Vector2(target.x, currentPosition.y);
            Vector2 direction = (targetPosition - currentPosition).normalized;

            animator.SetFloat("Speed", Math.Abs(direction.x));

            Vector2 desiredVelocity = direction * speed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, 0.1f);

            FlipSprite(direction.x);
        }
        
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

}