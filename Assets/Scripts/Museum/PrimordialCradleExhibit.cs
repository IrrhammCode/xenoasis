using System.Collections;
using UnityEngine;

/// <summary>
/// XENOASIS — PrimordialCradleExhibit.cs
/// Powers Station I: The Primordial Cradle (Museum of Being Human).
/// Orchestrates the interactive storyline:
/// 1. Water Ripple Ritual: Touching the sacred basin triggers fluid caustics, audio, and Pico haptics.
/// 2. Lotus Blossom: The frosted lotus blooms and reveals the floating Primordial Droplet memory pearl.
/// 3. Energy Ascendance: A vertical column of liquid light pulses upward into the 2.0m Living Terran Sphere.
/// 4. 6DoF Immersion: DioramaImmersionTrigger manages the underwater headset transition.
/// 5. Droplet Acquisition: Player can collect the sacred memory droplet for their diplomatic archive.
/// </summary>
public class PrimordialCradleExhibit : MuseumExhibitController
{
    [Header("Station 1 Physical Relics")]
    [SerializeField] private Transform sacredBasinTransform;
    [SerializeField] private Renderer waterSurfaceRenderer;
    [SerializeField] private Transform waterLotusTransform;
    [SerializeField] private GameObject memoryDropletObj;
    [SerializeField] private Light pedestalRelicSpotlight;

    [Header("Living Terran Sphere (2.0m)")]
    [SerializeField] private Transform dioramaSphereTransform;
    [SerializeField] private Transform earthGlobeTransform;
    [SerializeField] private DioramaImmersionTrigger immersionTrigger;
    [SerializeField] private Light spherePointLight;

    [Header("Curator Kiosk")]
    [SerializeField] private MuseumTerminalDisplay curatorTerminal;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip waterSplashClip;
    [SerializeField] private AudioClip lotusBloomClip;
    [SerializeField] private AudioClip dropletHarvestClip;
    [SerializeField] private AudioClip chimeTonesClip;

    [Header("Ritual State")]
    private bool isWaterActivated = false;
    private bool isLotusBloomed = false;
    private bool isDropletCollected = false;

    private Vector3 lotusOriginalScale = Vector3.one * 0.55f;
    private Vector3 lotusBloomedScale = Vector3.one * 0.72f;
    private Vector3 lotusOriginalPos;

    public void SetupCradle(Transform basin, Renderer waterRend, Transform lotus, GameObject droplet,
                            Light relicSpot, Transform sphere, Transform globe, Light sphereLight,
                            MuseumTerminalDisplay terminal, AudioClip splash, AudioClip bloom, AudioClip harvest, AudioClip chime)
    {
        this.sacredBasinTransform = basin;
        this.waterSurfaceRenderer = waterRend;
        this.waterLotusTransform = lotus;
        this.memoryDropletObj = droplet;
        this.pedestalRelicSpotlight = relicSpot;
        this.dioramaSphereTransform = sphere;
        this.earthGlobeTransform = globe;
        this.spherePointLight = sphereLight;
        this.curatorTerminal = terminal;
        this.waterSplashClip = splash;
        this.lotusBloomClip = bloom;
        this.dropletHarvestClip = harvest;
        this.chimeTonesClip = chime;
    }

    protected override void Start()
    {
        base.Start();

        exhibitTitle = "I. THE PRIMORDIAL CRADLE";
        exhibitEra = "ARCHEAN EON // 3.8 BILLION B.C.";
        exhibitDescription = "The genesis of Terran consciousness in the memory of water. From cosmic ice bombardment to the human internal ocean.";

        if (waterLotusTransform != null)
        {
            lotusOriginalPos = waterLotusTransform.localPosition;
            waterLotusTransform.localScale = lotusOriginalScale;
        }

        if (memoryDropletObj != null)
        {
            memoryDropletObj.SetActive(false);
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

    public void OnPlayerInteract(GameObject hitTarget)
    {
        // 1. Interacting with the Sacred Basin or Lotus
        if (hitTarget == sacredBasinTransform?.gameObject ||
            hitTarget.transform.IsChildOf(sacredBasinTransform) ||
            hitTarget == waterLotusTransform?.gameObject ||
            hitTarget.transform.IsChildOf(waterLotusTransform) ||
            hitTarget == waterSurfaceRenderer?.gameObject)
        {
            TriggerWaterRitual();
            return;
        }

        // 2. Interacting with the Memory Droplet
        if (hitTarget == memoryDropletObj || hitTarget.transform.IsChildOf(memoryDropletObj?.transform))
        {
            CollectMemoryDroplet();
            return;
        }

        // 3. Interacting with the Curator Terminal
        if (curatorTerminal != null && (hitTarget == curatorTerminal.gameObject || hitTarget.transform.IsChildOf(curatorTerminal.transform)))
        {
            curatorTerminal.AdvancePage();
            return;
        }

        // Default: Toggle Exhibit Interaction
        ToggleInteraction();
    }

    public void TriggerWaterRitual()
    {
        if (isWaterActivated && isLotusBloomed)
        {
            // Re-trigger pleasant water ripples and splash
            PlayWaterSplash();
            return;
        }

        isWaterActivated = true;
        Debug.Log("[PrimordialCradleExhibit] ✦ Sacred Water Ritual Activated!");

        PlayWaterSplash();

        if (Application.isPlaying)
        {
            StartCoroutine(AnimateLotusBloom());
        }
        else
        {
            SetBloomedImmediate();
        }
    }

    public void SetBloomedImmediate()
    {
        isLotusBloomed = true;
        if (waterLotusTransform != null)
        {
            if (lotusOriginalPos == Vector3.zero)
            {
                lotusOriginalPos = waterLotusTransform.localPosition != Vector3.zero ? waterLotusTransform.localPosition : new Vector3(0f, 1.58f, 0f);
            }
            waterLotusTransform.localScale = lotusBloomedScale;
            waterLotusTransform.localPosition = lotusOriginalPos + new Vector3(0f, 0.08f, 0f);
        }
        if (spherePointLight != null)
        {
            spherePointLight.intensity = 5.0f;
        }
        if (memoryDropletObj != null)
        {
            memoryDropletObj.SetActive(true);
        }
    }

    private void PlayWaterSplash()
    {
        if (audioSource != null && waterSplashClip != null)
        {
            audioSource.PlayOneShot(waterSplashClip, 0.9f);
        }

        // Pico 4 Ultra controller fluid vibration
        DioramaImmersionTrigger.TriggerPicoHaptics(0.65f, 0.22f);

        // Flash water surface intensity
        if (waterSurfaceRenderer != null && waterSurfaceRenderer.sharedMaterial != null)
        {
            StartCoroutine(FlashWaterGlow());
        }
    }

    private IEnumerator FlashWaterGlow()
    {
        float elapsed = 0f;
        float duration = 1.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (pedestalRelicSpotlight != null)
            {
                pedestalRelicSpotlight.intensity = 2.8f + Mathf.Sin(t * Mathf.PI) * 2.0f;
            }
            yield return null;
        }
        if (pedestalRelicSpotlight != null) pedestalRelicSpotlight.intensity = 2.8f;
    }

    private IEnumerator AnimateLotusBloom()
    {
        yield return new WaitForSeconds(0.35f);

        if (audioSource != null && lotusBloomClip != null)
        {
            audioSource.PlayOneShot(lotusBloomClip, 0.85f);
        }

        float elapsed = 0f;
        float duration = 2.4f;

        if (lotusOriginalPos == Vector3.zero && waterLotusTransform != null)
        {
            lotusOriginalPos = waterLotusTransform.localPosition != Vector3.zero ? waterLotusTransform.localPosition : new Vector3(0f, 1.58f, 0f);
        }
        Vector3 targetPos = lotusOriginalPos + new Vector3(0f, 0.08f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (waterLotusTransform != null)
            {
                waterLotusTransform.localScale = Vector3.Lerp(lotusOriginalScale, lotusBloomedScale, t);
                waterLotusTransform.localPosition = Vector3.Lerp(lotusOriginalPos, targetPos, t);
                waterLotusTransform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.Self);
            }

            if (spherePointLight != null)
            {
                spherePointLight.intensity = 3.5f + t * 1.5f;
            }

            yield return null;
        }

        isLotusBloomed = true;

        // Reveal the Primordial Memory Droplet
        if (memoryDropletObj != null)
        {
            memoryDropletObj.SetActive(true);
            if (audioSource != null && chimeTonesClip != null)
            {
                audioSource.PlayOneShot(chimeTonesClip, 0.75f);
            }
        }
    }

    public void CollectMemoryDroplet()
    {
        if (isDropletCollected || memoryDropletObj == null) return;

        isDropletCollected = true;
        Debug.Log("[PrimordialCradleExhibit] ✦ Primordial Memory Droplet Acquired!");

        if (audioSource != null && dropletHarvestClip != null)
        {
            audioSource.PlayOneShot(dropletHarvestClip, 1.0f);
        }

        DioramaImmersionTrigger.TriggerPicoHaptics(0.80f, 0.35f);

        StartCoroutine(AnimateDropletHarvest());
    }

    private IEnumerator AnimateDropletHarvest()
    {
        float elapsed = 0f;
        float duration = 1.0f;
        Vector3 startPos = memoryDropletObj.transform.position;
        Vector3 startScale = memoryDropletObj.transform.localScale;

        Transform targetHMD = Camera.main != null ? Camera.main.transform : transform;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (memoryDropletObj != null)
            {
                memoryDropletObj.transform.position = Vector3.Lerp(startPos, targetHMD.position - targetHMD.forward * 0.3f, t);
                memoryDropletObj.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            }
            yield return null;
        }

        if (memoryDropletObj != null)
        {
            memoryDropletObj.SetActive(false);
        }
    }
}
