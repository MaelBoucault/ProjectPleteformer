using UnityEngine;

public class FadeInAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public float targetVolume = 1f;
    public float fadeDuration = 3f;

    private float currentTime = 0f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource.volume < targetVolume)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
        }
    }
}
