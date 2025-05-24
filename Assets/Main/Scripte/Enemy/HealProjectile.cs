using UnityEngine;

public class HealProjectile : MonoBehaviour
{
    public Vector3 target;
    public float speed = 10f;
    public GameObject impactEffect;

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector3 targetPosition)
    {
        target = targetPosition;
    }
}
