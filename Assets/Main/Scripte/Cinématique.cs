using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cinématique : MonoBehaviour
{
    public Sprite[] ListeImage;
    public float timeBetweenImages = 10f;

    private int CrtImage = 0;
    private Image ImageRenderer;

    private void Start()
    {
        ImageRenderer = GetComponent<Image>();
        StartCoroutine(PlayCinematic());
    }

    IEnumerator PlayCinematic()
    {
        while (CrtImage < ListeImage.Length)
        {
            // Mettre à jour l'image et réinitialiser l'échelle à 1
            ImageRenderer.sprite = ListeImage[CrtImage];
            transform.localScale = new Vector3(1, 1, 1);

            // Animation de scale vers 2
            iTween.ScaleTo(gameObject, iTween.Hash(
                "scale", new Vector3(1.2f, 1.2f, 1),
                "time", timeBetweenImages,
                "easetype", iTween.EaseType.easeOutSine
            ));

            // Attendre avant de passer à l’image suivante
            yield return new WaitForSeconds(timeBetweenImages);

            CrtImage++;
        }

        // Petite pause puis chargement de la scène suivante
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
