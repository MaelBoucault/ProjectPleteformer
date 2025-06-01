using System;
using System.Collections;
using UnityEditor.Build; // Note: UnityEditor namespace should not be in runtime scripts
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerActionMove : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public int NbDash = 3;
    public int NbDashCurl = 3;
    public bool CanDash = false;


    [Header("Mana Settings")]
    public float maxMana = 150f;
    public float manaRegenRate = 10f;
    public float currentMana;

    [Header("Pilier Settings")]
    public int Pilier = 2;
    public int PilierMax = 2;
    public Text PilierTxt;
    public GameObject PilierPrefab;

    [Header("Prefabs instance")]
    public GameObject CirclePrefab;
    public GameObject Morve;


    // Private/Internal Variables
    private Rigidbody2D rb;
    internal bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;
    private Vector2 dashDirection;
    private bool isRegeneratingMana = false;

    private bool IsMorve = false;
    private bool IsInstancingPilier = false;

    private PlayerMovement PlayerMovementScript;
    private Animator Animator;

    private float horizontal;
    private float vertical;

    // Référence au PlayerSoundController
    private PlayerSoundController playerSoundController;


    void Awake()
    {
        NbDashCurl = NbDash;
        Animator = GetComponent<Animator>();
        PlayerMovementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;

        // Récupère le PlayerSoundController sur le même GameObject
        playerSoundController = GetComponent<PlayerSoundController>();
        if (playerSoundController == null)
        {
            Debug.LogWarning("PlayerActionMove: PlayerSoundController not found on this GameObject. Action sounds might not play.");
        }
    }

    void Update()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashForce;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        CanDash = cooldownTimer <= 0 && !isDashing && (NbDashCurl > 0);


        PilierTxt.text = Pilier.ToString();

        HandleMana();

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (CanDash && (Input.GetKeyDown(KeyCode.LeftShift)))
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

        //Pilier
        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (PilierPrefab != null && Pilier > 0 && !IsInstancingPilier)
            {
                IsInstancingPilier = true;
                Pilier--;

                dashDirection = new Vector2(horizontal, vertical).normalized;

                rb.linearVelocity = dashDirection * (dashForce + 10f);

                GameObject pilier = Instantiate(PilierPrefab, transform.position - new Vector3(0, 5, 0), Quaternion.identity);
                pilier.GetComponent<Pilier>().SetTarget(gameObject.GetComponent<PlayerMovement>().groundCheck.position);
                dashForce = 20f;
                CameraShake.Shake(0.1f, 0.15f);
                iTween.ScaleFrom(pilier, iTween.Hash(
                    "scale", new Vector3(0.5f, 0.5f, 0.5f),
                    "time", 1f,
                    "easetype", iTween.EaseType.easeOutBack
                ));
                StartCoroutine(ResetPilier());

                if (playerSoundController != null)
                {
                    playerSoundController.PlayAttackSound(); // Joue un son d'attaque pour le pilier
                }
            }
        }

        //Charge Au Sol
        if (Input.GetKeyDown(KeyCode.S) && !PlayerMovementScript.isGrounded)
        {
            dashDirection = new Vector2(horizontal, vertical).normalized;

            rb.linearVelocity = dashDirection * dashForce * 0.75f;

            Vector3 rayOrigin = transform.position - new Vector3(0, 2.5f, 0);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 10f);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == 3) // Assurez-vous que le layer 3 est le sol ou un objet chargeable
                {
                    Vector3 spawnPosition = hit.point;

                    GameObject shockwave = Instantiate(CirclePrefab, spawnPosition, Quaternion.identity);

                    Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();
                    if (shockwaveScript != null)
                    {
                        shockwaveScript.damage = 15f;
                        shockwaveScript.radius = 5f;
                        shockwaveScript.lifetime = 1f;
                    }

                    iTween.ScaleFrom(shockwave, iTween.Hash(
                        "scale", new Vector3(15f, 15f, 15f),
                        "time", 0.3f,
                        "easetype", iTween.EaseType.easeOutExpo
                    ));

                    if (playerSoundController != null)
                    {
                        playerSoundController.PlayAttackSound(); // Joue un son d'attaque pour la charge au sol
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) NbDashCurl = NbDash;

        if (collision.GetComponent<Scriptmorve>())
        {
            if (!collision.GetComponent<Pilier>() && !IsInstancingPilier)
                Pilier = PilierMax;
        }
    }


    void StartDash()
    {
        NbDashCurl--;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        dashDirection = new Vector2(horizontal, vertical).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(transform.localScale.x, 0).normalized;

        if (IsMorve) dashForce = 30f;
        else if (dashForce == 0f) dashForce = 20f; // valeur par défaut

        if (Math.Abs(dashDirection.x) > 0)
            Animator.SetBool("IsDashHorizontal", true);

        iTween.PunchPosition(gameObject, iTween.Hash(
            "amount", new Vector3(dashDirection.x, dashDirection.y, 0) * 1.2f,
            "time", 0.3f,
            "easetype", iTween.EaseType.easeOutQuad
        ));
        isDashing = true;

        if (playerSoundController != null)
        {
            playerSoundController.PlayDashSound(); // Joue le son de dash
        }
    }


    void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        Animator.SetBool("IsDashHorizontal", isDashing);
    }

    void HandleMana()
    {
        if (!isDashing && currentMana < maxMana && !isRegeneratingMana)
        {
            StartCoroutine(RechargeManaTire());
        }
    }


    IEnumerator RechargeManaTire()
    {
        isRegeneratingMana = true;

        while (currentMana < maxMana)
        {
            yield return new WaitForSeconds(2f);

            currentMana += manaRegenRate;
            currentMana = Mathf.Min(currentMana, maxMana);

        }

        isRegeneratingMana = false;
    }

    IEnumerator ResetPilier()
    {
        yield return new WaitForSeconds(0.2f);
        IsInstancingPilier = false;
    }
}
