using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — FloraBloom.cs
/// Offering 3: The Gift of Life
/// 
/// Controls the dormant alien-earth hybrid flora interaction.
/// Holding an open palm above the flora channels radiant solar energy (EnergyBeamVFX),
/// causing the dormant bud to gradually awaken, unfurl, and burst into full bioluminescent bloom.
/// </summary>
[RequireComponent(typeof(HandProximityDetector))]
public class FloraBloom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandProximityDetector handDetector;
    [SerializeField] private EnergyBeamVFX energyBeamVFX;
    [SerializeField] private Transform dormantFloraMesh;
    [SerializeField] private Transform bloomedFloraMesh;
    [SerializeField] private ParticleSystem pollenParticles;
    [SerializeField] private Light floraLight;

    [Header("Audio")]
    [SerializeField] private AudioSource floraAudioSource;
    [SerializeField] private AudioClip energyHumClip;
    [SerializeField] private AudioClip bloomChordClip;

    [Header("Bloom Progression Settings")]
    [Tooltip("Sustained seconds required with open palm to reach full bloom")]
    [SerializeField] private float bloomDurationRequired = 6.0f;

    [Tooltip("Rate at which energy decays if hand is pulled away")]
    [SerializeField] private float decayRate = 0.3f;

    [Header("Colors")]
    [SerializeField] private Color dormantColor = new Color(0.1f, 0.2f, 0.15f);
    [SerializeField] private Color fullBloomColor = new Color(0f, 1f, 0.82f); // Bioluminescent Cyan
    [SerializeField] private Color goldenAuraColor = new Color(1f, 0.85f, 0.4f); // Golden pollen glow

    private const int OFFERING_INDEX = 2; // 2 = Flora (Life)
    private bool isComplete = false;
    private float bloomProgress = 0f; // 0 to 1

    private void Start()
    {
        if (handDetector == null)
            handDetector = GetComponent<HandProximityDetector>();

        if (bloomedFloraMesh != null)
            bloomedFloraMesh.gameObject.SetActive(false);

        if (pollenParticles != null)
            pollenParticles.Stop();

        if (floraLight != null)
            floraLight.intensity = 0.2f;
    }

    private void Update()
    {
        if (isComplete) return;

        float proximity = handDetector.NormalizedProximity;
        bool isPalmFacing = handDetector.IsPalmFacingObject;
        Vector3 palmPos = handDetector.ClosestPalmPosition;
        Vector3 floraTopPos = transform.position + Vector3.up * 0.4f;

        // Condition: palm is held above the flora and facing it
        bool isOfferingEnergy = proximity > 0.3f && isPalmFacing;

        if (isOfferingEnergy)
        {
            bloomProgress += (Time.deltaTime / bloomDurationRequired);

            // Channel energy beam VFX
            if (energyBeamVFX != null)
                energyBeamVFX.UpdateBeam(palmPos, floraTopPos, proximity);

            if (floraAudioSource != null && energyHumClip != null && !floraAudioSource.isPlaying)
            {
                floraAudioSource.clip = energyHumClip;
                floraAudioSource.loop = true;
                floraAudioSource.Play();
            }

            if (floraAudioSource != null && floraAudioSource.isPlaying)
            {
                floraAudioSource.volume = Mathf.Lerp(0.2f, 0.8f, bloomProgress);
            }

            // Animate swelling / uncurling of dormant mesh
            if (dormantFloraMesh != null)
            {
                float swell = 1f + Mathf.Sin(Time.time * 6f) * 0.05f * bloomProgress;
                dormantFloraMesh.localScale = Vector3.one * (1f + bloomProgress * 0.2f) * swell;
            }

            if (floraLight != null)
            {
                floraLight.intensity = Mathf.Lerp(0.2f, 2.5f, bloomProgress);
                floraLight.color = Color.Lerp(dormantColor, goldenAuraColor, bloomProgress);
            }

            if (bloomProgress >= 1.0f)
            {
                CompleteOffering();
            }
        }
        else
        {
            // Forgiving decay
            bloomProgress = Mathf.Max(0f, bloomProgress - Time.deltaTime * decayRate);

            if (energyBeamVFX != null)
                energyBeamVFX.StopBeam();

            if (floraAudioSource != null && floraAudioSource.isPlaying)
            {
                floraAudioSource.volume = Mathf.Lerp(floraAudioSource.volume, 0f, Time.deltaTime * 3f);
                if (floraAudioSource.volume <= 0.05f)
                    floraAudioSource.Stop();
            }

            if (dormantFloraMesh != null)
            {
                dormantFloraMesh.localScale = Vector3.Lerp(dormantFloraMesh.localScale, Vector3.one, Time.deltaTime * 2f);
            }
        }
    }

    private void CompleteOffering()
    {
        if (isComplete) return;
        isComplete = true;

        Debug.Log("[FloraBloom] OFFERING 3 COMPLETE — The Gift of Life is sealed.");

        if (energyBeamVFX != null)
            energyBeamVFX.StopBeam();

        // Switch to bloomed mesh
        if (dormantFloraMesh != null)
            dormantFloraMesh.gameObject.SetActive(false);

        if (bloomedFloraMesh != null)
        {
            bloomedFloraMesh.gameObject.SetActive(true);
            bloomedFloraMesh.localScale = Vector3.zero;
            StartCoroutine(BloomUnfurlAnimation());
        }

        // Release pollen burst
        if (pollenParticles != null)
            pollenParticles.Play();

        // Audio chord
        if (floraAudioSource != null && bloomChordClip != null)
        {
            floraAudioSource.Stop();
            floraAudioSource.loop = false;
            floraAudioSource.PlayOneShot(bloomChordClip);
        }

        if (floraLight != null)
        {
            floraLight.intensity = 3.5f;
            floraLight.color = fullBloomColor;
        }

        if (OfferingManager.Instance != null)
            OfferingManager.Instance.CompleteOffering(OFFERING_INDEX);
    }

    private IEnumerator BloomUnfurlAnimation()
    {
        float duration = 2.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Overshoot bounce
            float scale = Mathf.SmoothStep(0f, 1f, t);
            if (bloomedFloraMesh != null)
                bloomedFloraMesh.localScale = Vector3.one * scale;
            yield return null;
        }
    }
}
