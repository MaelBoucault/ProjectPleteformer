using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BardeHealth : MonoBehaviour
{
    [Header("UI")]
    public Slider VieBarde;
    public TMP_Text CurrentHealthBarde;

    [Header("Effets")]
    public GameObject AurraToExplode;
    public float explosionScale = 3f;
    public float explosionTime = 0.5f;
    public GameObject Sprite;

    private EnnemieHealth ennemieHealth;
    private bool hasDied = false;
    


    private void Start()
    {
        ennemieHealth = GetComponent<EnnemieHealth>();
        if (ennemieHealth == null)
        {
            enabled = false;
            return;
        }

        UpdateUI();
    }

    private void Update()
    {
        if (ennemieHealth == null) return;

        if (!hasDied && ennemieHealth.health <= 0f)
        {
            hasDied = true;
            TriggerAuraExplosion();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        int ennemisRestants = GameObject
            .FindGameObjectsWithTag("Ennemies")
            .Select(e => e.GetComponent<EnnemieHealth>())
            .Where(h => h != null && h.health > 0)
            .Count();

        float displayHealth = ennemieHealth.health * (1 + ennemisRestants);
        float displayMaxHealth = ennemieHealth.maxHealth * (1 + ennemisRestants);

        if (VieBarde != null)
        {
            VieBarde.maxValue = displayMaxHealth;
            VieBarde.value = displayHealth;
        }

        if (CurrentHealthBarde != null)
        {
            CurrentHealthBarde.text = Mathf.CeilToInt(displayHealth) + " / " + Mathf.CeilToInt(displayMaxHealth);
        }
    }

    private void TriggerAuraExplosion()
    {
        BardeScripte bardeScript = GetComponent<BardeScripte>();
        if (bardeScript != null)
        {
            bardeScript.SendMessage("OnStartExplosion");
        }

        if (AurraToExplode == null) return;

        //Extention en y
        iTween.ScaleTo(Sprite, iTween.Hash(
            "scale", new Vector3 (0.1f,0.3f,0),
            "time", explosionTime * 0.5f,
            "easetype", iTween.EaseType.easeOutBack
        ));


        // Première phase : petite expansion rapide
        iTween.ScaleTo(AurraToExplode, iTween.Hash(
            "scale", Vector3.one * (explosionScale * 0.5f),
            "time", explosionTime * 0.5f,
            "easetype", iTween.EaseType.easeOutBack
        ));

        Invoke(nameof(TriggerSecondExplosion), 0.15f);
    }

    private void TriggerSecondExplosion()
    {
        if (AurraToExplode == null) return;

        //Extention en x
        iTween.ScaleTo(AurraToExplode, iTween.Hash(
            "scale", new Vector3(0.3f, 0.1f, 0),
            "time", explosionTime * 0.5f,
            "easetype", iTween.EaseType.easeInOutExpo,
            "oncomplete", "DestroyAura",
            "oncompletetarget", gameObject
        ));
        // Deuxième phase : expansion finale
        iTween.ScaleTo(AurraToExplode, iTween.Hash(
            "scale", Vector3.one * explosionScale,
            "time", explosionTime * 0.5f,
            "easetype", iTween.EaseType.easeInOutExpo,
            "oncomplete", "DestroyAura",
            "oncompletetarget", gameObject
        ));
    }

    private void DestroyAura()
    {
        if (AurraToExplode != null)
        {
            Destroy(AurraToExplode);
        }

        Destroy(gameObject,0f);
    }

}
