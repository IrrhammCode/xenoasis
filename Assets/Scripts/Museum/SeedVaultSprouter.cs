using System.Collections;
using UnityEngine;

/// <summary>
/// XENOASIS — SeedVaultSprouter.cs
/// Powers Pavilion 4: The Svalbard Global Seed Vault & Terran Tree of Life.
/// Preserves cryogenic cassettes of Earth crops (Rice, Wheat, Maize, Sequoia).
/// Interacting accelerates holographic germination, sprouting a vibrant green plant.
/// </summary>
public class SeedVaultSprouter : MuseumExhibitController
{
    [Header("Cryo Pod Components")]
    [SerializeField] private Transform[] seedVials;
    [SerializeField] private Transform holoPlantSprout;
    [SerializeField] private ParticleSystem growthStardustPS;
    [SerializeField] private Light bioGlowLight;
    [SerializeField] private TMPro.TextMeshPro specimenNameTMP;

    private readonly string[] specimenNames = new string[]
    {
        "ORYZA SATIVA (PADI / RICE) — SUMBER PANGAN ASIA",
        "TRITICUM AESTIVUM (GANDUM / WHEAT) — PERADABAN BULAN SABIT SUBUR",
        "ZEA MAYS (JAGUNG / MAIZE) — WARISAN MESOAMERIKA",
        "SEQUOIADENDRON GIGANTEUM (POHON RAKSASA SEQUOIA) — ARSIP BIO KUNO"
    };

    private int currentSpecimen = 0;
    private Coroutine sproutCoroutine;

    protected override void Start()
    {
        base.Start();
        if (holoPlantSprout != null) holoPlantSprout.localScale = Vector3.zero;
        if (growthStardustPS != null) growthStardustPS.Stop();
        UpdateSpecimenLabel();
    }

    protected override void Update()
    {
        base.Update();

        // Rotate holographic blooming plant
        if (holoPlantSprout != null && holoPlantSprout.localScale.x > 0.05f)
        {
            holoPlantSprout.Rotate(Vector3.up, 20f * Time.deltaTime, Space.Self);
        }

        // Pulse cryo pods gently
        if (bioGlowLight != null)
        {
            bioGlowLight.intensity = 1.4f + Mathf.Sin(Time.time * 2f) * 0.3f;
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        currentSpecimen = (currentSpecimen + 1) % specimenNames.Length;
        UpdateSpecimenLabel();

        if (sproutCoroutine != null) StopCoroutine(sproutCoroutine);
        sproutCoroutine = StartCoroutine(AnimatePlantSprout());
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
        if (sproutCoroutine != null) StopCoroutine(sproutCoroutine);
        sproutCoroutine = StartCoroutine(ShrinkPlant());
    }

    private IEnumerator AnimatePlantSprout()
    {
        if (growthStardustPS != null) growthStardustPS.Play();
        if (holoPlantSprout == null) yield break;

        holoPlantSprout.localScale = Vector3.zero;
        float elapsed = 0f;
        float dur = 1.8f;
        Vector3 targetScale = Vector3.one * 0.45f;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / dur);
            holoPlantSprout.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        holoPlantSprout.localScale = targetScale;
    }

    private IEnumerator ShrinkPlant()
    {
        if (holoPlantSprout == null) yield break;
        float elapsed = 0f;
        float dur = 0.8f;
        Vector3 startScale = holoPlantSprout.localScale;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            holoPlantSprout.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / dur);
            yield return null;
        }
        holoPlantSprout.localScale = Vector3.zero;
        if (growthStardustPS != null) growthStardustPS.Stop();
    }

    private void UpdateSpecimenLabel()
    {
        if (specimenNameTMP != null)
        {
            specimenNameTMP.text = $"<color=#00FF88><b>SPESIMEN BANK BENIH AKTIF:</b></color>\n<size=80%><color=#A7FFEB>{specimenNames[currentSpecimen]}</color></size>";
        }
    }
}
