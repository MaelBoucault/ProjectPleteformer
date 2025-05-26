using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        string objectName = gameObject.name;

        // Utilise la nouvelle méthode recommandée
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && obj.name == objectName)
            {
                Destroy(obj);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
