using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;


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

    public AnimationClip AnimationAttack;

    public AnimatorController AnimationController;

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

            if (shootTimer >= shootInterval && !hasShot)
            {
                animator.SetBool("Attacking", true);
                shootTimer = 0f;
                hasShot = true;
            }
        }
    }

    public void ShootKayou()
    {
        if (!enemyAI.Attack)
        {
            enemyAI.Attack = true;
            GameObject kayou = Instantiate(kayouPrefab, transform.position + new Vector3(0,1f,0), Quaternion.identity);

            Vector2 direction = (player.position - transform.position).normalized;

            Rigidbody2D kayouRb = kayou.GetComponent<Rigidbody2D>();
            if (kayouRb != null)
            {
                kayouRb.linearVelocity = direction * shootForce;
            }
        }
    }

    public void RestartAttack()
    {
        enemyAI.Attack = false;
        hasShot = false;
        animator.SetBool("Attacking",false);
    }
}
