using System;
using UnityEngine;
using UnityEngine.Timeline;


public class ProjectilScript : MonoBehaviour
{
    public float AmountDamage;
    public GameObject morvePrefab;

    bool StopPas2 = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && !collision.GetComponent<Scriptmorve>() && !StopPas2)
        {
            Destroy(gameObject);
            StopPas2 = true;
            Vector2 hitPoint = collision.ClosestPoint(transform.position - (transform.forward*5));
            Vector2 center = collision.bounds.center;
            Vector2 direction = (hitPoint - center);

            Vector2 offset = Vector2.zero;
            Quaternion rotation = Quaternion.identity;

            bool isHorizontal = Mathf.Abs(direction.x) >= Mathf.Abs(direction.y);

            /*if (isHorizontal)
            {
                float sign = Mathf.Sign(direction.x);
                offset = new Vector2(0.5f * sign, 0);
                rotation = Quaternion.Euler(0, 0, -90 * sign);
            }
            else
            {
                float sign = Mathf.Sign(direction.y);
                offset = new Vector2(0, 0.5f * sign);
                rotation = Quaternion.Euler(0, 0, sign > 0 ? 0 : 180);
            }*/

            float sign = Mathf.Sign(direction.y);
            offset = new Vector2(0, 0.5f * sign);
            rotation = Quaternion.Euler(0, 0, sign > 0 ? 0 : 180);


            Vector3 finalPosition = hitPoint + offset;

            GameObject morveInstance = Instantiate(morvePrefab, finalPosition, rotation);
            morveInstance.transform.SetParent(collision.transform);

            
        }

        if (collision.gameObject.CompareTag("Ennemies"))
        {
            collision.gameObject.GetComponent<EnnemieHealth>().UpdateHealth(-AmountDamage);
            Destroy(gameObject);
        }
    }


}