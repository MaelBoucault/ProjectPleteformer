using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject kayouPrefab;
    public float shootInterval = 2f;
    public float shootForce = 5f;

    private Transform player;
    private bool isChasing = false;
    private float shootTimer = 0f;
    private bool hasShot = false;

    private EnemyAI2D enemyAI;
    private Animator animator;
    private EnemyAudioController enemyAudioController;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI2D>();
        player = FindAnyObjectByType<PlayerActionMove>().transform;
        enemyAudioController = GetComponent<EnemyAudioController>();

        if (enemyAudioController == null)
        {
            Debug.LogError("EnemyShoot: EnemyAudioController not found on this GameObject! Add it to the prefab.");
        }

        shootForce = Random.Range(10f, 15f);
    }

    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerActionMove>()?.transform;
            if (player == null) return;
        }

        if (enemyAI != null)
        {
            isChasing = enemyAI.chasing;
        }

        if (isChasing)
        {
            shootTimer += Time.deltaTime;

            if (shootTimer >= shootInterval && !hasShot)
            {
                animator.SetBool("Attacking", true);
                shootTimer = 0f;
                hasShot = true;

                if (enemyAudioController != null)
                {
                    enemyAudioController.PlayOgreAttackCharge();
                }
            }
        }
    }

    public void ShootKayou()
    {
        if (kayouPrefab == null || player == null) return;

        GameObject kayou = Instantiate(kayouPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;

        Rigidbody2D kayouRb = kayou.GetComponent<Rigidbody2D>();
        if (kayouRb != null)
        {
            kayouRb.linearVelocity = direction * shootForce;
        }

        if (enemyAudioController != null)
        {
            enemyAudioController.PlayOgreAttackShoot();
        }
    }

    public void RestartAttack()
    {
        if (enemyAI != null)
        {
            enemyAI.Attack = false;
        }
        hasShot = false;
        animator.SetBool("Attacking", false);
    }
}
