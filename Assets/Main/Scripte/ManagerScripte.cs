using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScripte : MonoBehaviour
{

    PlayerActionMove Player;

    private void Start()
    {
        Player = FindAnyObjectByType<PlayerActionMove>();
    }

    void Update()
    {
        CheckForEnnemiesAndLoadNextLevel();
    }

    void CheckForEnnemiesAndLoadNextLevel()
    {
        var remainingEnemies = GameObject
            .FindGameObjectsWithTag("Ennemies")
            .Select(e => e.GetComponent<EnnemieHealth>())
            .Where(h => h != null && h.health > 0)
            .ToList();

        if (remainingEnemies.Count <= 0)
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        Player.transform.position = new Vector3(0,5,0);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}