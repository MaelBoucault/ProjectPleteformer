using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float destroyTime = 3f;

    private void Start()
    {
        Destroy(gameObject,destroyTime);
    }
}
