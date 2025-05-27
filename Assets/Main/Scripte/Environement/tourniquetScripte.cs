using UnityEngine;

public class TourniquetScripte : MonoBehaviour
{
    public Transform[] plateformes;  // Les 4 plateformes à faire tourner
    public float vitesseRotation = 30f;  // vitesse en degrés par seconde

    void Update()
    {
        // On fait tourner chaque plateforme autour du pivot (this.transform.position)
        foreach (Transform plateforme in plateformes)
        {
            // Faire tourner autour du pivot selon l'axe Z (dans 2D)
            plateforme.RotateAround(transform.position, Vector3.forward, vitesseRotation * Time.deltaTime);

            // Réinitialiser la rotation de la plateforme pour qu'elle reste droite (rotation nulle)
            plateforme.rotation = Quaternion.identity;
        }
    }
}
