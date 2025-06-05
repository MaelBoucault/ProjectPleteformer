using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using TMPro.EditorUtilities;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MenuScript : MonoBehaviour
{
    public GameObject Player;


    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReStartGame()
    {
        SceneManager.LoadScene(0);
    }


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
