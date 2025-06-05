using UnityEngine;

public class FloatingTiltingHouse : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatHeight = 0.5f;
    public float floatDuration = 2f;

    [Header("Rotation Settings")]
    public float tiltAngle = 10f;
    public float tiltDuration = 3f;

    void Start()
    {
        float delay = Random.Range(0f, 5f);

        iTween.MoveBy(gameObject, iTween.Hash(
            "y", floatHeight,
            "time", floatDuration,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong,
            "delay", delay
        ));

        delay = Random.Range(0f, 5f);
        iTween.RotateBy(gameObject, iTween.Hash(
            "z", tiltAngle / 360f,
            "time", tiltDuration,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong,
            "delay", delay
        ));
    }
}
