using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static IEnumerator Shake()
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float shakeAmount = 0.05f;
        float shakeDuration = 0.2f;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Camera.main.transform.position = originalPosition + (Random.insideUnitSphere * shakeAmount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPosition;
    }
}
