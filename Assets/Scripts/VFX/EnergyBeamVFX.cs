using UnityEngine;

/// <summary>
/// XENOASIS — EnergyBeamVFX.cs
/// Emits glowing solar/bioluminescent particles streaming from the player's palm
/// downward into the dormant flora bud.
/// </summary>
public class EnergyBeamVFX : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem beamParticles;
    [SerializeField] private LineRenderer energyLine;

    [Header("Settings")]
    [SerializeField] private Color beamColor = new Color(1f, 0.85f, 0.4f, 0.8f); // Warm Solar Gold

    private bool isEmitting = false;

    private void Start()
    {
        if (beamParticles != null)
            beamParticles.Stop();

        if (energyLine != null)
            energyLine.enabled = false;
    }

    /// <summary>
    /// Updates the beam position from tracked hand to the flora receptacle.
    /// </summary>
    public void UpdateBeam(Vector3 palmPosition, Vector3 targetFloraPosition, float intensity)
    {
        if (intensity <= 0.05f)
        {
            StopBeam();
            return;
        }

        if (!isEmitting)
        {
            isEmitting = true;
            if (beamParticles != null && !beamParticles.isPlaying)
                beamParticles.Play();
            if (energyLine != null)
                energyLine.enabled = true;
        }

        transform.position = palmPosition;
        transform.LookAt(targetFloraPosition);

        if (energyLine != null)
        {
            energyLine.positionCount = 2;
            energyLine.SetPosition(0, palmPosition);
            energyLine.SetPosition(1, targetFloraPosition);

            energyLine.startWidth = 0.02f * intensity;
            energyLine.endWidth = 0.05f * intensity;
        }

        if (beamParticles != null)
        {
            var emission = beamParticles.emission;
            emission.rateOverTime = Mathf.Lerp(15f, 80f, intensity);
        }
    }

    /// <summary>
    /// Stops emission of the energy stream.
    /// </summary>
    public void StopBeam()
    {
        if (!isEmitting) return;
        isEmitting = false;

        if (beamParticles != null)
            beamParticles.Stop();

        if (energyLine != null)
            energyLine.enabled = false;
    }
}
