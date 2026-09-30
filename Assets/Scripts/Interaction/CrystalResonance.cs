using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — CrystalResonance.cs
/// Offering 2: The Gift of Sound
/// 
/// Controls the hovering resonant crystal interaction.
/// When the player places both palms on either side, hand distance modulates
/// acoustic pitch and bioluminescent emission.
/// Holding the "harmonic sweet spot" unlocks the song and completes the offering.
/// </summary>
[RequireComponent(typeof(HandProximityDetector))]
public class CrystalResonance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandProximityDetector handDetector;
    [SerializeField] private Renderer crystalRenderer;
    [SerializeField] private Light crystalLight;
    [SerializeField] private SoundWaveVisualizer waveVisualizer;

    [Header("Audio")]
    [SerializeField] private AudioSource humSource;
    [SerializeField] private AudioSource resonanceSource;
    [SerializeField] private AudioClip resonantChimeClip;

    [Header("Sweet Spot Settings")]
    [Tooltip("Target normalized proximity range for harmonic resonance [min, max]")]
    [SerializeField] private float sweetSpotMin = 0.65f;
    [SerializeField] private float sweetSpotMax = 0.85f;
    [Tooltip("Duration in seconds the sweet spot must be held to seal the offering")]
    [SerializeField] private float sweetSpotDurationRequired = 3.0f;

    [Header("Pitch & Glow Settings")]
    [SerializeField] private float minPitch = 0.7f;
    [SerializeField] private float maxPitch = 1.6f;
    [SerializeField] private float harmonicPitch = 1.0f;
    [SerializeField] private Color idleColor = new Color(0.1f, 0.4f, 0.5f, 0.4f);
    [SerializeField] private Color activeColor = new Color(0f, 1f, 0.82f, 1f);
    [SerializeField] private Color goldenHarmonyColor = new Color(1f, 0.82f, 0.4f, 1f);

    private const int OFFERING_INDEX = 1; // 1 = Crystal (Sound)
    private bool isComplete = false;
    private float sweetSpotTimer = 0f;
    private MaterialPropertyBlock propBlock;
    private float pulseWaveTimer = 0f;

    private void Start()
    {
        if (handDetector == null)
            handDetector = GetComponent<HandProximityDetector>();

        if (crystalRenderer != null)
            propBlock = new MaterialPropertyBlock();

        if (humSource != null)
        {
            humSource.loop = true;
            humSource.volume = 0.1f;
            humSource.Play();
        }
    }

    private void Update()
    {
        if (isComplete) return;

        float proximity = handDetector.NormalizedProximity;
        bool bothHands = handDetector.IsBothHandsPresent;

        // Subtle hovering float
        transform.position += Vector3.up * (Mathf.Sin(Time.time * 2f) * 0.0005f);

        if (proximity > 0.05f)
        {
            // Pitch modulation
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, proximity);
            if (humSource != null)
            {
                humSource.pitch = Mathf.Lerp(humSource.pitch, targetPitch, Time.deltaTime * 5f);
                humSource.volume = Mathf.Lerp(0.15f, 0.8f, proximity);
            }

            // Check if player is in the harmonic sweet spot
            bool inSweetSpot = proximity >= sweetSpotMin && proximity <= sweetSpotMax && bothHands;

            if (inSweetSpot)
            {
                sweetSpotTimer += Time.deltaTime;
                SetVisuals(goldenHarmonyColor, 2.5f);

                // Snap pitch toward the perfect harmonic when in sweet spot
                if (humSource != null)
                    humSource.pitch = Mathf.Lerp(humSource.pitch, harmonicPitch, Time.deltaTime * 8f);

                // Pulse sound ripples more frequently
                pulseWaveTimer += Time.deltaTime;
                if (pulseWaveTimer >= 0.4f)
                {
                    pulseWaveTimer = 0f;
                    if (waveVisualizer != null)
                        waveVisualizer.EmitPulse(true);
                }

                if (sweetSpotTimer >= sweetSpotDurationRequired)
                {
                    CompleteOffering();
                }
            }
            else
            {
                sweetSpotTimer = Mathf.Max(0f, sweetSpotTimer - Time.deltaTime * 0.5f);
                Color currentColor = Color.Lerp(idleColor, activeColor, proximity);
                SetVisuals(currentColor, 1f + proximity * 1.5f);

                // Emit occasional subtle pulses
                pulseWaveTimer += Time.deltaTime;
                if (pulseWaveTimer >= 1.0f)
                {
                    pulseWaveTimer = 0f;
                    if (waveVisualizer != null)
                        waveVisualizer.EmitPulse(false);
                }
            }
        }
        else
        {
            sweetSpotTimer = 0f;
            SetVisuals(idleColor, 0.5f);
            if (humSource != null)
            {
                humSource.pitch = Mathf.Lerp(humSource.pitch, minPitch, Time.deltaTime * 2f);
                humSource.volume = Mathf.Lerp(humSource.volume, 0.1f, Time.deltaTime * 2f);
            }
        }
    }

    private void SetVisuals(Color emissiveColor, float intensity)
    {
        if (crystalRenderer != null)
        {
            crystalRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_EmissionColor", emissiveColor * intensity);
            crystalRenderer.SetPropertyBlock(propBlock);
        }

        if (crystalLight != null)
        {
            crystalLight.color = emissiveColor;
            crystalLight.intensity = intensity;
        }
    }

    private void CompleteOffering()
    {
        if (isComplete) return;
        isComplete = true;

        Debug.Log("[CrystalResonance] OFFERING 2 COMPLETE — The Gift of Sound is sealed.");

        SetVisuals(goldenHarmonyColor, 4.0f);

        if (waveVisualizer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(DelayedWave(i * 0.25f));
            }
        }

        if (resonanceSource != null && resonantChimeClip != null)
        {
            resonanceSource.PlayOneShot(resonantChimeClip);
        }

        if (OfferingManager.Instance != null)
            OfferingManager.Instance.CompleteOffering(OFFERING_INDEX);
    }

    private IEnumerator DelayedWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (waveVisualizer != null)
            waveVisualizer.EmitPulse(true);
    }
}
