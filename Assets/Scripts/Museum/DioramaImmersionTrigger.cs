using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// XENOASIS — DioramaImmersionTrigger.cs
/// Enables the 6DoF "Head-In-Sphere" micro-dive experience for the PICO 4 Ultra & VR HMDs.
/// When the visitor's head/camera physically enters the 2.0m diorama sphere:
/// 1. Muffles external museum audio via AudioLowPassFilter (simulating underwater immersion).
/// 2. Intensifies bioluminescent plankton/diatom particle fields around the player's view.
/// 3. Spatially triggers deep ocean hydrothermal abyss ambient audio.
/// 4. Sends subtle linear haptic rumble to PICO 4 Ultra controllers.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class DioramaImmersionTrigger : MonoBehaviour
{
    [Header("Sphere Boundary Settings")]
    [SerializeField] private float sphereRadius = 1.0f; // 2.0m diameter
    [SerializeField] private float enterHysteresis = 0.92f;
    [SerializeField] private float exitHysteresis = 1.05f;

    [Header("Underwater Immersion Effects")]
    [SerializeField] private ParticleSystem planktonParticles;
    [SerializeField] private AudioSource underwaterAudioSource;
    [SerializeField] private AudioClip underwaterAmbienceClip;
    [SerializeField] private float maxUnderwaterVolume = 0.85f;

    [Header("Lighting Atmosphere")]
    [SerializeField] private Light sphereCoreLight;
    [SerializeField] private Color exteriorGlow = new Color(0.10f, 0.90f, 1.0f);
    [SerializeField] private Color interiorAbyssGlow = new Color(0.02f, 0.45f, 0.85f);

    private bool isPlayerInside = false;
    private Camera playerCam;
    private AudioLowPassFilter camLowPassFilter;
    private float targetFilterFreq = 22000f;
    private float currentFilterFreq = 22000f;
    private float targetAudioVol = 0f;

    public bool IsPlayerInside => isPlayerInside;

    public void SetupImmersion(ParticleSystem ps, Light coreLight, AudioClip ambienceClip)
    {
        this.planktonParticles = ps;
        this.sphereCoreLight = coreLight;
        this.underwaterAmbienceClip = ambienceClip;
    }

    private void Start()
    {
        playerCam = Camera.main;
        if (playerCam != null)
        {
            camLowPassFilter = playerCam.GetComponent<AudioLowPassFilter>();
            if (camLowPassFilter == null)
            {
                camLowPassFilter = playerCam.gameObject.AddComponent<AudioLowPassFilter>();
                camLowPassFilter.cutoffFrequency = 22000f;
                camLowPassFilter.enabled = false;
            }
        }

        if (underwaterAudioSource == null)
        {
            underwaterAudioSource = gameObject.AddComponent<AudioSource>();
            underwaterAudioSource.loop = true;
            underwaterAudioSource.playOnAwake = false;
            underwaterAudioSource.spatialBlend = 0.7f;
            underwaterAudioSource.volume = 0f;
            if (underwaterAmbienceClip != null)
            {
                underwaterAudioSource.clip = underwaterAmbienceClip;
            }
        }

        var col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.radius = sphereRadius;
        }
    }

    private void Update()
    {
        if (playerCam == null)
        {
            playerCam = Camera.main;
            if (playerCam == null) return;
        }

        float distance = Vector3.Distance(transform.position, playerCam.transform.position);

        if (!isPlayerInside && distance < sphereRadius * enterHysteresis)
        {
            SetImmersionState(true);
        }
        else if (isPlayerInside && distance > sphereRadius * exitHysteresis)
        {
            SetImmersionState(false);
        }

        // Smooth audio low-pass filter transition
        if (camLowPassFilter != null && camLowPassFilter.enabled)
        {
            currentFilterFreq = Mathf.MoveTowards(currentFilterFreq, targetFilterFreq, Time.deltaTime * 35000f);
            camLowPassFilter.cutoffFrequency = currentFilterFreq;
            if (!isPlayerInside && Mathf.Approximately(currentFilterFreq, 22000f))
            {
                camLowPassFilter.enabled = false;
            }
        }

        // Smooth volume fade
        if (underwaterAudioSource != null)
        {
            underwaterAudioSource.volume = Mathf.MoveTowards(underwaterAudioSource.volume, targetAudioVol, Time.deltaTime * 2.5f);
            if (!isPlayerInside && underwaterAudioSource.volume <= 0.01f && underwaterAudioSource.isPlaying)
            {
                underwaterAudioSource.Stop();
            }
        }
    }

    private void SetImmersionState(bool inside)
    {
        isPlayerInside = inside;
        Debug.Log($"[DioramaImmersionTrigger] Head-In-Sphere immersion: {(inside ? "ENTERED HYDROSPHERE" : "EXITED TO ROTUNDA")}");

        targetFilterFreq = inside ? 850f : 22000f; // Muffled underwater audio frequency
        if (camLowPassFilter != null)
        {
            camLowPassFilter.enabled = true;
        }

        targetAudioVol = inside ? maxUnderwaterVolume : 0f;
        if (inside && underwaterAudioSource != null && !underwaterAudioSource.isPlaying)
        {
            underwaterAudioSource.Play();
        }

        if (planktonParticles != null)
        {
            var emission = planktonParticles.emission;
            emission.rateOverTime = inside ? 65f : 15f;
        }

        if (sphereCoreLight != null)
        {
            sphereCoreLight.color = inside ? interiorAbyssGlow : exteriorGlow;
            sphereCoreLight.intensity = inside ? 4.5f : 3.5f;
        }

        // Trigger PICO 4 Ultra Haptic Feedback on threshold crossing
        TriggerPicoHaptics(inside ? 0.35f : 0.15f, 0.15f);
    }

    public static void TriggerPicoHaptics(float amplitude, float duration)
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
            UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Controller,
            devices
        );

        foreach (var dev in devices)
        {
            if (dev.TryGetHapticCapabilities(out var caps) && caps.supportsImpulse)
            {
                dev.SendHapticImpulse(0u, amplitude, duration);
            }
        }
    }
}
