using System.Collections;
using UnityEngine;

/// <summary>
/// XENOASIS — CosmicHorizonExhibit.cs
/// Powers Station IV: The Cosmic Horizon (Museum of Being Human).
/// Orchestrates the narrative of spaceflight, interstellar messaging, and deep time:
/// 1. Voyager Turntable: Touching the Golden Record drops the diamond stylus needle and begins spinning.
/// 2. Interstellar Pulsar VFX: Ascending golden pulsar lines project upward toward the diorama sphere.
/// 3. Memory Disc Emergence: The Voyager Interstellar Memory Capsule floats for diplomatic collection.
/// 4. 6DoF Immersion: Stepping into DioramaSphere_CosmicHorizon immerses visitor in Earth's greetings & sounds.
/// </summary>
public class CosmicHorizonExhibit : MuseumExhibitController
{
    [Header("Station 4 Physical Relics")]
    [SerializeField] private Transform recordTransform;
    [SerializeField] private Transform apolloPlaqueTransform;
    [SerializeField] private Transform stylusArmTransform;
    [SerializeField] private ParticleSystem pulsarParticles;
    [SerializeField] private Light turntableLight;
    [SerializeField] private GameObject memoryDiscObj;
    [SerializeField] private Light pedestalRelicSpotlight;

    [Header("Living Terran Sphere (1.80m)")]
    [SerializeField] private Transform dioramaSphereTransform;
    [SerializeField] private DioramaImmersionTrigger immersionTrigger;
    [SerializeField] private Light spherePointLight;

    [Header("Curator Kiosk")]
    [SerializeField] private MuseumTerminalDisplay curatorTerminal;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip needleDropClip;
    [SerializeField] private AudioClip pulsarHumClip;
    [SerializeField] private AudioClip greetingOrMusicClip;
    [SerializeField] private AudioClip discHarvestClip;

    [Header("Playback State")]
    private bool isRecordPlaying = false;
    private bool isDiscCollected = false;

    private Vector3 discOriginalPos;
    private float baseTurntableLightIntensity = 3.2f;
    private float recordSpinSpeed = 90f; // degrees per second

    public void SetupStation(
        Transform record,
        Transform apollo,
        Transform stylus,
        ParticleSystem pulsarPS,
        Light tLight,
        GameObject disc,
        Light relicSpot,
        Transform sphere,
        Light sphereLight,
        MuseumTerminalDisplay terminal,
        AudioClip needle,
        AudioClip pulsar,
        AudioClip music,
        AudioClip harvest)
    {
        this.recordTransform = record;
        this.apolloPlaqueTransform = apollo;
        this.stylusArmTransform = stylus;
        this.pulsarParticles = pulsarPS;
        this.turntableLight = tLight;
        this.memoryDiscObj = disc;
        this.pedestalRelicSpotlight = relicSpot;
        this.dioramaSphereTransform = sphere;
        this.spherePointLight = sphereLight;
        this.curatorTerminal = terminal;
        this.needleDropClip = needle;
        this.pulsarHumClip = pulsar;
        this.greetingOrMusicClip = music;
        this.discHarvestClip = harvest;
    }

    protected override void Start()
    {
        base.Start();

        exhibitTitle = "IV. THE COSMIC HORIZON";
        exhibitEra = "VOYAGER & APOLLO // 1969 - 1977 A.D.";
        exhibitDescription = "Humanity's first steps into the celestial ocean: Tranquility Base and the golden interstellar message.";

        if (memoryDiscObj != null)
        {
            discOriginalPos = memoryDiscObj.transform.localPosition;
            if (discOriginalPos == Vector3.zero) discOriginalPos = new Vector3(0.02f, 1.88f, 0.10f);
            memoryDiscObj.SetActive(false);
        }

        if (pulsarParticles != null && !isRecordPlaying)
        {
            pulsarParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (turntableLight != null && !isRecordPlaying)
        {
            turntableLight.intensity = 0.5f; // Soft ambient gold
        }

        if (immersionTrigger == null && dioramaSphereTransform != null)
        {
            immersionTrigger = dioramaSphereTransform.GetComponent<DioramaImmersionTrigger>();
        }

        if (curatorTerminal == null)
        {
            curatorTerminal = GetComponentInChildren<MuseumTerminalDisplay>();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isRecordPlaying)
        {
            // Continuous spinning of the Golden Record
            if (recordTransform != null)
            {
                recordTransform.Rotate(Vector3.up, recordSpinSpeed * Time.deltaTime, Space.Self);
            }

            // Pulsing turntable light
            if (turntableLight != null)
            {
                float pulse = Mathf.Sin(Time.time * 2.2f) * 0.5f + 0.5f;
                turntableLight.intensity = baseTurntableLightIntensity + pulse * 1.0f;
            }
        }
    }

    public void OnPlayerInteract(GameObject hitTarget)
    {
        // 1. Interacting with Golden Record, Apollo Plaque, or Stylus
        if (hitTarget == recordTransform?.gameObject ||
            hitTarget.transform.IsChildOf(recordTransform) ||
            hitTarget == apolloPlaqueTransform?.gameObject ||
            hitTarget.transform.IsChildOf(apolloPlaqueTransform) ||
            hitTarget == stylusArmTransform?.gameObject ||
            hitTarget == pulsarParticles?.gameObject)
        {
            TriggerRecordPlayback();
            return;
        }

        // 2. Interacting with the Voyager Memory Disc
        if (hitTarget == memoryDiscObj || hitTarget.transform.IsChildOf(memoryDiscObj?.transform))
        {
            CollectMemoryDisc();
            return;
        }

        // 3. Interacting with the Curator Terminal
        if (curatorTerminal != null && (hitTarget == curatorTerminal.gameObject || hitTarget.transform.IsChildOf(curatorTerminal.transform)))
        {
            curatorTerminal.AdvancePage();
            return;
        }

        ToggleInteraction();
    }

    public void TriggerRecordPlayback()
    {
        if (isRecordPlaying)
        {
            PlayNeedleHapticsPulse();
            return;
        }

        isRecordPlaying = true;
        Debug.Log("[CosmicHorizonExhibit] ✦ Voyager Golden Record Playback Initiated!");

        PlayNeedleHapticsPulse();

        if (Application.isPlaying)
        {
            StartCoroutine(AnimatePlaybackSequence());
        }
        else
        {
            SetActivatedImmediate();
        }
    }

    public void SetActivatedImmediate()
    {
        isRecordPlaying = true;

        if (pulsarParticles != null)
        {
            pulsarParticles.Play();
            pulsarParticles.Emit(35);
            pulsarParticles.Simulate(0.6f, true, false);
        }

        if (turntableLight != null)
        {
            turntableLight.intensity = baseTurntableLightIntensity;
        }

        if (spherePointLight != null)
        {
            spherePointLight.intensity = 5.2f;
        }

        if (memoryDiscObj != null)
        {
            if (discOriginalPos == Vector3.zero) discOriginalPos = new Vector3(0.02f, 1.88f, 0.10f);
            memoryDiscObj.transform.localPosition = discOriginalPos + new Vector3(0f, 0.08f, 0f);
            memoryDiscObj.SetActive(true);
        }
    }

    private void PlayNeedleHapticsPulse()
    {
        if (pulsarParticles != null)
        {
            pulsarParticles.Emit(25);
        }

        if (audioSource != null && needleDropClip != null)
        {
            audioSource.PlayOneShot(needleDropClip, 0.90f);
        }

        // Mechanical stylus groove haptics on Pico 4 Ultra
        DioramaImmersionTrigger.TriggerPicoHaptics(0.85f, 0.22f);
    }

    private IEnumerator AnimatePlaybackSequence()
    {
        yield return new WaitForSeconds(0.25f);

        if (audioSource != null && pulsarHumClip != null)
        {
            audioSource.PlayOneShot(pulsarHumClip, 0.85f);
        }

        if (pulsarParticles != null)
        {
            pulsarParticles.Play();
        }

        float elapsed = 0f;
        float duration = 2.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (turntableLight != null)
            {
                turntableLight.intensity = Mathf.Lerp(0.5f, baseTurntableLightIntensity, t);
            }

            if (spherePointLight != null)
            {
                spherePointLight.intensity = 3.5f + t * 1.7f;
            }

            yield return null;
        }

        // Reveal the Voyager Golden Memory Disc
        if (memoryDiscObj != null)
        {
            if (discOriginalPos == Vector3.zero) discOriginalPos = new Vector3(0.02f, 1.88f, 0.10f);
            memoryDiscObj.transform.localPosition = discOriginalPos;
            memoryDiscObj.SetActive(true);

            if (audioSource != null && greetingOrMusicClip != null)
            {
                audioSource.PlayOneShot(greetingOrMusicClip, 0.80f);
            }
        }
    }

    public void CollectMemoryDisc()
    {
        if (isDiscCollected || memoryDiscObj == null) return;

        isDiscCollected = true;
        Debug.Log("[CosmicHorizonExhibit] ✦ Voyager Interstellar Memory Disc Acquired!");

        if (audioSource != null && discHarvestClip != null)
        {
            audioSource.PlayOneShot(discHarvestClip, 1.0f);
        }

        DioramaImmersionTrigger.TriggerPicoHaptics(0.90f, 0.35f);

        StartCoroutine(AnimateDiscHarvest());
    }

    private IEnumerator AnimateDiscHarvest()
    {
        float elapsed = 0f;
        float duration = 1.0f;
        Vector3 initialScale = memoryDiscObj.transform.localScale;
        Vector3 initialPos = memoryDiscObj.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            memoryDiscObj.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            memoryDiscObj.transform.position = initialPos + Vector3.up * (t * 0.45f);

            yield return null;
        }

        memoryDiscObj.SetActive(false);
    }
}
