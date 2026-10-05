using System.Collections;
using UnityEngine;
using Xenoasis.Player;

/// <summary>
/// XENOASIS — CivilizationSparkExhibit.cs
/// Powers Station II: The Spark of Ingenuity (Museum of Being Human).
/// Orchestrates the Prometheus narrative arc:
/// 1. Flint Strike Ritual: Striking the prehistoric catalyst sparks controlled fire.
/// 2. Prometheus Flame: Golden flames dance in the hearth with dynamic flickering firelight.
/// 3. Silicon Metamorphosis: Circuit traces ignite, demonstrating the evolution from fire to computation.
/// 4. Memory Core Acquisition: The faceted amber Prometheus Silicon Core rises for collection.
/// 5. 6DoF Immersion: Stepping into DioramaSphere_CivilizationSpark immerses visitor in primeval forge acoustics.
/// 6. World Dive: Dives player into World 2: The Forge of Civilization.
/// </summary>
public class CivilizationSparkExhibit : MuseumExhibitController
{
    [Header("Station 2 Physical Relics")]
    [SerializeField] private Transform forgeHearthTransform;
    [SerializeField] private ParticleSystem sparksParticleSystem;
    [SerializeField] private ParticleSystem flameParticleSystem;
    [SerializeField] private Light fireLight;
    [SerializeField] private GameObject memoryCoreObj;
    [SerializeField] private Light pedestalRelicSpotlight;

    [Header("Living Terran Sphere (1.80m)")]
    [SerializeField] private Transform dioramaSphereTransform;
    [SerializeField] private DioramaImmersionTrigger immersionTrigger;
    [SerializeField] private Light spherePointLight;

    [Header("World Dive Transition")]
    [SerializeField] private SphereDivingTransition sphereDivingTransition;
    [SerializeField] private GameObject diveButtonObj;

    [Header("Curator Kiosk")]
    [SerializeField] private MuseumTerminalDisplay curatorTerminal;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip sparkStrikeClip;
    [SerializeField] private AudioClip flameIgniteClip;
    [SerializeField] private AudioClip coreHarvestClip;
    [SerializeField] private AudioClip chimeTonesClip;

    [Header("Ritual State")]
    private bool isIgnited = false;
    private bool isCoreCollected = false;

    private Vector3 coreOriginalPos;
    private float baseFireLightIntensity = 3.5f;

    public void SetupStation(
        Transform hearth,
        ParticleSystem sparks,
        ParticleSystem flame,
        Light fLight,
        GameObject core,
        Light relicSpot,
        Transform sphere,
        Light sphereLight,
        MuseumTerminalDisplay terminal,
        AudioClip strike,
        AudioClip ignite,
        AudioClip harvest,
        AudioClip chime)
    {
        this.forgeHearthTransform = hearth;
        this.sparksParticleSystem = sparks;
        this.flameParticleSystem = flame;
        this.fireLight = fLight;
        this.memoryCoreObj = core;
        this.pedestalRelicSpotlight = relicSpot;
        this.dioramaSphereTransform = sphere;
        this.spherePointLight = sphereLight;
        this.curatorTerminal = terminal;
        this.sparkStrikeClip = strike;
        this.flameIgniteClip = ignite;
        this.coreHarvestClip = harvest;
        this.chimeTonesClip = chime;
    }

    protected override void Start()
    {
        base.Start();

        exhibitTitle = "II. THE SPARK OF INGENUITY";
        exhibitEra = "PALEOLITHIC TO SILICON // 1.500.000 B.C. - 2026 A.D.";
        exhibitDescription = "The externalization of Terran evolution. From flint and fire expanding the cerebral cortex, to purified sand computing consciousness.";

        if (memoryCoreObj != null)
        {
            coreOriginalPos = memoryCoreObj.transform.localPosition;
            if (coreOriginalPos == Vector3.zero) coreOriginalPos = new Vector3(0f, 1.85f, 0f);
            memoryCoreObj.SetActive(false);
        }

        if (flameParticleSystem != null && !isIgnited)
        {
            flameParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (fireLight != null && !isIgnited)
        {
            fireLight.intensity = 0f;
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

        // Gentle organic firelight flicker when ignited
        if (isIgnited && fireLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * 6.5f, 0f);
            fireLight.intensity = baseFireLightIntensity + (noise - 0.5f) * 1.2f;
        }
    }

    public void OnPlayerInteract(GameObject hitTarget)
    {
        // 1. Interacting with the Hearth / Relic / Base
        if (hitTarget == forgeHearthTransform?.gameObject ||
            hitTarget.transform.IsChildOf(forgeHearthTransform) ||
            hitTarget == flameParticleSystem?.gameObject)
        {
            TriggerIgnitionRitual();
            return;
        }

        // 2. Interacting with the Prometheus Memory Core
        if (hitTarget == memoryCoreObj || hitTarget.transform.IsChildOf(memoryCoreObj?.transform))
        {
            CollectMemoryCore();
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

    public void TriggerIgnitionRitual()
    {
        if (isIgnited)
        {
            // Re-ignite spark burst on repeated touch
            PlaySparkBurst();
            return;
        }

        isIgnited = true;
        Debug.Log("[CivilizationSparkExhibit] ✦ Prometheus Fire Ignited!");

        PlaySparkBurst();

        if (Application.isPlaying)
        {
            StartCoroutine(AnimateIgnitionSequence());
        }
        else
        {
            SetIgnitedImmediate();
        }
    }

    public void SetIgnitedImmediate()
    {
        isIgnited = true;

        if (flameParticleSystem != null)
        {
            flameParticleSystem.Play();
            flameParticleSystem.Emit(30);
            flameParticleSystem.Simulate(0.5f, true, false);
        }

        if (sparksParticleSystem != null)
        {
            sparksParticleSystem.Emit(30);
            sparksParticleSystem.Simulate(0.2f, true, false);
        }

        if (fireLight != null)
        {
            fireLight.intensity = baseFireLightIntensity;
        }

        if (spherePointLight != null)
        {
            spherePointLight.intensity = 5.2f;
        }

        if (memoryCoreObj != null)
        {
            if (coreOriginalPos == Vector3.zero) coreOriginalPos = new Vector3(0f, 1.85f, 0f);
            memoryCoreObj.transform.localPosition = coreOriginalPos + new Vector3(0f, 0.08f, 0f);
            memoryCoreObj.SetActive(true);
        }
    }

    private void PlaySparkBurst()
    {
        if (sparksParticleSystem != null)
        {
            sparksParticleSystem.Emit(35);
        }

        if (audioSource != null && sparkStrikeClip != null)
        {
            audioSource.PlayOneShot(sparkStrikeClip, 0.95f);
        }

        // Sharp tactile flint strike haptics on Pico 4 Ultra
        DioramaImmersionTrigger.TriggerPicoHaptics(0.90f, 0.06f);
    }

    private IEnumerator AnimateIgnitionSequence()
    {
        yield return new WaitForSeconds(0.20f);

        if (audioSource != null && flameIgniteClip != null)
        {
            audioSource.PlayOneShot(flameIgniteClip, 0.90f);
        }

        if (flameParticleSystem != null)
        {
            flameParticleSystem.Play();
        }

        float elapsed = 0f;
        float duration = 2.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (fireLight != null)
            {
                fireLight.intensity = t * baseFireLightIntensity;
            }

            if (spherePointLight != null)
            {
                spherePointLight.intensity = 3.8f + t * 1.4f;
            }

            yield return null;
        }

        // Reveal the Prometheus Silicon Memory Core
        if (memoryCoreObj != null)
        {
            if (coreOriginalPos == Vector3.zero) coreOriginalPos = new Vector3(0f, 1.85f, 0f);
            memoryCoreObj.transform.localPosition = coreOriginalPos;
            memoryCoreObj.SetActive(true);

            if (audioSource != null && chimeTonesClip != null)
            {
                audioSource.PlayOneShot(chimeTonesClip, 0.80f);
            }
        }
    }

    public void CollectMemoryCore()
    {
        if (isCoreCollected || memoryCoreObj == null) return;

        isCoreCollected = true;
        Debug.Log("[CivilizationSparkExhibit] ✦ Prometheus Silicon Memory Core Acquired!");

        if (audioSource != null && coreHarvestClip != null)
        {
            audioSource.PlayOneShot(coreHarvestClip, 1.0f);
        }

        DioramaImmersionTrigger.TriggerPicoHaptics(0.85f, 0.35f);

        StartCoroutine(AnimateCoreHarvest());
    }

    private IEnumerator AnimateCoreHarvest()
    {
        float elapsed = 0f;
        float duration = 1.0f;
        Vector3 initialScale = memoryCoreObj.transform.localScale;
        Vector3 initialPos = memoryCoreObj.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            memoryCoreObj.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            memoryCoreObj.transform.position = initialPos + Vector3.up * (t * 0.45f);

            yield return null;
        }

        memoryCoreObj.SetActive(false);
    }

    /// <summary>
    /// Triggers the sphere diving transition into World 2: The Forge of Civilization.
    /// The VR hands grab the Living Terran Sphere, lift it to the player's head,
    /// and the viewport is enveloped by fire/amber refraction before scene transition.
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
                Debug.Log("[CivilizationSparkExhibit] ✦ DIVING INTO WORLD 2 — THE FORGE OF CIVILIZATION!");
                DioramaImmersionTrigger.TriggerPicoHaptics(0.9f, 0.15f);
                sphereDivingTransition.BeginDiveTransition(dioramaSphereTransform, "World2_ForgeOfCivilization");
            }
        }
        else
        {
            Debug.LogWarning("[CivilizationSparkExhibit] SphereDivingTransition or Diorama Sphere not assigned!");
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
