using UnityEngine;
using System.Collections;


public class Scriptmorve : MonoBehaviour
{
    public GameObject MorveEnfant;

    private void Start()
    {
        StartCoroutine(StartDespawn());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerActionMove>().CanDash = true;
        }
    }


    IEnumerator StartDespawn()
    {
        yield return new WaitForSeconds(10f);
        StartCoroutine(Despawn());
    
    }

    private IEnumerator Despawn()
    {
        float elapsedTime = 0f;
        Color originalColor = MorveEnfant.GetComponent<SpriteRenderer>().color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsedTime < 1.5f)
        {
            float lerp = Mathf.PingPong(Time.time * 5, 1);
            MorveEnfant.GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, transparentColor, lerp);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        MorveEnfant.GetComponent<SpriteRenderer>().color = originalColor;

        Destroy(gameObject);
    }

}
