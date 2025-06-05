using UnityEngine;

public class TitlePulsation : MonoBehaviour
{
    public float scaleFactor = 1.5f;  // Facteur de grossissement maximum
    public float pulseTime = 1.0f;    // Temps pour une pulsation compl�te
    public iTween.EaseType easeType = iTween.EaseType.easeInOutSine;  // Type d'animation

    void Start()
    {
        PulseTitle();
    }

    void PulseTitle()
    {
        // On commence avec la taille initiale
        Vector3 initialScale = transform.localScale;

        // Appliquer l'animation de pulsation (grandir puis r�tr�cir)
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", initialScale * scaleFactor, // Taille maximum
            "time", pulseTime,                   // Dur�e de la pulsation
            "easeType", easeType,                // Type d'animation
            "loopType", iTween.LoopType.pingPong // Pour un effet de "ping-pong"
        ));
    }
}
