using UnityEngine;

public class EnemyPatrolSmooth : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float smoothTime = 0.3f;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 5f;
    public float fieldOfView = 120f; // Degrees
    public LayerMask obstacleMask;

    private Transform target;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        if (PlayerInSight())
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (target == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime, speed);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    void FollowPlayer()
    {
        if (player == null) return;

        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, playerPos, ref velocity, smoothTime, speed);
    }

    bool PlayerInSight()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in detection range
        if (distanceToPlayer > detectionRange)
            return false;

        // Check if player is within field of view
        float angle = Vector3.Angle(transform.right, directionToPlayer);
        if (angle > fieldOfView / 2f)
            return false;

        // Raycast to check for obstacles
        if (Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
            return false;

        return true;
    }


    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;

        // Dessiner une ligne vers le joueur (facultatif)
        Gizmos.DrawLine(transform.position, player.position);

        // Rayon de détection
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Afficher le cône du champ de vision
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fieldOfView / 2) * transform.right;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fieldOfView / 2) * transform.right;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);
    }

}
