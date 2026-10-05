using System.Collections;
using UnityEngine;
using Xenoasis.Player;

/// <summary>
/// XENOASIS — HumanArchiveExhibit.cs
/// Powers Station III: The Human Archive (Museum of Being Human).
/// Orchestrates the narrative of language, biosphere custodianship, and DNA:
/// 1. Seed & Glyph Awakening: Touching the Svalbard Vault or Rosetta Stone triggers cryogenic resonance.
/// 2. Spores & Glyphs VFX: Ascending genetic light connects the relics to the DNA double helix above.
/// 3. Memory Seed Emergence: The Svalbard Genetic Memory Capsule floats for collection.
/// 4. 6DoF Immersion: Stepping into DioramaSphere_HumanArchive surrounds the visitor in whispered human languages.
/// </summary>
public class HumanArchiveExhibit : MuseumExhibitController
{
    [Header("Station 3 Physical Relics")]
    [SerializeField] private Transform rosettaTransform;
    [SerializeField] private Transform seedVaultTransform;
    [SerializeField] private ParticleSystem sproutParticleSystem;
    [SerializeField] private Light vaultGlowLight;
    [SerializeField] private GameObject memorySeedObj;
    [SerializeField] private Light pedestalRelicSpotlight;

    [Header("Living Terran Sphere (1.80m)")]
    [SerializeField] private Transform dioramaSphereTransform;
    [SerializeField] private DioramaImmersionTrigger immersionTrigger;
    [SerializeField] private Light spherePointLight;

    [Header("Curator Kiosk")]
    [SerializeField] private MuseumTerminalDisplay curatorTerminal;

    [Header("World Dive Transition")]
    [SerializeField] private SphereDivingTransition sphereDivingTransition;
    [SerializeField] private GameObject diveButtonObj;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip cryoReleaseClip;
    [SerializeField] private AudioClip glyphAscendClip;
    [SerializeField] private AudioClip seedHarvestClip;
    [SerializeField] private AudioClip chimeTonesClip;

    [Header("Ritual State")]
    private bool isGerminated = false;
    private bool isSeedCollected = false;

    private Vector3 seedOriginalPos;
    private float baseVaultLightIntensity = 2.8f;

    public void SetupStation(
        Transform rosetta,
        Transform vault,
        ParticleSystem sproutPS,
        Light vLight,
        GameObject seed,
        Light relicSpot,
        Transform sphere,
        Light sphereLight,
        MuseumTerminalDisplay terminal,
        AudioClip cryo,
        AudioClip ascend,
        AudioClip harvest,
        AudioClip chime)
    {
        this.rosettaTransform = rosetta;
        this.seedVaultTransform = vault;
        this.sproutParticleSystem = sproutPS;
        this.vaultGlowLight = vLight;
        this.memorySeedObj = seed;
        this.pedestalRelicSpotlight = relicSpot;
        this.dioramaSphereTransform = sphere;
        this.spherePointLight = sphereLight;
        this.curatorTerminal = terminal;
        this.cryoReleaseClip = cryo;
        this.glyphAscendClip = ascend;
        this.seedHarvestClip = harvest;
        this.chimeTonesClip = chime;
    }

    protected override void Start()
    {
        base.Start();

        exhibitTitle = "III. THE HUMAN ARCHIVE";
        exhibitEra = "PTOLEMAIC TO GENOME // 196 B.C. - 2026 A.D.";
        exhibitDescription = "Exogenous memory preservation. From the Rosetta decryption and Svalbard seed sanctuary to decoding the human genome.";

        if (memorySeedObj != null)
        {
            seedOriginalPos = memorySeedObj.transform.localPosition;
            if (seedOriginalPos == Vector3.zero) seedOriginalPos = new Vector3(0.02f, 1.88f, 0f);
            memorySeedObj.SetActive(false);
        }

        if (sproutParticleSystem != null && !isGerminated)
        {
            sproutParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (vaultGlowLight != null && !isGerminated)
        {
            vaultGlowLight.intensity = 0.6f; // Soft idle stasis glow
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

        // Pulsing cryogenic stasis light when germinated
        if (isGerminated && vaultGlowLight != null)
        {
            float pulse = Mathf.Sin(Time.time * 2.8f) * 0.5f + 0.5f;
            vaultGlowLight.intensity = baseVaultLightIntensity + pulse * 1.0f;
        }
    }

    public void OnPlayerInteract(GameObject hitTarget)
    {
        // 1. Interacting with Rosetta Stone or Svalbard Vault
        if (hitTarget == rosettaTransform?.gameObject ||
            hitTarget.transform.IsChildOf(rosettaTransform) ||
            hitTarget == seedVaultTransform?.gameObject ||
            hitTarget.transform.IsChildOf(seedVaultTransform) ||
            hitTarget == sproutParticleSystem?.gameObject)
        {
            TriggerGerminationRitual();
            return;
        }

        // 2. Interacting with the Genetic Memory Seed
        if (hitTarget == memorySeedObj || hitTarget.transform.IsChildOf(memorySeedObj?.transform))
        {
            CollectMemorySeed();
            return;
        }

        // 3. Interacting with the Curator Terminal
        if (curatorTerminal != null && (hitTarget == curatorTerminal.gameObject || hitTarget.transform.IsChildOf(curatorTerminal.transform)))
        {
            curatorTerminal.AdvancePage();
            return;
        }

        // 4. Interacting with the Dive Button / Living Terran Sphere
        if (diveButtonObj != null && (hitTarget == diveButtonObj || hitTarget.transform.IsChildOf(diveButtonObj.transform)))
        {
            TriggerWorldDive();
            return;
        }

        ToggleInteraction();
    }

    public void TriggerGerminationRitual()
    {
        if (isGerminated)
        {
            PlayStasisPulse();
            return;
        }

        isGerminated = true;
        Debug.Log("[HumanArchiveExhibit] ✦ Svalbard Genetic Resonance Activated!");

        PlayStasisPulse();

        if (Application.isPlaying)
        {
            StartCoroutine(AnimateGerminationSequence());
        }
        else
        {
            SetGerminatedImmediate();
        }
    }

    public void SetGerminatedImmediate()
    {
        isGerminated = true;

        if (sproutParticleSystem != null)
        {
            sproutParticleSystem.Play();
            sproutParticleSystem.Emit(35);
            sproutParticleSystem.Simulate(0.6f, true, false);
        }

        if (vaultGlowLight != null)
        {
            vaultGlowLight.intensity = baseVaultLightIntensity;
        }

        if (spherePointLight != null)
        {
            spherePointLight.intensity = 5.2f;
        }

        if (memorySeedObj != null)
        {
            if (seedOriginalPos == Vector3.zero) seedOriginalPos = new Vector3(0.02f, 1.88f, 0f);
            memorySeedObj.transform.localPosition = seedOriginalPos + new Vector3(0f, 0.08f, 0f);
            memorySeedObj.SetActive(true);
        }
    }

    private void PlayStasisPulse()
    {
        if (sproutParticleSystem != null)
        {
            sproutParticleSystem.Emit(25);
        }

        if (audioSource != null && cryoReleaseClip != null)
        {
            audioSource.PlayOneShot(cryoReleaseClip, 0.90f);
        }

        // Deep cellular resonance haptics on Pico 4 Ultra
        DioramaImmersionTrigger.TriggerPicoHaptics(0.75f, 0.20f);
    }

    private IEnumerator AnimateGerminationSequence()
    {
        yield return new WaitForSeconds(0.25f);

        if (audioSource != null && glyphAscendClip != null)
        {
            audioSource.PlayOneShot(glyphAscendClip, 0.85f);
        }

        if (sproutParticleSystem != null)
        {
            sproutParticleSystem.Play();
        }

        float elapsed = 0f;
        float duration = 2.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (vaultGlowLight != null)
            {
                vaultGlowLight.intensity = Mathf.Lerp(0.6f, baseVaultLightIntensity, t);
            }

            if (spherePointLight != null)
            {
                spherePointLight.intensity = 3.5f + t * 1.7f;
            }

            yield return null;
        }

        // Reveal the Svalbard Genetic Memory Seed
        if (memorySeedObj != null)
        {
            if (seedOriginalPos == Vector3.zero) seedOriginalPos = new Vector3(0.02f, 1.88f, 0f);
            memorySeedObj.transform.localPosition = seedOriginalPos;
            memorySeedObj.SetActive(true);

            if (audioSource != null && chimeTonesClip != null)
            {
                audioSource.PlayOneShot(chimeTonesClip, 0.80f);
            }
        }
    }

    public void CollectMemorySeed()
    {
        if (isSeedCollected || memorySeedObj == null) return;

        isSeedCollected = true;
        Debug.Log("[HumanArchiveExhibit] ✦ Svalbard Genetic Memory Seed Acquired!");

        if (audioSource != null && seedHarvestClip != null)
        {
            audioSource.PlayOneShot(seedHarvestClip, 1.0f);
        }

        DioramaImmersionTrigger.TriggerPicoHaptics(0.85f, 0.35f);

        StartCoroutine(AnimateSeedHarvest());
    }

    private IEnumerator AnimateSeedHarvest()
    {
        float elapsed = 0f;
        float duration = 1.0f;
        Vector3 initialScale = memorySeedObj.transform.localScale;
        Vector3 initialPos = memorySeedObj.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            memorySeedObj.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            memorySeedObj.transform.position = initialPos + Vector3.up * (t * 0.45f);

            yield return null;
        }

        memorySeedObj.SetActive(false);
    }

    /// <summary>
    /// Triggers the sphere diving transition into World 3: The Heart of Humanity.
    /// The VR hands cradle the Living Terran Sphere, lift it to the player's head,
    /// and the viewport is enveloped by glacial cyan refraction before scene transition.
    /// </summary>
    public void TriggerWorldDive()
    {
        if (sphereDivingTransition == null)
        {
            sphereDivingTransition = FindObjectOfType<SphereDivingTransition>();
        }

        if (sphereDivingTransition != null && dioramaSphereTransform != null)
        {
            if (!sphereDivingTransition.IsTransitioning)
            {
                Debug.Log("[HumanArchiveExhibit] ✦ DIVING INTO WORLD 3 — THE HEART OF HUMANITY!");
                DioramaImmersionTrigger.TriggerPicoHaptics(0.9f, 0.15f);
                sphereDivingTransition.BeginDiveTransition(dioramaSphereTransform, "World3_HeartOfHumanity");
            }
        }
        else
        {
            Debug.LogWarning("[HumanArchiveExhibit] SphereDivingTransition or Diorama Sphere not assigned!");
        }
    }

    /// <summary>
    /// Wires up the SphereDivingTransition reference and dive button.
    /// </summary>
    public void SetupDiveTransition(SphereDivingTransition transition, GameObject diveButton)
    {
        this.sphereDivingTransition = transition;
        this.diveButtonObj = diveButton;
    }
}
