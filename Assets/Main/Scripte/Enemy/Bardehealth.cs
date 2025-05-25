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

    private EnnemieHealth ennemieHealth;
    private bool hasDied = false;
    


    private void Start()
    {
        ennemieHealth = GetComponent<EnnemieHealth>();
        if (ennemieHealth == null)
        {
            Debug.LogError("BardeHealth : Aucun EnnemieHealth trouvé sur ce GameObject.");
            enabled = false;
            return;
        }

        UpdateUI(); // Initialisation
    }

    private void Update()
    {
        if (ennemieHealth == null) return;

        // ⚠️ Détection de la mort du barde
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

        // Première phase : petite expansion rapide
        iTween.ScaleTo(AurraToExplode, iTween.Hash(
            "scale", Vector3.one * (explosionScale * 0.5f),
            "time", explosionTime * 0.5f,
            "easetype", iTween.EaseType.easeOutBack
        ));

        // Délai avant la deuxième phase
        Invoke(nameof(TriggerSecondExplosion), 0.1f); // petit temps d'attente entre les deux phases
    }

    private void TriggerSecondExplosion()
    {
        if (AurraToExplode == null) return;

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
