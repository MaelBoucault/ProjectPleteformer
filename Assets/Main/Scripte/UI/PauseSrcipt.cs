using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseSrcipt : MonoBehaviour
{
    //public GameObject M;
    //public GameObject MiniMap;

    internal bool isPause = false;

    public GameObject menuPause;


    private void Awake()
    {
        Time.timeScale = 1f;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                isPause = false;
                menuPause.SetActive(isPause);
                Time.timeScale = 1f;
            }
            else
            {
                isPause = true;
                menuPause.SetActive(isPause);
                Time.timeScale = 0f;
            }
        }
    }

}
