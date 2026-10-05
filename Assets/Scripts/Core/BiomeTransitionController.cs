using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — BiomeTransitionController.cs
/// Manages dynamic panoramic skybox, curved Ultra-HD vista screen, and lighting transitions
/// as offerings / sample pods are completed.
/// Supported Biomes: Default (Night/Space) → Lake (Water) → Mountain (Crystal) → Forest (Flora) → Golden Climax Convergence.
/// </summary>
public class BiomeTransitionController : MonoBehaviour
{
    public static BiomeTransitionController Instance { get; private set; }

    [Header("Curved Ultra-HD Vista Screen")]
    [SerializeField] private MeshRenderer curvedVistaRenderer;
    [SerializeField] private Texture2D defaultVistaTex;
    [SerializeField] private Texture2D lakeVistaTex;
    [SerializeField] private Texture2D mountainVistaTex;
    [SerializeField] private Texture2D forestVistaTex;

    [Header("Skybox Materials (Panoramic)")]
    [Tooltip("Default panoramic skybox before any offerings are completed.")]
    [SerializeField] private Material defaultSkybox;

    [Tooltip("Panoramic skybox for the Lake biome (Offering 0: Water).")]
    [SerializeField] private Material lakeSkybox;

    [Tooltip("Panoramic skybox for the Mountain biome (Offering 1: Crystal).")]
    [SerializeField] private Material mountainSkybox;

    [Tooltip("Panoramic skybox for the Forest biome (Offering 2: Flora).")]
    [SerializeField] private Material forestSkybox;

    [Header("Ambient Light Colors per Biome")]
    [SerializeField] private Color defaultAmbient = new Color(0.08f, 0.09f, 0.18f, 1f);
    [SerializeField] private Color lakeAmbient = new Color(0.12f, 0.28f, 0.38f, 1f);
    [SerializeField] private Color mountainAmbient = new Color(0.22f, 0.25f, 0.42f, 1f);
    [SerializeField] private Color forestAmbient = new Color(0.15f, 0.32f, 0.18f, 1f);

    [Header("Directional Light Intensities per Biome")]
    [SerializeField] private float defaultLightIntensity = 0.6f;
    [SerializeField] private float lakeLightIntensity = 1.0f;
    [SerializeField] private float mountainLightIntensity = 1.2f;
    [SerializeField] private float forestLightIntensity = 0.9f;

    [Header("Directional Light Colors per Biome")]
    [SerializeField] private Color defaultLightColor = new Color(0.70f, 0.75f, 0.90f, 1f);
    [SerializeField] private Color lakeLightColor = new Color(0.75f, 0.92f, 1.00f, 1f);
    [SerializeField] private Color mountainLightColor = new Color(0.90f, 0.95f, 1.00f, 1f);
    [SerializeField] private Color forestLightColor = new Color(0.98f, 0.94f, 0.82f, 1f);

    [Header("Climax Lighting Settings")]
    [SerializeField] private Color climaxAmbient = new Color(0.85f, 0.65f, 0.25f, 1f);
    [SerializeField] private Color climaxLightColor = new Color(1.00f, 0.92f, 0.65f, 1f);
    [SerializeField] private float climaxLightIntensity = 2.5f;

    [Header("Transition Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float transitionDuration = 3.0f;

    // Internal state
    private Material currentSkyboxMaterial;
    private Material tempBlendMaterial;
    private Material vistaScreenMaterial;
    private Texture2D currentVistaTex;
    private Coroutine activeTransitionCoroutine;

    private const string BlendShaderName = "XENOASIS/BiomeSkyboxBlend";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (RenderSettings.skybox == null && defaultSkybox != null)
        {
            RenderSettings.skybox = defaultSkybox;
        }

        currentSkyboxMaterial = RenderSettings.skybox != null ? RenderSettings.skybox : defaultSkybox;
        RenderSettings.ambientLight = defaultAmbient;

        if (directionalLight != null)
        {
            directionalLight.intensity = defaultLightIntensity;
            directionalLight.color = defaultLightColor;
        }

        if (curvedVistaRenderer != null)
        {
            vistaScreenMaterial = curvedVistaRenderer.material; // Create instance
            currentVistaTex = defaultVistaTex != null ? defaultVistaTex : lakeVistaTex;
            if (vistaScreenMaterial != null && currentVistaTex != null)
            {
                vistaScreenMaterial.SetTexture("_MainTex", currentVistaTex);
                vistaScreenMaterial.SetFloat("_BlendFactor", 0f);
                vistaScreenMaterial.SetFloat("_Exposure", 1.0f);
            }
        }

        if (OfferingManager.Instance != null)
        {
            OfferingManager.Instance.OfferingCompleted += OnOfferingCompleted;
        }
    }

    private void OnDestroy()
    {
        if (OfferingManager.Instance != null)
        {
            OfferingManager.Instance.OfferingCompleted -= OnOfferingCompleted;
        }

        if (tempBlendMaterial != null)
        {
            Destroy(tempBlendMaterial);
            tempBlendMaterial = null;
        }
    }

    private void OnOfferingCompleted(int offeringIndex)
    {
        Debug.Log($"[BiomeTransitionController] Offering {offeringIndex} sealed. Initiating biome shift...");

        switch (offeringIndex)
        {
            case 0: // Water -> Lake
                TransitionToBiome(lakeSkybox, lakeVistaTex, lakeAmbient, lakeLightIntensity, lakeLightColor);
                break;
            case 1: // Crystal -> Mountain
                TransitionToBiome(mountainSkybox, mountainVistaTex, mountainAmbient, mountainLightIntensity, mountainLightColor);
                break;
            case 2: // Flora -> Forest
                TransitionToBiome(forestSkybox, forestVistaTex, forestAmbient, forestLightIntensity, forestLightColor);
                break;
        }
    }

    public void TransitionToBiome(Material targetSkybox, Texture2D targetVista, Color targetAmbient, float targetLightIntensity, Color targetLightColor, float duration = -1f)
    {
        if (activeTransitionCoroutine != null)
        {
            StopCoroutine(activeTransitionCoroutine);
        }

        float time = duration > 0f ? duration : transitionDuration;
        activeTransitionCoroutine = StartCoroutine(TransitionCoroutine(targetSkybox, targetVista, targetAmbient, targetLightIntensity, targetLightColor, time));
    }

    private IEnumerator TransitionCoroutine(Material targetSkybox, Texture2D targetVista, Color targetAmbient, float targetLightIntensity, Color targetLightColor, float duration)
    {
        Shader blendShader = Shader.Find(BlendShaderName);
        if (blendShader != null)
        {
            if (tempBlendMaterial != null) Destroy(tempBlendMaterial);
            tempBlendMaterial = new Material(blendShader) { name = "Temp_BiomeSkyboxBlend" };

            Material activeSkybox = RenderSettings.skybox != null ? RenderSettings.skybox : currentSkyboxMaterial;
            Texture currentTex = activeSkybox != null && activeSkybox.HasProperty("_MainTex") ? activeSkybox.GetTexture("_MainTex") : null;
            Texture targetTex = targetSkybox != null && targetSkybox.HasProperty("_MainTex") ? targetSkybox.GetTexture("_MainTex") : null;

            if (currentTex != null) tempBlendMaterial.SetTexture("_MainTex", currentTex);
            else if (targetTex != null) tempBlendMaterial.SetTexture("_MainTex", targetTex);

            if (targetTex != null) tempBlendMaterial.SetTexture("_BlendTex", targetTex);
            else if (currentTex != null) tempBlendMaterial.SetTexture("_BlendTex", currentTex);

            tempBlendMaterial.SetFloat("_BlendFactor", 0f);
            RenderSettings.skybox = tempBlendMaterial;
        }

        // Configure curved vista screen blend
        if (vistaScreenMaterial != null && targetVista != null)
        {
            Texture srcVista = currentVistaTex != null ? currentVistaTex : targetVista;
            vistaScreenMaterial.SetTexture("_MainTex", srcVista);
            vistaScreenMaterial.SetTexture("_BlendTex", targetVista);
            vistaScreenMaterial.SetFloat("_BlendFactor", 0f);
        }

        Color startAmbient = RenderSettings.ambientLight;
        float startLightInt = directionalLight != null ? directionalLight.intensity : targetLightIntensity;
        Color startLightColor = directionalLight != null ? directionalLight.color : targetLightColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (tempBlendMaterial != null)
            {
                tempBlendMaterial.SetFloat("_BlendFactor", smoothT);
            }

            if (vistaScreenMaterial != null)
            {
                vistaScreenMaterial.SetFloat("_BlendFactor", smoothT);
            }

            RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, smoothT);

            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(startLightInt, targetLightIntensity, smoothT);
                directionalLight.color = Color.Lerp(startLightColor, targetLightColor, smoothT);
            }

            yield return null;
        }

        if (targetSkybox != null)
        {
            RenderSettings.skybox = targetSkybox;
            currentSkyboxMaterial = targetSkybox;
        }

        if (vistaScreenMaterial != null && targetVista != null)
        {
            vistaScreenMaterial.SetTexture("_MainTex", targetVista);
            vistaScreenMaterial.SetFloat("_BlendFactor", 0f);
            currentVistaTex = targetVista;
        }

        RenderSettings.ambientLight = targetAmbient;

        if (directionalLight != null)
        {
            directionalLight.intensity = targetLightIntensity;
            directionalLight.color = targetLightColor;
        }

        if (tempBlendMaterial != null)
        {
            Destroy(tempBlendMaterial);
            tempBlendMaterial = null;
        }

        activeTransitionCoroutine = null;
        Debug.Log("[BiomeTransitionController] Biome shift complete.");
    }

    public void TriggerClimax()
    {
        Debug.Log("[BiomeTransitionController] Triggering Climax Convergence...");
        if (activeTransitionCoroutine != null) StopCoroutine(activeTransitionCoroutine);
        activeTransitionCoroutine = StartCoroutine(ClimaxConvergenceCoroutine());
    }

    private IEnumerator ClimaxConvergenceCoroutine()
    {
        Color startAmbient = RenderSettings.ambientLight;
        float startLightInt = directionalLight != null ? directionalLight.intensity : 1f;
        Color startLightCol = directionalLight != null ? directionalLight.color : Color.white;

        float elapsed = 0f;
        float duration = 4.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            RenderSettings.ambientLight = Color.Lerp(startAmbient, climaxAmbient, smoothT);

            if (directionalLight != null)
            {
                directionalLight.color = Color.Lerp(startLightCol, climaxLightColor, smoothT);
                directionalLight.intensity = Mathf.Lerp(startLightInt, climaxLightIntensity, smoothT);
            }

            if (vistaScreenMaterial != null)
            {
                vistaScreenMaterial.SetFloat("_Exposure", Mathf.Lerp(1.0f, 1.45f, smoothT));
                vistaScreenMaterial.SetColor("_Tint", Color.Lerp(Color.white, new Color(1f, 0.92f, 0.75f), smoothT));
            }

            yield return null;
        }

        activeTransitionCoroutine = null;
        Debug.Log("[BiomeTransitionController] Climax convergence complete.");
    }
}
