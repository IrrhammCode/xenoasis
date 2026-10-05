using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — UFOController.cs
/// Controls the celestial UFO craft hovering above the futuristic glass dome.
/// Manages gentle idle hovering, slow rotation, and the dramatic beacon response sequence.
/// </summary>
public class UFOController : MonoBehaviour
{
    [Header("Hover & Rotation Motion")]
    [Tooltip("Amplitude of vertical sine hover oscillation in meters.")]
    [SerializeField] private float hoverAmplitude = 0.3f;

    [Tooltip("Frequency multiplier for hover oscillation.")]
    [SerializeField] private float hoverSpeed = 0.5f;

    [Tooltip("Speed in degrees per second for continuous Y-axis rotation.")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Lighting References")]
    [Tooltip("Primary downward spotlight illuminating the sanctuary dome.")]
    [SerializeField] private Light spotLight;

    [Tooltip("Array of peripheral lights along the UFO hull edge.")]
    [SerializeField] private Light[] hullLights;

    [Header("Spotlight Intensities")]
    [Tooltip("Baseline spotlight intensity in idle state.")]
    [SerializeField] private float defaultSpotIntensity = 2f;

    [Tooltip("Max spotlight intensity during the climax sequence.")]
    [SerializeField] private float climaxSpotIntensity = 15f;

    [Header("Climax Response Settings")]
    [Tooltip("Delay in seconds after beacon ignition before the UFO initiates response.")]
    [SerializeField] private float climaxResponseDelay = 2f;

    [Tooltip("Particle system emitting downward golden stardust/rain upon response.")]
    [SerializeField] private ParticleSystem goldenRainParticles;

    // Movement state
    private Vector3 basePosition;
    public Vector3 BasePosition => basePosition;

    // Lighting state cache
    private float[] initialHullIntensities;
    private bool isResponding = false;
    private Coroutine responseCoroutine;

    private readonly Color warmGold = new Color(1.0f, 0.9f, 0.5f, 1.0f);

    private void Start()
    {
        // 1. Set basePosition from transform.position
        basePosition = transform.position;

        // Initialize spotlight
        if (spotLight != null)
        {
            spotLight.intensity = defaultSpotIntensity;
        }

        // Cache baseline hull light intensities
        if (hullLights != null)
        {
            initialHullIntensities = new float[hullLights.Length];
            for (int i = 0; i < hullLights.Length; i++)
            {
                if (hullLights[i] != null)
                {
                    initialHullIntensities[i] = hullLights[i].intensity;
                }
            }
        }
    }

    private void Update()
    {
        // Gentle sine hover on Y axis
        float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = basePosition + new Vector3(0f, yOffset, 0f);

        // Slow continuous Y-axis rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Active climax hull pulsing
        if (isResponding)
        {
            UpdateHullPulsing();
        }
    }

    /// <summary>
    /// Initiates UFO climax response when the beacon fires.
    /// </summary>
    public void RespondToBeacon()
    {
        if (responseCoroutine != null)
        {
            StopCoroutine(responseCoroutine);
        }
        responseCoroutine = StartCoroutine(BeaconResponseSequence());
    }

    /// <summary>
    /// Coroutine sequence executing beacon response:
    /// 1. Waits climaxResponseDelay seconds
    /// 2. Over 3 seconds, increases spotlight intensity to climaxSpotIntensity
    /// 3. Changes spotlight color to warm gold (1, 0.9, 0.5)
    /// 4. Hull lights pulse brighter
    /// 5. Emits downward particle burst if goldenRainParticles exists
    /// </summary>
    private IEnumerator BeaconResponseSequence()
    {
        Debug.Log($"[UFOController] Beacon ignited. Waiting {climaxResponseDelay}s before responding...");

        // 2. Waits climaxResponseDelay seconds
        yield return new WaitForSeconds(climaxResponseDelay);

        Debug.Log("[UFOController] UFO responding to beacon: Activating golden beam & rain burst.");
        isResponding = true;

        // 6. Emits a downward particle burst (if ParticleSystem reference exists)
        if (goldenRainParticles != null)
        {
            if (!goldenRainParticles.isPlaying)
            {
                goldenRainParticles.Play();
            }
            goldenRainParticles.Emit(120);
        }

        // 3, 4, 5. Over 3 seconds, increases spotlight intensity to climaxSpotIntensity,
        // changes spotlight color to warm gold (1, 0.9, 0.5), hull lights pulse brighter
        float rampDuration = 3.0f;
        float elapsed = 0f;

        Color startSpotColor = spotLight != null ? spotLight.color : Color.white;
        float startSpotIntensity = spotLight != null ? spotLight.intensity : defaultSpotIntensity;

        while (elapsed < rampDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rampDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Increase spotlight intensity and tint to warm gold
            if (spotLight != null)
            {
                spotLight.intensity = Mathf.Lerp(startSpotIntensity, climaxSpotIntensity, smoothT);
                spotLight.color = Color.Lerp(startSpotColor, warmGold, smoothT);
            }

            // Hull lights pulse brighter
            if (hullLights != null)
            {
                for (int i = 0; i < hullLights.Length; i++)
                {
                    if (hullLights[i] != null)
                    {
                        float baseIntensity = (initialHullIntensities != null && i < initialHullIntensities.Length)
                            ? initialHullIntensities[i]
                            : 1.0f;

                        float pulseFactor = 1.0f + 0.6f * Mathf.Sin(Time.time * 8f + i);
                        hullLights[i].intensity = Mathf.Lerp(baseIntensity, baseIntensity * 3.5f * pulseFactor, smoothT);
                        hullLights[i].color = Color.Lerp(hullLights[i].color, warmGold, smoothT * 0.6f);
                    }
                }
            }

            yield return null;
        }

        // Lock final climax state
        if (spotLight != null)
        {
            spotLight.intensity = climaxSpotIntensity;
            spotLight.color = warmGold;
        }
    }

    /// <summary>
    /// Sustained dynamic pulsing for hull lights during climax phase.
    /// </summary>
    private void UpdateHullPulsing()
    {
        if (hullLights == null) return;

        for (int i = 0; i < hullLights.Length; i++)
        {
            if (hullLights[i] != null)
            {
                float baseIntensity = (initialHullIntensities != null && i < initialHullIntensities.Length)
                    ? initialHullIntensities[i]
                    : 1.0f;

                float rhythmicPulse = 1.0f + 0.45f * Mathf.Sin(Time.time * 6f + (i * 0.8f));
                hullLights[i].intensity = baseIntensity * 3.5f * rhythmicPulse;
            }
        }
    }
}
