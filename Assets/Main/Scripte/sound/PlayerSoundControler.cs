using UnityEngine;
using System.Collections.Generic; // Added for Dictionary if needed, but not strictly for this change

public class PlayerSoundController : MonoBehaviour
{
    // Référence à votre script de son (non nécessaire si vous utilisez SoundScripte.Instance directement)
    // public SoundScripte soundManager; 

    [Header("Randomization Settings")]
    [Range(0, 3f)]
    [Tooltip("The range by which the pitch of the sound will be randomly varied.")]
    public float randomPitchRange = 0.5f; // Default pitch range for player sounds

    [Range(0f, 1f)]
    [Tooltip("The probability (0-1) that a sound will play. 1 means always play, 0.25 means 1 chance out of 4.")]
    public float soundPlayProbability = 1f; // Default to 1 (always play)

    private AudioSource playerAudioSource; // Reference to the player's AudioSource

    void Awake()
    {
        // Get the AudioSource component attached to the player GameObject
        // If your player has multiple AudioSources, you might need to specify which one.
        // For simplicity, we assume one AudioSource on the player.
        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            Debug.LogWarning($"PlayerSoundController: No AudioSource found on '{gameObject.name}'. Please add one to play sounds directly.");
        }
    }

    void Update()
    {
        // Exemple: Jouer un son de saut quand la touche Espace est pressée
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayJumpSound();
        }

        // Exemple: Jouer un son d'attaque quand la touche Souris 0 (clic gauche) est pressée
        if (Input.GetMouseButtonDown(0))
        {
            PlayAttackSound();
        }

        // Exemple: Jouer un son de "Hit" si le joueur est touché (simulé ici par la touche H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayHitSound();
        }
    }

    /// <summary>
    /// Plays a sound for the player, with optional randomization for pitch and probability.
    /// </summary>
    /// <param name="type">The AudioType of the sound to play.</param>
    /// <param name="useRandomPitch">If true, apply random pitch variation.</param>
    /// <param name="useProbability">If true, apply sound play probability.</param>
    private void PlayPlayerSound(AudioType type, bool useRandomPitch = true, bool useProbability = true)
    {
        if (SoundScripte.Instance == null)
        {
            Debug.LogWarning("SoundScripte.Instance is not found. Make sure SoundScripte is in your scene and set up correctly.");
            return;
        }

        if (playerAudioSource == null)
        {
            Debug.LogWarning($"PlayerSoundController: Cannot play sound '{type}' because no AudioSource is assigned to the player GameObject '{gameObject.name}'.");
            return;
        }

        // Apply probability check
        if (useProbability && Random.Range(0f, 1f) > soundPlayProbability)
        {
            // Debug.Log($"Sound '{type}' skipped due to probability for {gameObject.name}.");
            return; // Don't play the sound
        }

        AudioClip clip = SoundScripte.Instance.getClip(type);

        if (clip == null)
        {
            return; // getClip already logs an error if clip is null
        }

        float originalPitch = playerAudioSource.pitch; // Store original pitch

        if (useRandomPitch)
        {
            // Ensure pitch doesn't go too low or too high
            playerAudioSource.pitch = originalPitch + Random.Range(-randomPitchRange, randomPitchRange);
            // Clamp pitch to a reasonable range, e.g., 0.5 to 2.0
            playerAudioSource.pitch = Mathf.Clamp(playerAudioSource.pitch, 0.5f, 2.0f);
        }

        // Play the sound using the player's dedicated AudioSource
        playerAudioSource.PlayOneShot(clip);

        // Reset pitch to original after playing to avoid affecting subsequent sounds
        playerAudioSource.pitch = originalPitch;
    }

    /// <summary>
    /// Joue le son de saut du joueur.
    /// </summary>
    public void PlayJumpSound()
    {
        PlayPlayerSound(AudioType.Jump); // Uses default randomization settings
        Debug.Log("Playing Jump Sound");
    }

    /// <summary>
    /// Joue un son d'attaque aléatoire pour le joueur.
    /// </summary>
    public void PlayAttackSound()
    {
        // Génère un nombre aléatoire entre 0 et 2 (inclus)
        int randomAttack = Random.Range(0, 3);
        AudioType attackType;

        switch (randomAttack)
        {
            case 0:
                attackType = AudioType.AttaqueJoueur1;
                break;
            case 1:
                attackType = AudioType.AttaqueJoueur2;
                break;
            case 2:
                attackType = AudioType.AttaqueJoueur3;
                break;
            default:
                // Fallback au cas où, bien que Random.Range devrait éviter cela
                attackType = AudioType.AttaqueJoueur1;
                break;
        }

        PlayPlayerSound(attackType); // Uses default randomization settings
        Debug.Log($"Playing Player Attack Sound: {attackType}");
    }

    /// <summary>
    /// Joue le son de "Hit" du joueur.
    /// </summary>
    public void PlayHitSound()
    {
        PlayPlayerSound(AudioType.Hit); // Uses default randomization settings
        Debug.Log("Playing Hit Sound");
    }

    /// <summary>
    /// Joue le son de mort du joueur.
    /// Death sounds often should always play without pitch variation or probability.
    /// </summary>
    public void PlayDieSound()
    {
        PlayPlayerSound(AudioType.Die, useRandomPitch: false, useProbability: false);
        Debug.Log("Playing Die Sound");
    }

    /// <summary>
    /// Joue le son de dash du joueur.
    /// </summary>
    public void PlayDashSound()
    {
        PlayPlayerSound(AudioType.Dash);
        Debug.Log("Playing Dash Sound");
    }

    /// <summary>
    /// Joue le son de récompense du joueur.
    /// </summary>
    public void PlayRewardSound()
    {
        PlayPlayerSound(AudioType.Reward);
        Debug.Log("Playing Reward Sound");
    }
}