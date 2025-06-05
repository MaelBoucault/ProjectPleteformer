using UnityEngine;

public class BgParalax : MonoBehaviour
{
    private float startpos;
    private Transform cam;
    private float parallaxEffect;

    public float minEffect = 0.5f;
    public float maxEffect = 1.5f;
    public float maxScale = 10f;

    void Start()
    {
        startpos = transform.position.x;
        cam = Camera.main.transform;

        float scale = transform.localScale.x;
        scale = Mathf.Clamp(scale, 0f, maxScale);

        float t = scale / maxScale;
        parallaxEffect = Mathf.Lerp(minEffect, maxEffect, t);
    }

    void Update()
    {
        float distance = cam.position.x * parallaxEffect;
        transform.position = new Vector3(startpos + distance, transform.position.y);
    }
}
