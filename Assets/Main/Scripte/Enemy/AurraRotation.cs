using UnityEngine;

public class AurraRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
    }
}
