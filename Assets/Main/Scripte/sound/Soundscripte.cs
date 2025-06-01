using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    //Joueur
    Jump,
    Dash,
    Die,
    AttaqueJoueur1,
    AttaqueJoueur2,
    AttaqueJoueur3,
    Hit,
    Reward,

    // Sounds specific to the Ogre
    OgreGrr,
    OgreAttackCharge, // New: Ogre charging its attack
    OgreAttackShoot,  // New: Ogre firing its attack
    HitOgre,
    DieOgre,

    // Sounds specific to the "Yeux" (Eyes) enemy type
    YeuxCharge, // The "Eyes" enemy charging
    YeuxTire,   // The "Eyes" enemy firing
    HitYeux,    // New: when the "Eyes" enemy is hit
    DieYeux,    // New: when the "Eyes" enemy dies

    // Sounds specific to the Enfant
    EnfantGneuGneu,
    EnfantPause,
    HitEnfant,
    DieEnfant,

    // Sounds specific to the Barde
    BardeFuit,
    BardeHeal,
    BardeTp,
    BardeAttack,
    HitBarde,
    DieBarde,
}

public enum AudioSourceType
{
    Game,
    Player
}

public class SoundScripte : MonoBehaviour
{
    static public SoundScripte Instance;

    [Tooltip("Volume global for all AudioSources managed by this script.")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private Dictionary<AudioSourceType, AudioSource> audioSourcesMap = new Dictionary<AudioSourceType, AudioSource>();

    [System.Serializable]
    public struct AudioSourceConfig
    {
        public AudioSourceType type;
        public AudioSource source;
    }

    [Tooltip("Configure all your game's AudioSources and their types here.")]
    public AudioSourceConfig[] audioSourceConfigs;

    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public AudioType type;
    }

    [Tooltip("Associate all your AudioClips with their respective types here.")]
    public AudioData[] audioData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var config in audioSourceConfigs)
        {
            if (config.source == null)
            {
                Debug.LogWarning($"SoundScripte: AudioSource for type {config.type} is not assigned in the inspector!");
                continue;
            }

            if (!audioSourcesMap.ContainsKey(config.type))
            {
                audioSourcesMap.Add(config.type, config.source);
            }
            else
            {
                Debug.LogWarning($"SoundScripte: Duplicate AudioSourceType {config.type} found in configuration. Only the first one will be used.");
            }
        }
    }

    void Start()
    {
        foreach (var sourceEntry in audioSourcesMap)
        {
            sourceEntry.Value.volume = volume;
        }
    }

    public void PlaySound(AudioType type, AudioSourceType sourceType)
    {
        AudioClip clip = getClip(type);

        if (clip == null)
        {
            return;
        }

        if (audioSourcesMap.TryGetValue(sourceType, out AudioSource targetSource))
        {
            targetSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError($"SoundScripte: No AudioSource configured for type {sourceType}. Please check your 'Audio Source Configs' in the Inspector.");
        }
    }

    public AudioClip getClip(AudioType type)
    {
        foreach (AudioData data in audioData)
        {
            if (data.type == type)
            {
                return data.clip;
            }
        }
        Debug.LogError("SoundScripte: No AudioClip found for type " + type + ". Make sure it's assigned in 'Audio Data' in the Inspector.");
        return null;
    }
}