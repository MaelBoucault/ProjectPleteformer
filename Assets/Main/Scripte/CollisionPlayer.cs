using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionPlayer : MonoBehaviour
{
    private PlayerSoundController playerSoundController;

    void Awake()
    {
        playerSoundController = GetComponent<PlayerSoundController>();
        if (playerSoundController == null)
        {
            Debug.LogWarning("CollisionPlayer: PlayerSoundController not found on this GameObject. Sound calls might fail.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fall")
        {
            gameObject.GetComponent<PlayerHealth>().UpdateHealth(-gameObject.GetComponent<PlayerHealth>().MaxHealth, new Vector3(0, 0, 0));
            if (playerSoundController != null)
            {
                playerSoundController.PlayDieSound();
            }
        }

        if (collision.gameObject.tag == "Respawn")
        {
            gameObject.transform.position = new Vector3(0, 0, 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            gameObject.transform.position = new Vector3(0, 0, 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
