using UnityEngine;

public class PlateformeScript : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Color BaseColor;

    public Color ColorMorve;

    public bool IsMorve = false;


    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BaseColor = SpriteRenderer.color;
    }

    private void Update()
    {
        SwitchColor();
    }

    public void SwitchColor()
    {
        if (IsMorve)
        {
            SpriteRenderer.color = ColorMorve;
        }
        else
        {
            SpriteRenderer.color = BaseColor;
        }
    }
}
