using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerActionMove : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float dashManaCost = 5f;

    [Header("Mana Settings")]
    public float maxMana = 150f;
    public float manaRegenRate = 10f;

    public float currentMana;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;

    private Vector2 dashDirection;

    public bool IsDashing => isDashing;

    public GameObject PilierPrefab;
    public int Pilier = 2;
    public int PilierMax = 2;
    public Text PilierTxt;
    public GameObject CirclePrefab;


    internal bool CanDash = false;
    private bool isRegeneratingMana = false;

    private PlayerMovement PlayerMovementScript;

    public GameObject Morve;

    void Awake()
    {
        PlayerMovementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;

    }

    void Update()
    {
        PilierTxt.text = Pilier.ToString();

        HandleMana();

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (CanDash && Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0 && !isDashing && currentMana >= dashManaCost)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }

        Morve.SetActive(CanDash);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Scriptmorve>())
        {
            CanDash = true;
            if (!collision.GetComponent<Pilier>())
                Pilier = PilierMax;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Scriptmorve>())
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ResetCanDash());
            }
        }
    }


    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        dashDirection = new Vector2(horizontal, vertical).normalized;

        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(transform.localScale.x, 0).normalized;

        if (dashDirection == Vector2.down && CirclePrefab != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f);

            if (hit.collider != null)
            {
                Vector3 spawnPosition = hit.point;

                GameObject shockwave = Instantiate(CirclePrefab, spawnPosition, Quaternion.identity);
                Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();

                if (shockwaveScript != null)
                {
                    shockwaveScript.damage = 10f;
                    shockwaveScript.radius = 5f;
                    shockwaveScript.lifetime = 1f;
                }

                iTween.ScaleFrom(shockwave, iTween.Hash(
                    "scale", new Vector3(0.1f, 0.1f, 0.1f),
                    "time", 0.2f,
                    "easetype", iTween.EaseType.easeOutExpo
                ));
            }
        }

        if ((vertical > 0.1f && horizontal == 0) && PilierPrefab != null && Pilier > 0)
        {
            StartCoroutine(DepancePilier());

            GameObject pilier = Instantiate(PilierPrefab, transform.position - new Vector3 (0,5,0), Quaternion.identity);
            pilier.GetComponent<Pilier>().SetTarget(gameObject.GetComponent<PlayerMovement>().groundCheck.position);
            dashForce = 20f;
            CameraShake.Shake(0.1f, 0.15f);
            iTween.ScaleFrom(pilier, iTween.Hash(
                "scale", new Vector3(0.1f, 0.1f, 0.1f),
                "time", 0.3f,
                "easetype", iTween.EaseType.easeOutBack
            ));

        }
        if ((vertical == 0 && horizontal >= 0.1f) && GetComponent<PlayerMovement>().isGrounded)
        {
            dashForce = 28f;
            CameraShake.Shake(0.2f, 0.3f);
        }
        else if (!(GetComponent<PlayerMovement>().isGrounded))
        {
            dashForce = 25f;
            CameraShake.Shake(0.15f, 0.2f);
        }else CameraShake.Shake(0.15f, 0.2f);

        rb.linearVelocity = dashDirection * dashForce;
        

        GameObject visual = transform.gameObject;

        iTween.PunchPosition(visual, iTween.Hash(
            "amount", new Vector3(dashDirection.x, dashDirection.y, 0) * 1.2f,
            "time", 0.3f,
            "easetype", iTween.EaseType.easeOutQuad
        ));

    }



    void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }

    void HandleMana()
    {
        if (!isDashing && currentMana < maxMana && !isRegeneratingMana)
        {
            StartCoroutine(RechargeManaDash());
        }
    }


    IEnumerator RechargeManaDash()
    {
        isRegeneratingMana = true;

        while (currentMana < maxMana && !isDashing)
        {
            yield return new WaitForSeconds(2f); // Recharge toutes les 2 secondes

            currentMana += manaRegenRate;
            currentMana = Mathf.Min(currentMana, maxMana);

        }

        isRegeneratingMana = false;
    }

    IEnumerator DepancePilier()
    {
        yield return new WaitForSeconds(0.2f);
        Pilier--;
    }


    IEnumerator ResetCanDash()
    {
        yield return new WaitForSeconds(5f);
        CanDash = false;
    }
}