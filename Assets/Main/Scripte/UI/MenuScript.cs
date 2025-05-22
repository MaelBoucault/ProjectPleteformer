using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject Player;

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenSite(string url)
    {
        Application.OpenURL(url);
    }

    public void MannetteSwitch(GameObject button)
    {
        if (!Player.TryGetComponent<ItemThrower>(out ItemThrower itemThrower)) return;
        if (!button.TryGetComponent<Image>(out Image image)) return;

        itemThrower.useController = !itemThrower.useController;

        Color baseColor = image.color;

        image.color = new Color(baseColor.r, baseColor.g, baseColor.b, itemThrower.useController ? 0.5f : 1f);
    }

}
