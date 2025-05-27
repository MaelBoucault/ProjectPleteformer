using UnityEngine;

public class FloatingTiltingHouse : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatHeight = 0.5f;         // Hauteur du flottement
    public float floatDuration = 2f;         // Durée de va-et-vient

    [Header("Rotation Settings")]
    public float tiltAngle = 10f;            // Degrés de rotation sur Z (ex: 10 = -10° à +10°)
    public float tiltDuration = 3f;          // Temps d'une oscillation complète

    void Start()
    {
        float delay = Random.Range(0f, 5f);

        // Mouvement vertical (flottement)
        iTween.MoveBy(gameObject, iTween.Hash(
            "y", floatHeight,
            "time", floatDuration,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong,
            "delay", delay
        ));

        delay = Random.Range(0f, 5f);
        // Rotation sur l'axe Z (balancement)
        iTween.RotateBy(gameObject, iTween.Hash(
            "z", tiltAngle / 360f,
            "time", tiltDuration,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong,
            "delay", delay
        ));
    }
}
