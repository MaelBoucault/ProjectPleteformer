using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    public GameObject Player;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void Start()
    {
        if (Player == null)
        {
            Player = GameObject.Find("Player");
        }

        if (offset == Vector3.zero)
        {
            offset = new Vector3(0, 0, -20);
        }
    }

    void LateUpdate()
    {
        if (Player != null)
        {

            Vector3 desiredPosition = new Vector3(Player.transform.position.x + offset.x, Player.transform.position.y + offset.y, offset.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
        }
    }
}
