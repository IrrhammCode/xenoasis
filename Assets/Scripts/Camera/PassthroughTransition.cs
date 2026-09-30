using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — PassthroughTransition.cs
/// Controls the PICO 4 Ultra Mixed Reality (MR) Video See-Through transition.
/// Starts the experience showing the player's physical room, then fades into
/// the cosmic Welcome Chamber.
/// </summary>
public class PassthroughTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Enable MR Passthrough at start of experience")]
    [SerializeField] private bool enablePassthroughOnStart = false;

    [Tooltip("Duration in seconds before dissolving real room to VR")]
    [SerializeField] private float passthroughDuration = 10f;

    [Tooltip("Duration of the crossfade dissolve from Passthrough to VR")]
    [SerializeField] private float crossfadeDuration = 3f;

    [Header("Environment References")]
    [SerializeField] private GameObject virtualEnvironmentRoot;
    [SerializeField] private ParticleSystem realWorldAnomaliesVFX;

    private bool isPassthroughActive = false;

    private void Start()
    {
        if (enablePassthroughOnStart)
        {
            StartCoroutine(PassthroughSequence());
        }
    }

    /// <summary>
    /// Starts in See-Through mode, triggers anomalies, and transitions into VR.
    /// </summary>
    public IEnumerator PassthroughSequence()
    {
        SetPassthrough(true);

        // Hide virtual world initially
        if (virtualEnvironmentRoot != null)
            virtualEnvironmentRoot.SetActive(false);

        // Spawn glowing cyan anomalies into the real room
        if (realWorldAnomaliesVFX != null)
            realWorldAnomaliesVFX.Play();

        // Wait while player sees anomalies emerging in physical space
        yield return new WaitForSeconds(passthroughDuration);

        // Reveal the virtual environment
        if (virtualEnvironmentRoot != null)
            virtualEnvironmentRoot.SetActive(true);

        // Turn off passthrough, transitioning fully into the VR Sanctuary
        SetPassthrough(false);

        // Allow crossfade dissolve time
        yield return new WaitForSeconds(crossfadeDuration);

        if (realWorldAnomaliesVFX != null)
            realWorldAnomaliesVFX.Stop();
    }

    public void SetPassthrough(bool enable)
    {
        isPassthroughActive = enable;

#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            // PICO XR SDK API for MR Video See-Through
            Unity.XR.PXR.PXR_MixedReality.EnableVideoSeeThroughManual(enable);
            Debug.Log($"[PassthroughTransition] PICO Passthrough set to: {enable}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[PassthroughTransition] Failed to set PICO Passthrough: {ex.Message}");
        }
#else
        Debug.Log($"[PassthroughTransition] Editor Mock Passthrough set to: {enable}");
#endif
    }
}
