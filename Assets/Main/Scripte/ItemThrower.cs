using UnityEngine;
using System.Collections;

public class ItemThrower : MonoBehaviour
{
    [Header("Projectil")]
    public GameObject[] itemPrefabs;
    public Transform throwOrigin;
    public float throwForce = 10f;

    [Header("Animation")]
    public Animator Animator;

    private PlayerActionMove PlayerActionMove;
    public bool useController = false;
    private bool isShooting = false;
    private Coroutine shootCoroutine;


    public GameObject PauseSrciptCanvas;

    private void Start()
    {
        PlayerActionMove = GetComponent<PlayerActionMove>();
    }

    void Update()
    {
        if (!PauseSrciptCanvas.GetComponent<PauseSrcipt>().isPause)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (shootCoroutine == null)
                {
                    isShooting = true;
                    shootCoroutine = StartCoroutine(ShootRepeatedly());
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                isShooting = false;
                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
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

            yield return new WaitForSeconds(0.5f);
        }
    }

    void ThrowItem()
    {
        PlayerActionMove.currentMana -= 10f;
        PlayerActionMove.UpdateManaUI();
        if (itemPrefabs.Length == 0) return;

        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        GameObject item = Instantiate(itemPrefab, throwOrigin.position, Quaternion.identity);

        Vector2 direction;

        if (useController)
        {
            // Visée avec le stick gauche
            float aimX = Input.GetAxis("Horizontal");
            float aimY = Input.GetAxis("Vertical");
            direction = new Vector2(aimX, aimY);

            if (direction.sqrMagnitude < 0.1f)
                direction = Vector2.right;
            else
                direction.Normalize();
        }
        else
        {
            // Visée souris
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
            direction = (mouseWorldPos - throwOrigin.position).normalized;
        }

        // Flip du sprite
        if (direction.x > 0)
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);

        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        item.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        StartCoroutine(RestartBoolAttack());
    }

    IEnumerator RestartBoolAttack()
    {
        yield return new WaitForSeconds(0.1f);
        Animator.SetBool("Attack", false);
    }

}
