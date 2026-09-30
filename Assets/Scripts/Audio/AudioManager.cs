using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — AudioManager.cs
/// Manages spatial ambient drone, interactive offering soundscapes, and climax crescendo.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientDroneSource;
    [SerializeField] private AudioSource waterAmbienceSource;
    [SerializeField] private AudioSource climaxCrescendoSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip cosmicDroneClip;
    [SerializeField] private AudioClip waterDropsClip;
    [SerializeField] private AudioClip climaxCrescendoClip;
    [SerializeField] private AudioClip offeringCompleteClip;

    [Header("Volume Balances")]
    [Range(0f, 1f)] [SerializeField] private float targetDroneVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float targetWaterVolume = 0.4f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Starts ambient cosmic soundscapes with smooth fade-in.
    /// </summary>
    public void StartAmbient()
    {
        if (ambientDroneSource != null && cosmicDroneClip != null)
        {
            ambientDroneSource.clip = cosmicDroneClip;
            ambientDroneSource.loop = true;
            ambientDroneSource.volume = 0f;
            ambientDroneSource.Play();
            StartCoroutine(FadeSourceVolume(ambientDroneSource, targetDroneVolume, 4f));
        }

        if (waterAmbienceSource != null && waterDropsClip != null)
        {
            waterAmbienceSource.clip = waterDropsClip;
            waterAmbienceSource.loop = true;
            waterAmbienceSource.volume = 0f;
            waterAmbienceSource.Play();
            StartCoroutine(FadeSourceVolume(waterAmbienceSource, targetWaterVolume, 5f));
        }
    }

    /// <summary>
    /// Plays the dramatic 30-45s ascending climax crescendo when all 3 offerings are sealed.
    /// </summary>
    public void PlayClimaxCrescendo()
    {
        // Duck ambient slightly
        if (ambientDroneSource != null)
            StartCoroutine(FadeSourceVolume(ambientDroneSource, 0.15f, 2f));
        if (waterAmbienceSource != null)
            StartCoroutine(FadeSourceVolume(waterAmbienceSource, 0.1f, 2f));

        if (climaxCrescendoSource != null && climaxCrescendoClip != null)
        {
            climaxCrescendoSource.clip = climaxCrescendoClip;
            climaxCrescendoSource.volume = 0f;
            climaxCrescendoSource.Play();
            StartCoroutine(FadeSourceVolume(climaxCrescendoSource, 1f, 15f));
        }
    }

    /// <summary>
    /// Plays celebratory offering completed chime.
    /// </summary>
    public void PlayOfferingCompleteSFX()
    {
        if (sfxSource != null && offeringCompleteClip != null)
        {
            sfxSource.PlayOneShot(offeringCompleteClip);
        }
    }

    /// <summary>
    /// Smoothly fades out all active audio channels.
    /// </summary>
    public void FadeOutAll(float duration)
    {
        if (ambientDroneSource != null)
            StartCoroutine(FadeSourceVolume(ambientDroneSource, 0f, duration));
        if (waterAmbienceSource != null)
            StartCoroutine(FadeSourceVolume(waterAmbienceSource, 0f, duration));
        if (climaxCrescendoSource != null)
            StartCoroutine(FadeSourceVolume(climaxCrescendoSource, 0f, duration));
        if (sfxSource != null)
            StartCoroutine(FadeSourceVolume(sfxSource, 0f, duration));
    }

    private IEnumerator FadeSourceVolume(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;
        if (targetVolume <= 0.001f)
            source.Stop();
    }
}
