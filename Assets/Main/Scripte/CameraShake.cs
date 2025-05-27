using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static void Shake(float intensity = 0.1f, float duration = 0.2f)
    {
        GameObject cam = Camera.main.gameObject;
        intensity = intensity *2;
        duration = duration * 1.5f;
        iTween.ShakePosition(cam, iTween.Hash(
            "amount", new Vector3(intensity, intensity, 0),
            "time", duration,
            "easetype", iTween.EaseType.easeOutExpo
        ));
    }
}
