using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;

    [Header("Jump Mechanics")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.8f;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    private float coyoteTimer;
    private float jumpBufferTimer;

    public Slider SliderJump;
    public float JumpNbMax = 2;
    public float JumpNb;

    public bool Jump = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    internal bool isGrounded = false;


    [Header("Animation")]
    public Animator animator;

    private PlayerActionMove dashScript;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dashScript = GetComponent<PlayerActionMove>();
        JumpNb = JumpNbMax;
    }

    void Update()
    {
        HandleJumpMana();

        if (dashScript != null && dashScript.IsDashing)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (horizontal > 0 && !animator.GetBool("Attack"))
        {
            gameObject.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 1);
        }
        if (horizontal < 0 && !animator.GetBool("Attack"))
        {
            gameObject.GetComponent<Transform>().localScale = new Vector3(-0.5f, 0.5f, 1);
        }

        bool isGrounded = CheckGrounded();

        if (isGrounded)
        {
            JumpNb = JumpNbMax;
            Jump = false;
        }

        // Coyote time logic
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;

        // Jump buffer
        if (Input.GetButtonDown("Jump") && JumpNb >= 1)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0)
        {
            if (coyoteTimer > 0 && JumpNb > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferTimer = 0;
                JumpNb--;
                Jump = true;
            }
            else if (JumpNb > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferTimer = 0;
                JumpNb--;
                Jump = true;
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    bool CheckGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleJumpMana()
    {
        isGrounded = CheckGrounded();

        if (JumpNb < JumpNbMax && isGrounded)
        {
            JumpNb += 0.1f * Time.deltaTime;
            JumpNb = Mathf.Min(JumpNb, JumpNbMax);
        }

        if (SliderJump != null)
        {
            SliderJump.value = (JumpNb / JumpNbMax) * 100f;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
