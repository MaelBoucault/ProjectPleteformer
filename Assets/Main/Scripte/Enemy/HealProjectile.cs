using Unity.VisualScripting;
using UnityEngine;

public class HealProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 targetPosition;
    private Transform targetTransform;
    private bool hasReachedTarget = false;

    public void SetTarget(Vector3 position, Transform target = null)
    {
        targetPosition = position;
        targetTransform = target;
    }

    private void Update()
    {
        if (hasReachedTarget) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasReachedTarget = true;

            if (targetTransform != null)
            {
                transform.SetParent(targetTransform);
                transform.localPosition = Vector3.zero;
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Scriptmorve>() == true)
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().UpdateHealth(-20,transform.position);
        }
    }
}
