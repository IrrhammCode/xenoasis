using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — BeaconClimaxVFX.cs
/// Controls the grand climax sequence:
/// 1. Energy streams converge from the 3 golden pedestals to the center.
/// 2. Vertical beacon column erupts skyward into the void.
/// 3. Particle burst and intense radiant flare.
/// </summary>
public class BeaconClimaxVFX : MonoBehaviour
{
    [Header("Convergence Beams")]
    [Tooltip("LineRenderers or Particle Beams connecting pedestals to center")]
    [SerializeField] private LineRenderer[] pedestalBeams;
    [SerializeField] private Transform[] pedestalOrigins;
    [SerializeField] private Transform centerConvergencePoint;

    [Header("Beacon Column")]
    [SerializeField] private ParticleSystem verticalBeaconParticles;
    [SerializeField] private ParticleSystem stardustAcceleratorParticles;
    [SerializeField] private Light beaconCenterLight;

    [Header("Settings")]
    [SerializeField] private float beamConvergeDuration = 3.0f;
    [SerializeField] private float maxLightIntensity = 12f;
    [SerializeField] private Color beaconColor = new Color(1f, 0.95f, 0.7f); // Warm Solar Flare Gold

    private void Start()
    {
        // Hide beams and particles initially
        if (pedestalBeams != null)
        {
            foreach (var beam in pedestalBeams)
            {
                if (beam != null) beam.enabled = false;
            }
        }

        if (verticalBeaconParticles != null)
            verticalBeaconParticles.Stop();

        if (stardustAcceleratorParticles != null)
            stardustAcceleratorParticles.Stop();

        if (beaconCenterLight != null)
            beaconCenterLight.intensity = 0f;
    }

    /// <summary>
    /// Triggered by GameManager when all 3 offerings are sealed.
    /// </summary>
    public void IgniteBeacon()
    {
        StartCoroutine(BeaconSequence());
    }

    private IEnumerator BeaconSequence()
    {
        Debug.Log("[BeaconClimaxVFX] Beacon sequence initiated.");

        // Beat 1: Converge beams from pedestals to center
        float elapsed = 0f;
        if (pedestalBeams != null && pedestalOrigins != null && centerConvergencePoint != null)
        {
            for (int i = 0; i < pedestalBeams.Length; i++)
            {
                if (pedestalBeams[i] != null)
                {
                    pedestalBeams[i].enabled = true;
                    pedestalBeams[i].positionCount = 2;
                    if (i < pedestalOrigins.Length && pedestalOrigins[i] != null)
                        pedestalBeams[i].SetPosition(0, pedestalOrigins[i].position);
                }
            }

            while (elapsed < beamConvergeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / beamConvergeDuration;

                for (int i = 0; i < pedestalBeams.Length; i++)
                {
                    if (pedestalBeams[i] != null && i < pedestalOrigins.Length && pedestalOrigins[i] != null)
                    {
                        Vector3 currentTarget = Vector3.Lerp(pedestalOrigins[i].position, centerConvergencePoint.position, t);
                        pedestalBeams[i].SetPosition(1, currentTarget);
                    }
                }
                yield return null;
            }
        }

        // Beat 2: Ignite vertical pillar into deep void
        if (verticalBeaconParticles != null)
            verticalBeaconParticles.Play();

        if (stardustAcceleratorParticles != null)
            stardustAcceleratorParticles.Play();

        // Light expansion
        elapsed = 0f;
        float lightDuration = 4.0f;
        while (elapsed < lightDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lightDuration;
            if (beaconCenterLight != null)
            {
                beaconCenterLight.intensity = Mathf.Lerp(0f, maxLightIntensity, t);
                beaconCenterLight.color = beaconColor;
            }
            yield return null;
        }
    }
}
