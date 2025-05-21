using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject kayouPrefab;
    public float shootInterval = 2f;
    public float shootForce = 5f;
    private Transform player;
    private bool isChasing = false;
    private float shootTimer = 0f;

    private EnemyAI2D enemyAI;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI2D>();
        player = enemyAI.player;

        shootForce = Random.Range(10f, 15f);
    }

    void Update()
    {
        isChasing = enemyAI.chasing;

        if (isChasing)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                ShootKayou();
                shootTimer = 0f;
            }
        }
    }

    void ShootKayou()
    {
        animator.SetBool("Attacking", enemyAI.Attack);
        enemyAI.Attack = true;
        if (kayouPrefab == null || player == null) return;

        GameObject kayou = Instantiate(kayouPrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;

        Rigidbody2D kayouRb = kayou.GetComponent<Rigidbody2D>();
        if (kayouRb != null)
        {
            kayouRb.linearVelocity = direction * shootForce;
        }

        StartCoroutine(RestartAttack());
    }

    IEnumerator RestartAttack()
    {
        yield return new WaitForSeconds(5f);

        enemyAI.Attack = false;
    }
}
