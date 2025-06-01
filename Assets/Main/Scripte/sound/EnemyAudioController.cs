using UnityEngine;
using System.Collections.Generic;

public enum EnemySpecificType
{
    None,
    Ogre,
    Enfant,
    Barde,
    Yeux,
    // Add other enemy types here if needed
}

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioController : MonoBehaviour
{
    private AudioSource enemyAudioSource;

    [Tooltip("Select the specific type of this enemy. Used to map sounds.")]
    [SerializeField]
    private EnemySpecificType enemySpecificType = EnemySpecificType.None;

    private Dictionary<string, AudioType> enemySoundMap;

    [Header("Randomization Settings")]
    [Range(0, 3f)]
    [Tooltip("The range by which the pitch of the sound will be randomly varied.")]
    public float randomPitchRange = 1.5f;

    [Range(0f, 1f)]
    [Tooltip("The probability (0-1) that a sound will play. 1 means always play, 0.25 means 1 chance out of 4.")]
    public float soundPlayProbability = 1f; // Default to 1 (always play)

    void Awake()
    {
        enemyAudioSource = GetComponent<AudioSource>();

        enemySoundMap = new Dictionary<string, AudioType>();

        switch (enemySpecificType)
        {
            case EnemySpecificType.Ogre:
                enemySoundMap.Add("Grr", AudioType.OgreGrr);
                enemySoundMap.Add("AttackCharge", AudioType.OgreAttackCharge); // Specific Ogre charge sound
                enemySoundMap.Add("AttackShoot", AudioType.OgreAttackShoot);   // Specific Ogre shoot sound
                enemySoundMap.Add("Hit", AudioType.HitOgre);
                enemySoundMap.Add("Die", AudioType.DieOgre);
                enemySoundMap.Add("Charge", AudioType.OgreAttackCharge); // For generic EnemyShoot script
                enemySoundMap.Add("Tire", AudioType.OgreAttackShoot);    // For generic EnemyShoot script
                break;
            case EnemySpecificType.Enfant:
                enemySoundMap.Add("GneuGneu", AudioType.EnfantGneuGneu);
                enemySoundMap.Add("Pause", AudioType.EnfantPause);
                enemySoundMap.Add("Hit", AudioType.HitEnfant);
                enemySoundMap.Add("Die", AudioType.DieEnfant);
                break;
            case EnemySpecificType.Barde:
                enemySoundMap.Add("Fuit", AudioType.BardeFuit);
                enemySoundMap.Add("Heal", AudioType.BardeHeal);
                enemySoundMap.Add("Tp", AudioType.BardeTp);
                enemySoundMap.Add("Attack", AudioType.BardeAttack);
                enemySoundMap.Add("Hit", AudioType.HitBarde);
                enemySoundMap.Add("Die", AudioType.DieBarde);
                break;
            case EnemySpecificType.Yeux:
                enemySoundMap.Add("Charge", AudioType.YeuxCharge);
                enemySoundMap.Add("Tire", AudioType.YeuxTire);
                enemySoundMap.Add("Hit", AudioType.HitYeux);
                enemySoundMap.Add("Die", AudioType.DieYeux);
                break;
            case EnemySpecificType.None:
                Debug.LogWarning($"EnemyAudioController: 'Enemy Specific Type' is not set for GameObject '{gameObject.name}'. Please configure it in the Inspector.");
                break;
            default:
                Debug.LogWarning($"EnemyAudioController: Unhandled 'Enemy Specific Type' '{enemySpecificType}' for GameObject '{gameObject.name}'. Check the switch-case in Awake().");
                break;
        }

        if (enemySoundMap.Count == 0 && enemySpecificType != EnemySpecificType.None)
        {
            Debug.LogWarning($"EnemyAudioController: No specific sounds defined for {enemySpecificType} in Awake().");
        }
    }

    /// <summary>
    /// Plays a sound specific to this enemy, with optional randomization.
    /// </summary>
    /// <param name="soundKey">The key of the sound to play (e.g., "Grr", "Charge").</param>
    /// <param name="useRandomPitch">If true, apply random pitch variation.</param>
    /// <param name="useProbability">If true, apply sound play probability.</param>
    public void PlayEnemySound(string soundKey, bool useRandomPitch = true, bool useProbability = true)
    {
        if (enemyAudioSource == null)
        {
            Debug.LogError($"EnemyAudioController: AudioSource is not assigned on '{gameObject.name}'!");
            return;
        }

        if (SoundScripte.Instance == null)
        {
            Debug.LogError("EnemyAudioController: SoundScripte.Instance is not found! Make sure your global SoundManager is in the scene and active.");
            return;
        }

        // Apply probability check
        if (useProbability && Random.Range(0f, 1f) > soundPlayProbability)
        {
            // Debug.Log($"Sound '{soundKey}' skipped due to probability for {gameObject.name}.");
            return; // Don't play the sound
        }

        if (enemySoundMap.TryGetValue(soundKey, out AudioType audioType))
        {
            AudioClip clip = SoundScripte.Instance.getClip(audioType);
            if (clip != null)
            {
                float originalPitch = enemyAudioSource.pitch; // Store original pitch

                if (useRandomPitch)
                {
                    enemyAudioSource.pitch = originalPitch + Random.Range(-randomPitchRange, randomPitchRange);
                }

                enemyAudioSource.PlayOneShot(clip);

                enemyAudioSource.pitch = originalPitch; // Reset pitch to original after playing
            }
        }
        else
        {
            Debug.LogWarning($"EnemyAudioController: Sound key '{soundKey}' not mapped for enemy '{gameObject.name}' (Type: {enemySpecificType}). Check 'enemySoundMap' in Awake().");
        }
    }

    // --- Utility functions for specific calls (all will now use the new PlayEnemySound) ---
    // By default, these calls will use randomization (pitch & probability)

    public void PlayOgreGrr() => PlayEnemySound("Grr");
    public void PlayOgreAttackCharge() => PlayEnemySound("AttackCharge");
    public void PlayOgreAttackShoot() => PlayEnemySound("AttackShoot");

    public void PlayYeuxCharge() => PlayEnemySound("Charge");
    public void PlayYeuxTire() => PlayEnemySound("Tire");

    public void PlayEnfantGneuGneu() => PlayEnemySound("GneuGneu");
    public void PlayEnfantPause() => PlayEnemySound("Pause");

    public void PlayBardeFuit() => PlayEnemySound("Fuit");
    public void PlayBardeHeal() => PlayEnemySound("Heal");
    public void PlayBardeTp() => PlayEnemySound("Tp");
    public void PlayBardeAttack() => PlayEnemySound("Attack");

    public void PlayHitSound() => PlayEnemySound("Hit");
    public void PlayDieSound() => PlayEnemySound("Die", useRandomPitch: false, useProbability: false); // Death sounds often should always play without pitch variation
}