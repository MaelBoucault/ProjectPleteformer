using UnityEngine;
using System.Collections;

public class ItemThrower : MonoBehaviour
{
    [Header("projectil")]
    public GameObject[] itemPrefabs;
    public Transform throwOrigin;
    public float throwForce = 10f;

    [Header("Animation")]
    public Animator Animator;

    private PlayerActionMove PlayerActionMove;

    private bool isShooting = false;
    private Coroutine shootCoroutine;

    private void Start()
    {
        PlayerActionMove = GetComponent<PlayerActionMove>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (shootCoroutine == null)
            {
                isShooting = true;
                shootCoroutine = StartCoroutine(ShootRepeatedly());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
        }
    }

    IEnumerator ShootRepeatedly()
    {
        while (isShooting)
        {
            if (PlayerActionMove.currentMana >= 10f)
            {
                Animator.SetBool("Attack", true);
                ThrowItem();
            }

            yield return new WaitForSeconds(0.5f); // Tir toutes les 0.5 secondes
        }
    }

    void ThrowItem()
    {
        PlayerActionMove.currentMana -= 10f;
        PlayerActionMove.UpdateManaUI();
        if (itemPrefabs.Length == 0) return;

        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        GameObject item = Instantiate(itemPrefab, throwOrigin.position, Quaternion.identity);

        // Direction vers la souris
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
        Vector2 direction = (mouseWorldPos - throwOrigin.position).normalized;

        // Flip le joueur selon la direction
        if (mouseWorldPos.x > transform.position.x)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
        else if (mouseWorldPos.x < transform.position.x)
        {
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
        }

        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float offset = -90f;
        item.transform.rotation = Quaternion.Euler(0f, 0f, angle + offset);

        StartCoroutine(RestartBoolAttack());
    }

    IEnumerator RestartBoolAttack()
    {
        yield return new WaitForSeconds(0.1f);
        Animator.SetBool("Attack", false);
    }
}
