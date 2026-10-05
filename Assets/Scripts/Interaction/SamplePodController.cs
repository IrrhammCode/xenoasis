using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// XENOASIS — SamplePodController.cs
/// Controls an interactive Living Sample Pod in the Terran Archive:
/// - Pod 0: Hydrosphere (Pure Water Basin) -> triggers Mirror Lake Biome
/// - Pod 1: Geosphere (Resonant Crystal) -> triggers Mountain Peak Biome
/// - Pod 2: Biosphere (Living Flora) -> triggers Primeval Forest Biome
/// </summary>
public class SamplePodController : MonoBehaviour
{
    [Header("Pod Configuration")]
    [SerializeField] private int podIndex = 0; // 0=Water, 1=Crystal, 2=Flora
    [SerializeField] private string podName = "HYDROSPHERE";
    [SerializeField] private string description = "Pure Earth Spring Water";
    [SerializeField] private Color activeColor = new Color(0f, 0.9f, 1f);

    [Header("Components")]
    [SerializeField] private Transform floatingArtifact;
    [SerializeField] private Light podStasisLight;
    [SerializeField] private ParticleSystem stasisParticles;
    [SerializeField] private TMPro.TextMeshPro labelTMP;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateSound;

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 25f;
    [SerializeField] private float bobbingAmplitude = 0.04f;
    [SerializeField] private float bobbingFrequency = 1.5f;

    public bool IsActivated { get; private set; } = false;

    private Vector3 initialArtifactPos;

    private void Start()
    {
        if (floatingArtifact != null)
        {
            initialArtifactPos = floatingArtifact.localPosition;
        }

        UpdateDisplay(false);
    }

    private void Update()
    {
        if (floatingArtifact != null)
        {
            // Gentle hovering and rotation inside the stasis field
            floatingArtifact.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            float yOffset = Mathf.Sin(Time.time * bobbingFrequency + podIndex) * bobbingAmplitude;
            floatingArtifact.localPosition = initialArtifactPos + new Vector3(0, yOffset, 0);
        }
    }

    public void OnPlayerInteract()
    {
        if (IsActivated) return;
        ActivatePod();
    }

    private void OnMouseDown()
    {
        OnPlayerInteract();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerInteract();
    }

    public void ActivatePod()
    {
        IsActivated = true;
        rotationSpeed *= 2.2f;

        if (podStasisLight != null)
        {
            podStasisLight.intensity *= 2.5f;
            podStasisLight.color = activeColor;
        }

        if (stasisParticles != null)
        {
            stasisParticles.Play();
        }

        if (audioSource != null && activateSound != null)
        {
            audioSource.PlayOneShot(activateSound);
        }

        UpdateDisplay(true);

        // Notify OfferingManager
        if (OfferingManager.Instance != null)
        {
            OfferingManager.Instance.CompleteOffering(podIndex);
        }
    }

    private void UpdateDisplay(bool active)
    {
        if (labelTMP != null)
        {
            if (active)
            {
                labelTMP.text = $"<b>[ SYNCHRONIZED ]</b>\n{podName}\n<i>{description}</i>";
                labelTMP.color = activeColor;
            }
            else
            {
                labelTMP.text = $"<b>[ SAMPLE POD 0{podIndex + 1} ]</b>\n{podName}\n<size=70%>Touch to Synchronize Biome</size>";
                labelTMP.color = new Color(0.8f, 0.9f, 1f, 0.75f);
            }
        }
    }
}
