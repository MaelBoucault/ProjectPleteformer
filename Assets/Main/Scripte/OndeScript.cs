using UnityEngine;

public class OndeScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnnemieHealth>())
        {
            collision.GetComponent<EnnemieHealth>().UpdateHealth(-2500);
        }
    }
}
