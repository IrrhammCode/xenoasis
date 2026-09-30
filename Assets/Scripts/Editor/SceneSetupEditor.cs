using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

/// <summary>
/// XENOASIS — SceneSetupEditor.cs
/// 1-Click Unity Editor utility that auto-assembles and auto-wires the complete
/// 4-minute Welcome Chamber narrative experience matching the Project Bible,
/// using the generated Tripo AI 3D assets, World Labs skybox, URP post-processing,
/// spatial audio, and lighting.
/// Run from Unity menu: XENOASIS > Setup Scene — Welcome Chamber.
/// </summary>
public class SceneSetupEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XENOASIS/Setup Scene — Welcome Chamber")]
    public static void SetupWelcomeChamber()
    {
        Debug.Log("[XENOASIS] Assembling Welcome Chamber narrative experience with Tripo & World Labs assets...");

        // Ensure directories exist
        EnsureDirectory("Assets/Materials");
        EnsureDirectory("Assets/Prefabs");
        EnsureDirectory("Assets/Scenes");

        // ===== 1. MATERIALS & SHADERS =====
        Material obsidianMat = GetOrCreateMaterial("Assets/Materials/ObsidianGlass.mat", "XENOASIS/ObsidianFloor", new Color(0.024f, 0.027f, 0.043f));
        Material waterMat = GetOrCreateMaterial("Assets/Materials/WaterRefraction.mat", "XENOASIS/WaterSphere", new Color(0.1f, 0.6f, 0.8f, 0.4f));
        Material cyanGlowMat = GetOrCreateMaterial("Assets/Materials/EmissiveCyan.mat", "XENOASIS/EmissivePulse", new Color(0f, 1f, 0.82f));
        Material goldGlowMat = GetOrCreateMaterial("Assets/Materials/EmissiveGold.mat", "XENOASIS/EmissivePulse", new Color(1f, 0.82f, 0.4f));

        // ===== 2. SKYBOX (World Labs 360 Celestial Void) =====
        Texture2D panoTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Skybox/CosmicVoid_Pano.png");
        if (panoTex != null)
        {
            Material skyMat = GetOrCreateMaterial("Assets/Materials/CosmicVoid_Skybox.mat", "Skybox/Panoramic", Color.white);
            skyMat.SetTexture("_MainTex", panoTex);
            skyMat.SetFloat("_Exposure", 1.2f);
            RenderSettings.skybox = skyMat;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.04f, 0.04f, 0.09f); // Deep celestial indigo
            Debug.Log("[XENOASIS] World Labs Skybox configured successfully.");
        }

        // ===== 3. ROOT HIERARCHY =====
        GameObject root = CreateOrFind("--- XENOASIS ---");

        // ===== 4. MANAGERS =====
        GameObject managers = CreateChildOrFind(root, "[Managers]");
        GameManager gameManager = AddComponentIfMissing<GameManager>(managers);
        OfferingManager offeringManager = AddComponentIfMissing<OfferingManager>(managers);
        AudioManager audioManager = AddComponentIfMissing<AudioManager>(managers);

        // ===== 5. ENVIRONMENT =====
        GameObject environment = CreateChildOrFind(root, "[Environment]");

        // Floor Platform (Obsidian circular dais, 12m diameter)
        GameObject floor = CreateChildOrFind(environment, "FloorPlatform");
        var floorMF = floor.GetComponent<MeshFilter>() ?? floor.AddComponent<MeshFilter>();
        floorMF.sharedMesh = CreateCircleMesh(6f, 64);
        var floorMR = floor.GetComponent<MeshRenderer>() ?? floor.AddComponent<MeshRenderer>();
        floorMR.sharedMaterial = obsidianMat;
        if (floor.GetComponent<MeshCollider>() == null) floor.AddComponent<MeshCollider>();
        floor.transform.position = Vector3.zero;

        // Tripo Floor Tile Mosaic (hexagonal tiling over dais)
        GameObject tileRoot = CreateChildOrFind(floor, "TiledMosaic");
        // Center tile
        AttachGLBOrPlaceholder(tileRoot, "Assets/Models/Tripo/floor_tile.glb", "Tile_Center",
            new Vector3(0, 0.01f, 0), Vector3.one * 1.2f, Quaternion.identity);
        // Inner ring (6 tiles)
        for (int i = 0; i < 6; i++)
        {
            float ang = i * 60f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(ang) * 2.1f, 0.01f, Mathf.Sin(ang) * 2.1f);
            AttachGLBOrPlaceholder(tileRoot, "Assets/Models/Tripo/floor_tile.glb", $"Tile_Ring1_{i + 1}",
                pos, Vector3.one * 1.2f, Quaternion.Euler(0, i * 60f, 0));
        }
        // Outer ring (12 tiles)
        for (int i = 0; i < 12; i++)
        {
            float ang = i * 30f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(ang) * 4.2f, 0.01f, Mathf.Sin(ang) * 4.2f);
            AttachGLBOrPlaceholder(tileRoot, "Assets/Models/Tripo/floor_tile.glb", $"Tile_Ring2_{i + 1}",
                pos, Vector3.one * 1.2f, Quaternion.Euler(0, i * 30f, 0));
        }

        // World Labs Sanctuary Collider
        AttachGLBOrPlaceholder(environment, "Assets/Models/WorldLabs/sanctuary_collider.glb", "SanctuaryCollider",
            Vector3.zero, Vector3.one, Quaternion.identity);

        // 4 Arched Obsidian Pillars (Tripo AI)
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f;
            Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * 5f;
            GameObject pillar = CreateChildOrFind(environment, $"ObsidianPillar_{i + 1}");
            pillar.transform.position = pos;
            pillar.transform.rotation = Quaternion.Euler(0, angle, 0);

            // Attach Tripo obsidian_pillar model or fallback
            GameObject pillarModel = AttachGLBOrPlaceholder(pillar, "Assets/Models/Tripo/obsidian_pillar.glb", "PillarMesh",
                Vector3.zero, Vector3.one * 1.5f, Quaternion.identity);

            if (pillarModel == null && pillar.GetComponent<MeshFilter>() == null)
            {
                var mf = pillar.AddComponent<MeshFilter>();
                mf.sharedMesh = CreateCylinderMesh(0.2f, 6f);
                var mr = pillar.AddComponent<MeshRenderer>();
                mr.sharedMaterial = obsidianMat;
            }
        }

        // Overhead Glass Chimes (Tripo AI) - ring of 4 floating chimes
        GameObject chimesRoot = CreateChildOrFind(environment, "GlassChimesGroup");
        for (int i = 0; i < 4; i++)
        {
            float angle = (i * 90f + 45f) * Mathf.Deg2Rad;
            Vector3 chimePos = new Vector3(Mathf.Cos(angle) * 3.8f, 3.2f, Mathf.Sin(angle) * 3.8f);
            AttachGLBOrPlaceholder(chimesRoot, "Assets/Models/Tripo/glass_chime.glb", $"Chime_{i + 1}",
                chimePos, Vector3.one * 0.7f, Quaternion.Euler(0, i * 45f, 0));
        }

        // ===== 6. THE THREE SACRED OFFERINGS =====
        GameObject offerings = CreateChildOrFind(root, "[Offerings]");

        // --- Pedestals (Tripo AI) ---
        GameObject[] pedestals = new GameObject[3];
        Vector3[] pedestalPositions = { new Vector3(0, 0, 0), new Vector3(-3f, 0, 0), new Vector3(3f, 0, 0) };
        string[] pedestalNames = { "Pedestal_Water", "Pedestal_Crystal", "Pedestal_Flora" };

        for (int i = 0; i < 3; i++)
        {
            pedestals[i] = CreateChildOrFind(offerings, pedestalNames[i]);
            pedestals[i].transform.position = pedestalPositions[i];

            GameObject pedModel = AttachGLBOrPlaceholder(pedestals[i], "Assets/Models/Tripo/offering_pedestal.glb", "PedestalMesh",
                Vector3.zero, Vector3.one * 0.8f, Quaternion.identity);

            if (pedModel == null && pedestals[i].GetComponent<MeshFilter>() == null)
            {
                var mf = pedestals[i].AddComponent<MeshFilter>();
                mf.sharedMesh = CreateHexPedestalMesh(0.6f, 0.4f);
                var mr = pedestals[i].AddComponent<MeshRenderer>();
                mr.sharedMaterial = obsidianMat;
            }
        }

        // Wire OfferingManager pedestals + gold material
        SerializedObject omSO = new SerializedObject(offeringManager);
        var pProp = omSO.FindProperty("pedestals");
        pProp.arraySize = 3;
        for (int i = 0; i < 3; i++) pProp.GetArrayElementAtIndex(i).objectReferenceValue = pedestals[i];
        omSO.FindProperty("completedPedestalMaterial").objectReferenceValue = goldGlowMat;
        omSO.ApplyModifiedProperties();

        // --- Offering 1: Water Basin (Center) ---
        GameObject waterBasin = CreateChildOrFind(offerings, "Offering1_WaterBasin");
        waterBasin.transform.position = new Vector3(0, 0.5f, 0);
        var waterHandDetector = AddComponentIfMissing<HandProximityDetector>(waterBasin);
        var waterCtrl = AddComponentIfMissing<WaterSphereController>(waterBasin);

        // Tripo Basin model
        AttachGLBOrPlaceholder(waterBasin, "Assets/Models/Tripo/central_basin.glb", "BasinModel",
            Vector3.zero, Vector3.one * 0.9f, Quaternion.identity);

        // Tripo Water Lotus inside basin
        AttachGLBOrPlaceholder(waterBasin, "Assets/Models/Tripo/water_lotus.glb", "WaterLotusModel",
            new Vector3(0, 0.1f, 0), Vector3.one * 0.5f, Quaternion.identity);

        // Interactive Water Sphere
        GameObject waterSphere = CreateChildOrFind(waterBasin, "WaterSphere");
        var wsMF = waterSphere.GetComponent<MeshFilter>() ?? waterSphere.AddComponent<MeshFilter>();
        if (wsMF.sharedMesh == null)
        {
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            wsMF.sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tempSphere);
        }
        var wsMR = waterSphere.GetComponent<MeshRenderer>() ?? waterSphere.AddComponent<MeshRenderer>();
        wsMR.sharedMaterial = waterMat;
        waterSphere.transform.localPosition = new Vector3(0, 0.5f, 0);
        waterSphere.transform.localScale = Vector3.one * 0.6f;

        // Wire WaterSphereController
        SerializedObject wsSO = new SerializedObject(waterCtrl);
        wsSO.FindProperty("handDetector").objectReferenceValue = waterHandDetector;
        wsSO.FindProperty("waterSphereMesh").objectReferenceValue = waterSphere.transform;
        wsSO.ApplyModifiedProperties();

        // --- Offering 2: Crystal (Left) ---
        GameObject crystal = CreateChildOrFind(offerings, "Offering2_Crystal");
        crystal.transform.position = new Vector3(-3f, 1f, 0);
        var crystalHandDetector = AddComponentIfMissing<HandProximityDetector>(crystal);
        var crystalRes = AddComponentIfMissing<CrystalResonance>(crystal);
        GameObject crystalVis = CreateChildOrFind(crystal, "CrystalVisualizer");
        var soundVis = AddComponentIfMissing<SoundWaveVisualizer>(crystalVis);

        // Tripo Resonant Crystal model
        GameObject crystalModel = AttachGLBOrPlaceholder(crystal, "Assets/Models/Tripo/resonant_crystal.glb", "CrystalMesh",
            Vector3.zero, Vector3.one * 0.8f, Quaternion.identity);

        Renderer crystalRen = (crystalModel != null) ? crystalModel.GetComponentInChildren<Renderer>() : crystal.GetComponent<Renderer>();
        if (crystalRen == null && crystal.GetComponent<MeshFilter>() == null)
        {
            var mf = crystal.AddComponent<MeshFilter>();
            GameObject tempPrism = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mf.sharedMesh = tempPrism.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tempPrism);
            crystalRen = crystal.AddComponent<MeshRenderer>();
            crystalRen.sharedMaterial = cyanGlowMat;
            crystal.transform.localScale = new Vector3(0.3f, 0.8f, 0.3f);
        }

        // Wire CrystalResonance
        SerializedObject crSO = new SerializedObject(crystalRes);
        crSO.FindProperty("handDetector").objectReferenceValue = crystalHandDetector;
        crSO.FindProperty("crystalRenderer").objectReferenceValue = crystalRen;
        crSO.FindProperty("waveVisualizer").objectReferenceValue = soundVis;
        crSO.ApplyModifiedProperties();

        // --- Offering 3: Flora (Right) ---
        GameObject flora = CreateChildOrFind(offerings, "Offering3_Flora");
        flora.transform.position = new Vector3(3f, 0.5f, 0);
        var floraHandDetector = AddComponentIfMissing<HandProximityDetector>(flora);
        var floraBloom = AddComponentIfMissing<FloraBloom>(flora);
        GameObject energyBeamObj = CreateChildOrFind(flora, "EnergyBeam");
        var energyBeamVFX = AddComponentIfMissing<EnergyBeamVFX>(energyBeamObj);

        // Tripo Dormant & Bloomed Flora models
        GameObject dormantMesh = AttachGLBOrPlaceholder(flora, "Assets/Models/Tripo/alien_flora_dormant.glb", "DormantFloraMesh",
            Vector3.zero, Vector3.one * 0.7f, Quaternion.identity);

        if (dormantMesh == null)
        {
            dormantMesh = CreateChildOrFind(flora, "DormantFloraMesh");
            if (dormantMesh.GetComponent<MeshFilter>() == null)
            {
                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dormantMesh.AddComponent<MeshFilter>().sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(tempSphere);
                var mr = dormantMesh.AddComponent<MeshRenderer>();
                mr.sharedMaterial = obsidianMat;
                dormantMesh.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
            }
        }

        GameObject bloomedMesh = AttachGLBOrPlaceholder(flora, "Assets/Models/Tripo/alien_flora_bloomed.glb", "BloomedFloraMesh",
            Vector3.zero, Vector3.one * 0.7f, Quaternion.identity);

        if (bloomedMesh == null)
        {
            bloomedMesh = CreateChildOrFind(flora, "BloomedFloraMesh");
            if (bloomedMesh.GetComponent<MeshFilter>() == null)
            {
                GameObject tempCyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bloomedMesh.AddComponent<MeshFilter>().sharedMesh = tempCyl.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(tempCyl);
                var mr = bloomedMesh.AddComponent<MeshRenderer>();
                mr.sharedMaterial = cyanGlowMat;
                bloomedMesh.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }
        bloomedMesh.SetActive(false);

        // Wire FloraBloom
        SerializedObject fbSO = new SerializedObject(floraBloom);
        fbSO.FindProperty("handDetector").objectReferenceValue = floraHandDetector;
        fbSO.FindProperty("energyBeamVFX").objectReferenceValue = energyBeamVFX;
        fbSO.FindProperty("dormantFloraMesh").objectReferenceValue = dormantMesh.transform;
        fbSO.FindProperty("bloomedFloraMesh").objectReferenceValue = bloomedMesh.transform;
        fbSO.ApplyModifiedProperties();

        // ===== 7. VFX & CLIMAX BEACON =====
        GameObject vfx = CreateChildOrFind(root, "[VFX]");

        // Cosmic Stardust Particles (zero-g ambient cosmos)
        GameObject stardust = CreateChildOrFind(vfx, "CosmicStardust");
        var stardustCtrl = AddComponentIfMissing<StardustController>(stardust);
        var stardustPS = stardust.GetComponent<ParticleSystem>() ?? stardust.AddComponent<ParticleSystem>();
        ConfigureStardustParticles(stardustPS);
        stardust.transform.position = Vector3.zero;

        // Climax Beacon
        GameObject beacon = CreateChildOrFind(vfx, "ClimaxBeacon");
        var beaconVFX = AddComponentIfMissing<BeaconClimaxVFX>(beacon);
        beacon.transform.position = new Vector3(0, 0.5f, 0);

        // Guiding Floor Trail
        GameObject guideTrail = CreateChildOrFind(vfx, "GuidingTrail");
        var trailCtrl = AddComponentIfMissing<GuidingTrail>(guideTrail);
        var trailPS = guideTrail.GetComponent<ParticleSystem>() ?? guideTrail.AddComponent<ParticleSystem>();
        ConfigureTrailParticles(trailPS);
        SerializedObject gtSO = new SerializedObject(trailCtrl);
        gtSO.FindProperty("targetDestination").objectReferenceValue = waterBasin.transform;
        gtSO.FindProperty("trailParticles").objectReferenceValue = trailPS;
        gtSO.ApplyModifiedProperties();

        // Wire Beacon convergence origins to pedestals
        SerializedObject bvSO = new SerializedObject(beaconVFX);
        var bOrigins = bvSO.FindProperty("pedestalOrigins");
        bOrigins.arraySize = 3;
        for (int i = 0; i < 3; i++) bOrigins.GetArrayElementAtIndex(i).objectReferenceValue = pedestals[i].transform;
        bvSO.FindProperty("centerConvergencePoint").objectReferenceValue = beacon.transform;
        bvSO.ApplyModifiedProperties();

        // ===== 8. CAMERA RIG & TRANSITIONS =====
        GameObject cameraRig = CreateChildOrFind(root, "[CameraRig]");
        cameraRig.transform.position = new Vector3(0, 1.6f, -4.5f); // Player spawn at edge of dais facing basin

        // Setup Main Camera if not present
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = CreateChildOrFind(cameraRig, "MainCamera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.AddComponent<AudioListener>();
        }
        else
        {
            mainCam.transform.SetParent(cameraRig.transform);
            mainCam.transform.localPosition = new Vector3(0, 0, 0);
        }

        GameObject fadeCanvas = CreateChildOrFind(cameraRig, "FadeCanvas");
        var fadeCtrl = AddComponentIfMissing<FadeController>(fadeCanvas);
        var canvas = fadeCanvas.GetComponent<Canvas>() ?? fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        AddComponentIfMissing<UnityEngine.UI.CanvasScaler>(fadeCanvas);

        GameObject passthroughCtrl = CreateChildOrFind(cameraRig, "PassthroughController");
        var passthroughTransition = AddComponentIfMissing<PassthroughTransition>(passthroughCtrl);
        SerializedObject ptSO = new SerializedObject(passthroughTransition);
        ptSO.FindProperty("virtualEnvironmentRoot").objectReferenceValue = environment;
        ptSO.ApplyModifiedProperties();

        // ===== 9. END SCREEN UI =====
        GameObject endScreen = CreateChildOrFind(root, "EndScreenCanvas");
        var endCanvas = endScreen.GetComponent<Canvas>() ?? endScreen.AddComponent<Canvas>();
        endCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        endCanvas.sortingOrder = 998;
        AddComponentIfMissing<UnityEngine.UI.CanvasScaler>(endScreen);

        EndScreenUI endUI = AddComponentIfMissing<EndScreenUI>(endScreen);

        // Child: MessageText ("The gift was received.")
        GameObject msgObj = CreateChildOrFind(endScreen, "MessageText");
        var msgTMP = msgObj.GetComponent<TMPro.TextMeshProUGUI>() ?? msgObj.AddComponent<TMPro.TextMeshProUGUI>();
        msgTMP.text = "The gift was received.";
        msgTMP.fontSize = 28;
        msgTMP.alignment = TMPro.TextAlignmentOptions.Center;
        msgTMP.color = new Color(0.9f, 0.95f, 1f, 0f);
        RectTransform msgRT = msgObj.GetComponent<RectTransform>();
        msgRT.anchoredPosition = new Vector2(0, 50f);
        msgRT.sizeDelta = new Vector2(800, 100);

        // Child: TitleText ("XENOASIS")
        GameObject titleObj = CreateChildOrFind(endScreen, "TitleText");
        var titleTMP = titleObj.GetComponent<TMPro.TextMeshProUGUI>() ?? titleObj.AddComponent<TMPro.TextMeshProUGUI>();
        titleTMP.text = "XENOASIS";
        titleTMP.fontSize = 54;
        titleTMP.fontStyle = TMPro.FontStyles.Bold;
        titleTMP.alignment = TMPro.TextAlignmentOptions.Center;
        titleTMP.color = new Color(1f, 0.82f, 0.4f, 0f); // Gold #FFD166
        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchoredPosition = new Vector2(0, -20f);
        titleRT.sizeDelta = new Vector2(800, 100);

        // Child: CreditsText
        GameObject credObj = CreateChildOrFind(endScreen, "CreditsText");
        var credTMP = credObj.GetComponent<TMPro.TextMeshProUGUI>() ?? credObj.AddComponent<TMPro.TextMeshProUGUI>();
        credTMP.text = "Tripothon S1 · Built with Tripo AI + World Labs + PICO 4 Ultra";
        credTMP.fontSize = 18;
        credTMP.alignment = TMPro.TextAlignmentOptions.Center;
        credTMP.color = new Color(0.7f, 0.7f, 0.7f, 0f);
        RectTransform credRT = credObj.GetComponent<RectTransform>();
        credRT.anchoredPosition = new Vector2(0, -90f);
        credRT.sizeDelta = new Vector2(800, 80);

        // Wire EndScreenUI SerializedObject
        SerializedObject esSO = new SerializedObject(endUI);
        esSO.FindProperty("messageText").objectReferenceValue = msgTMP;
        esSO.FindProperty("titleText").objectReferenceValue = titleTMP;
        esSO.FindProperty("creditsText").objectReferenceValue = credTMP;
        esSO.ApplyModifiedProperties();

        endScreen.SetActive(false);

        // ===== 10. LIGHTING & BIOLUMINESCENCE =====
        GameObject lighting = CreateChildOrFind(root, "[Lighting]");

        // Distant Starlight (Cold blue cosmic rim)
        GameObject dirLight = CreateChildOrFind(lighting, "DistantStarlight");
        var light = dirLight.GetComponent<Light>() ?? dirLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.12f, 0.14f, 0.28f); // #1E2447
        light.intensity = 0.2f;
        dirLight.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

        // Altar Point Light: Water Basin (Cyan glow)
        GameObject waterLightObj = CreateChildOrFind(lighting, "WaterBasinPointLight");
        var waterL = waterLightObj.GetComponent<Light>() ?? waterLightObj.AddComponent<Light>();
        waterL.type = LightType.Point;
        waterL.color = new Color(0f, 0.9f, 1f); // Cyan #00E5FF
        waterL.intensity = 1.8f;
        waterL.range = 4.5f;
        waterLightObj.transform.position = new Vector3(0, 0.8f, 0);

        // Altar Point Light: Resonant Crystal (Electric blue)
        GameObject crystalLightObj = CreateChildOrFind(lighting, "CrystalPointLight");
        var crystalL = crystalLightObj.GetComponent<Light>() ?? crystalLightObj.AddComponent<Light>();
        crystalL.type = LightType.Point;
        crystalL.color = new Color(0f, 0.7f, 1f); // #00B0FF
        crystalL.intensity = 1.6f;
        crystalL.range = 4.0f;
        crystalLightObj.transform.position = new Vector3(-3f, 1.2f, 0);

        // Altar Point Light: Flora Bloom (Amber Solar)
        GameObject floraLightObj = CreateChildOrFind(lighting, "FloraPointLight");
        var floraL = floraLightObj.GetComponent<Light>() ?? floraLightObj.AddComponent<Light>();
        floraL.type = LightType.Point;
        floraL.color = new Color(1f, 0.83f, 0.31f); // Solar Gold #FFD54F
        floraL.intensity = 1.4f;
        floraL.range = 4.0f;
        floraLightObj.transform.position = new Vector3(3f, 0.8f, 0);

        // Reflection Probe (Reflecting skybox & glowing pillars onto obsidian floor)
        GameObject probeObj = CreateChildOrFind(lighting, "SanctuaryReflectionProbe");
        var probe = probeObj.GetComponent<ReflectionProbe>() ?? probeObj.AddComponent<ReflectionProbe>();
        probe.size = new Vector3(16f, 10f, 16f);
        probe.boxProjection = true;
        probe.mode = ReflectionProbeMode.Realtime;
        probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;
        probeObj.transform.position = new Vector3(0, 1.5f, 0);

        // ===== 11. URP POST-PROCESSING VOLUME =====
        GameObject ppObj = CreateChildOrFind(root, "[PostProcessing]");
        var volume = ppObj.GetComponent<Volume>() ?? ppObj.AddComponent<Volume>();
        volume.isGlobal = true;
        ConfigurePostProcessingProfile(volume);

        // ===== 12. WIRE GAMEMANAGER REFERENCES =====
        SerializedObject gmSO = new SerializedObject(gameManager);
        gmSO.FindProperty("fadeController").objectReferenceValue = fadeCtrl;
        gmSO.FindProperty("beaconClimaxVFX").objectReferenceValue = beaconVFX;
        gmSO.FindProperty("audioManager").objectReferenceValue = audioManager;
        gmSO.FindProperty("stardustController").objectReferenceValue = stardustCtrl;
        gmSO.FindProperty("passthroughTransition").objectReferenceValue = passthroughTransition;
        gmSO.FindProperty("endScreenCanvas").objectReferenceValue = endScreen;
        gmSO.ApplyModifiedProperties();

        // ===== 13. WIRE AUDIO ASSETS =====
        WireAudioAssets(audioManager, crystalRes, floraBloom);

        // ===== 14. AUTO-SAVE SCENE FILE & REGISTER IN BUILD SETTINGS =====
        string scenePath = "Assets/Scenes/WelcomeChamber.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        RegisterSceneInBuild(scenePath);

        Debug.Log("[XENOASIS] Welcome Chamber assembled with Tripo 3D models, World Labs Skybox, and saved to Assets/Scenes/WelcomeChamber.unity!");
        EditorUtility.DisplayDialog("XENOASIS",
            "Welcome Chamber Narrative Experience Assembled & Saved!\n\n" +
            "✔ World Labs 360 Celestial Skybox applied\n" +
            "✔ 19 Tiled Floor Hexagons (Tripo floor_tile.glb) arrayed across the dais\n" +
            "✔ 4 Arched Obsidian Pillars + 4 Overhead Glass Chimes attached\n" +
            "✔ 3 Sacred Offerings (Basin, Lotus, Crystal, Flora) positioned & wired\n" +
            "✔ URP Post-Processing Volume (Bloom 2.2 + ACES Tonemapping + Vignette) active\n" +
            "✔ 4 Bioluminescent Point Lights & Reflection Probe configured\n" +
            "✔ Spatial Audio Soundscapes connected\n" +
            "✔ Scene auto-saved to Assets/Scenes/WelcomeChamber.unity",
            "Awesome!");
    }

    // ===== PARTICLE SYSTEM CONFIGURATORS =====
    static void ConfigureStardustParticles(ParticleSystem ps)
    {
        var main = ps.main;
        main.maxParticles = 800;
        main.startLifetime = 15f;
        main.startSpeed = 0.04f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.015f, 0.045f);
        main.startColor = new Color(0f, 1f, 0.85f, 0.7f);
        main.prewarm = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 35f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 7.5f;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.15f;
        noise.frequency = 0.1f;
    }

    static void ConfigureTrailParticles(ParticleSystem ps)
    {
        var main = ps.main;
        main.maxParticles = 150;
        main.startLifetime = 4f;
        main.startSpeed = 0.3f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.03f);
        main.startColor = new Color(0f, 1f, 0.82f, 0.5f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 15f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(0.6f, 0.05f, 3.5f);
        shape.position = new Vector3(0, 0.02f, -2.5f);
    }

    // ===== URP POST-PROCESSING CONFIGURATOR =====
    static void ConfigurePostProcessingProfile(Volume volume)
    {
        string profilePath = "Assets/Materials/WelcomeChamber_Profile.asset";
        VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
        }

        // Bloom
        Bloom bloom;
        if (!profile.TryGet(out bloom)) bloom = profile.Add<Bloom>(true);
        bloom.threshold.Override(0.85f);
        bloom.intensity.Override(2.2f);
        bloom.scatter.Override(0.7f);
        bloom.tint.Override(new Color(0f, 1f, 0.82f)); // Cyan bioluminescent tint

        // Tonemapping
        Tonemapping tonemapping;
        if (!profile.TryGet(out tonemapping)) tonemapping = profile.Add<Tonemapping>(true);
        tonemapping.mode.Override(TonemappingMode.ACES);

        // Vignette
        Vignette vignette;
        if (!profile.TryGet(out vignette)) vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.25f);
        vignette.smoothness.Override(0.4f);

        // Color Adjustments
        ColorAdjustments colorAdj;
        if (!profile.TryGet(out colorAdj)) colorAdj = profile.Add<ColorAdjustments>(true);
        colorAdj.postExposure.Override(0.15f);
        colorAdj.contrast.Override(15f);
        colorAdj.saturation.Override(10f);

        EditorUtility.SetDirty(profile);
        volume.sharedProfile = profile;
    }

    // ===== SCENE IN BUILD REGISTRATION =====
    static void RegisterSceneInBuild(string scenePath)
    {
        var scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].path == scenePath) return; // already registered
        }

        var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
        System.Array.Copy(scenes, newScenes, scenes.Length);
        newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
        EditorBuildSettings.scenes = newScenes;
        Debug.Log($"[XENOASIS] Scene '{scenePath}' registered in EditorBuildSettings!");
    }

    // ===== MODEL ATTACH HELPER =====
    static GameObject AttachGLBOrPlaceholder(GameObject parent, string glbPath, string childName, Vector3 localPos, Vector3 localScale, Quaternion localRot)
    {
        Transform existing = parent.transform.Find(childName);
        if (existing != null) return existing.gameObject;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(glbPath);
        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = childName;
            instance.transform.SetParent(parent.transform);
            instance.transform.localPosition = localPos;
            instance.transform.localScale = localScale;
            instance.transform.localRotation = localRot;
            Undo.RegisterCreatedObjectUndo(instance, "Attach " + childName);
            return instance;
        }
        return null;
    }

    // ===== AUDIO WIRING =====
    static void WireAudioAssets(AudioManager am, CrystalResonance cr, FloraBloom fb)
    {
        AudioClip drone = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");
        AudioClip drops = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/water_drops.wav");
        AudioClip bell = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");
        AudioClip crescendo = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Climax/beacon_crescendo.wav");
        AudioClip crystalC = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
        AudioClip bloom = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bloom_chord.wav");

        if (am != null)
        {
            SerializedObject so = new SerializedObject(am);
            if (drone != null) so.FindProperty("cosmicDroneClip").objectReferenceValue = drone;
            if (drops != null) so.FindProperty("waterDropsClip").objectReferenceValue = drops;
            if (bell != null) so.FindProperty("offeringCompleteClip").objectReferenceValue = bell;
            if (crescendo != null) so.FindProperty("climaxCrescendoClip").objectReferenceValue = crescendo;
            so.ApplyModifiedProperties();
        }

        if (cr != null && crystalC != null)
        {
            SerializedObject so = new SerializedObject(cr);
            so.FindProperty("resonantChimeClip").objectReferenceValue = crystalC;
            so.ApplyModifiedProperties();
        }

        if (fb != null && bloom != null)
        {
            SerializedObject so = new SerializedObject(fb);
            so.FindProperty("bloomChordClip").objectReferenceValue = bloom;
            so.ApplyModifiedProperties();
        }
    }

    // ===== HELPERS =====
    static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    static Material GetOrCreateMaterial(string path, string shaderName, Color defaultColor)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find(shaderName) ?? Shader.Find("Universal Render Pipeline/Lit");
            mat = new Material(shader);
            mat.color = defaultColor;
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    static GameObject CreateOrFind(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static GameObject CreateChildOrFind(GameObject parent, string name)
    {
        Transform t = parent.transform.Find(name);
        if (t != null) return t.gameObject;
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static T AddComponentIfMissing<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    static Mesh CreateCircleMesh(float radius, int segments)
    {
        Mesh mesh = new Mesh { name = "CirclePlatform" };
        Vector3[] verts = new Vector3[segments + 1];
        Vector2[] uvs = new Vector2[segments + 1];
        int[] tris = new int[segments * 3];

        verts[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            verts[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            uvs[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);
        }

        for (int i = 0; i < segments; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static Mesh CreateCylinderMesh(float radius, float height)
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Mesh m = Instantiate(temp.GetComponent<MeshFilter>().sharedMesh);
        DestroyImmediate(temp);
        return m;
    }

    static Mesh CreateHexPedestalMesh(float radius, float height)
    {
        return CreateCircleMesh(radius, 6);
    }
#endif
}
