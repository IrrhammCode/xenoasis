using UnityEngine;

/// <summary>
/// XENOASIS — PrometheusToolEvolution.cs
/// Powers Pavilion 3: The Spark of Prometheus & Tool Evolution.
/// Exhibits: Paleolithic Flint Handaxe & Primitive Fire, The Wooden Wheel,
/// Steam Engine Gear, and Photolithographic Silicon Wafer (Microprocessor).
/// </summary>
public class PrometheusToolEvolution : MuseumExhibitController
{
    [Header("Evolution Tier Artifacts")]
    [SerializeField] private Transform stoneHandaxe;
    [SerializeField] private Transform primitiveWheel;
    [SerializeField] private Transform siliconWafer;
    [SerializeField] private ParticleSystem fireSparkPS;
    [SerializeField] private ParticleSystem quantumDataStreamPS;
    [SerializeField] private Light fireGlowLight;

    private float baseFireIntensity = 1.2f;

    protected override void Start()
    {
        base.Start();
        if (quantumDataStreamPS != null) quantumDataStreamPS.Stop();
        if (fireGlowLight != null) baseFireIntensity = fireGlowLight.intensity;
    }

    protected override void Update()
    {
        base.Update();

        // Rotate silicon wafer slowly with iridescent shimmer
        if (siliconWafer != null)
        {
            siliconWafer.Rotate(Vector3.up, 24f * Time.deltaTime, Space.Self);
        }

        if (primitiveWheel != null && isInteracting)
        {
            primitiveWheel.Rotate(Vector3.forward, 45f * Time.deltaTime, Space.Self);
        }

        // Fire flicker
        if (fireGlowLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * 8f, 0f);
            fireGlowLight.intensity = (isInteracting ? 2.5f : baseFireIntensity) + noise * 0.4f;
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        if (fireSparkPS != null) fireSparkPS.Play();
        if (quantumDataStreamPS != null) quantumDataStreamPS.Play();
    }

    public override void StopInteraction()
    {
        base.StopInteraction();

        if (quantumDataStreamPS != null) quantumDataStreamPS.Stop();
    }
}
