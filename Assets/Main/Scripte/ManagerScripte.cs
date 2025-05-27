using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ManagerScripte : MonoBehaviour
{
    private int nbEnnemies;
    private float nbTotalHealth;
    private float nbTotalHealthMax;

    public Slider SliderTotalHealth;


    private void Start()
    {
        UpdateEnnemiesCount();

        nbTotalHealthMax = nbTotalHealth;

        SliderTotalHealth.maxValue = nbTotalHealthMax;

    }
    void Update()
    {
        UpdateEnnemiesCount();

        SliderTotalHealth.value = nbTotalHealth;
    }

    void UpdateEnnemiesCount()
    {
        var enemies = GameObject
        .FindGameObjectsWithTag("Ennemies")
        .Select(e => e.GetComponent<EnnemieHealth>())
        .Where(h => h != null && h.health > 0)
        .ToList();

        nbEnnemies = enemies.Count;

        for (int i = 0; i < nbEnnemies; i++)
        {
            nbTotalHealthMax += enemies[i].GetComponent<EnnemieHealth>().health;
        }
    }
}
