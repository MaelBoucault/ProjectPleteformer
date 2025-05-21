using UnityEngine;

public class Pilier : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private bool moving = false;

    public void SetTarget(Vector3 targetPosition)
    {
        GameObject go = new GameObject("Target");
        go.transform.position = targetPosition;
        target = go.transform;
        moving = true;
    }

    void Update()
    {
        if (moving && target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                moving = false;
                Destroy(target.gameObject);
            }
        }
    }
}
