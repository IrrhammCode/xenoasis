using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — WaterSphereController.cs
/// Offering 1: The Gift of Water
/// 
/// Controls the floating water sphere interaction.
/// When the player's hands enter the sphere, water ripples and droplets detach.
/// When the player cups and lifts, the sphere fragments into floating ice crystals.
/// </summary>
[RequireComponent(typeof(HandProximityDetector))]
public class WaterSphereController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandProximityDetector handDetector;
    [SerializeField] private Transform waterSphereMesh;
    [SerializeField] private ParticleSystem rippleParticles;
    [SerializeField] private ParticleSystem dropletParticles;
    [SerializeField] private ParticleSystem iceCrystalParticles;
    [SerializeField] private Light sphereLight;

    [Header("Sphere Settings")]
    [SerializeField] private float baseScale = 0.6f;
    [SerializeField] private float pulseAmplitude = 0.02f;
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Interaction Settings")]
    [Tooltip("How many seconds the player must hold both hands near the sphere to complete")]
    [SerializeField] private float holdDurationRequired = 4.0f;

    [Tooltip("Minimum proximity required to count as 'holding' (0-1)")]
    [SerializeField] private float holdProximityThreshold = 0.6f;

    [Header("Visual Feedback")]
    [SerializeField] private Color idleColor = new Color(0f, 1f, 0.82f, 0.5f);    // Cyan transparent
    [SerializeField] private Color activeColor = new Color(0.5f, 0.9f, 1f, 0.7f);  // Bright cyan
    [SerializeField] private Color completeColor = new Color(0f, 0.8f, 1f, 0.9f);  // Intense cyan

    [Header("Light Settings")]
    [SerializeField] private float idleLightIntensity = 0.5f;
    [SerializeField] private float activeLightIntensity = 2.0f;
    [SerializeField] private float completeLightIntensity = 3.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource waterAmbientSource;
    [SerializeField] private AudioSource interactionSource;
    [SerializeField] private AudioClip rippleSound;
    [SerializeField] private AudioClip fragmentSound;
    [SerializeField] private AudioClip completionChime;

    // State
    private bool isComplete = false;
    private float holdTimer = 0f;
    private float currentPulse = 0f;
    private Renderer sphereRenderer;
    private MaterialPropertyBlock propBlock;

    // Offering index for this interaction (0 = Water)
    private const int OFFERING_INDEX = 0;

    private void Start()
    {
        if (handDetector == null)
            handDetector = GetComponent<HandProximityDetector>();

        if (waterSphereMesh != null)
        {
            sphereRenderer = waterSphereMesh.GetComponent<Renderer>();
            propBlock = new MaterialPropertyBlock();
        }

        // Start with idle state
        SetVisualState(0f);

        // Start idle animation
        if (rippleParticles != null) rippleParticles.Stop();
        if (dropletParticles != null) dropletParticles.Stop();
        if (iceCrystalParticles != null) iceCrystalParticles.Stop();
    }

    private void Update()
    {
        if (isComplete) return;

        float proximity = handDetector.NormalizedProximity;

        // Idle pulse animation
        currentPulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        float scale = baseScale + currentPulse + (proximity * 0.05f);

        if (waterSphereMesh != null)
        {
            waterSphereMesh.localScale = Vector3.one * scale;
            waterSphereMesh.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // Visual feedback based on proximity
        SetVisualState(proximity);

        // Ripple particles when hand is near
        if (proximity > 0.2f && rippleParticles != null && !rippleParticles.isPlaying)
        {
            rippleParticles.Play();
        }
        else if (proximity <= 0.2f && rippleParticles != null && rippleParticles.isPlaying)
        {
            rippleParticles.Stop();
        }

        // Droplet particles when hand is close
        if (proximity > 0.5f && dropletParticles != null && !dropletParticles.isPlaying)
        {
            dropletParticles.Play();
            if (interactionSource != null && rippleSound != null)
                interactionSource.PlayOneShot(rippleSound, 0.3f);
        }
        else if (proximity <= 0.5f && dropletParticles != null && dropletParticles.isPlaying)
        {
            dropletParticles.Stop();
        }

        // Hold detection — both hands required for completion
        if (handDetector.IsBothHandsPresent && proximity >= holdProximityThreshold)
        {
            holdTimer += Time.deltaTime;

            // Scale sphere down as it "fragments"
            float fragmentProgress = holdTimer / holdDurationRequired;
            float fragmentScale = Mathf.Lerp(baseScale, baseScale * 0.3f, fragmentProgress);
            if (waterSphereMesh != null)
                waterSphereMesh.localScale = Vector3.one * fragmentScale;

            // Increase droplet emission rate as hold progresses
            if (dropletParticles != null)
            {
                var emission = dropletParticles.emission;
                emission.rateOverTime = Mathf.Lerp(10f, 200f, fragmentProgress);
            }

            if (holdTimer >= holdDurationRequired)
            {
                CompleteOffering();
            }
        }
        else
        {
            // Slowly reset hold timer (forgiving design)
            holdTimer = Mathf.Max(0f, holdTimer - Time.deltaTime * 0.3f);
        }

        // Audio: adjust ambient volume based on proximity
        if (waterAmbientSource != null)
        {
            waterAmbientSource.volume = Mathf.Lerp(0.1f, 0.6f, proximity);
        }
    }

    private void SetVisualState(float proximity)
    {
        if (sphereRenderer == null) return;

        // Interpolate emissive color
        Color emissiveColor = Color.Lerp(idleColor, activeColor, proximity);
        sphereRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_EmissionColor", emissiveColor * (1f + proximity * 2f));
        sphereRenderer.SetPropertyBlock(propBlock);

        // Light intensity
        if (sphereLight != null)
        {
            sphereLight.intensity = Mathf.Lerp(idleLightIntensity, activeLightIntensity, proximity);
            sphereLight.color = emissiveColor;
        }
    }

    private void CompleteOffering()
    {
        if (isComplete) return;
        isComplete = true;

        Debug.Log("[WaterSphere] OFFERING 1 COMPLETE — The Gift of Water is sealed.");

        // Stop all interaction particles
        if (rippleParticles != null) rippleParticles.Stop();
        if (dropletParticles != null) dropletParticles.Stop();

        // Play ice crystal fragmentation
        if (iceCrystalParticles != null) iceCrystalParticles.Play();

        // Shrink sphere to nothing
        StartCoroutine(ShrinkAndDissolve());

        // Play completion audio
        if (interactionSource != null)
        {
            if (fragmentSound != null) interactionSource.PlayOneShot(fragmentSound);
            if (completionChime != null)
                StartCoroutine(PlayDelayed(completionChime, 0.5f));
        }

        // Set light to completion state
        if (sphereLight != null)
        {
            sphereLight.intensity = completeLightIntensity;
            sphereLight.color = completeColor;
        }

        // Notify the OfferingManager
        if (OfferingManager.Instance != null)
            OfferingManager.Instance.CompleteOffering(OFFERING_INDEX);
    }

    private IEnumerator ShrinkAndDissolve()
    {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 startScale = waterSphereMesh != null ? waterSphereMesh.localScale : Vector3.one * baseScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(startScale.x, 0f, t);

            if (waterSphereMesh != null)
                waterSphereMesh.localScale = Vector3.one * scale;

            yield return null;
        }

        if (waterSphereMesh != null)
            waterSphereMesh.gameObject.SetActive(false);
    }

    private IEnumerator PlayDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (interactionSource != null)
            interactionSource.PlayOneShot(clip);
    }
}
