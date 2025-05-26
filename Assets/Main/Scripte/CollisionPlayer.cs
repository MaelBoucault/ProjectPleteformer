using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fall")
        {
            gameObject.GetComponent<PlayerHealth>().UpdateHealth(-gameObject.GetComponent<PlayerHealth>().MaxHealth, new Vector3(0,0,0));
        }

        if (collision.gameObject.tag == "Respawn")
        {
            gameObject.transform.position = new Vector3(0,0,0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }
}
