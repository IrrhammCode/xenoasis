using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// XENOASIS — UFODescentSequence.cs
/// Orchestrates the 30-Second Full-Screen Cinematic Space-to-Earth & Vertical Tractor Beam Descent:
/// 1. Deep Space Orbit & 3D Earth Approach (0.0s - 8.0s, 8s):
///    100% full-screen 3D view of Planet Earth globe rotating in deep space with blue atmospheric haze.
///    Floating holographic visor HUD displays orbital altitude (36,000 km -> 1,000 km) and Mach 12 -> 28.
/// 2. Atmospheric Re-Entry Burn (8.0s - 15.0s, 7s):
///    Fiery plasma wrap-around VFX, craft vibration, descent through Eksosfer -> Mesosfer (peak temp 2,950°C).
/// 3. Tropospheric Cloud Canopy Penetration (15.0s - 21.0s, 6s):
///    Skybox crossfades to Earth landscape, volumetric clouds stream past camera through Stratosfer -> Troposfer.
/// 4. Earth Biosphere & Sanctuary Reveal (21.0s - 25.0s, 4s):
///    Clouds clear, pristine alpine lake & mountains revealed, camera glides underneath hovering UFO mothership.
/// 5. Ventral Tractor Beam Descent ("Diturunin dari UFO") (25.0s - 30.0s, 5s):
///    UFO ventral core illuminates, tractor beam cylinder and golden stardust burst downward.
///    The player is lowered vertically from above (Y=22m -> 1.4m) through the glass dome skylight onto the central dais.
/// 6. Touchdown & Human Ambassador Welcome (30.0s+):
///    Player touches down safely, beam softens, Terran Ambassador greets the alien traveler, WASD & VR unlocked.
/// </summary>
public class UFODescentSequence : MonoBehaviour
{
    public static UFODescentSequence Instance { get; private set; }

    [Header("Sequence Timing (30s Total)")]
    [SerializeField] private float spaceApproachDuration = 8.0f;
    [SerializeField] private float reEntryDuration = 7.0f;
    [SerializeField] private float cloudPenetrationDuration = 6.0f;
    [SerializeField] private float earthApproachDuration = 4.0f;
    [SerializeField] private float beamDescentDuration = 5.0f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool allowSkipWithKey = true;

    [Header("Key References")]
    [SerializeField] private Transform playerRig;
    [SerializeField] private Transform chamberSpawnPoint;
    [SerializeField] private GameObject spaceEarthObject;
    [SerializeField] private GameObject humanAmbassadorHolo;
    [SerializeField] private GameObject tractorBeamVisual;
    [SerializeField] private Light tractorSpotlight;
    [SerializeField] private ParticleSystem tractorStardust;

    [SerializeField] private ParticleSystem warpStreaksVFX;
    [SerializeField] private ParticleSystem reEntryPlasmaVFX;
    [SerializeField] private ParticleSystem cloudVFX;
    [SerializeField] private TMPro.TextMeshPro hudStatusText;

    [Header("Cockpit MFD Telemetry Screens (Visor HUD)")]
    [SerializeField] private TMPro.TextMeshPro mfdCenterText;
    [SerializeField] private TMPro.TextMeshPro mfdLeftText;
    [SerializeField] private TMPro.TextMeshPro mfdRightText;

    [Header("Skybox Transitions")]
    [SerializeField] private Material spaceSkybox;
    [SerializeField] private Material earthSkybox;
    [SerializeField] private Texture2D starlessSpaceSkyboxTex;
    private Material dynamicBlendSkyboxMat;

    [Header("Audio")]
    [SerializeField] private AudioSource sequenceAudioSource;
    [SerializeField] private AudioClip warpHumClip;
    [SerializeField] private AudioClip reEntryRoarClip;
    [SerializeField] private AudioClip tractorBeamClip;
    [SerializeField] private AudioClip welcomeVoiceClip;

    [Header("New Dynamic Interactive Elements")]
    [SerializeField] private GameObject ufoMothership;
    [SerializeField] private GameObject ufoCockpitBridge;
    [SerializeField] private ChamberRoofShutter chamberRoofShutter;
    [SerializeField] private PlanetarySectorSelector planetarySectorSelector;
    [SerializeField] private AlienPlayerRig alienPlayerRig;

    [Header("Waypoints")]
    [SerializeField] private Vector3 orbitPosition = new Vector3(0f, 1000f, 0f);
    [SerializeField] private Vector3 reEntryStartPosition = new Vector3(0f, 960f, 160f);
    [SerializeField] private Vector3 cloudEntryPosition = new Vector3(0f, 350f, 40f);
    [SerializeField] private Vector3 surfaceApproachPosition = new Vector3(0f, 50f, 35f);
    [SerializeField] private Vector3 hoverPosition = new Vector3(0f, 18f, 0f);
    [SerializeField] private Vector3 touchdownPosition = new Vector3(0f, 0.24f, 0f);

    public enum SequenceState
    {
        NotStarted,
        SpaceFlight,
        ReEntryBurn,
        CloudPenetration,
        SurfaceApproach,
        TractorBeamDescent,
        TouchdownWelcome,
        ChamberExploration,
        // Aliases for backward compatibility
        Docking = TractorBeamDescent,
        DockedWelcome = TouchdownWelcome
    }

    public SequenceState CurrentState { get; private set; } = SequenceState.NotStarted;

    public UnityEvent OnDescentStarted = new UnityEvent();
    public UnityEvent OnAtmosphericEntry = new UnityEvent();
    public UnityEvent OnCloudBreakthrough = new UnityEvent();
    public UnityEvent OnTractorBeamIgnited = new UnityEvent();
    public UnityEvent OnTouchdown = new UnityEvent();

    // Legacy event aliases
    public UnityEvent OnShipDocked => OnTouchdown;
    public UnityEvent OnAirlockOpened => OnTouchdown;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (autoStart)
        {
            StartCoroutine(RunDescentSequence());
        }
    }

    private void Update()
    {
        // Allow skipping descent during testing in Editor with KeyCode.K
        if (allowSkipWithKey && Input.GetKeyDown(KeyCode.K))
        {
            if (CurrentState != SequenceState.ChamberExploration)
            {
                StopAllCoroutines();
                FastForwardToChamber();
            }
        }
    }

    public void BeginSequence()
    {
        StopAllCoroutines();
        StartCoroutine(RunDescentSequence());
    }

    private IEnumerator RunDescentSequence()
    {
        OnDescentStarted?.Invoke();

        // 1. Initial Setup: Distant Earth in Deep Cosmos
        Vector3 nearEarthPos = new Vector3(0f, -60f, 520f);
        Vector3 nearEarthScale = Vector3.one * 360f;
        Vector3 farEarthPos = new Vector3(0f, -25f, 2720f);
        Vector3 farEarthScale = Vector3.one * 43.2f; // Small distant celestial marble in deep cosmos

        // Set up dynamic blend skybox for continuous atmospheric entry transition
        Shader blendShader = Shader.Find("XENOASIS/BiomeSkyboxBlend");
        if (blendShader != null)
        {
            dynamicBlendSkyboxMat = new Material(blendShader);
            Texture spaceTex = spaceSkybox != null ? spaceSkybox.GetTexture("_MainTex") : null;
            Texture earthTex = earthSkybox != null ? earthSkybox.GetTexture("_MainTex") : null;
            if (spaceTex != null) dynamicBlendSkyboxMat.SetTexture("_MainTex", spaceTex);
            if (starlessSpaceSkyboxTex != null) dynamicBlendSkyboxMat.SetTexture("_StarlessTex", starlessSpaceSkyboxTex);
            if (earthTex != null) dynamicBlendSkyboxMat.SetTexture("_BlendTex", earthTex);
            dynamicBlendSkyboxMat.SetFloat("_BlendFactor", 0.0f);
            dynamicBlendSkyboxMat.SetFloat("_StarIntensity", 1.0f);
            dynamicBlendSkyboxMat.SetColor("_AtmoColor", Color.clear);
            dynamicBlendSkyboxMat.SetFloat("_Exposure", 1.0f);
            dynamicBlendSkyboxMat.SetColor("_Tint", Color.white);
            RenderSettings.skybox = dynamicBlendSkyboxMat;
        }
        else if (spaceSkybox != null)
        {
            RenderSettings.skybox = spaceSkybox;
        }

        RenderSettings.fog = false;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.04f, 0.05f, 0.08f);

        if (spaceEarthObject != null)
        {
            spaceEarthObject.SetActive(true);
            spaceEarthObject.transform.localScale = farEarthScale;
            spaceEarthObject.transform.localPosition = farEarthPos;
        }

        if (playerRig != null)
        {
            playerRig.SetParent(null);
            playerRig.position = orbitPosition;
            playerRig.rotation = Quaternion.Euler(8f, 0f, 0f);
        }

        if (ufoCockpitBridge != null)
        {
            ufoCockpitBridge.SetActive(true);
            if (playerRig != null)
            {
                ufoCockpitBridge.transform.SetParent(playerRig);
                ufoCockpitBridge.transform.localPosition = new Vector3(0f, -1.05f, -0.15f);
                ufoCockpitBridge.transform.localRotation = Quaternion.identity;

                Camera cam = Camera.main;
                if (cam != null)
                {
                    cam.nearClipPlane = 0.02f;
                    if (cam.transform.parent == playerRig)
                    {
                        cam.transform.localPosition = Vector3.zero;
                        cam.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        if (tractorBeamVisual != null) tractorBeamVisual.SetActive(false);
        if (humanAmbassadorHolo != null) humanAmbassadorHolo.SetActive(false);
        if (reEntryPlasmaVFX != null) reEntryPlasmaVFX.Stop();
        if (cloudVFX != null) cloudVFX.Stop();
        if (tractorStardust != null) tractorStardust.Stop();

        // Ensure chamber roof shutter starts closed so it can animate opening when descending
        if (chamberRoofShutter != null)
        {
            chamberRoofShutter.SetShutterStateImmediate(true);
        }

        PlaySound(warpHumClip, 0.75f, true);

        // =========================================================================
        // DIRECT FLIGHT START: Immediately engage deep space warp towards Earth
        // =========================================================================
        CurrentState = SequenceState.SpaceFlight;
        if (planetarySectorSelector != null)
        {
            planetarySectorSelector.gameObject.SetActive(false);
        }

        // =========================================================================
        // PHASE 1: DEEP SPACE WARP TO EARTH (FAR AWAY -> LIGHTSPEED -> DECELERATE TO ORBIT)
        // 1. UFO starts far away in deep cosmos. Earth is a distant glowing blue speck.
        // 2. Lightspeed Warp Drive engages: warp star streaks flash past windows.
        // 3. Smooth Deceleration: As Earth grows large, warp drops and UFO smoothly
        //    slows down ("mulai pelan-pelan") into steady orbit before re-entry.
        // =========================================================================
        CurrentState = SequenceState.SpaceFlight;

        // Earth starts FAR in the cosmic distance
        if (spaceEarthObject != null)
        {
            spaceEarthObject.transform.localPosition = farEarthPos;
            spaceEarthObject.transform.localScale = farEarthScale;
        }

        // Engage hyper-light warp streaks
        if (warpStreaksVFX != null) warpStreaksVFX.Play();
        PlaySound(warpHumClip, 0.95f, true);

        float elapsed = 0f;
        Vector3 startP = orbitPosition;
        Vector3 endP = reEntryStartPosition;
        Quaternion startR = Quaternion.Euler(6f, 0f, 0f);
        Quaternion endR = Quaternion.Euler(14f, 0f, 0f);

        while (elapsed < spaceApproachDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spaceApproachDuration);

            // Sub-Phase 1A: Hyper-Light Warp Travel (t: 0.0 -> 0.52)
            // UFO rushes across hundreds of thousands of km towards distant Earth
            if (t < 0.52f)
            {
                float warpT = t / 0.52f;
                float warpCurve = Mathf.Pow(warpT, 2.2f);

                if (playerRig != null)
                {
                    playerRig.position = Vector3.Lerp(startP, Vector3.Lerp(startP, endP, 0.35f), warpCurve);
                    float warpVibe = Mathf.Sin(Time.time * 68f) * 0.012f;
                    playerRig.position += new Vector3(warpVibe, warpVibe * 0.5f, 0f);
                    playerRig.rotation = Quaternion.Slerp(startR, Quaternion.Euler(8f, 0, 0), warpCurve);
                }

                if (spaceEarthObject != null)
                {
                    // Earth rushes forward from far distance to intermediate orbital view
                    spaceEarthObject.transform.localPosition = Vector3.Lerp(farEarthPos, nearEarthPos + new Vector3(0f, 15f, 320f), warpCurve);
                    spaceEarthObject.transform.localScale = Vector3.Lerp(farEarthScale, nearEarthScale * 0.90f, warpCurve);
                }
            }
            // Sub-Phase 1B: Warp Drop & Smooth Deceleration ("Mulai Pelan-Pelan") (t: 0.52 -> 1.0)
            // Reverse thrusters brake the UFO smoothly into low orbital rendezvous
            else
            {
                float decelT = (t - 0.52f) / 0.48f;
                float smoothDecel = Mathf.SmoothStep(0f, 1f, decelT);

                // Disengage warp streaks gracefully
                if (warpStreaksVFX != null && warpStreaksVFX.isPlaying)
                {
                    warpStreaksVFX.Stop();
                }

                if (playerRig != null)
                {
                    Vector3 decelStartP = Vector3.Lerp(startP, endP, 0.35f);
                    playerRig.position = Vector3.Lerp(decelStartP, endP, smoothDecel);
                    playerRig.rotation = Quaternion.Slerp(Quaternion.Euler(8f, 0, 0), endR, smoothDecel);
                }

                if (spaceEarthObject != null)
                {
                    Vector3 decelStartEarthPos = nearEarthPos + new Vector3(0f, 15f, 320f);
                    Vector3 decelStartEarthScale = nearEarthScale * 0.90f;
                    // Earth majestically expands to wrap around forward canopy
                    spaceEarthObject.transform.localPosition = Vector3.Lerp(decelStartEarthPos, nearEarthPos + new Vector3(0f, 25f, -90f), smoothDecel);
                    spaceEarthObject.transform.localScale = Vector3.Lerp(decelStartEarthScale, nearEarthScale * 1.85f, smoothDecel);
                }

                // Pre-entry atmospheric wash as UFO grazes outermost thermosphere
                if (smoothDecel > 0.5f && dynamicBlendSkyboxMat != null)
                {
                    float preAtmo = (smoothDecel - 0.5f) / 0.5f;
                    // Faint thermosphere glow building smoothly
                    dynamicBlendSkyboxMat.SetColor("_AtmoColor", Color.Lerp(Color.clear, new Color(0.02f, 0.10f, 0.28f, 0.35f), preAtmo));
                    // Gently soften deep cosmos stars as upper atmosphere begins to glow
                    dynamicBlendSkyboxMat.SetFloat("_StarIntensity", Mathf.Lerp(1.0f, 0.40f, preAtmo));
                }
            }

            yield return null;
        }

        if (warpStreaksVFX != null) warpStreaksVFX.Stop();

        // =========================================================================
        // PHASE 2: ATMOSPHERIC RE-ENTRY BURN (LAPISAN 1 & 2) (8.0s - 15.0s)
        // Silky-Smooth Continuous Atmospheric Transformation: Eksosfer -> Ionosfer -> Mesosfer
        // =========================================================================
        CurrentState = SequenceState.ReEntryBurn;
        OnAtmosphericEntry?.Invoke();
        if (reEntryPlasmaVFX != null) reEntryPlasmaVFX.Play();
        PlaySound(reEntryRoarClip, 0.90f, false);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        elapsed = 0f;
        startP = playerRig != null ? playerRig.position : reEntryStartPosition;
        endP = cloudEntryPosition;
        startR = playerRig != null ? playerRig.rotation : Quaternion.Euler(14f, 0f, 0f);
        endR = Quaternion.Euler(22f, 0f, 0f);

        Vector3 phase2EarthStartScale = spaceEarthObject != null ? spaceEarthObject.transform.localScale : nearEarthScale * 1.85f;

        while (elapsed < reEntryDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / reEntryDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Aerodynamic turbulence vibration - starts smoothly at 0 and peaks during re-entry
            float shake = Mathf.Sin(Time.time * 48f) * 0.08f * Mathf.Sin(t * Mathf.PI);
            if (playerRig != null)
            {
                Vector3 basePos = Vector3.Lerp(startP, endP, smoothT);
                playerRig.position = basePos + new Vector3(shake, shake * 0.6f, 0f);
                playerRig.rotation = Quaternion.Slerp(startR, endR, smoothT);
            }

            // Earth horizon realistically expands to wrap around forward canopy
            if (spaceEarthObject != null)
            {
                spaceEarthObject.transform.localScale = Vector3.Lerp(phase2EarthStartScale, phase2EarthStartScale * 2.6f, smoothT);
                spaceEarthObject.transform.localPosition = Vector3.Lerp(nearEarthPos + new Vector3(0f, 25f, -90f), nearEarthPos + new Vector3(0f, 40f, -140f), smoothT);
            }

            // Sub-Layer 1: Eksosfer & Ionosfer (smoothT: 0.0 -> 0.4)
            if (smoothT < 0.4f)
            {
                float atmoT = smoothT / 0.4f;
                float altKm = Mathf.Lerp(1000f, 120f, atmoT);
                UpdateHUD($"<b><color=#00E5FF>[ LAPISAN 1: EKSOSFER & IONOSFER ]</color></b>\n<size=70%>ALTITUDE: {altKm:F0} KM · IONISASI PLASMA TERDETEKSI\nAURORA BOREALIS AKTIF // PERISAI MENYERAP GESEKAN</size>", new Color(0f, 0.9f, 1f));

                RenderSettings.fogColor = Color.Lerp(new Color(0.01f, 0.03f, 0.08f), new Color(0.04f, 0.20f, 0.42f), atmoT);
                RenderSettings.fogDensity = Mathf.Lerp(0.0001f, 0.0016f, atmoT);
                RenderSettings.ambientLight = Color.Lerp(new Color(0.04f, 0.05f, 0.08f), new Color(0.08f, 0.18f, 0.30f), atmoT);

                if (dynamicBlendSkyboxMat != null)
                {
                    // As requested: Stars are smoothly washed out to 100% ZERO visibility upon entering atmospheric ionization!
                    dynamicBlendSkyboxMat.SetFloat("_StarIntensity", Mathf.Lerp(0.40f, 0.0f, atmoT * 2.0f));
                    Color atmoWash = Color.Lerp(new Color(0.02f, 0.10f, 0.28f, 0.35f), new Color(0.06f, 0.32f, 0.70f, 0.85f), atmoT);
                    dynamicBlendSkyboxMat.SetColor("_AtmoColor", atmoWash);
                    dynamicBlendSkyboxMat.SetFloat("_BlendFactor", Mathf.Lerp(0f, 0.20f, atmoT));
                }
            }
            // Sub-Layer 2: Mesosfer - Compressive Plasma Sheath (smoothT: 0.4 -> 1.0)
            else
            {
                float plasmaT = (smoothT - 0.4f) / 0.6f;
                float altKm = Mathf.Lerp(120f, 25f, plasmaT);
                float tempC = Mathf.Lerp(450f, 2950f, Mathf.Sin(plasmaT * Mathf.PI));

                float plasmaCurve = Mathf.Sin(plasmaT * Mathf.PI * 0.5f);
                Color plasmaColor = Color.Lerp(new Color(0.04f, 0.20f, 0.42f), new Color(0.92f, 0.44f, 0.08f), plasmaCurve);
                RenderSettings.fogColor = plasmaColor;
                RenderSettings.fogDensity = Mathf.Lerp(0.0016f, 0.0075f, plasmaT);
                RenderSettings.ambientLight = Color.Lerp(new Color(0.08f, 0.18f, 0.30f), new Color(0.55f, 0.28f, 0.08f), plasmaT);

                if (dynamicBlendSkyboxMat != null)
                {
                    // 100% incandescent fiery amber plasma sheath, ZERO stars!
                    dynamicBlendSkyboxMat.SetFloat("_StarIntensity", 0.0f);
                    Color plasmaWash = Color.Lerp(new Color(0.06f, 0.32f, 0.70f, 0.85f), new Color(0.96f, 0.48f, 0.08f, 1.0f), plasmaCurve);
                    dynamicBlendSkyboxMat.SetColor("_AtmoColor", plasmaWash);
                    dynamicBlendSkyboxMat.SetFloat("_BlendFactor", Mathf.Lerp(0.20f, 0.45f, plasmaT));
                }
            }

            yield return null;
        }

        if (reEntryPlasmaVFX != null) reEntryPlasmaVFX.Stop();

        // =========================================================================
        // PHASE 3: TROPOSPHERIC CLOUD CANOPY PENETRATION (15.0s - 21.0s)
        // Silky-Smooth Continuous Transformation: Stratosfer -> Troposfer Cloud Deck
        // =========================================================================
        CurrentState = SequenceState.CloudPenetration;
        OnCloudBreakthrough?.Invoke();
        if (cloudVFX != null) cloudVFX.Play();

        elapsed = 0f;
        startP = playerRig != null ? playerRig.position : cloudEntryPosition;
        endP = surfaceApproachPosition;
        startR = playerRig != null ? playerRig.rotation : Quaternion.Euler(22f, 0f, 0f);
        endR = Quaternion.Euler(6f, 0f, 0f);

        while (elapsed < cloudPenetrationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cloudPenetrationDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (playerRig != null)
            {
                playerRig.position = Vector3.Lerp(startP, endP, smoothT);
                playerRig.rotation = Quaternion.Slerp(startR, endR, smoothT);
            }

            // Sub-Layer 3: Stratosfer & Lapisan Ozon (smoothT: 0.0 -> 0.45)
            if (smoothT < 0.45f)
            {
                float stratT = smoothT / 0.45f;
                float altKm = Mathf.Lerp(45f, 12f, stratT);
                UpdateHUD($"<b><color=#00D4FF>[ LAPISAN 3: STRATOSFER & LAPISAN OZON ]</color></b>\n<size=70%>ALTITUDE: {altKm:F0} KM · LAPISAN OZON (O3) TERDETEKSI\nAERO-BRAKE AKTIF // DEK AWAN PUTIH TERBENTANG DI BAWAH</size>", new Color(0f, 0.85f, 1f));

                Color stratBlue = new Color(0.08f, 0.45f, 0.88f);
                RenderSettings.fogColor = Color.Lerp(new Color(0.92f, 0.44f, 0.08f), stratBlue, stratT);
                RenderSettings.fogDensity = Mathf.Lerp(0.0075f, 0.0035f, stratT);
                RenderSettings.ambientLight = Color.Lerp(new Color(0.55f, 0.28f, 0.08f), new Color(0.22f, 0.42f, 0.68f), stratT);

                if (dynamicBlendSkyboxMat != null)
                {
                    // Transition smoothly from plasma amber into daytime azure blue ozone sky (ZERO stars)
                    dynamicBlendSkyboxMat.SetFloat("_StarIntensity", 0.0f);
                    Color stratWash = Color.Lerp(new Color(0.96f, 0.48f, 0.08f, 1.0f), new Color(0.12f, 0.52f, 0.92f, 1.0f), stratT);
                    dynamicBlendSkyboxMat.SetColor("_AtmoColor", stratWash);
                    dynamicBlendSkyboxMat.SetFloat("_BlendFactor", Mathf.Lerp(0.45f, 0.75f, stratT));
                }
            }
            // Sub-Layer 4: Troposfer Cloud Canopy Penetration (smoothT: 0.45 -> 1.0)
            else
            {
                float tropT = (smoothT - 0.45f) / 0.55f;
                float altM = Mathf.Lerp(12000f, 1800f, tropT);

                Color cloudWhite = new Color(0.88f, 0.92f, 0.96f);
                RenderSettings.fogColor = Color.Lerp(new Color(0.08f, 0.45f, 0.88f), cloudWhite, Mathf.Sin(tropT * Mathf.PI * 0.5f));
                // Fog peaks inside cloud layer then clears as we pierce through
                RenderSettings.fogDensity = Mathf.Lerp(0.0035f, 0.022f, Mathf.Sin(tropT * Mathf.PI));
                RenderSettings.ambientLight = Color.Lerp(new Color(0.22f, 0.42f, 0.68f), new Color(0.68f, 0.75f, 0.82f), tropT);

                if (dynamicBlendSkyboxMat != null)
                {
                    // Wash into dense white cloud canopy, blending into Earth skybox (ZERO stars)
                    dynamicBlendSkyboxMat.SetFloat("_StarIntensity", 0.0f);
                    Color cloudWash = Color.Lerp(new Color(0.12f, 0.52f, 0.92f, 1.0f), new Color(0.92f, 0.95f, 0.98f, 0.95f), Mathf.Sin(tropT * Mathf.PI * 0.5f));
                    dynamicBlendSkyboxMat.SetColor("_AtmoColor", cloudWash);
                    dynamicBlendSkyboxMat.SetFloat("_BlendFactor", Mathf.Lerp(0.75f, 1.0f, tropT));
                }

                // Seamlessly hide space globe once camera is fully enclosed in tropospheric cloud deck
                if (tropT > 0.40f && spaceEarthObject != null && spaceEarthObject.activeSelf)
                {
                    spaceEarthObject.SetActive(false);
                }
            }

            yield return null;
        }

        if (cloudVFX != null) cloudVFX.Stop();

        // =========================================================================
        // PHASE 4: EARTH BIOSPHERE REVEAL & RENDEZVOUS AT UFO (21.0s - 25.0s)
        // Clouds clear, alpine lake and obsidian dome sanctuary revealed in morning sunlight
        // =========================================================================
        CurrentState = SequenceState.SurfaceApproach;
        if (spaceEarthObject != null) spaceEarthObject.SetActive(false);

        elapsed = 0f;
        startP = playerRig != null ? playerRig.position : surfaceApproachPosition;
        endP = hoverPosition;
        startR = playerRig != null ? playerRig.rotation : Quaternion.Euler(10f, 0f, 0f);
        endR = Quaternion.Euler(18f, 0f, 0f);

        while (elapsed < earthApproachDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / earthApproachDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (playerRig != null)
            {
                playerRig.position = Vector3.Lerp(startP, endP, smoothT);
                playerRig.rotation = Quaternion.Slerp(startR, endR, smoothT);
            }

            RenderSettings.fogColor = Color.Lerp(new Color(0.88f, 0.92f, 0.96f), new Color(0.72f, 0.82f, 0.90f), smoothT);
            RenderSettings.fogDensity = Mathf.Lerp(0.004f, 0.0001f, smoothT);
            RenderSettings.ambientLight = Color.Lerp(new Color(0.68f, 0.75f, 0.82f), new Color(0.65f, 0.72f, 0.80f), smoothT);

            if (dynamicBlendSkyboxMat != null)
            {
                dynamicBlendSkyboxMat.SetFloat("_StarIntensity", 0.0f);
                Color morningClear = Color.Lerp(new Color(0.92f, 0.95f, 0.98f, 0.95f), new Color(0.65f, 0.82f, 0.98f, 0.0f), smoothT);
                dynamicBlendSkyboxMat.SetColor("_AtmoColor", morningClear);
                dynamicBlendSkyboxMat.SetFloat("_BlendFactor", 1.0f);
            }

            if (smoothT > 0.90f)
            {
                RenderSettings.fog = false;
            }

            yield return null;
        }
        RenderSettings.fog = false;
        if (earthSkybox != null) RenderSettings.skybox = earthSkybox;

        // =========================================================================
        // PHASE 5: VENTRAL TRACTOR BEAM DESCENT (3-STAGE ANIMATED SEQUENCE)
        // Stage 5A: Hover & Beam Ignition inside Cockpit (Warmup & Ventral Hatch Iris Open)
        // Stage 5B: Anti-Gravity Float Down through Ventral Iris Hatch into Open Air
        // Stage 5C: Majestic Descent through Sanctuary Dome Skylight to Dais
        // =========================================================================
        CurrentState = SequenceState.TractorBeamDescent;
        OnTractorBeamIgnited?.Invoke();

        // 100% Earth skybox enforcement and clear atmospheric conditions
        if (earthSkybox != null) RenderSettings.skybox = earthSkybox;
        RenderSettings.fog = false;

        // --- STAGE 5A: COCKPIT WARMUP & HATCH OPENING (Inside Cockpit) ---
        if (tractorSpotlight != null) tractorSpotlight.intensity = 2.5f;
        PlaySound(tractorBeamClip ?? warpHumClip, 0.90f, true);

        float stage5ATime = 2.5f;
        elapsed = 0f;
        Vector3 hoverOrigin = playerRig != null ? playerRig.position : hoverPosition;
        Quaternion hoverRot = playerRig != null ? playerRig.rotation : Quaternion.Euler(18f, 0f, 0f);

        while (elapsed < stage5ATime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stage5ATime);
            float pulse = Mathf.Sin(t * Mathf.PI * 4f) * 0.05f;

            if (playerRig != null)
            {
                // Gentle anti-gravity levitation float anticipation inside cockpit
                playerRig.position = hoverOrigin + new Vector3(0f, pulse + 0.12f * t, 0f);
            }

            yield return null;
        }

        // Activate Tractor Beam Visual & Stardust
        if (tractorBeamVisual != null) tractorBeamVisual.SetActive(true);
        if (tractorSpotlight != null) tractorSpotlight.intensity = 6.0f;
        if (tractorStardust != null)
        {
            tractorStardust.Play();
            tractorStardust.Emit(100);
        }

        // --- STAGE 5B: ANTI-GRAVITY FLOAT DOWN THROUGH VENTRAL HATCH (Cockpit Egress) ---
        // Cockpit remains fixed with UFO mothership overhead while player descends out of ventral iris
        if (ufoCockpitBridge != null)
        {
            if (ufoMothership != null)
            {
                ufoCockpitBridge.transform.SetParent(ufoMothership.transform);
                ufoCockpitBridge.transform.localPosition = new Vector3(0, -1.5f, 0);
                ufoCockpitBridge.transform.localRotation = Quaternion.identity;
            }
            else
            {
                ufoCockpitBridge.transform.SetParent(null);
                ufoCockpitBridge.transform.position = hoverOrigin + new Vector3(0f, -1.05f, -0.15f);
            }
        }

        float stage5BTime = 2.5f;
        elapsed = 0f;
        Vector3 hatchStartP = hoverOrigin;
        Vector3 hatchEndP = new Vector3(0f, 15f, 0f); // Float out of saucer into open mountain air
        Quaternion hatchStartR = hoverRot;
        Quaternion hatchEndR = Quaternion.Euler(8f, 0f, 0f);

        while (elapsed < stage5BTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stage5BTime);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (playerRig != null)
            {
                playerRig.position = Vector3.Lerp(hatchStartP, hatchEndP, smoothT);
                playerRig.rotation = Quaternion.Slerp(hatchStartR, hatchEndR, smoothT);
            }

            float currentAlt = Mathf.Lerp(18f, 15f, smoothT);
            UpdateHUD($"<b><color=#00FFFF>[ EGRESS VENTRAL ] MELAYANG KELUAR DARI UFO</color></b>\n<size=70%>KETINGGIAN: {currentAlt:F1} M · MEMASUKI PILAR CAHAYA TRACTOR BEAM\nMENDEKATI KUBAH OBSERVATORIUM // LEMBAH GUNUNG TERBENTANG</size>", new Color(0f, 0.95f, 1f));

            yield return null;
        }

        // --- STAGE 5C: MAJESTIC DESCENT THROUGH DOME SKYLIGHT TO DAIS ---
        // Animate the roof shutter opening smoothly as the traveler approaches the skylight ("animasi atas nya kebuka gitu")!
        if (chamberRoofShutter != null)
        {
            chamberRoofShutter.OpenShutter();
        }

        float stage5CTime = 4.5f;
        elapsed = 0f;
        Vector3 domeStartP = hatchEndP;
        Vector3 domeEndP = touchdownPosition;
        Quaternion domeStartR = hatchEndR;
        Quaternion domeEndR = Quaternion.identity;

        while (elapsed < stage5CTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stage5CTime);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (playerRig != null)
            {
                playerRig.position = Vector3.Lerp(domeStartP, domeEndP, smoothT);
                playerRig.rotation = Quaternion.Slerp(domeStartR, domeEndR, smoothT);
            }

            float currentAlt = Mathf.Lerp(15f, 1.4f, smoothT);
            UpdateHUD($"<b><color=#00FFFF>[ TRACTOR BEAM AKTIF ] MENEMBUS ATAP KUBAH OBSERVATORIUM</color></b>\n<size=70%>KETINGGIAN: {currentAlt:F1} M · MENDEKATI DAIS PENYAMBUTAN PUSAT\nDESELERASI LEMBUT // SISTEM ANTI-GRAVITASI TOUCHDOWN</size>", new Color(0f, 0.95f, 1f));

            if (tractorStardust != null && elapsed % 0.4f < Time.deltaTime)
            {
                tractorStardust.Emit(15);
            }

            yield return null;
        }

        // =========================================================================
        // PHASE 6: TOUCHDOWN ON DAIS & DIPLOMATIC WELCOME (30.0s+)
        // =========================================================================
        CurrentState = SequenceState.TouchdownWelcome;
        OnTouchdown?.Invoke();

        if (playerRig != null)
        {
            playerRig.position = touchdownPosition;
            playerRig.rotation = Quaternion.identity;
        }

        // Soften beam to ambient pillar, then fade out
        if (tractorSpotlight != null) tractorSpotlight.intensity = 1.0f;
        if (tractorBeamVisual != null) tractorBeamVisual.SetActive(false);

        // 1. UFO Mothership ascends into high hover ("pesawat ufo keatas dikit")
        if (ufoMothership != null)
        {
            StartCoroutine(AscendUFO(ufoMothership.transform, new Vector3(0, 48f, 0), 4.5f));
        }

        // 2. Chamber roof shutter closes and seals ("si gedung di tutup atap nya")
        if (chamberRoofShutter != null)
        {
            chamberRoofShutter.CloseShutter();
        }

        // Activate Human Diplomatic Ambassador
        if (humanAmbassadorHolo != null)
        {
            humanAmbassadorHolo.SetActive(true);
        }
        PlaySound(welcomeVoiceClip, 1.0f, false);

        UpdateHUD("<b><color=#FFD700>[ TOUCHDOWN BERHASIL · KUBAH TERSEGEL ]</color></b>\n<size=70%>ATAP OBSERVATORIUM DITUTUP // DUTA BESAR MANUSIA MENYAMBUT ANDA</size>", new Color(1f, 0.85f, 0.3f));

        yield return new WaitForSeconds(3.0f);

        CurrentState = SequenceState.ChamberExploration;
        UpdateHUD("<b>[ SELAMAT DATANG DI BUMI, DUTA XENOTERRA ]</b>\n<size=70%>Gunakan [WASD] untuk Berjalan, [Klik Kanan Mouse] untuk Melihat Sekitar\nSentuh Pod Sampel untuk Mensinkronkan Biosfer Bumi</size>", new Color(0.9f, 0.95f, 1f));
        Debug.Log("[UFODescentSequence] ✦ Descent completed, UFO ascended, Chamber roof sealed!");
    }

    private IEnumerator AscendUFO(Transform ufo, Vector3 targetPos, float duration)
    {
        Vector3 startPos = ufo.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            ufo.position = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
    }

    public void FastForwardToChamber()
    {
        StopAllCoroutines();
        CurrentState = SequenceState.ChamberExploration;
        if (reEntryPlasmaVFX != null) reEntryPlasmaVFX.Stop();
        if (cloudVFX != null) cloudVFX.Stop();
        if (spaceEarthObject != null) spaceEarthObject.SetActive(false);
        if (earthSkybox != null) RenderSettings.skybox = earthSkybox;
        RenderSettings.fog = false;

        if (tractorBeamVisual != null) tractorBeamVisual.SetActive(true);
        if (tractorSpotlight != null) tractorSpotlight.intensity = 2.5f;

        if (ufoCockpitBridge != null)
        {
            if (ufoMothership != null)
            {
                ufoCockpitBridge.transform.SetParent(ufoMothership.transform);
                ufoCockpitBridge.transform.localPosition = new Vector3(0, -1.5f, 0);
            }
            else
            {
                ufoCockpitBridge.SetActive(false);
            }
        }
        if (chamberRoofShutter != null) chamberRoofShutter.CloseShutter();

        if (humanAmbassadorHolo != null)
        {
            humanAmbassadorHolo.SetActive(true);
        }

        if (playerRig != null)
        {
            Vector3 targetP = (chamberSpawnPoint != null) ? chamberSpawnPoint.position : touchdownPosition;
            Quaternion targetR = (chamberSpawnPoint != null) ? chamberSpawnPoint.rotation : Quaternion.identity;
            playerRig.SetParent(null);
            playerRig.position = targetP;
            playerRig.rotation = targetR;
        }

        UpdateHUD("<b>[ SELAMAT DATANG DI BUMI ]</b>\n<size=70%>Sentuh Pod Sampel untuk Mengubah Dunia Luar (Air, Kristal, Tanaman)\n[ WASD / Klik Kanan Mouse: Jalan — Klik Kiri / Spasi: Interaksi ]</size>", new Color(0.9f, 0.95f, 1f));
        Debug.Log("[UFODescentSequence] Fast-forwarded to ChamberExploration mode.");
    }

    private void UpdateHUD(string text, Color color)
    {
        // User requested 100% removal of all HUD text sentences to ensure a pure cinematic flight experience
        if (hudStatusText != null)
        {
            hudStatusText.text = "";
        }
    }

    private void PlaySound(AudioClip clip, float volume, bool loop)
    {
        if (sequenceAudioSource != null && clip != null)
        {
            sequenceAudioSource.clip = clip;
            sequenceAudioSource.volume = volume;
            sequenceAudioSource.loop = loop;
            sequenceAudioSource.Play();
        }
    }
}
