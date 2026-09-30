using UnityEngine;

/// <summary>
/// XENOASIS — StardustController.cs
/// Manages the ambient cosmic stardust floating particles.
/// Supports acceleration during the Climax phase.
/// </summary>
public class StardustController : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem stardustParticles;

    [Header("Settings")]
    [SerializeField] private float normalSpeed = 0.05f;
    [SerializeField] private float climaxSpeed = 1.2f;

    private void Start()
    {
        if (stardustParticles == null)
            stardustParticles = GetComponent<ParticleSystem>();

        SetSimulationSpeed(normalSpeed);
    }

    public void SetClimaxMode(bool active)
    {
        SetSimulationSpeed(active ? climaxSpeed : normalSpeed);
    }

    private void SetSimulationSpeed(float speed)
    {
        if (stardustParticles != null)
        {
            var main = stardustParticles.main;
            main.simulationSpeed = speed;
        }
    }
}
