using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

/// <summary>
/// XENOASIS — SceneSetupEditor.cs
/// 1-Click Unity Editor utility that auto-assembles and auto-wires the complete
/// cinematic First Contact experience matching the Project Bible & user requests:
/// 1. Orbital UFO Cockpit Bridge (Y=600) with photorealistic 8K Earth vista below & stars
/// 2. Pilot console with radar holo-disk, angled HUD, and canopy frame struts
/// 3. Atmospheric Re-entry Burn (camera shake & plasma particles)
/// 4. Cloud Penetration (dense mist rushing past)
/// 5. Approach & Docking at Terran Embassy Pad with retro-steam & clamp sounds
/// 6. Airlock Opening & Escort by Human Holographic Diplomatic Envoy
/// 7. The Grand Terran Embassy Chamber (Living Archive):
///    - Solid dark composite floor with illuminated cyan runway trim & central dais
///    - Alternating titanium wall panels with vertical accent glow lines
///    - Cantilevered ceiling with cyan architectural light ring & Tripo glass chimes
///    - Curved 180° Panoramic Viewport framed by titanium balustrade sill & header
///    - Ultra-HD Curved Vista Screen outside the window displaying razor-sharp 16:9 Earth biomes
///    - Centerpiece: Civilization & DNA Holo-Table (spinning Earth globe + gold DNA helix)
///    - 3 Interactive Living Sample Pods (Hydrosphere, Geosphere, Biosphere)
/// Run from Unity menu: XENOASIS > Setup Scene — Welcome Chamber.
/// </summary>
public class SceneSetupEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XENOASIS/Setup Scene - Welcome Chamber")]
    public static void SetupWelcomeChamber()
    {
        Debug.Log("[XENOASIS] Assembling the Grand Terran Embassy & Living Archive Experience...");

        EnsureURPConfigured();
        CockpitInfogramTextureGenerator.GenerateInfogramTextures();
        EnsureDirectory("Assets/Materials");
        EnsureDirectory("Assets/Prefabs");
        EnsureDirectory("Assets/Scenes");
        EnsureDirectory("Assets/Textures/Vistas");

        // ===== 1. MATERIALS & SHADERS =====
        Material scifiWallWhite = GetOrCreateMaterial("Assets/Materials/SciFiWall_White.mat", "XENOASIS/SciFiPanel", new Color(0.88f, 0.90f, 0.94f));
        scifiWallWhite.SetColor("_BaseColor", new Color(0.88f, 0.90f, 0.94f));
        scifiWallWhite.SetFloat("_Metallic", 0.3f);
        scifiWallWhite.SetFloat("_Smoothness", 0.85f);
        scifiWallWhite.SetColor("_StripeColor", new Color(0f, 0.85f, 1f, 1f));
        scifiWallWhite.SetFloat("_StripeEmission", 2.0f);
        scifiWallWhite.SetFloat("_StripeInterval", 2.2f);
        scifiWallWhite.SetFloat("_StripeWidth", 0.035f);
        EditorUtility.SetDirty(scifiWallWhite);

        Material scifiWallDark = GetOrCreateMaterial("Assets/Materials/SciFiWall_Dark.mat", "XENOASIS/SciFiPanel", new Color(0.05f, 0.06f, 0.08f));
        scifiWallDark.SetColor("_BaseColor", new Color(0.05f, 0.06f, 0.08f));
        scifiWallDark.SetFloat("_Metallic", 0.8f);
        scifiWallDark.SetFloat("_Smoothness", 0.9f);
        scifiWallDark.SetColor("_StripeColor", new Color(0f, 0.85f, 1f, 0.6f));
        scifiWallDark.SetFloat("_StripeEmission", 1.0f);
        scifiWallDark.SetFloat("_StripeInterval", 3.0f);
        scifiWallDark.SetFloat("_StripeWidth", 0.025f);
        EditorUtility.SetDirty(scifiWallDark);

        // Luxury Museum Floor (Pearl-White Terrazzo / Titanium matching museum building architecture)
        Material museumFloorMat = GetOrCreateMaterial("Assets/Materials/Museum_LuxuryFloor.mat", "XENOASIS/SciFiPanel", new Color(0.88f, 0.90f, 0.94f));
        museumFloorMat.SetColor("_BaseColor", new Color(0.88f, 0.90f, 0.94f));
        museumFloorMat.SetFloat("_Metallic", 0.15f);
        museumFloorMat.SetFloat("_Smoothness", 0.92f);
        museumFloorMat.SetColor("_StripeColor", new Color(0f, 0.85f, 1f, 0.5f));
        museumFloorMat.SetFloat("_StripeEmission", 0.8f);
        museumFloorMat.SetFloat("_StripeInterval", 2.5f);
        museumFloorMat.SetFloat("_StripeWidth", 0.02f);
        EditorUtility.SetDirty(museumFloorMat);

        // Sleek Central Landing Dais Plinth (Dark titanium with cyan inlays)
        Material museumDaisMat = GetOrCreateMaterial("Assets/Materials/Museum_DaisPlinth.mat", "XENOASIS/SciFiPanel", new Color(0.12f, 0.14f, 0.18f));
        museumDaisMat.SetColor("_BaseColor", new Color(0.12f, 0.14f, 0.18f));
        museumDaisMat.SetFloat("_Metallic", 0.6f);
        museumDaisMat.SetFloat("_Smoothness", 0.88f);
        museumDaisMat.SetColor("_StripeColor", new Color(0f, 0.85f, 1f, 0.6f));
        museumDaisMat.SetFloat("_StripeEmission", 1.0f);
        museumDaisMat.SetFloat("_StripeInterval", 2.0f);
        museumDaisMat.SetFloat("_StripeWidth", 0.02f);
        EditorUtility.SetDirty(museumDaisMat);

        Material scifiFloorMat = GetOrCreateMaterial("Assets/Materials/SciFiFloor.mat", "XENOASIS/SciFiPanel", new Color(0.04f, 0.05f, 0.07f));
        scifiFloorMat.SetColor("_BaseColor", new Color(0.04f, 0.05f, 0.07f));
        scifiFloorMat.SetFloat("_Metallic", 0.5f);
        scifiFloorMat.SetFloat("_Smoothness", 0.92f);
        scifiFloorMat.SetColor("_StripeColor", new Color(0f, 0.9f, 0.85f, 1f));
        scifiFloorMat.SetFloat("_StripeEmission", 1.8f);
        scifiFloorMat.SetFloat("_StripeInterval", 1.8f);
        scifiFloorMat.SetFloat("_StripeWidth", 0.03f);
        scifiFloorMat.SetFloat("_HorizontalStripes", 1.0f);
        EditorUtility.SetDirty(scifiFloorMat);

        Material glassViewportMat = GetOrCreateMaterial("Assets/Materials/GlassViewport.mat", "XENOASIS/GlassDome", new Color(0.01f, 0.02f, 0.04f, 0.02f));
        glassViewportMat.SetFloat("_FresnelPower", 3.5f);
        glassViewportMat.SetColor("_FresnelColor", new Color(0f, 0.85f, 1f, 0.15f));
        glassViewportMat.SetFloat("_GridScale", 4.0f);
        glassViewportMat.SetFloat("_GridLineWidth", 0.008f);
        glassViewportMat.SetFloat("_GridEmission", 0.18f);
        glassViewportMat.SetColor("_GridColor", new Color(0f, 1f, 0.85f, 0.18f));
        glassViewportMat.SetFloat("_Surface", 1.0f);
        glassViewportMat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        glassViewportMat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        glassViewportMat.SetInt("_ZWrite", 0);
        glassViewportMat.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(glassViewportMat);

        Material holoCyanMat = GetOrCreateMaterial("Assets/Materials/Holo_Cyan.mat", "XENOASIS/HologramDisplay", new Color(0f, 0.85f, 1f, 0.75f));
        Material holoGoldMat = GetOrCreateMaterial("Assets/Materials/Holo_Gold.mat", "XENOASIS/HologramDisplay", new Color(1f, 0.85f, 0.35f, 0.75f));
        holoGoldMat.SetColor("_RimColor", new Color(1f, 0.95f, 0.6f, 1f));

        Material waterMat = GetOrCreateMaterial("Assets/Materials/WaterRefraction.mat", "XENOASIS/WaterSphere", new Color(0.1f, 0.65f, 0.85f, 0.35f));
        Material cyanGlowMat = GetOrCreateMaterial("Assets/Materials/EmissiveCyan.mat", "XENOASIS/EmissivePulse", new Color(0f, 1f, 0.85f));
        Material goldGlowMat = GetOrCreateMaterial("Assets/Materials/EmissiveGold.mat", "XENOASIS/EmissivePulse", new Color(1f, 0.82f, 0.4f));
        Material particleMat = GetOrCreateParticleMaterial("Assets/Materials/Particle_Stardust.mat");
        Material cockpitHullDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat") ?? scifiWallDark;
        Material cockpitHullWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullWhite.mat") ?? scifiWallWhite;

        // Vista Screen Materials
        Material curvedVistaMat = GetOrCreateMaterial("Assets/Materials/CurvedVista_Screen.mat", "XENOASIS/PanoramicVistaBlend", Color.white);
        Material orbitalCanopyMat = GetOrCreateMaterial("Assets/Materials/OrbitalCanopy_Screen.mat", "XENOASIS/PanoramicVistaBlend", Color.white);

        // ===== 2. VISTA TEXTURES (Ultra-HD 16:9 Widescreen) =====
        Texture2D orbitVistaTex = LoadAndConfigureVistaTexture("Assets/Textures/Vistas/OrbitEarth_HD.jpg");
        Texture2D lakeVistaTex = LoadAndConfigureVistaTexture("Assets/Textures/Vistas/Lake_HD.jpg");
        Texture2D mountainVistaTex = LoadAndConfigureVistaTexture("Assets/Textures/Vistas/Mountain_HD.jpg");
        Texture2D forestVistaTex = LoadAndConfigureVistaTexture("Assets/Textures/Vistas/Forest_HD.jpg");

        if (orbitVistaTex != null)
        {
            orbitalCanopyMat.SetTexture("_MainTex", orbitVistaTex);
            orbitalCanopyMat.SetFloat("_Exposure", 1.1f);
        }

        if (lakeVistaTex != null)
        {
            curvedVistaMat.SetTexture("_MainTex", lakeVistaTex);
            curvedVistaMat.SetFloat("_Exposure", 1.0f);
        }

        // Skybox Panoramas
        string biomePath = "Assets/Textures/Skybox/Biomes/";
        string[] biomeNames = { "NightSky_UFO", "MirrorLake", "MountainPeak", "PrimevalForest" };
        Material[] biomeSkyboxMats = new Material[4];

        for (int i = 0; i < biomeNames.Length; i++)
        {
            string panoPath = biomePath + biomeNames[i] + ".jpg";
            TextureImporter ti = AssetImporter.GetAtPath(panoPath) as TextureImporter;
            if (ti != null)
            {
                ti.maxTextureSize = 4096;
                ti.wrapMode = TextureWrapMode.Clamp;
                ti.textureCompression = TextureImporterCompression.CompressedHQ;
                ti.anisoLevel = 16;
                ti.filterMode = FilterMode.Trilinear;
                ti.SaveAndReimport();
            }

            Texture2D panoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(panoPath);
            Material skyMat = GetOrCreateMaterial($"Assets/Materials/Skybox_{biomeNames[i]}.mat", "Skybox/Panoramic", Color.white);
            if (panoTex != null)
            {
                skyMat.SetTexture("_MainTex", panoTex);
                skyMat.SetFloat("_Exposure", 1.0f);
            }
            biomeSkyboxMats[i] = skyMat;
        }

        Material earthDefaultSkybox = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_WorldLabs_TerranEmbassy.mat") ?? biomeSkyboxMats[0];
        if (earthDefaultSkybox != null) RenderSettings.skybox = earthDefaultSkybox;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.65f, 0.72f, 0.80f);
        RenderSettings.fog = false;

        // ===== 3. ROOT HIERARCHY CLEANUP =====
        GameObject oldRoot = GameObject.Find("--- XENOASIS ---");
        if (oldRoot != null) DestroyImmediate(oldRoot);
        GameObject oldLight = GameObject.Find("Directional Light");
        if (oldLight != null && oldLight.transform.parent == null) DestroyImmediate(oldLight);
        GameObject oldRootEarth = GameObject.Find("PlanetEarth_SpaceVista");
        if (oldRootEarth != null && oldRootEarth.transform.parent == null) DestroyImmediate(oldRootEarth);
        GameObject oldRootGlobe = GameObject.Find("PlanetEarth_Globe3D");
        if (oldRootGlobe != null && oldRootGlobe.transform.parent == null) DestroyImmediate(oldRootGlobe);

        GameObject root = new GameObject("--- XENOASIS ---");
        Undo.RegisterCreatedObjectUndo(root, "Create --- XENOASIS ---");

        // ===== 4. MANAGERS =====
        GameObject managers = CreateChildOrFind(root, "[Managers]");
        GameManager gameManager = AddComponentIfMissing<GameManager>(managers);
        OfferingManager offeringManager = AddComponentIfMissing<OfferingManager>(managers);
        AudioManager audioManager = AddComponentIfMissing<AudioManager>(managers);
        BiomeTransitionController biomeCtrl = AddComponentIfMissing<BiomeTransitionController>(managers);
        UFODescentSequence ufoDescent = AddComponentIfMissing<UFODescentSequence>(managers);

        // ===== 5. ENVIRONMENT =====
        GameObject environment = CreateChildOrFind(root, "[Environment]");

        // --- 5A. THE GRAND TERRAN EMBASSY CHAMBER (Living Archive) ---
        GameObject chamber = CreateChildOrFind(environment, "TerranLivingArchive");
        chamber.transform.position = Vector3.zero;

        // Clean up legacy primitive objects and old drum architecture to guarantee zero clutter
        string[] obsoleteChamberParts = {
            "ChamberFloor", "ChamberFoundationSubfloor", "CenterTableDais", "CenterTableDaisRim",
            "RunwayStrip_L", "RunwayStrip_R", "RearWalls", "ChamberCeiling", "CeilingLightRing",
            "PanoramicViewport", "DockingAirlock", "TerranEmbassy_Architecture"
        };
        foreach (var obs in obsoleteChamberParts)
        {
            Transform t = chamber.transform.Find(obs);
            if (t != null) DestroyImmediate(t.gameObject);
        }
        Transform oldVista = environment.transform.Find("CurvedUltraHDVistaScreen");
        if (oldVista != null) DestroyImmediate(oldVista.gameObject);

        // --- 5A. GRAND 44-METER MONUMENTAL MUSEUM PAVILION ARCHITECTURE ---
        // Expansive solid 3D manifold architecture: 44m diameter, tiered foundation anchored to lake bed,
        // 16 monumental titanium columns, 4 panoramic cardinal viewports looking out to Earth biomes,
        // 12 solid 35cm-thick gallery walls with cyan conduits, sweeping exterior aerodynamic buttresses,
        // solid 3D roof canopy slab (r=5.5m to 20.8m), dual glowing cyan skylight bezels,
        // and grand geodesic crystal glass dome crowning the central aperture.
        Transform oldArch = chamber.transform.Find("MuseumOfHumanity_Architecture");
        if (oldArch != null) DestroyImmediate(oldArch.gameObject);
        Transform oldPav = chamber.transform.Find("GrandPavilion_Architecture");
        if (oldPav != null) DestroyImmediate(oldPav.gameObject);
        Transform oldRotunda = chamber.transform.Find("Museum_Interior_Rotunda");
        if (oldRotunda != null) DestroyImmediate(oldRotunda.gameObject);

        BuildGrandMuseumPavilion(chamber, scifiWallDark, scifiWallWhite, museumDaisMat, museumFloorMat, glassViewportMat, cyanGlowMat);

        // Central Dome Skylight Daylight Illuminator (Illuminating the grand 44m atrium)
        GameObject domeLightObj = CreateChildOrFind(chamber, "MuseumDomeLight");
        domeLightObj.transform.localPosition = new Vector3(0, 7.5f, 0);
        var domeLight = AddComponentIfMissing<Light>(domeLightObj);
        domeLight.type = LightType.Point;
        domeLight.color = new Color(0.88f, 0.96f, 1.0f);
        domeLight.intensity = 2.8f;
        domeLight.range = 35.0f;

        // Motorized Iris Roof Shutter on the Skylight Dome
        GameObject roofShutterObj = CreateChildOrFind(chamber, "ChamberRoofShutter");
        roofShutterObj.transform.localPosition = Vector3.zero;
        var roofShutter = AddComponentIfMissing<ChamberRoofShutter>(roofShutterObj);
        SerializedObject shutterSO = new SerializedObject(roofShutter);
        var pShutterMat = shutterSO.FindProperty("shutterMaterial");
        if (pShutterMat != null) pShutterMat.objectReferenceValue = scifiWallDark;
        var pSealMat = shutterSO.FindProperty("sealGlowMaterial");
        if (pSealMat != null) pSealMat.objectReferenceValue = cyanGlowMat;
        shutterSO.FindProperty("ceilingHeight").floatValue = 8.16f;
        shutterSO.FindProperty("openRadius").floatValue = 5.80f;
        shutterSO.FindProperty("closedRadius").floatValue = 0.0f;
        shutterSO.ApplyModifiedProperties();
        roofShutter.BuildIrisBlades();

        // --- 5E. HUMAN DIPLOMATIC ENVOY (Tripo 3D AI Model Ambassador) ---
        // Stationed on the right flank of the Central Landing Dais to greet the alien traveler
        Transform oldAmb = chamber.transform.Find("HumanAmbassadorStation");
        if (oldAmb != null) DestroyImmediate(oldAmb.gameObject);
        GameObject ambassadorObj = CreateChildOrFind(chamber, "HumanAmbassadorStation");
        ambassadorObj.transform.position = new Vector3(3.2f, 0.08f, 1.2f);
        ambassadorObj.transform.rotation = Quaternion.LookRotation(new Vector3(-3.2f, 0f, -1.2f), Vector3.up);

        GameObject holoPad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        holoPad.name = "HoloProjectorPad";
        holoPad.transform.SetParent(ambassadorObj.transform, false);
        holoPad.transform.localPosition = Vector3.zero;
        holoPad.transform.localScale = new Vector3(1.3f, 0.08f, 1.3f);
        holoPad.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

        GameObject padRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        padRim.name = "HoloPadRim";
        padRim.transform.SetParent(holoPad.transform, false);
        padRim.transform.localPosition = new Vector3(0, 0.52f, 0);
        padRim.transform.localScale = new Vector3(1.05f, 0.05f, 1.05f);
        padRim.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        // Tripo 3D Model: Human Diplomatic Ambassador
        GameObject ambModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_human_ambassador_statue.glb");
        if (ambModelPrefab != null)
        {
            GameObject ambModel = (GameObject)PrefabUtility.InstantiatePrefab(ambModelPrefab, ambassadorObj.transform);
            ambModel.name = "HumanAmbassador_Tripo3D";
            ambModel.transform.localPosition = new Vector3(0, 0.92f, 0);
            ambModel.transform.localRotation = Quaternion.Euler(0, 90f, 0); // Face landing dais
            ambModel.transform.localScale = Vector3.one * 1.85f;
        }

        GameObject ambLightObj = CreateChildOrFind(ambassadorObj, "HoloLight");
        var ambLight = AddComponentIfMissing<Light>(ambLightObj);
        ambLight.type = LightType.Point;
        ambLight.color = new Color(0f, 0.85f, 1f);
        ambLight.intensity = 2.2f;
        ambLight.range = 4.0f;
        ambLightObj.transform.localPosition = new Vector3(0, 1.4f, 0);

        var ambassadorComp = AddComponentIfMissing<HumanAmbassador>(ambassadorObj);

        // Angled Placard Stand in front of Ambassador's plinth facing the Dais
        GameObject ambPlacard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ambPlacard.name = "Placard_Stand";
        ambPlacard.transform.SetParent(ambassadorObj.transform, false);
        ambPlacard.transform.localPosition = new Vector3(0, 0.35f, 0.75f);
        ambPlacard.transform.localRotation = Quaternion.Euler(-25f, 180f, 0);
        ambPlacard.transform.localScale = new Vector3(1.15f, 0.52f, 0.05f);
        ambPlacard.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

        GameObject ambTextObj = CreateChildOrFind(ambPlacard, "WelcomeSubtitle");
        ambTextObj.transform.localPosition = new Vector3(0f, 0f, -0.52f);
        ambTextObj.transform.localRotation = Quaternion.identity;
        ambTextObj.transform.localScale = Vector3.one * 0.12f;
        var ambTMP = AddComponentIfMissing<TMPro.TextMeshPro>(ambTextObj);
        ambTMP.text = "<b><color=#00E5FF>DUTA BESAR PERADABAN MANUSIA</color></b>\n<size=75%><i>\"Selamat Datang di Bumi. Kami mempersembahkan Museum Peradaban Manusia untuk Anda.\"</i></size>";
        ambTMP.fontSize = 1.3f;
        ambTMP.alignment = TMPro.TextAlignmentOptions.Center;
        ambTMP.color = new Color(0.9f, 0.98f, 1f, 0.95f);
        RectTransform ambRT = ambTextObj.GetComponent<RectTransform>();
        ambRT.sizeDelta = new Vector2(8.5f, 4.0f);

        // --- 5F. ALIEN BIO-MIRROR & 3D ALIEN DIPLOMAT (Tripo AI) ---
        // Mirror station on left flank of dais showing the player's full 3D extraterrestrial alien character
        Transform oldBio = chamber.transform.Find("AlienBioMirrorStation");
        if (oldBio != null) DestroyImmediate(oldBio.gameObject);
        GameObject bioMirrorStation = CreateChildOrFind(chamber, "AlienBioMirrorStation");
        bioMirrorStation.transform.position = new Vector3(-3.2f, 0.08f, 1.2f);
        bioMirrorStation.transform.rotation = Quaternion.LookRotation(new Vector3(3.2f, 0f, -1.2f), Vector3.up);

        GameObject bioPad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bioPad.name = "BioProjectorPad";
        bioPad.transform.SetParent(bioMirrorStation.transform, false);
        bioPad.transform.localPosition = Vector3.zero;
        bioPad.transform.localScale = new Vector3(1.3f, 0.08f, 1.3f);
        bioPad.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

        GameObject bioRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bioRim.name = "BioPadRim";
        bioRim.transform.SetParent(bioPad.transform, false);
        bioRim.transform.localPosition = new Vector3(0, 0.52f, 0);
        bioRim.transform.localScale = new Vector3(1.05f, 0.05f, 1.05f);
        bioRim.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        AttachGLBOrPlaceholder(bioMirrorStation, "Assets/Models/Tripo/alien_diplomat_character.glb", "AlienDiplomat_Tripo3D",
            new Vector3(0, 0.92f, 0f), Vector3.one * 1.85f, Quaternion.Euler(0, 90f, 0));

        // Angled Placard Stand in front of Bio-Mirror plinth facing the Dais
        GameObject bioPlacard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bioPlacard.name = "Placard_Stand";
        bioPlacard.transform.SetParent(bioMirrorStation.transform, false);
        bioPlacard.transform.localPosition = new Vector3(0, 0.35f, 0.75f);
        bioPlacard.transform.localRotation = Quaternion.Euler(-25f, 180f, 0);
        bioPlacard.transform.localScale = new Vector3(1.15f, 0.52f, 0.05f);
        bioPlacard.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

        GameObject bioTextObj = CreateChildOrFind(bioPlacard, "BioSubtitle");
        bioTextObj.transform.localPosition = new Vector3(0f, 0f, -0.52f);
        bioTextObj.transform.localRotation = Quaternion.identity;
        bioTextObj.transform.localScale = Vector3.one * 0.12f;
        var bioTMP = AddComponentIfMissing<TMPro.TextMeshPro>(bioTextObj);
        bioTMP.text = "<b><color=#00E5FF>BIO-RESONANSI TAMU KOSMIK</color></b>\n<size=75%><i>\"Identitas Diplomatik: Terverifikasi. Akses Penuh ke Museum Diberikan.\"</i></size>";
        bioTMP.fontSize = 1.3f;
        bioTMP.alignment = TMPro.TextAlignmentOptions.Center;
        bioTMP.color = new Color(0.9f, 0.98f, 1f, 0.95f);
        RectTransform bioRT = bioTextObj.GetComponent<RectTransform>();
        bioRT.sizeDelta = new Vector2(8.5f, 4.0f);

        // --- 5G. CENTERPIECE: HUMAN CIVILIZATION & DNA HOLO-TABLE ---
        Transform oldTable = chamber.transform.Find("CivilizationHoloTable");
        if (oldTable != null) DestroyImmediate(oldTable.gameObject);
        GameObject holoTable = CreateChildOrFind(chamber, "CivilizationHoloTable");
        holoTable.transform.position = new Vector3(0, 0.08f, 3.8f);

        GameObject tableBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tableBase.name = "TableBase";
        tableBase.transform.SetParent(holoTable.transform);
        tableBase.transform.localPosition = new Vector3(0, 0.35f, 0);
        tableBase.transform.localScale = new Vector3(2.2f, 0.35f, 2.2f);
        tableBase.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

        GameObject tableRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tableRim.name = "TableRim";
        tableRim.transform.SetParent(holoTable.transform);
        tableRim.transform.localPosition = new Vector3(0, 0.72f, 0);
        tableRim.transform.localScale = new Vector3(2.0f, 0.04f, 2.0f);
        tableRim.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        GameObject holoEarth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        holoEarth.name = "HoloEarth";
        holoEarth.transform.SetParent(holoTable.transform);
        holoEarth.transform.localPosition = new Vector3(0, 1.35f, 0);
        holoEarth.transform.localScale = Vector3.one * 0.75f;
        holoEarth.GetComponent<MeshRenderer>().sharedMaterial = holoCyanMat;

        GameObject dnaRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        dnaRing.name = "DnaHelixRing";
        dnaRing.transform.SetParent(holoTable.transform);
        dnaRing.transform.localPosition = new Vector3(0, 1.35f, 0);
        dnaRing.transform.localScale = new Vector3(1.1f, 0.02f, 1.1f);
        dnaRing.transform.localRotation = Quaternion.Euler(25f, 0, 0);
        dnaRing.GetComponent<MeshRenderer>().sharedMaterial = holoGoldMat;

        GameObject tableTextObj = CreateChildOrFind(holoTable, "TableLabel");
        tableTextObj.transform.localPosition = new Vector3(0, 0.85f, -1.05f);
        tableTextObj.transform.localRotation = Quaternion.Euler(38f, 0, 0);
        var tableTMP = AddComponentIfMissing<TMPro.TextMeshPro>(tableTextObj);
        tableTMP.text = "<b>TERRAN CIVILIZATION & BIOSPHERE</b>\n<size=70%>Homo sapiens · Planet Terra · Sol System III</size>";
        tableTMP.fontSize = 1.4f;
        tableTMP.alignment = TMPro.TextAlignmentOptions.Center;
        tableTMP.color = new Color(0.9f, 0.95f, 1f, 0.9f);
        RectTransform tableRT = tableTextObj.GetComponent<RectTransform>();
        tableRT.sizeDelta = new Vector2(3.6f, 0.8f);

        // --- 5G. THE THREE INTERACTIVE LIVING SAMPLE PODS ---
        GameObject podsRoot = CreateChildOrFind(chamber, "LivingSamplePods");
        Vector3[] podPositions = {
            new Vector3(-8.0f, 0.08f, 8.5f),  // Pod 1 (NW): Hydrosphere
            new Vector3(0f, 0.08f, 12.0f),    // Pod 2 (N Center): Geosphere
            new Vector3(8.0f, 0.08f, 8.5f)    // Pod 3 (NE): Biosphere
        };
        string[] podTitles = { "HYDROSPHERE", "GEOSPHERE", "BIOSPHERE" };
        string[] podDescs = { "Pure Earth Spring Water", "Resonant Tectonic Minerals", "Living Terran Flora" };
        Color[] podColors = {
            new Color(0f, 0.85f, 1f),     // Water Cyan
            new Color(0.7f, 0.4f, 1f),    // Crystal Violet
            new Color(0.2f, 1f, 0.4f)     // Flora Emerald
        };

        GameObject[] samplePodObjs = new GameObject[3];
        SamplePodController[] podControllers = new SamplePodController[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject pod = CreateChildOrFind(podsRoot, $"SamplePod_{i + 1}_{podTitles[i]}");
            pod.transform.position = podPositions[i];
            pod.transform.rotation = Quaternion.LookRotation((Vector3.zero - podPositions[i]).normalized, Vector3.up);
            samplePodObjs[i] = pod;

            GameObject basePed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePed.name = "PodBase";
            basePed.transform.SetParent(pod.transform);
            basePed.transform.localPosition = new Vector3(0, 0.4f, 0);
            basePed.transform.localScale = new Vector3(1.6f, 0.4f, 1.6f);
            basePed.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;

            GameObject glowRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            glowRing.name = "GlowRing";
            glowRing.transform.SetParent(pod.transform);
            glowRing.transform.localPosition = new Vector3(0, 0.82f, 0);
            glowRing.transform.localScale = new Vector3(1.4f, 0.04f, 1.4f);
            glowRing.GetComponent<MeshRenderer>().sharedMaterial = (i == 0) ? cyanGlowMat : goldGlowMat;

            GameObject glassCyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            glassCyl.name = "StasisGlassCylinder";
            glassCyl.transform.SetParent(pod.transform);
            glassCyl.transform.localPosition = new Vector3(0, 1.5f, 0);
            glassCyl.transform.localScale = new Vector3(1.2f, 0.7f, 1.2f);
            glassCyl.GetComponent<MeshRenderer>().sharedMaterial = glassViewportMat;

            GameObject podLightObj = CreateChildOrFind(pod, "StasisLight");
            var pLight = AddComponentIfMissing<Light>(podLightObj);
            pLight.type = LightType.Point;
            pLight.color = podColors[i];
            pLight.intensity = 2.2f;
            pLight.range = 5.0f;
            podLightObj.transform.localPosition = new Vector3(0, 1.5f, 0);

            GameObject artifactRoot = CreateChildOrFind(pod, "FloatingArtifact");
            artifactRoot.transform.localPosition = new Vector3(0, 1.5f, 0);

            if (i == 0) // Water
            {
                AttachGLBOrPlaceholder(artifactRoot, "Assets/Models/Tripo/central_basin.glb", "BasinModel",
                    new Vector3(0, -0.2f, 0), Vector3.one * 0.45f, Quaternion.identity);
                AttachGLBOrPlaceholder(artifactRoot, "Assets/Models/Tripo/water_lotus.glb", "WaterLotusModel",
                    new Vector3(0, -0.1f, 0), Vector3.one * 0.3f, Quaternion.identity);

                GameObject ws = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ws.name = "WaterSphereMesh";
                ws.transform.SetParent(artifactRoot.transform);
                ws.transform.localPosition = new Vector3(0, 0.1f, 0);
                ws.transform.localScale = Vector3.one * 0.35f;
                ws.GetComponent<MeshRenderer>().sharedMaterial = waterMat;
            }
            else if (i == 1) // Crystal
            {
                AttachGLBOrPlaceholder(artifactRoot, "Assets/Models/Tripo/resonant_crystal.glb", "CrystalMesh",
                    Vector3.zero, Vector3.one * 0.5f, Quaternion.identity);
            }
            else if (i == 2) // Flora
            {
                AttachGLBOrPlaceholder(artifactRoot, "Assets/Models/Tripo/alien_flora_bloomed.glb", "FloraMesh",
                    new Vector3(0, -0.2f, 0), Vector3.one * 0.45f, Quaternion.identity);
            }

            var podCtrl = AddComponentIfMissing<SamplePodController>(pod);
            podControllers[i] = podCtrl;

            GameObject podLabelObj = CreateChildOrFind(pod, "PodLabel");
            float[] labelYaw = { 135f, 180f, -135f };
            float[] labelY = { 2.45f, 2.65f, 2.45f };
            podLabelObj.transform.localPosition = new Vector3(0, labelY[i], 0);
            podLabelObj.transform.localRotation = Quaternion.Euler(0, labelYaw[i], 0);
            var labelTMP = AddComponentIfMissing<TMPro.TextMeshPro>(podLabelObj);
            labelTMP.text = $"<b>{podTitles[i]}</b>\n<size=70%>{podDescs[i]}</size>";
            labelTMP.fontSize = 1.9f;
            labelTMP.alignment = TMPro.TextAlignmentOptions.Center;
            RectTransform pRT = podLabelObj.GetComponent<RectTransform>();
            pRT.sizeDelta = new Vector2(3.5f, 1.0f);

            SerializedObject pSO = new SerializedObject(podCtrl);
            pSO.FindProperty("podIndex").intValue = i;
            pSO.FindProperty("podName").stringValue = podTitles[i];
            pSO.FindProperty("description").stringValue = podDescs[i];
            pSO.FindProperty("activeColor").colorValue = podColors[i];
            pSO.FindProperty("floatingArtifact").objectReferenceValue = artifactRoot.transform;
            pSO.FindProperty("podStasisLight").objectReferenceValue = pLight;
            pSO.FindProperty("labelTMP").objectReferenceValue = labelTMP;
            pSO.ApplyModifiedProperties();

            var handDetector = AddComponentIfMissing<HandProximityDetector>(pod);
            if (handDetector.OnHandTouching == null) handDetector.OnHandTouching = new UnityEngine.Events.UnityEvent();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(handDetector.OnHandTouching, podCtrl.OnPlayerInteract);
        }

        // Wire OfferingManager pedestals
        SerializedObject omSO = new SerializedObject(offeringManager);
        var pProp = omSO.FindProperty("pedestals");
        pProp.arraySize = 3;
        for (int i = 0; i < 3; i++) pProp.GetArrayElementAtIndex(i).objectReferenceValue = samplePodObjs[i];
        omSO.FindProperty("completedPedestalMaterial").objectReferenceValue = goldGlowMat;
        omSO.ApplyModifiedProperties();

        // Overhead Glass Chimes (Tripo AI) hanging gracefully from interior ceiling soffit
        GameObject chimesRoot = CreateChildOrFind(chamber, "GlassChimesGroup");
        for (int i = 0; i < 6; i++)
        {
            float angle = (i * 60f + 30f) * Mathf.Deg2Rad;
            Vector3 chimePos = new Vector3(Mathf.Cos(angle) * 5.6f, 7.35f, Mathf.Sin(angle) * 5.6f);
            AttachGLBOrPlaceholder(chimesRoot, "Assets/Models/Tripo/glass_chime.glb", $"Chime_{i + 1}",
                chimePos, Vector3.one * 1.0f, Quaternion.Euler(0, i * 60f, 0));
        }

        // --- 5B. THE MUSEUM OF HUMANITY (5 Interactive Cultural & Scientific Pavilions) ---
        GameObject museumRoot = CreateChildOrFind(chamber, "MuseumOfHumanity");
        museumRoot.transform.localPosition = Vector3.zero;
        museumRoot.transform.localRotation = Quaternion.identity;

        string[] museumPrefabPaths = {
            "Assets/Prefabs/Museum/Exhibit_1_VoyagerGoldenRecord.prefab",
            "Assets/Prefabs/Museum/Exhibit_2_RosettaStoneAndArt.prefab",
            "Assets/Prefabs/Museum/Exhibit_3_PrometheusAndSilicon.prefab",
            "Assets/Prefabs/Museum/Exhibit_4_SvalbardSeedVault.prefab",
            "Assets/Prefabs/Museum/Exhibit_5_Apollo11AndDiplomaticPeace.prefab"
        };

        string[] museumNames = {
            "Exhibit_1_VoyagerGoldenRecord",
            "Exhibit_2_RosettaStoneAndArt",
            "Exhibit_3_PrometheusAndSilicon",
            "Exhibit_4_SvalbardSeedVault",
            "Exhibit_5_Apollo11AndDiplomaticPeace"
        };

        Vector3[] museumPositions = {
            new Vector3(-11.5f, 0.08f, 3.5f),   // Exhibit 1: Voyager Golden Record
            new Vector3(-12.5f, 0.08f, -4.0f),  // Exhibit 2: Rosetta Stone & Art Gallery
            new Vector3(-6.5f, 0.08f, -10.5f),  // Exhibit 3: Prometheus Fire & Microprocessor
            new Vector3(6.5f, 0.08f, -10.5f),   // Exhibit 4: Svalbard Seed Vault
            new Vector3(12.5f, 0.08f, -4.0f)    // Exhibit 5: Apollo 11 Plaque & Peace Contact Scanner
        };

        for (int m = 0; m < 5; m++)
        {
            GameObject mPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(museumPrefabPaths[m]);
            if (mPrefab != null)
            {
                Transform existing = museumRoot.transform.Find(museumNames[m]);
                if (existing != null) DestroyImmediate(existing.gameObject);

                GameObject mInst = (GameObject)PrefabUtility.InstantiatePrefab(mPrefab, museumRoot.transform);
                mInst.name = museumNames[m];
                mInst.transform.localPosition = museumPositions[m];
                // Point +Z outward so -Z (front face with placard & prompt) faces chamber center
                mInst.transform.localRotation = Quaternion.LookRotation(new Vector3(museumPositions[m].x, 0f, museumPositions[m].z), Vector3.up);
            }
        }

        // --- 5B-2. INTERACTIVE HOLOGRAPHIC INFORMATION TERMINALS (Tripo 3D AI Models) ---
        GameObject terminalsRoot = CreateChildOrFind(chamber, "MuseumInformationTerminals");
        Vector3[] terminalPositions = {
            new Vector3(-4.5f, 0.08f, 6.0f),
            new Vector3(4.5f, 0.08f, 6.0f)
        };
        float[] terminalYaws = { -155f, 155f };

        GameObject terminalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_interactive_terminal.glb");
        for (int t = 0; t < 2; t++)
        {
            string tName = $"InteractiveTerminal_{t + 1}";
            Transform existing = terminalsRoot.transform.Find(tName);
            if (existing != null) DestroyImmediate(existing.gameObject);

            GameObject termObj = new GameObject(tName);
            termObj.transform.SetParent(terminalsRoot.transform, false);
            termObj.transform.localPosition = terminalPositions[t];
            termObj.transform.localRotation = Quaternion.LookRotation((Vector3.zero - terminalPositions[t]).normalized, Vector3.up);

            if (terminalPrefab != null)
            {
                GameObject tModel = (GameObject)PrefabUtility.InstantiatePrefab(terminalPrefab, termObj.transform);
                tModel.name = "TerminalModel_Tripo3D";
                tModel.transform.localPosition = new Vector3(0, 0.62f, 0);
                tModel.transform.localRotation = Quaternion.Euler(0, 90f, 0);
                tModel.transform.localScale = Vector3.one * 1.25f;
            }

            // Holographic Terminal UI Placard with dark backing plate (facing the dais)
            GameObject termPlacard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            termPlacard.name = "TerminalHoloPlate";
            termPlacard.transform.SetParent(termObj.transform, false);
            termPlacard.transform.localPosition = new Vector3(0, 1.48f, 0.05f);
            termPlacard.transform.localRotation = Quaternion.Euler(-15f, 180f, 0);
            termPlacard.transform.localScale = new Vector3(1.30f, 0.38f, 0.03f);
            termPlacard.GetComponent<MeshRenderer>().sharedMaterial = scifiWallDark;
            var tCol = termPlacard.GetComponent<Collider>();
            if (tCol != null) DestroyImmediate(tCol);

            GameObject termLabelObj = new GameObject("TerminalLabel");
            termLabelObj.transform.SetParent(termPlacard.transform, false);
            termLabelObj.transform.localPosition = new Vector3(0, 0, -0.55f);
            termLabelObj.transform.localRotation = Quaternion.identity;
            termLabelObj.transform.localScale = Vector3.one * 0.12f;
            var termTMP = termLabelObj.AddComponent<TMPro.TextMeshPro>();
            termTMP.text = "<b><color=#00E5FF>TERMINAL ARSIP MUSEUM</color></b>\n<size=70%><color=#E0F7FA>Pangkalan Data Peradaban Sol-3</color></size>";
            termTMP.fontSize = 1.6f;
            termTMP.alignment = TMPro.TextAlignmentOptions.Center;
            termTMP.rectTransform.sizeDelta = new Vector2(10.5f, 3.0f);
        }

        // --- 5H. SPACE ENVIRONMENT & 100% FULL-SCREEN 3D EARTH (Deep Space at Y=1000) ---
        // Clean up legacy side cockpit bridge if present
        GameObject oldBridge = GameObject.Find("[UFOCockpitBridge]");
        if (oldBridge != null) DestroyImmediate(oldBridge);

        GameObject spaceEnv = CreateChildOrFind(root, "[SpaceEnvironment]");
        spaceEnv.transform.position = new Vector3(0, 1000f, 0);

        // 3D Planet Earth Celestial Globe in Deep Space (True Photorealistic Sphere)
        GameObject earthGlobeObj = CreateChildOrFind(spaceEnv, "PlanetEarth_Globe3D");
        earthGlobeObj.transform.localPosition = new Vector3(0f, -25f, 2720f);
        earthGlobeObj.transform.localRotation = Quaternion.Euler(23.44f, 15f, 0f);
        earthGlobeObj.transform.localScale = Vector3.one * 43.2f; // Small distant celestial marble in deep cosmos
        var earthMF = AddComponentIfMissing<MeshFilter>(earthGlobeObj);
        earthMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Sphere);
        var earthMR = AddComponentIfMissing<MeshRenderer>(earthGlobeObj);
        earthMR.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PlanetEarth_Globe3D.mat");

        AddComponentIfMissing<PlanetEarthGlobe>(earthGlobeObj);

        // Atmosphere Haze Rim Shell (Inverted Fresnel Glow)
        GameObject atmoShellObj = CreateChildOrFind(earthGlobeObj, "EarthAtmosphereHaze");
        atmoShellObj.transform.localPosition = Vector3.zero;
        atmoShellObj.transform.localRotation = Quaternion.identity;
        atmoShellObj.transform.localScale = Vector3.one * 1.025f;
        var atmoMF = AddComponentIfMissing<MeshFilter>(atmoShellObj);
        atmoMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Sphere);
        var atmoMR = AddComponentIfMissing<MeshRenderer>(atmoShellObj);
        atmoMR.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PlanetEarth_AtmosphereHaze.mat");

        // Directional Space Sunlight
        GameObject spaceSun = CreateChildOrFind(spaceEnv, "DeepSpaceSun");
        var sunLight = AddComponentIfMissing<Light>(spaceSun);
        sunLight.type = LightType.Directional;
        sunLight.color = new Color(1.0f, 0.98f, 0.95f);
        sunLight.intensity = 1.4f;
        spaceSun.transform.rotation = Quaternion.Euler(25f, -35f, 0f);

        // Orbital Planetary Sector Selector in Deep Space
        GameObject sectorSelectorObj = CreateChildOrFind(spaceEnv, "PlanetarySectorSelector");
        sectorSelectorObj.transform.localPosition = new Vector3(0, 0, 3.5f);
        sectorSelectorObj.transform.localRotation = Quaternion.identity;
        var sectorSel = AddComponentIfMissing<PlanetarySectorSelector>(sectorSelectorObj);

        GameObject selCanvasObj = CreateChildOrFind(sectorSelectorObj, "SelectorCanvas");
        selCanvasObj.transform.localPosition = Vector3.zero;

        GameObject selTitleObj = CreateChildOrFind(selCanvasObj, "SectorTitle");
        selTitleObj.transform.localPosition = new Vector3(0, 1.45f, 0);
        var sectorTitleTMP = AddComponentIfMissing<TMPro.TextMeshPro>(selTitleObj);
        sectorTitleTMP.fontSize = 1.15f;
        sectorTitleTMP.alignment = TMPro.TextAlignmentOptions.Center;
        sectorTitleTMP.color = new Color(0f, 0.95f, 1f);
        selTitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(8.5f, 1.0f);

        GameObject selDescObj = CreateChildOrFind(selCanvasObj, "SectorDetails");
        selDescObj.transform.localPosition = new Vector3(0, -0.85f, 0);
        var descTMP = AddComponentIfMissing<TMPro.TextMeshPro>(selDescObj);
        descTMP.fontSize = 0.82f;
        descTMP.alignment = TMPro.TextAlignmentOptions.Center;
        descTMP.color = new Color(0.85f, 0.95f, 1f);
        selDescObj.GetComponent<RectTransform>().sizeDelta = new Vector2(8.5f, 2.0f);

        GameObject selPromptObj = CreateChildOrFind(selCanvasObj, "PromptInstruction");
        selPromptObj.transform.localPosition = new Vector3(0, -1.65f, 0);
        var promptTMP = AddComponentIfMissing<TMPro.TextMeshPro>(selPromptObj);
        promptTMP.fontSize = 1.05f;
        promptTMP.alignment = TMPro.TextAlignmentOptions.Center;
        promptTMP.color = new Color(1f, 0.85f, 0.3f);
        selPromptObj.GetComponent<RectTransform>().sizeDelta = new Vector2(7.5f, 1.0f);

        GameObject reticleObj = CreateChildOrFind(selCanvasObj, "TargetingReticle");
        reticleObj.transform.localPosition = new Vector3(0, 0.05f, 0.8f);
        var reticleMR = AddComponentIfMissing<MeshRenderer>(reticleObj);
        reticleMR.sharedMaterial = cyanGlowMat;
        var reticleMF = AddComponentIfMissing<MeshFilter>(reticleObj);
        reticleMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Cylinder);
        reticleObj.transform.localScale = new Vector3(0.25f, 0.002f, 0.25f);
        reticleObj.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        var rcol = reticleObj.GetComponent<Collider>();
        if (rcol != null) DestroyImmediate(rcol);

        SerializedObject ssSO = new SerializedObject(sectorSel);
        ssSO.FindProperty("selectorCanvas").objectReferenceValue = selCanvasObj;
        ssSO.FindProperty("sectorTitleTMP").objectReferenceValue = sectorTitleTMP;
        ssSO.FindProperty("sectorDetailsTMP").objectReferenceValue = descTMP;
        ssSO.FindProperty("promptInstructionTMP").objectReferenceValue = promptTMP;
        ssSO.FindProperty("targetingReticle").objectReferenceValue = reticleObj.transform;
        ssSO.ApplyModifiedProperties();

        // --- 5I. UFO MOTHERSHIP (Centered at (0, 26, 0) directly above the Living Archive Dome) ---
        GameObject ufoMothership = CreateChildOrFind(environment, "UFOMothership");
        ufoMothership.transform.position = new Vector3(0, 26f, 0);
        ufoMothership.transform.localScale = Vector3.one;

        // Clean up legacy primitive mesh filter/renderer if attached directly to ufoMothership
        var legacyMF = ufoMothership.GetComponent<MeshFilter>();
        if (legacyMF != null) DestroyImmediate(legacyMF);
        var legacyMR = ufoMothership.GetComponent<MeshRenderer>();
        if (legacyMR != null) DestroyImmediate(legacyMR);

        // Attach Real 3D UFO Mothership Model Prefab
        AttachGLBOrPlaceholder(ufoMothership, "Assets/Prefabs/UFO_Mothership.prefab", "UFO_HullModel", Vector3.zero, Vector3.one, Quaternion.identity);

        var ufoCtrl = AddComponentIfMissing<UFOController>(ufoMothership);

        // Perimeter Hull Lights along the plasma trench (8 lights around ring at r=7.0m)
        GameObject hullLightsRoot = CreateChildOrFind(ufoMothership, "HullLightsGroup");
        Light[] hullLightArray = new Light[8];
        for (int i = 0; i < 8; i++)
        {
            float ang = i * 45f * Mathf.Deg2Rad;
            GameObject hlObj = CreateChildOrFind(hullLightsRoot, $"HullLight_{i + 1}");
            hlObj.transform.localPosition = new Vector3(Mathf.Sin(ang) * 7.0f, -0.45f, Mathf.Cos(ang) * 7.0f);
            var hl = AddComponentIfMissing<Light>(hlObj);
            hl.type = LightType.Point;
            hl.color = new Color(0f, 0.85f, 1f);
            hl.intensity = 1.2f;
            hl.range = 8.0f;
            hullLightArray[i] = hl;
        }

        GameObject ufoSpotObj = CreateChildOrFind(ufoMothership, "TractorSpotlight");
        var ufoSpotLight = AddComponentIfMissing<Light>(ufoSpotObj);
        ufoSpotLight.type = LightType.Spot;
        ufoSpotLight.color = new Color(0f, 0.9f, 1f);
        ufoSpotLight.intensity = 4.0f;
        ufoSpotLight.range = 50f;
        ufoSpotLight.spotAngle = 55f;
        ufoSpotObj.transform.localPosition = new Vector3(0, -1.5f, 0);
        ufoSpotObj.transform.localRotation = Quaternion.Euler(90f, 0, 0);

        GameObject goldenRainObj = CreateChildOrFind(ufoMothership, "GoldenRainParticles");
        var goldenRainPS = AddComponentIfMissing<ParticleSystem>(goldenRainObj);
        ConfigureGoldenRainParticles(goldenRainPS, particleMat);
        goldenRainObj.transform.localPosition = new Vector3(0, -1.5f, 0);
        goldenRainPS.Stop();

        // Ventral Tractor Beam Holographic Cylinder (Lowering player from UFO into Chamber)
        GameObject beamObj = CreateChildOrFind(ufoMothership, "VentralTractorBeam");
        beamObj.transform.localPosition = new Vector3(0, -13f, 0);
        beamObj.transform.localScale = new Vector3(3.8f, 13f, 3.8f);
        var bMF = AddComponentIfMissing<MeshFilter>(beamObj);
        bMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Cylinder);
        var bMR = AddComponentIfMissing<MeshRenderer>(beamObj);
        bMR.sharedMaterial = holoCyanMat;
        var bCol = beamObj.GetComponent<Collider>();
        if (bCol != null) DestroyImmediate(bCol);
        beamObj.SetActive(false);

        SerializedObject ufoSO = new SerializedObject(ufoCtrl);
        ufoSO.FindProperty("spotLight").objectReferenceValue = ufoSpotLight;
        var pHL = ufoSO.FindProperty("hullLights");
        pHL.arraySize = 8;
        for (int i = 0; i < 8; i++) pHL.GetArrayElementAtIndex(i).objectReferenceValue = hullLightArray[i];
        ufoSO.FindProperty("goldenRainParticles").objectReferenceValue = goldenRainPS;
        ufoSO.ApplyModifiedProperties();

        // ===== 6. VFX & CLIMAX BEACON =====
        GameObject vfx = CreateChildOrFind(root, "[VFX]");

        GameObject stardust = CreateChildOrFind(vfx, "CosmicStardust");
        var stardustCtrl = AddComponentIfMissing<StardustController>(stardust);
        var stardustPS = AddComponentIfMissing<ParticleSystem>(stardust);
        ConfigureStardustParticles(stardustPS, particleMat);
        stardust.transform.position = Vector3.zero;

        GameObject beacon = CreateChildOrFind(vfx, "ClimaxBeacon");
        var beaconVFX = AddComponentIfMissing<BeaconClimaxVFX>(beacon);
        beacon.transform.position = new Vector3(0, 0.5f, 0);

        SerializedObject bvSO = new SerializedObject(beaconVFX);
        var bOrigins = bvSO.FindProperty("pedestalOrigins");
        bOrigins.arraySize = 3;
        for (int i = 0; i < 3; i++) bOrigins.GetArrayElementAtIndex(i).objectReferenceValue = samplePodObjs[i].transform;
        bvSO.FindProperty("centerConvergencePoint").objectReferenceValue = beacon.transform;
        bvSO.ApplyModifiedProperties();

        // ===== 7. CAMERA RIG & PLAYER SETUP =====
        GameObject cameraRig = CreateChildOrFind(root, "[CameraRig]");
        // Spawn player in Deep Space with 100% full-screen 3D view of Earth
        cameraRig.transform.position = new Vector3(0, 1000f, 0f);
        cameraRig.transform.rotation = Quaternion.Euler(8f, 0, 0);

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
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;
        }
        mainCam.nearClipPlane = 0.02f;
        mainCam.farClipPlane = 10000f;
        AddComponentIfMissing<EditorFreeLookCamera>(mainCam.gameObject);
        var alienRig = AddComponentIfMissing<AlienPlayerRig>(cameraRig);

        // ===== 7B. UFO COCKPIT FLIGHT DECK (Player is 100% inside the enclosed UFO in space and during descent) =====
        GameObject cockpitBridgeObj = CreateChildOrFind(cameraRig, "[UFOCockpitBridge]");
        cockpitBridgeObj.transform.localPosition = new Vector3(0f, -1.05f, -0.15f);
        cockpitBridgeObj.transform.localRotation = Quaternion.identity;

        // Clean out legacy partial desks/canopy bars if present
        for (int i = cockpitBridgeObj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(cockpitBridgeObj.transform.GetChild(i).gameObject);
        }

        // 1. Instantiate Complete 360° Enclosed Alien UFO Cockpit Hull
        GameObject cockpitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UFO_CockpitBridge.prefab");
        GameObject bridgeInstance = null;
        if (cockpitPrefab != null)
        {
            bridgeInstance = (GameObject)PrefabUtility.InstantiatePrefab(cockpitPrefab, cockpitBridgeObj.transform);
            bridgeInstance.name = "UFO_Cockpit_Interior";
            bridgeInstance.transform.localPosition = Vector3.zero;
            bridgeInstance.transform.localRotation = Quaternion.identity;
            bridgeInstance.transform.localScale = Vector3.one;
        }
        else
        {
            // Fallback direct load of enclosed OBJ model
            GameObject cockpitModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/UFO/UFOCockpit_Interior.obj");
            if (cockpitModel != null)
            {
                bridgeInstance = new GameObject("UFO_Cockpit_Interior");
                bridgeInstance.transform.SetParent(cockpitBridgeObj.transform, false);
                GameObject hullInstFallback = Instantiate(cockpitModel, bridgeInstance.transform);
                hullInstFallback.name = "UFO_Cockpit_Enclosure";
                hullInstFallback.transform.localPosition = Vector3.zero;
                hullInstFallback.transform.localRotation = Quaternion.identity;
                hullInstFallback.transform.localScale = Vector3.one;
            }
        }

        if (bridgeInstance != null)
        {
            Transform hullTrans = bridgeInstance.transform.Find("UFO_Cockpit_Enclosure") ?? bridgeInstance.transform;
            GameObject hullInst = hullTrans.gameObject;

                Material cScreenGlass = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_ScreenGlass.mat") ?? scifiWallDark;
                Material leftScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Left_EarthScan.mat") ?? cScreenGlass;
                Material rightScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Right_EarthOrbit.mat") ?? cScreenGlass;
                Material monLeftMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Left.mat");
                Material monCenterMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Center.mat");
                Material monRightMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Right.mat");
                Mesh monLeftMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Left_Mesh.asset");
                Mesh monCenterMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Center_Mesh.asset");
                Mesh monRightMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Right_Mesh.asset");

                if (leftScreenMat != null) { leftScreenMat.SetFloat("_Cull", 0f); leftScreenMat.doubleSidedGI = true; }
                if (rightScreenMat != null) { rightScreenMat.SetFloat("_Cull", 0f); rightScreenMat.doubleSidedGI = true; }
                if (scifiWallDark != null) { scifiWallDark.SetFloat("_Cull", 0f); scifiWallDark.doubleSidedGI = true; }
                if (scifiWallWhite != null) { scifiWallWhite.SetFloat("_Cull", 0f); scifiWallWhite.doubleSidedGI = true; }

                Mesh cleanDarkMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Dark_Clean.asset");
                Mesh cleanGlowMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Glow_Cyan_Clean.asset");
                Mesh centerHousing = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Center_Housing.asset");
                Mesh leftHousing = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Left_Housing.asset");
                Mesh rightHousing = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Right_Housing.asset");
                Mesh mountsMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/UFO/Cockpit_Monitor_Mounts.asset");

                foreach (var mr in hullInst.GetComponentsInChildren<MeshRenderer>(true))
                {
                    // OBJ importer inverts X: Screen_RightPanoramic is physical Left (-1.78m), Screen_LeftPanoramic is physical Right (+1.78m)
                    if (mr.name.Contains("Screen_RightPanoramic")) mr.sharedMaterial = leftScreenMat; // Left: Earth Biosphere & Scan
                    else if (mr.name.Contains("Screen_LeftPanoramic")) mr.sharedMaterial = rightScreenMat; // Right: Sol-3 Orbit Dynamics
                    else if (mr.name.Contains("Cockpit_Screen"))
                    {
                        mr.gameObject.SetActive(false); // Hide the old dark combined mesh
                    }
                    else if (mr.name.Contains("Cockpit_Dark"))
                    {
                        mr.sharedMaterial = scifiWallDark;
                        if (cleanDarkMesh != null) mr.GetComponent<MeshFilter>().sharedMesh = cleanDarkMesh;
                    }
                    else if (mr.name.Contains("Cockpit_Light")) mr.sharedMaterial = scifiWallWhite;
                    else if (mr.name.Contains("GlassViewport") || mr.name.Contains("Cockpit_Glass")) mr.sharedMaterial = glassViewportMat;
                    else if (mr.name.Contains("Glow_Cyan"))
                    {
                        mr.sharedMaterial = cyanGlowMat;
                        if (cleanGlowMesh != null) mr.GetComponent<MeshFilter>().sharedMesh = cleanGlowMesh;
                    }
                    else if (mr.name.Contains("Glow_Gold")) mr.sharedMaterial = goldGlowMat;
                }

                // Mount individual high-fidelity elevated telemetry monitors with housings
                void MountMonitor(string name, Mesh screenMesh, Material screenMat, Mesh housingMesh)
                {
                    if (screenMesh == null || screenMat == null) return;
                    var monObj = CreateChildOrFind(hullInst, name);
                    monObj.transform.localPosition = Vector3.zero;
                    var mf = AddComponentIfMissing<MeshFilter>(monObj);
                    var mr = AddComponentIfMissing<MeshRenderer>(monObj);
                    mf.sharedMesh = screenMesh;
                    mr.sharedMaterial = screenMat;

                    if (housingMesh != null)
                    {
                        var hObj = CreateChildOrFind(monObj, "Housing");
                        hObj.transform.localPosition = Vector3.zero;
                        var hmf = AddComponentIfMissing<MeshFilter>(hObj);
                        var hmr = AddComponentIfMissing<MeshRenderer>(hObj);
                        hmf.sharedMesh = housingMesh;
                        hmr.sharedMaterials = new Material[] { scifiWallDark, cyanGlowMat };
                    }
                }

                MountMonitor("Cockpit_Monitor_Left", monLeftMesh, monLeftMat, leftHousing);
                MountMonitor("Cockpit_Monitor_Center", monCenterMesh, monCenterMat, centerHousing);
                MountMonitor("Cockpit_Monitor_Right", monRightMesh, monRightMat, rightHousing);

                if (mountsMesh != null)
                {
                    var mObj = CreateChildOrFind(hullInst, "Monitor_Mounting_Brackets");
                    mObj.transform.localPosition = Vector3.zero;
                    var mmf = AddComponentIfMissing<MeshFilter>(mObj);
                    var mmr = AddComponentIfMissing<MeshRenderer>(mObj);
                    mmf.sharedMesh = mountsMesh;
                    mmr.sharedMaterials = new Material[] { scifiWallDark, cyanGlowMat };
                }

            // Mount Tripo PBR command module at front console if not already present
            GameObject tripoModule = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_module_tripo.glb");
            if (tripoModule != null && bridgeInstance.transform.Find("Tripo_PBR_CommandDesk") == null && cockpitBridgeObj.transform.Find("Tripo_PBR_CommandDesk") == null)
            {
                GameObject tripoChild = Instantiate(tripoModule, cockpitBridgeObj.transform);
                tripoChild.name = "Tripo_PBR_CommandDesk";
                tripoChild.transform.localPosition = new Vector3(0f, 0.44f, 1.38f);
                tripoChild.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                tripoChild.transform.localScale = Vector3.one * 1.35f;
            }

            // Secondary Tripo Sci-Fi Console & Tactical Station in Rear Bridge if not already present
            GameObject tripoInterior = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_interior.glb");
            if (tripoInterior != null && bridgeInstance.transform.Find("Tripo_Rear_AvionicsNexus") == null && cockpitBridgeObj.transform.Find("Tripo_Rear_AvionicsNexus") == null)
            {
                GameObject rearTripo = Instantiate(tripoInterior, cockpitBridgeObj.transform);
                rearTripo.name = "Tripo_Rear_AvionicsNexus";
                rearTripo.transform.localPosition = new Vector3(0f, 0.22f, -1.90f);
                rearTripo.transform.localRotation = Quaternion.identity;
                rearTripo.transform.localScale = Vector3.one * 1.15f;
            }
        }

        // 2. Cockpit Internal Ambient Lighting (Cosmic Cyan/Blue Glow)
        GameObject cockLightObj = CreateChildOrFind(cockpitBridgeObj, "CockpitInternalLight");
        cockLightObj.transform.localPosition = new Vector3(0, 1.85f, 0.4f);
        var cLight = AddComponentIfMissing<Light>(cockLightObj);
        cLight.type = LightType.Point;
        cLight.color = new Color(0.12f, 0.85f, 1.0f);
        cLight.intensity = 2.2f;
        cLight.range = 5.5f;

        // Rear Cabin Illumination (Soft warm cyan fill illuminating rear cabin and pilot chair from behind)
        GameObject rearLightObj = CreateChildOrFind(cockpitBridgeObj, "CockpitRearLight");
        rearLightObj.transform.localPosition = new Vector3(0, 1.95f, -1.20f);
        var rLight = AddComponentIfMissing<Light>(rearLightObj);
        rLight.type = LightType.Point;
        rLight.color = new Color(0.20f, 0.85f, 1.0f);
        rLight.intensity = 2.0f;
        rLight.range = 6.0f;

        // Clean up obstructive FlightVisorHUD to ensure 100% crystal-clear canopy viewport
        Transform oldVisor = cameraRig.transform.Find("FlightVisorHUD");
        if (oldVisor != null) DestroyImmediate(oldVisor.gameObject);

        // Clean up any text HUD overlays on cockpit MFDs to maintain pure, cinematic visuals
        Transform oldCenterUI = cockpitBridgeObj.transform.Find("MFD_Center_UI");
        if (oldCenterUI != null) DestroyImmediate(oldCenterUI.gameObject);
        Transform oldLeftUI = cockpitBridgeObj.transform.Find("MFD_Left_UI");
        if (oldLeftUI != null) DestroyImmediate(oldLeftUI.gameObject);
        Transform oldRightUI = cockpitBridgeObj.transform.Find("MFD_Right_UI");
        if (oldRightUI != null) DestroyImmediate(oldRightUI.gameObject);

        // 3D Miniature Holographic Earth & Radar Scanner on Console Desk
        GameObject miniHoloRoot = CreateChildOrFind(cockpitBridgeObj, "ConsoleHoloDisplay");
        miniHoloRoot.transform.localPosition = new Vector3(0f, 0.86f, 1.25f);

        GameObject miniEarth = CreateChildOrFind(miniHoloRoot, "HoloEarthMini");
        miniEarth.transform.localPosition = Vector3.zero;
        miniEarth.transform.localScale = Vector3.one * 0.16f;
        var miniMF = AddComponentIfMissing<MeshFilter>(miniEarth);
        miniMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Sphere);
        var miniMR = AddComponentIfMissing<MeshRenderer>(miniEarth);
        miniMR.sharedMaterial = holoCyanMat;

        GameObject miniRing = CreateChildOrFind(miniHoloRoot, "HoloRadarRing");
        miniRing.transform.localPosition = Vector3.zero;
        miniRing.transform.localScale = new Vector3(0.24f, 0.005f, 0.24f);
        miniRing.transform.localRotation = Quaternion.Euler(20f, 0, 0);
        var ringMF = AddComponentIfMissing<MeshFilter>(miniRing);
        ringMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Cylinder);
        var ringMR = AddComponentIfMissing<MeshRenderer>(miniRing);
        ringMR.sharedMaterial = holoGoldMat;

        // Attach CockpitInfogramDisplay for animated 3D mini-globe & radar ring
        var infogramComp = AddComponentIfMissing<CockpitInfogramDisplay>(cockpitBridgeObj);
        SerializedObject infoSO = new SerializedObject(infogramComp);
        var pEarth = infoSO.FindProperty("holoEarthMini");
        if (pEarth != null) pEarth.objectReferenceValue = miniEarth.transform;
        var pRing = infoSO.FindProperty("holoRadarRing");
        if (pRing != null) pRing.objectReferenceValue = miniRing.transform;
        infoSO.ApplyModifiedProperties();

        // Warp Streaks Particle System (Hyperspace / Lightspeed flight towards Earth)
        GameObject warpObj = CreateChildOrFind(cameraRig, "WarpStreaksVFX");
        var warpPS = AddComponentIfMissing<ParticleSystem>(warpObj);
        ConfigureWarpStreaksParticles(warpPS, particleMat);
        warpObj.transform.localPosition = new Vector3(0, 0, 14f);
        warpPS.Stop();

        // Re-entry Plasma Particle System (attached to camera rig)
        GameObject plasmaObj = CreateChildOrFind(cameraRig, "ReEntryPlasmaVFX");
        var plasmaPS = AddComponentIfMissing<ParticleSystem>(plasmaObj);
        ConfigureReEntryPlasmaParticles(plasmaPS, particleMat);
        plasmaObj.transform.localPosition = new Vector3(0, -0.2f, 1.5f);
        plasmaPS.Stop();

        // Cloud Penetration Particle System (attached to camera rig)
        GameObject cloudVFXObj = CreateChildOrFind(cameraRig, "CloudPenetrationVFX");
        var cloudPS = AddComponentIfMissing<ParticleSystem>(cloudVFXObj);
        ConfigureCloudVFXParticles(cloudPS, particleMat);
        cloudVFXObj.transform.localPosition = new Vector3(0, 0.2f, 2.0f);
        cloudPS.Stop();

        // Central Landing Dais Spawn Point inside Chamber
        GameObject chamberSpawn = CreateChildOrFind(chamber, "PlayerChamberSpawn");
        chamberSpawn.transform.localPosition = new Vector3(0, 0.24f, 0f);
        chamberSpawn.transform.localRotation = Quaternion.identity;

        // Wire UFODescentSequence
        SerializedObject dsSO = new SerializedObject(ufoDescent);
        dsSO.FindProperty("playerRig").objectReferenceValue = cameraRig.transform;
        dsSO.FindProperty("chamberSpawnPoint").objectReferenceValue = chamberSpawn.transform;
        dsSO.FindProperty("spaceEarthObject").objectReferenceValue = earthGlobeObj;
        dsSO.FindProperty("humanAmbassadorHolo").objectReferenceValue = ambassadorObj;
        dsSO.FindProperty("tractorBeamVisual").objectReferenceValue = beamObj;
        dsSO.FindProperty("tractorSpotlight").objectReferenceValue = ufoSpotLight;
        dsSO.FindProperty("tractorStardust").objectReferenceValue = goldenRainPS;
        dsSO.FindProperty("warpStreaksVFX").objectReferenceValue = warpPS;
        dsSO.FindProperty("reEntryPlasmaVFX").objectReferenceValue = plasmaPS;
        dsSO.FindProperty("cloudVFX").objectReferenceValue = cloudPS;
        dsSO.FindProperty("hudStatusText").objectReferenceValue = null; // Clean viewport
        dsSO.FindProperty("mfdCenterText").objectReferenceValue = null;
        dsSO.FindProperty("mfdLeftText").objectReferenceValue = null;
        dsSO.FindProperty("mfdRightText").objectReferenceValue = null;
        dsSO.FindProperty("spaceSkybox").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CosmicVoid_Skybox.mat");
        dsSO.FindProperty("earthSkybox").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_WorldLabs_TerranEmbassy.mat") ?? AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_MirrorLake.mat");
        dsSO.FindProperty("ufoMothership").objectReferenceValue = ufoMothership;
        dsSO.FindProperty("ufoCockpitBridge").objectReferenceValue = cockpitBridgeObj;
        dsSO.FindProperty("chamberRoofShutter").objectReferenceValue = roofShutter;
        dsSO.FindProperty("planetarySectorSelector").objectReferenceValue = sectorSel;
        dsSO.FindProperty("alienPlayerRig").objectReferenceValue = alienRig;
        dsSO.FindProperty("orbitPosition").vector3Value = new Vector3(0f, 1000f, 0f);
        dsSO.FindProperty("reEntryStartPosition").vector3Value = new Vector3(0f, 960f, 160f);
        dsSO.FindProperty("cloudEntryPosition").vector3Value = new Vector3(0f, 350f, 40f);
        dsSO.FindProperty("surfaceApproachPosition").vector3Value = new Vector3(0f, 50f, 35f);
        dsSO.FindProperty("hoverPosition").vector3Value = new Vector3(0f, 18f, 0f);
        dsSO.FindProperty("touchdownPosition").vector3Value = new Vector3(0f, 0.24f, 0f);
        dsSO.ApplyModifiedProperties();

        GameObject fadeCanvas = CreateChildOrFind(cameraRig, "FadeCanvas");
        var fadeCtrl = AddComponentIfMissing<FadeController>(fadeCanvas);
        var canvas = AddComponentIfMissing<Canvas>(fadeCanvas);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        AddComponentIfMissing<UnityEngine.UI.CanvasScaler>(fadeCanvas);

        GameObject passthroughCtrl = CreateChildOrFind(cameraRig, "PassthroughController");
        var passthroughTransition = AddComponentIfMissing<PassthroughTransition>(passthroughCtrl);
        SerializedObject ptSO = new SerializedObject(passthroughTransition);
        ptSO.FindProperty("virtualEnvironmentRoot").objectReferenceValue = environment;
        ptSO.ApplyModifiedProperties();

        // ===== 8. END SCREEN UI =====
        GameObject endScreen = CreateChildOrFind(root, "EndScreenCanvas");
        var endCanvas = AddComponentIfMissing<Canvas>(endScreen);
        endCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        endCanvas.sortingOrder = 998;
        AddComponentIfMissing<UnityEngine.UI.CanvasScaler>(endScreen);

        EndScreenUI endUI = AddComponentIfMissing<EndScreenUI>(endScreen);

        GameObject msgObj = CreateChildOrFind(endScreen, "MessageText");
        var msgTMP = AddComponentIfMissing<TMPro.TextMeshProUGUI>(msgObj);
        msgTMP.text = "The gift was received.\nWelcome to Earth.";
        msgTMP.fontSize = 28;
        msgTMP.alignment = TMPro.TextAlignmentOptions.Center;
        msgTMP.color = new Color(0.9f, 0.95f, 1f, 0f);
        RectTransform msgRT = msgObj.GetComponent<RectTransform>();
        msgRT.anchoredPosition = new Vector2(0, 50f);
        msgRT.sizeDelta = new Vector2(800, 100);

        GameObject titleObj = CreateChildOrFind(endScreen, "TitleText");
        var titleTMP = AddComponentIfMissing<TMPro.TextMeshProUGUI>(titleObj);
        titleTMP.text = "XENOASIS";
        titleTMP.fontSize = 42;
        titleTMP.alignment = TMPro.TextAlignmentOptions.Center;
        titleTMP.color = new Color(1f, 0.85f, 0.35f, 0f);
        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchoredPosition = new Vector2(0, -30f);
        titleRT.sizeDelta = new Vector2(800, 80);

        SerializedObject uiSO = new SerializedObject(endUI);
        var pTitle = uiSO.FindProperty("titleText");
        if (pTitle != null) pTitle.objectReferenceValue = titleTMP;
        var pMsg = uiSO.FindProperty("messageText");
        if (pMsg != null) pMsg.objectReferenceValue = msgTMP;
        uiSO.ApplyModifiedProperties();
        endScreen.SetActive(false);

        // ===== 9. CLEAN UP INSTRUCTION HUD (No text clutter during flight) =====
        Transform oldHUD = root.transform.Find("InstructionHUD");
        if (oldHUD != null) DestroyImmediate(oldHUD.gameObject);

        // ===== 10. LIGHTING =====
        GameObject lighting = CreateChildOrFind(root, "[Lighting]");
        GameObject dirLightObj = CreateChildOrFind(lighting, "TerranSunLight");
        var dirLight = AddComponentIfMissing<Light>(dirLightObj);
        dirLight.type = LightType.Directional;
        dirLight.intensity = 1.0f;
        dirLight.color = new Color(0.75f, 0.92f, 1.0f);
        dirLightObj.transform.rotation = Quaternion.Euler(45f, 30f, 0);

        // Chamber Warm Interior Fill Light
        GameObject fillLightObj = CreateChildOrFind(lighting, "ChamberFillLight");
        var fillLight = AddComponentIfMissing<Light>(fillLightObj);
        fillLight.type = LightType.Point;
        fillLight.intensity = 1.2f;
        fillLight.range = 14f;
        fillLight.color = new Color(0.9f, 0.95f, 1f);
        fillLightObj.transform.position = new Vector3(0, 3.5f, 0);

        // ===== 11. POST-PROCESSING VOLUME =====
        GameObject postProcessing = CreateChildOrFind(root, "[PostProcessing]");
        var volume = AddComponentIfMissing<Volume>(postProcessing);
        volume.isGlobal = true;
        volume.priority = 1f;
        ConfigurePostProcessingProfile(volume);

        // ===== 12. WIRE BIOME CONTROLLER =====
        SerializedObject bcSO = new SerializedObject(biomeCtrl);
        bcSO.FindProperty("directionalLight").objectReferenceValue = dirLight;
        bcSO.FindProperty("defaultSkybox").objectReferenceValue = biomeSkyboxMats[0];
        bcSO.FindProperty("lakeSkybox").objectReferenceValue = biomeSkyboxMats[1];
        bcSO.FindProperty("mountainSkybox").objectReferenceValue = biomeSkyboxMats[2];
        bcSO.FindProperty("forestSkybox").objectReferenceValue = biomeSkyboxMats[3];
        bcSO.FindProperty("curvedVistaRenderer").objectReferenceValue = null;
        bcSO.FindProperty("defaultVistaTex").objectReferenceValue = lakeVistaTex;
        bcSO.FindProperty("lakeVistaTex").objectReferenceValue = lakeVistaTex;
        bcSO.FindProperty("mountainVistaTex").objectReferenceValue = mountainVistaTex;
        bcSO.FindProperty("forestVistaTex").objectReferenceValue = forestVistaTex;
        bcSO.ApplyModifiedProperties();

        // ===== 13. WIRE GAME MANAGER =====
        SerializedObject gmSO = new SerializedObject(gameManager);
        var pUfo = gmSO.FindProperty("ufoController");
        if (pUfo != null) pUfo.objectReferenceValue = ufoCtrl;
        var pBiome = gmSO.FindProperty("biomeTransitionController");
        if (pBiome != null) pBiome.objectReferenceValue = biomeCtrl;
        var pBeacon = gmSO.FindProperty("beaconClimaxVFX");
        if (pBeacon != null) pBeacon.objectReferenceValue = beaconVFX;
        var pFade = gmSO.FindProperty("fadeController");
        if (pFade != null) pFade.objectReferenceValue = fadeCtrl;
        var pAudio = gmSO.FindProperty("audioManager");
        if (pAudio != null) pAudio.objectReferenceValue = audioManager;
        var pStardust = gmSO.FindProperty("stardustController");
        if (pStardust != null) pStardust.objectReferenceValue = stardustCtrl;
        var pPass = gmSO.FindProperty("passthroughTransition");
        if (pPass != null) pPass.objectReferenceValue = passthroughTransition;
        var pEndCanvas = gmSO.FindProperty("endScreenCanvas");
        if (pEndCanvas != null) pEndCanvas.objectReferenceValue = endScreen;
        var pDescent = gmSO.FindProperty("ufoDescentSequence");
        if (pDescent != null) pDescent.objectReferenceValue = ufoDescent;
        gmSO.ApplyModifiedProperties();

        // Clean up any recording objects
        var oldRec = GameObject.Find("LiveDescentRecorder");
        if (oldRec != null) DestroyImmediate(oldRec);

        // ===== 14. SAVE SCENE & BUILD SETTINGS =====
        string scenePath = "Assets/Scenes/WelcomeChamber.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        RegisterSceneInBuild(scenePath);

        Debug.Log("[XENOASIS] ✦ The Grand Terran Embassy & Living Archive assembled successfully!\n" +
                  "✔ Orbital UFO Bridge with cockpit canopy, pilot console & 8K Earth vista\n" +
                  "✔ Re-entry Plasma & Cloud Penetration descent animation system\n" +
                  "✔ Docking Airlock with animated pressure door & illuminated gangway\n" +
                  "✔ Human Diplomatic Envoy Hologram with first contact welcome greeting\n" +
                  "✔ Modern Sci-Fi Architecture (solid composite floor, titanium walls, ceiling light ring)\n" +
                  "✔ Grand Curved Viewport with titanium sill balustrade framing outside Ultra-HD biomes\n" +
                  "✔ Ultra-HD 16:9 Curved Vista Screen (Lake, Mountain, Forest) with zero polar distortion\n" +
                  "✔ 3 Living Sample Pods with dynamic biome crossfades & climax convergence\n" +
                  "✔ Civilization & DNA Holo-Table centerpiece\n" +
                  "✔ Auto-saved to Assets/Scenes/WelcomeChamber.unity");
    }

    // ===== VISTA & GEOMETRY GENERATORS =====
    static Texture2D LoadAndConfigureVistaTexture(string path)
    {
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.maxTextureSize = 4096;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.filterMode = FilterMode.Trilinear;
            ti.anisoLevel = 16;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Mesh CreateCurvedScreenMesh(float radius, float startAngleDeg, float endAngleDeg, float bottomY, float topY, int segmentsH, int segmentsV)
    {
        Mesh mesh = new Mesh { name = "CurvedVistaScreenMesh" };
        int numVerts = (segmentsH + 1) * (segmentsV + 1);
        Vector3[] verts = new Vector3[numVerts];
        Vector2[] uvs = new Vector2[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        int[] tris = new int[segmentsH * segmentsV * 6];

        for (int v = 0; v <= segmentsV; v++)
        {
            float vNorm = (float)v / segmentsV;
            float y = Mathf.Lerp(bottomY, topY, vNorm);

            for (int h = 0; h <= segmentsH; h++)
            {
                float hNorm = (float)h / segmentsH;
                float angleDeg = Mathf.Lerp(startAngleDeg, endAngleDeg, hNorm);
                float angleRad = angleDeg * Mathf.Deg2Rad;

                float x = Mathf.Sin(angleRad) * radius;
                float z = Mathf.Cos(angleRad) * radius;

                int idx = v * (segmentsH + 1) + h;
                verts[idx] = new Vector3(x, y, z);
                uvs[idx] = new Vector2(hNorm, vNorm);
                normals[idx] = new Vector3(-Mathf.Sin(angleRad), 0, -Mathf.Cos(angleRad));
            }
        }

        int triIdx = 0;
        for (int v = 0; v < segmentsV; v++)
        {
            for (int h = 0; h < segmentsH; h++)
            {
                int bl = v * (segmentsH + 1) + h;
                int br = bl + 1;
                int tl = (v + 1) * (segmentsH + 1) + h;
                int tr = tl + 1;

                tris[triIdx++] = bl;
                tris[triIdx++] = tl;
                tris[triIdx++] = br;

                tris[triIdx++] = br;
                tris[triIdx++] = tl;
                tris[triIdx++] = tr;
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        return mesh;
    }

    static void ConfigureReEntryPlasmaParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && mat != null) psr.sharedMaterial = mat;

        var main = ps.main;
        main.maxParticles = 600;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(15f, 30f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.45f, 0.1f, 0.9f), new Color(1f, 0.85f, 0.2f, 0.95f));
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emission = ps.emission;
        emission.rateOverTime = 250f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 20f;
        shape.radius = 1.8f;
        shape.rotation = new Vector3(180f, 0, 0);
    }

    static void ConfigureCloudVFXParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && mat != null) psr.sharedMaterial = mat;

        var main = ps.main;
        main.maxParticles = 400;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(20f, 40f);
        main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 4.0f);
        main.startColor = new Color(0.95f, 0.97f, 1f, 0.35f);
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emission = ps.emission;
        emission.rateOverTime = 120f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(6f, 3f, 4f);
    }

    static void ConfigureWarpStreaksParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            psr.renderMode = ParticleSystemRenderMode.Stretch;
            psr.velocityScale = 0.04f;
            psr.lengthScale = 2.8f;
            if (mat != null) psr.sharedMaterial = mat;
        }

        var main = ps.main;
        main.maxParticles = 600;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.65f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(65f, 95f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.07f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.6f, 0.85f, 1f, 0.95f), new Color(0.9f, 0.95f, 1f, 1.0f));
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emission = ps.emission;
        emission.rateOverTime = 380f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 4.0f;
        shape.radius = 2.4f;
        shape.rotation = new Vector3(180f, 0, 0); // shoots toward camera past cockpit
    }

    static void ConfigureLandingSteamParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && mat != null) psr.sharedMaterial = mat;

        var main = ps.main;
        main.maxParticles = 300;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 1.2f);
        main.startColor = new Color(0.85f, 0.92f, 1f, 0.5f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 80f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 2.5f;
        shape.rotation = new Vector3(90f, 0, 0);
    }

    static void ConfigureStardustParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && mat != null) psr.sharedMaterial = mat;

        var main = ps.main;
        main.maxParticles = 200;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(0f, 0.9f, 1f, 0.7f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 25f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 8.5f;
    }

    static void ConfigureGoldenRainParticles(ParticleSystem ps, Material mat)
    {
        var psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && mat != null) psr.sharedMaterial = mat;

        var main = ps.main;
        main.maxParticles = 500;
        main.startLifetime = 8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.08f);
        main.startColor = new Color(1f, 0.85f, 0.3f, 0.9f);
        main.gravityModifier = 0.3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 80f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 3f;
        shape.rotation = new Vector3(180f, 0, 0);
    }

    // ===== URP POST-PROCESSING CONFIGURATOR =====
    static void ConfigurePostProcessingProfile(Volume volume)
    {
        EnsureDirectory("Assets/Settings");
        string profilePath = "Assets/Settings/WelcomeChamber_Profile.asset";
        VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
        }

        Bloom bloom;
        if (!profile.TryGet(out bloom)) bloom = profile.Add<Bloom>(true);
        bloom.threshold.Override(0.85f);
        bloom.intensity.Override(2.2f);
        bloom.scatter.Override(0.7f);
        bloom.tint.Override(new Color(0f, 1f, 0.82f));

        Tonemapping tonemapping;
        if (!profile.TryGet(out tonemapping)) tonemapping = profile.Add<Tonemapping>(true);
        tonemapping.mode.Override(TonemappingMode.ACES);

        Vignette vignette;
        if (!profile.TryGet(out vignette)) vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.25f);
        vignette.smoothness.Override(0.4f);

        ColorAdjustments colorAdj;
        if (!profile.TryGet(out colorAdj)) colorAdj = profile.Add<ColorAdjustments>(true);
        colorAdj.postExposure.Override(0.15f);
        colorAdj.contrast.Override(15f);
        colorAdj.saturation.Override(10f);

        EditorUtility.SetDirty(profile);
        volume.sharedProfile = profile;
    }

    // ===== BUILD & ATTACH HELPERS =====
    static void RegisterSceneInBuild(string scenePath)
    {
        var scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].path == scenePath) return;
        }

        var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
        System.Array.Copy(scenes, newScenes, scenes.Length);
        newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
        EditorBuildSettings.scenes = newScenes;
    }

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

    static void EnsureURPConfigured()
    {
        EnsureDirectory("Assets/Settings");
        string rendererPath = "Assets/Settings/UniversalRenderer.asset";
        string pipelineAssetPath = "Assets/Settings/UniversalRenderPipelineAsset.asset";

        UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
        if (rendererData == null)
        {
            rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(rendererData, rendererPath);
        }

        UniversalRenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelineAssetPath);
        if (pipelineAsset == null)
        {
            pipelineAsset = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(pipelineAsset, pipelineAssetPath);
        }

        GraphicsSettings.defaultRenderPipeline = pipelineAsset;
        int currentLevel = QualitySettings.GetQualityLevel();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = pipelineAsset;
        }
        QualitySettings.SetQualityLevel(currentLevel, false);
        AssetDatabase.SaveAssets();
    }

    static Material GetOrCreateParticleMaterial(string path)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            mat = new Material(shader);
            Texture2D defaultTex = AssetDatabase.GetBuiltinExtraResource<Texture2D>("Default-Particle.psd");
            if (defaultTex != null) mat.mainTexture = defaultTex;
            mat.SetFloat("_Surface", 1.0f);
            mat.SetFloat("_Blend", 0.0f);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = (int)RenderQueue.Transparent;
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

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
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", defaultColor);
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", defaultColor);
            mat.color = defaultColor;
        }
        return mat;
    }

    static Mesh GetPrimitiveMesh(PrimitiveType type)
    {
        GameObject temp = GameObject.CreatePrimitive(type);
        Mesh mesh = temp.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(temp);
        return mesh;
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

    // ===== SCREENSHOT CAPTURE UTILITIES =====
    [MenuItem("XENOASIS/Capture Chamber View", false, 50)]
    public static void CaptureChamberView()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogError("[XENOASIS] No active camera found to capture!");
            return;
        }

        Vector3 origPos = cam.transform.position;
        Quaternion origRot = cam.transform.rotation;

        cam.transform.position = new Vector3(0, 1.4f, -3.5f);
        cam.transform.rotation = Quaternion.Euler(4f, 0, 0);

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;
        RenderTexture prevActive = RenderTexture.active;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        cam.targetTexture = prevRT;
        RenderTexture.active = prevActive;
        cam.transform.position = origPos;
        cam.transform.rotation = origRot;

        byte[] bytes = screenShot.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(screenShot);

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "chamber_interior_view.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("[XENOASIS] Chamber interior view captured successfully to " + path);
    }

    [MenuItem("XENOASIS/Capture Cockpit View", false, 51)]
    public static void CaptureCockpitView()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogError("[XENOASIS] No active camera found to capture!");
            return;
        }

        Vector3 origPos = cam.transform.position;
        Quaternion origRot = cam.transform.rotation;

        // Seated in pilot command chair looking over wrap-around console & MFDs at Earth
        cam.transform.position = new Vector3(0, 601.25f, 0.20f);
        cam.transform.rotation = Quaternion.Euler(10f, 0, 0);

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;
        RenderTexture prevActive = RenderTexture.active;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        cam.targetTexture = prevRT;
        RenderTexture.active = prevActive;
        cam.transform.position = origPos;
        cam.transform.rotation = origRot;

        byte[] bytes = screenShot.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(screenShot);

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "cockpit_orbit_view.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("[XENOASIS] Pilot seat view captured successfully to " + path);
    }

    [MenuItem("XENOASIS/Capture Cockpit Wide View", false, 52)]
    public static void CaptureCockpitWideView()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogError("[XENOASIS] No active camera found to capture!");
            return;
        }

        Vector3 origPos = cam.transform.position;
        Quaternion origRot = cam.transform.rotation;

        // Rear bridge overview looking down at command chair, consoles, ribs & canopy
        Vector3 targetPos = new Vector3(0, 601.05f, 1.3f);
        Vector3 camPos = new Vector3(0, 601.85f, -1.05f);
        cam.transform.position = camPos;
        cam.transform.rotation = Quaternion.LookRotation((targetPos - camPos).normalized, Vector3.up);

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;
        RenderTexture prevActive = RenderTexture.active;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        cam.targetTexture = prevRT;
        RenderTexture.active = prevActive;
        cam.transform.position = origPos;
        cam.transform.rotation = origRot;

        byte[] bytes = screenShot.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(screenShot);

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "cockpit_wide_view.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("[XENOASIS] Cockpit wide view captured successfully to " + path);
    }

    [MenuItem("XENOASIS/Capture UFO Exterior View", false, 53)]
    public static void CaptureUFOExteriorView()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogError("[XENOASIS] No active camera found to capture!");
            return;
        }

        Vector3 origPos = cam.transform.position;
        Quaternion origRot = cam.transform.rotation;

        // Close-up detailed exterior angle looking at the hovering UFO Mothership
        Vector3 targetPos = new Vector3(0, 32.5f, 15f);
        Vector3 ufoCamPos = new Vector3(14f, 35.5f, 3f);
        cam.transform.position = ufoCamPos;
        cam.transform.rotation = Quaternion.LookRotation((targetPos - ufoCamPos).normalized, Vector3.up);

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;
        RenderTexture prevActive = RenderTexture.active;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        cam.targetTexture = prevRT;
        RenderTexture.active = prevActive;
        cam.transform.position = origPos;
        cam.transform.rotation = origRot;

        byte[] bytes = screenShot.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(screenShot);

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "ufo_exterior_view.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("[XENOASIS] UFO exterior view captured successfully to " + path);
    }

    [MenuItem("XENOASIS/Capture All Biomes", false, 52)]
    public static void CaptureAllBiomes()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogError("[XENOASIS] No active camera found to capture!");
            return;
        }

        Vector3 origPos = cam.transform.position;
        Quaternion origRot = cam.transform.rotation;

        cam.transform.position = new Vector3(0, 1.4f, -3.5f);
        cam.transform.rotation = Quaternion.Euler(4f, 0, 0);

        var btc = UnityEngine.Object.FindObjectOfType<BiomeTransitionController>();
        SerializedObject so = btc != null ? new SerializedObject(btc) : null;
        Texture2D lakeTex = so != null ? so.FindProperty("lakeVistaTex").objectReferenceValue as Texture2D : null;
        Texture2D mtnTex = so != null ? so.FindProperty("mountainVistaTex").objectReferenceValue as Texture2D : null;
        Texture2D forestTex = so != null ? so.FindProperty("forestVistaTex").objectReferenceValue as Texture2D : null;
        MeshRenderer vistaMR = so != null ? so.FindProperty("curvedVistaRenderer").objectReferenceValue as MeshRenderer : null;
        Light dirLight = so != null ? so.FindProperty("directionalLight").objectReferenceValue as Light : null;

        var biomes = new (string name, Texture2D tex, Color amb, float lightInt, Color lightCol)[] {
            ("biome_1_lake", lakeTex, new Color(0.12f, 0.28f, 0.38f), 1.0f, new Color(0.75f, 0.92f, 1.00f)),
            ("biome_2_mountain", mtnTex, new Color(0.22f, 0.25f, 0.42f), 1.2f, new Color(0.90f, 0.95f, 1.00f)),
            ("biome_3_forest", forestTex, new Color(0.15f, 0.32f, 0.18f), 0.9f, new Color(0.98f, 0.94f, 0.82f))
        };

        int width = 1280;
        int height = 720;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;
        RenderTexture prevActive = RenderTexture.active;
        cam.targetTexture = rt;

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        foreach (var b in biomes)
        {
            if (vistaMR != null && b.tex != null)
            {
                vistaMR.sharedMaterial.SetTexture("_MainTex", b.tex);
                vistaMR.sharedMaterial.SetFloat("_BlendFactor", 0f);
            }
            RenderSettings.ambientLight = b.amb;
            if (dirLight != null)
            {
                dirLight.intensity = b.lightInt;
                dirLight.color = b.lightCol;
            }

            cam.Render();
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            byte[] bytes = screenShot.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(screenShot);

            string path = Path.Combine(dir, b.name + ".png");
            File.WriteAllBytes(path, bytes);
            Debug.Log($"[XENOASIS] Biome screenshot captured: {path}");
        }

        cam.targetTexture = prevRT;
        RenderTexture.active = prevActive;
        cam.transform.position = origPos;
        cam.transform.rotation = origRot;
        UnityEngine.Object.DestroyImmediate(rt);
    }

    public static void BuildGrandMuseumPavilion(GameObject chamber, Material darkWallMat, Material whiteWallMat, Material daisMat, Material luxuryFloorMat, Material glassMat, Material cyanGlowMat)
    {
        // 1. Root Grand Museum Pavilion Object
        Transform existingPavilion = chamber.transform.Find("GrandMuseumPavilion_Architecture");
        if (existingPavilion != null) DestroyImmediate(existingPavilion.gameObject);

        GameObject pavilion = new GameObject("GrandMuseumPavilion_Architecture");
        pavilion.transform.SetParent(chamber.transform, false);

        float rotundaRadius = 18.5f;
        float wallHeight = 8.0f;
        float deckRadius = 20.8f;
        float apertureRadius = 5.5f;
        float plinthRadius = 23.0f;

        // 2. Massive Tiered Foundation Plinth (Anchors building firmly to the lake bed)
        GameObject foundation = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        foundation.name = "Pavilion_TieredFoundation";
        foundation.transform.SetParent(pavilion.transform, false);
        foundation.transform.localPosition = new Vector3(0, -0.40f, 0);
        foundation.transform.localScale = new Vector3(plinthRadius * 2f, 0.40f, plinthRadius * 2f);
        foundation.GetComponent<MeshRenderer>().sharedMaterial = daisMat;

        GameObject fTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fTrim.name = "Foundation_CyanGlowTrim";
        fTrim.transform.SetParent(foundation.transform, false);
        fTrim.transform.localPosition = new Vector3(0, 0.52f, 0);
        fTrim.transform.localScale = new Vector3(1.01f, 0.05f, 1.01f);
        fTrim.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        // 3. Expanded Luxury Terrazzo Floor (37m diameter)
        GameObject floorObj = CreateChildOrFind(chamber, "Museum_Floor_Foundation");
        floorObj.transform.localPosition = new Vector3(0, -0.02f, 0);
        floorObj.transform.localScale = new Vector3(rotundaRadius * 2.02f, 0.05f, rotundaRadius * 2.02f);
        var floorMF = AddComponentIfMissing<MeshFilter>(floorObj);
        floorMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Cylinder);
        var floorMR = AddComponentIfMissing<MeshRenderer>(floorObj);
        floorMR.sharedMaterial = luxuryFloorMat;
        AddComponentIfMissing<MeshCollider>(floorObj);

        // Clean up legacy children of floor if any
        for (int i = floorObj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(floorObj.transform.GetChild(i).gameObject);
        }

        // Concentric Cyan Illuminated Rings on Floor
        string[] rNames = { "Floor_GlowRing_Outer", "Floor_GlowRing_Mid", "Floor_GlowRing_Inner" };
        float[] rRadii = { 17.5f, 11.5f, 5.4f };
        for (int i = 0; i < 3; i++)
        {
            GameObject ring = new GameObject(rNames[i]);
            ring.transform.SetParent(floorObj.transform, false);
            ring.transform.localPosition = new Vector3(0, 0.55f + i * 0.02f, 0);
            var mf = ring.AddComponent<MeshFilter>();
            var mr = ring.AddComponent<MeshRenderer>();
            float r = rRadii[i];
            mf.sharedMesh = CreateRingMesh(r - 0.08f, r + 0.08f, 48);
            mr.sharedMaterial = cyanGlowMat;
        }

        // 4. Central Landing Dais Plinth (Arrival Touchdown Point for Tractor Beam)
        GameObject daisObj = CreateChildOrFind(chamber, "Landing_Dais_Plinth");
        daisObj.transform.localPosition = new Vector3(0, 0.08f, 0);
        daisObj.transform.localScale = new Vector3(10.5f, 0.16f, 10.5f);
        var daisMF = AddComponentIfMissing<MeshFilter>(daisObj);
        daisMF.sharedMesh = GetPrimitiveMesh(PrimitiveType.Cylinder);
        var daisMR = AddComponentIfMissing<MeshRenderer>(daisObj);
        daisMR.sharedMaterial = daisMat;
        AddComponentIfMissing<MeshCollider>(daisObj);

        // Clean up legacy children of dais
        for (int i = daisObj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(daisObj.transform.GetChild(i).gameObject);
        }

        GameObject daisGlow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisGlow.name = "Dais_GlowRim";
        daisGlow.transform.SetParent(daisObj.transform, false);
        daisGlow.transform.localPosition = new Vector3(0, 0.52f, 0);
        daisGlow.transform.localScale = new Vector3(1.03f, 0.04f, 1.03f);
        daisGlow.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        // 5. Perimeter Colonnade & 360-degree Enclosed Walls (16 bays)
        int segments = 16;
        float halfStep = 360f / segments;
        GameObject colonnadeObj = new GameObject("Pavilion_Colonnade");
        colonnadeObj.transform.SetParent(pavilion.transform, false);

        for (int i = 0; i < segments; i++)
        {
            float colAngle = i * halfStep * Mathf.Deg2Rad;
            Vector3 colPos = new Vector3(Mathf.Sin(colAngle) * rotundaRadius, wallHeight * 0.5f, Mathf.Cos(colAngle) * rotundaRadius);

            // Monumental Column
            GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            col.name = $"Column_{i:D2}";
            col.transform.SetParent(colonnadeObj.transform, false);
            col.transform.localPosition = colPos;
            col.transform.localScale = new Vector3(0.90f, wallHeight * 0.5f, 0.90f);
            col.GetComponent<MeshRenderer>().sharedMaterial = whiteWallMat;

            // Cyan Conduit on column
            GameObject cGlow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cGlow.name = "Column_Conduit";
            cGlow.transform.SetParent(col.transform, false);
            cGlow.transform.localPosition = new Vector3(0, 0, -0.48f);
            cGlow.transform.localScale = new Vector3(0.28f, 0.98f, 0.15f);
            cGlow.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
            var cCol = cGlow.GetComponent<Collider>();
            if (cCol != null) DestroyImmediate(cCol);

            // Bay between i and i+1
            float midAngleDeg = (i + 0.5f) * halfStep;
            float midAngleRad = midAngleDeg * Mathf.Deg2Rad;
            Vector3 bayPos = new Vector3(Mathf.Sin(midAngleRad) * rotundaRadius, wallHeight * 0.5f, Mathf.Cos(midAngleRad) * rotundaRadius);
            float chordWidth = 2f * rotundaRadius * Mathf.Sin((halfStep * 0.5f) * Mathf.Deg2Rad) * 1.02f;

            bool isWindow = (i == 0 || i == 4 || i == 8 || i == 12);
            if (isWindow)
            {
                // Panoramic Observation Window Bay
                GameObject bay = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bay.name = $"PanoramicWindowBay_{i:D2}";
                bay.transform.SetParent(colonnadeObj.transform, false);
                bay.transform.localPosition = bayPos + new Vector3(0, wallHeight * 0.08f, 0);
                bay.transform.localRotation = Quaternion.Euler(0, midAngleDeg, 0);
                bay.transform.localScale = new Vector3(chordWidth, wallHeight * 0.84f, 0.08f);
                bay.GetComponent<MeshRenderer>().sharedMaterial = glassMat;

                // Window Sill Balustrade
                GameObject sill = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sill.name = "WindowSill";
                sill.transform.SetParent(colonnadeObj.transform, false);
                sill.transform.localPosition = new Vector3(Mathf.Sin(midAngleRad) * rotundaRadius, wallHeight * 0.08f, Mathf.Cos(midAngleRad) * rotundaRadius);
                sill.transform.localRotation = Quaternion.Euler(0, midAngleDeg, 0);
                sill.transform.localScale = new Vector3(chordWidth, wallHeight * 0.16f, 0.65f);
                sill.GetComponent<MeshRenderer>().sharedMaterial = darkWallMat;

                GameObject sGlow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sGlow.name = "Sill_GlowTrim";
                sGlow.transform.SetParent(sill.transform, false);
                sGlow.transform.localPosition = new Vector3(0, 0.52f, 0);
                sGlow.transform.localScale = new Vector3(1.0f, 0.05f, 0.15f);
                sGlow.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
            }
            else
            {
                // Solid Double-Sided Titanium Wall Bay (35cm thick solid wall — zero transparency!)
                GameObject bay = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bay.name = $"GalleryWallBay_{i:D2}";
                bay.transform.SetParent(colonnadeObj.transform, false);
                bay.transform.localPosition = bayPos;
                bay.transform.localRotation = Quaternion.Euler(0, midAngleDeg, 0);
                bay.transform.localScale = new Vector3(chordWidth, wallHeight, 0.35f);
                bay.GetComponent<MeshRenderer>().sharedMaterial = whiteWallMat;

                // Horizontal Cyan Conduit Groove on Inside
                GameObject groove = GameObject.CreatePrimitive(PrimitiveType.Cube);
                groove.name = "CyanGroove_Inside";
                groove.transform.SetParent(bay.transform, false);
                groove.transform.localPosition = new Vector3(0, -0.15f, -0.52f);
                groove.transform.localScale = new Vector3(0.98f, 0.035f, 0.10f);
                groove.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
                var gCol = groove.GetComponent<Collider>();
                if (gCol != null) DestroyImmediate(gCol);

                // Upper Cove Trim
                GameObject cove = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cove.name = "CoveGlowTrim";
                cove.transform.SetParent(bay.transform, false);
                cove.transform.localPosition = new Vector3(0, 0.47f, -0.52f);
                cove.transform.localScale = new Vector3(1.0f, 0.04f, 0.15f);
                cove.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
                var cvCol = cove.GetComponent<Collider>();
                if (cvCol != null) DestroyImmediate(cvCol);
            }
        }

        // 6. Sweeping Exterior Aerodynamic Buttresses (8 radial wings capped cleanly under roof overhang)
        GameObject buttressGroup = new GameObject("Pavilion_ExteriorButtresses");
        buttressGroup.transform.SetParent(pavilion.transform, false);

        for (int i = 0; i < 8; i++)
        {
            float angleDeg = i * 45f;
            float rad = angleDeg * Mathf.Deg2Rad;
            float midR = rotundaRadius + 1.25f;
            Vector3 bPos = new Vector3(Mathf.Sin(rad) * midR, wallHeight * 0.42f, Mathf.Cos(rad) * midR);

            GameObject buttress = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttress.name = $"ButtressPylon_{i:D2}";
            buttress.transform.SetParent(buttressGroup.transform, false);
            buttress.transform.localPosition = bPos;
            buttress.transform.rotation = Quaternion.LookRotation(new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad))) * Quaternion.Euler(14f, 0, 0);
            buttress.transform.localScale = new Vector3(1.4f, wallHeight * 0.90f, 1.8f);
            buttress.GetComponent<MeshRenderer>().sharedMaterial = whiteWallMat;

            GameObject bRib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bRib.name = "Buttress_GlowRib";
            bRib.transform.SetParent(buttress.transform, false);
            bRib.transform.localPosition = new Vector3(0, 0, 0.52f);
            bRib.transform.localScale = new Vector3(0.35f, 0.98f, 0.15f);
            bRib.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
            var brCol = bRib.GetComponent<Collider>();
            if (brCol != null) DestroyImmediate(brCol);

            // Capital Load Bracket tucked cleanly beneath roof soffit
            GameObject bCap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bCap.name = "Buttress_CapitalBracket";
            bCap.transform.SetParent(buttress.transform, false);
            bCap.transform.localPosition = new Vector3(0, 0.48f, 0.18f);
            bCap.transform.localScale = new Vector3(1.15f, 0.10f, 1.1f);
            bCap.GetComponent<MeshRenderer>().sharedMaterial = darkWallMat;
            var bcCol = bCap.GetComponent<Collider>();
            if (bcCol != null) DestroyImmediate(bcCol);
        }

        // 7. Solid 3D Architectural Roof Canopy Slab (60cm thick solid manifold ring slab)
        float yBottom = wallHeight - 0.12f; // 7.88m
        float yTop = wallHeight + 0.48f;    // 8.48m

        Mesh roofSlab = Create3DSlabRingMesh(apertureRadius, deckRadius, yBottom, yTop, 48);
        GameObject roofSlabObj = new GameObject("Pavilion_RoofCanopySlab");
        roofSlabObj.transform.SetParent(pavilion.transform, false);
        var rsMF = roofSlabObj.AddComponent<MeshFilter>();
        rsMF.sharedMesh = roofSlab;
        var rsMR = roofSlabObj.AddComponent<MeshRenderer>();
        rsMR.sharedMaterial = whiteWallMat;

        // --- 7A. SOLID ARCHITECTURAL PARAPET OUTER RIM (Curbing outer perimeter) ---
        float parapetInR = deckRadius - 0.40f; // 20.40m
        float parapetOutR = deckRadius + 0.22f; // 21.02m
        float parapetTop = yTop + 0.36f; // 8.84m

        GameObject parapetObj = new GameObject("Pavilion_Roof_Parapet_OuterRim");
        parapetObj.transform.SetParent(pavilion.transform, false);
        var parMF = parapetObj.AddComponent<MeshFilter>();
        parMF.sharedMesh = Create3DSlabRingMesh(parapetInR, parapetOutR, yTop, parapetTop, 48);
        var parMR = parapetObj.AddComponent<MeshRenderer>();
        parMR.sharedMaterial = darkWallMat;

        // Outer Parapet Cyan Beacon Ring
        GameObject parapetBeacon = new GameObject("Roof_Parapet_BeaconRing");
        parapetBeacon.transform.SetParent(pavilion.transform, false);
        parapetBeacon.transform.localPosition = new Vector3(0, parapetTop + 0.02f, 0);
        var pbMF = parapetBeacon.AddComponent<MeshFilter>();
        pbMF.sharedMesh = CreateRingMesh(parapetInR + 0.08f, parapetOutR - 0.08f, 48);
        var pbMR = parapetBeacon.AddComponent<MeshRenderer>();
        pbMR.sharedMaterial = cyanGlowMat;

        // --- 7B. SOLID RAISED APERTURE COLLAR BEZEL (Framing center skylight) ---
        float collarInR = apertureRadius - 0.12f; // 5.38m
        float collarOutR = apertureRadius + 0.45f; // 5.95m
        float collarTop = yTop + 0.30f; // 8.78m

        GameObject collarObj = new GameObject("Pavilion_Roof_Aperture_CollarBezel");
        collarObj.transform.SetParent(pavilion.transform, false);
        var colMF = collarObj.AddComponent<MeshFilter>();
        colMF.sharedMesh = Create3DSlabRingMesh(collarInR, collarOutR, yTop, collarTop, 48);
        var colMR = collarObj.AddComponent<MeshRenderer>();
        colMR.sharedMaterial = darkWallMat;

        // Collar Cyan Glow Ring
        GameObject collarGlow = new GameObject("Roof_Aperture_CollarGlow");
        collarGlow.transform.SetParent(pavilion.transform, false);
        collarGlow.transform.localPosition = new Vector3(0, collarTop + 0.02f, 0);
        var cgMF = collarGlow.AddComponent<MeshFilter>();
        cgMF.sharedMesh = CreateRingMesh(collarInR + 0.06f, collarOutR - 0.06f, 48);
        var cgMR = collarGlow.AddComponent<MeshRenderer>();
        cgMR.sharedMaterial = cyanGlowMat;

        // Bottom Aperture Ceiling Glow Ring (Visible from inside the rotunda looking up)
        GameObject btmApertureGlow = new GameObject("ApertureGlow_Bottom");
        btmApertureGlow.transform.SetParent(pavilion.transform, false);
        btmApertureGlow.transform.localPosition = new Vector3(0, yBottom - 0.03f, 0);
        var bagMF = btmApertureGlow.AddComponent<MeshFilter>();
        bagMF.sharedMesh = CreateRingMesh(apertureRadius - 0.12f, apertureRadius + 0.22f, 48);
        var bagMR = btmApertureGlow.AddComponent<MeshRenderer>();
        bagMR.sharedMaterial = cyanGlowMat;

        // --- 7C. 16 RADIAL TITANIUM DECK DIVIDERS (Engineering structural sectors) ---
        GameObject dividersGroup = new GameObject("Pavilion_Roof_RadialDividers");
        dividersGroup.transform.SetParent(pavilion.transform, false);

        float divLen = parapetInR - collarOutR; // 20.40m - 5.95m = 14.45m
        float divMidR = (collarOutR + parapetInR) * 0.5f; // 13.175m

        for (int d = 0; d < 16; d++)
        {
            float divDeg = d * (360f / 16f);
            float divRad = divDeg * Mathf.Deg2Rad;
            Vector3 divPos = new Vector3(Mathf.Sin(divRad) * divMidR, yTop + 0.035f, Mathf.Cos(divRad) * divMidR);

            GameObject divider = GameObject.CreatePrimitive(PrimitiveType.Cube);
            divider.name = $"RoofDivider_{d:D2}";
            divider.transform.SetParent(dividersGroup.transform, false);
            divider.transform.localPosition = divPos;
            divider.transform.localRotation = Quaternion.Euler(0, divDeg, 0);
            divider.transform.localScale = new Vector3(0.32f, 0.07f, divLen);
            divider.GetComponent<MeshRenderer>().sharedMaterial = darkWallMat;
            var dCol = divider.GetComponent<Collider>();
            if (dCol != null) DestroyImmediate(dCol);

            // Thin illuminated cyan accent conduit on top of each divider
            GameObject dTrim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dTrim.name = "Divider_CyanConduit";
            dTrim.transform.SetParent(divider.transform, false);
            dTrim.transform.localPosition = new Vector3(0, 0.52f, 0);
            dTrim.transform.localScale = new Vector3(0.20f, 0.10f, 0.98f);
            dTrim.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;
            var dtCol = dTrim.GetComponent<Collider>();
            if (dtCol != null) DestroyImmediate(dtCol);
        }

        // 8. Grand Geodesic Crystal Glass Dome Crowning the Raised Aperture Collar
        GameObject grandDome = new GameObject("Pavilion_CrowningGlassDome");
        grandDome.transform.SetParent(pavilion.transform, false);
        grandDome.transform.localPosition = new Vector3(0, collarTop - 0.02f, 0);
        var gdMF = grandDome.AddComponent<MeshFilter>();
        gdMF.sharedMesh = CreateDomeMesh(apertureRadius * 1.02f, 3.0f, 16, 48);
        var gdMR = grandDome.AddComponent<MeshRenderer>();
        gdMR.sharedMaterial = glassMat;
    }

    public static Mesh Create3DSlabRingMesh(float innerRadius, float outerRadius, float yBottom, float yTop, int segments)
    {
        Mesh mesh = new Mesh { name = "Rotunda_3DSlabRingMesh" };
        var verts = new System.Collections.Generic.List<Vector3>();
        var norms = new System.Collections.Generic.List<Vector3>();
        var uvs = new System.Collections.Generic.List<Vector2>();
        var tris = new System.Collections.Generic.List<int>();

        // 1. Bottom Face (normals strictly down -Y, visible from inside the hall looking up)
        int bStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);

            verts.Add(new Vector3(s * innerRadius, yBottom, c * innerRadius));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(s * innerRadius / (outerRadius * 2f) + 0.5f, c * innerRadius / (outerRadius * 2f) + 0.5f));

            verts.Add(new Vector3(s * outerRadius, yBottom, c * outerRadius));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(s * 0.5f + 0.5f, c * 0.5f + 0.5f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = bStart + i * 2;
            tris.Add(vi); tris.Add(vi + 2); tris.Add(vi + 1);
            tris.Add(vi + 1); tris.Add(vi + 2); tris.Add(vi + 3);
        }

        // 2. Top Face (normals strictly up +Y, visible from outside/above during descent)
        int tStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);

            verts.Add(new Vector3(s * innerRadius, yTop, c * innerRadius));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(s * innerRadius / (outerRadius * 2f) + 0.5f, c * innerRadius / (outerRadius * 2f) + 0.5f));

            verts.Add(new Vector3(s * outerRadius, yTop, c * outerRadius));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(s * 0.5f + 0.5f, c * 0.5f + 0.5f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = tStart + i * 2;
            tris.Add(vi); tris.Add(vi + 1); tris.Add(vi + 2);
            tris.Add(vi + 1); tris.Add(vi + 3); tris.Add(vi + 2);
        }

        // 3. Inner Rim (aperture cylinder wall, normals inward toward center)
        int irStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            Vector3 inNorm = new Vector3(-s, 0, -c);

            verts.Add(new Vector3(s * innerRadius, yBottom, c * innerRadius));
            norms.Add(inNorm);
            uvs.Add(new Vector2(norm * 4f, 0f));

            verts.Add(new Vector3(s * innerRadius, yTop, c * innerRadius));
            norms.Add(inNorm);
            uvs.Add(new Vector2(norm * 4f, 1f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = irStart + i * 2;
            tris.Add(vi); tris.Add(vi + 1); tris.Add(vi + 2);
            tris.Add(vi + 1); tris.Add(vi + 3); tris.Add(vi + 2);
        }

        // 4. Outer Rim (perimeter cylinder wall, normals outward away from center)
        int orStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            Vector3 outNorm = new Vector3(s, 0, c);

            verts.Add(new Vector3(s * outerRadius, yBottom, c * outerRadius));
            norms.Add(outNorm);
            uvs.Add(new Vector2(norm * 8f, 0f));

            verts.Add(new Vector3(s * outerRadius, yTop, c * outerRadius));
            norms.Add(outNorm);
            uvs.Add(new Vector2(norm * 8f, 1f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = orStart + i * 2;
            tris.Add(vi); tris.Add(vi + 2); tris.Add(vi + 1);
            tris.Add(vi + 1); tris.Add(vi + 2); tris.Add(vi + 3);
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateDomeMesh(float radius, float domeHeight, int rings, int segments)
    {
        Mesh mesh = new Mesh { name = "Rotunda_DomeMesh" };
        var verts = new System.Collections.Generic.List<Vector3>();
        var norms = new System.Collections.Generic.List<Vector3>();
        var uvs = new System.Collections.Generic.List<Vector2>();
        var tris = new System.Collections.Generic.List<int>();

        // 1. Inward facing dome (visible from inside hall looking up)
        int inStart = verts.Count;
        for (int r = 0; r <= rings; r++)
        {
            float rNorm = (float)r / rings;
            float phi = rNorm * Mathf.PI * 0.5f;
            float cosPhi = Mathf.Cos(phi);
            float sinPhi = Mathf.Sin(phi);
            float ringR = radius * cosPhi;
            float y = domeHeight * sinPhi;

            for (int s = 0; s <= segments; s++)
            {
                float sNorm = (float)s / segments;
                float theta = sNorm * Mathf.PI * 2f;
                float x = Mathf.Sin(theta) * ringR;
                float z = Mathf.Cos(theta) * ringR;

                Vector3 p = new Vector3(x, y, z);
                verts.Add(p);
                norms.Add(-p.normalized);
                uvs.Add(new Vector2(sNorm, rNorm));
            }
        }
        for (int r = 0; r < rings; r++)
        {
            for (int s = 0; s < segments; s++)
            {
                int bl = inStart + r * (segments + 1) + s;
                int br = bl + 1;
                int tl = inStart + (r + 1) * (segments + 1) + s;
                int tr = tl + 1;

                tris.Add(bl); tris.Add(br); tris.Add(tl);
                tris.Add(br); tris.Add(tr); tris.Add(tl);
            }
        }

        // 2. Outward facing dome (visible from outside/above during descent)
        int outStart = verts.Count;
        for (int r = 0; r <= rings; r++)
        {
            float rNorm = (float)r / rings;
            float phi = rNorm * Mathf.PI * 0.5f;
            float cosPhi = Mathf.Cos(phi);
            float sinPhi = Mathf.Sin(phi);
            float ringR = radius * cosPhi;
            float y = domeHeight * sinPhi;

            for (int s = 0; s <= segments; s++)
            {
                float sNorm = (float)s / segments;
                float theta = sNorm * Mathf.PI * 2f;
                float x = Mathf.Sin(theta) * ringR;
                float z = Mathf.Cos(theta) * ringR;

                Vector3 p = new Vector3(x, y, z);
                verts.Add(p);
                norms.Add(p.normalized);
                uvs.Add(new Vector2(sNorm, rNorm));
            }
        }
        for (int r = 0; r < rings; r++)
        {
            for (int s = 0; s < segments; s++)
            {
                int bl = outStart + r * (segments + 1) + s;
                int br = bl + 1;
                int tl = outStart + (r + 1) * (segments + 1) + s;
                int tr = tl + 1;

                tris.Add(bl); tris.Add(tl); tris.Add(br);
                tris.Add(br); tris.Add(tl); tris.Add(tr);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateRingMesh(float innerRadius, float outerRadius, int segments)
    {
        Mesh mesh = new Mesh { name = "Rotunda_RingMesh" };
        var verts = new System.Collections.Generic.List<Vector3>();
        var norms = new System.Collections.Generic.List<Vector3>();
        var uvs = new System.Collections.Generic.List<Vector2>();
        var tris = new System.Collections.Generic.List<int>();

        // Downward facing (-Y)
        int downStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            verts.Add(new Vector3(s * innerRadius, 0f, c * innerRadius));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(norm, 0f));

            verts.Add(new Vector3(s * outerRadius, 0f, c * outerRadius));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(norm, 1f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = downStart + i * 2;
            tris.Add(vi); tris.Add(vi + 1); tris.Add(vi + 2);
            tris.Add(vi + 1); tris.Add(vi + 3); tris.Add(vi + 2);
        }

        // Upward facing (+Y)
        int upStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            verts.Add(new Vector3(s * innerRadius, 0f, c * innerRadius));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(norm, 0f));

            verts.Add(new Vector3(s * outerRadius, 0f, c * outerRadius));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(norm, 1f));
        }
        for (int i = 0; i < segments; i++)
        {
            int vi = upStart + i * 2;
            tris.Add(vi); tris.Add(vi + 2); tris.Add(vi + 1);
            tris.Add(vi + 1); tris.Add(vi + 2); tris.Add(vi + 3);
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh GetOrCreateCleanMuseumMesh(string glbPath, string cleanAssetPath, float scale, Vector3 offset)
    {
        Mesh cleanMesh = AssetDatabase.LoadAssetAtPath<Mesh>(cleanAssetPath);
        if (cleanMesh != null) return cleanMesh;

        GameObject glbObj = AssetDatabase.LoadAssetAtPath<GameObject>(glbPath);
        if (glbObj == null) return null;
        var mf = glbObj.GetComponentInChildren<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return null;

        var origMesh = mf.sharedMesh;
        var verts = origMesh.vertices;
        var normals = origMesh.normals;
        var uvs = origMesh.uv;
        var origTris = origMesh.triangles;

        var newTris = new System.Collections.Generic.List<int>();
        for (int i = 0; i < origTris.Length; i += 3)
        {
            int i0 = origTris[i];
            int i1 = origTris[i + 1];
            int i2 = origTris[i + 2];
            var c = (verts[i0] + verts[i1] + verts[i2]) / 3f;
            Vector3 wp = new Vector3(c.x * scale + offset.x, c.y * scale + offset.y, c.z * scale + offset.z);
            float r = Mathf.Sqrt(wp.x * wp.x + wp.z * wp.z);

            // Clear ground floor hall and upper rotunda clutter (r < 9.8m, 0.05m < y < 17.5m)
            if (r < 9.8f && wp.y > 0.05f && wp.y < 17.5f) continue;

            // Clear central diaphragm below dome (r < 5.0m, 17.0m <= y <= 19.2m)
            if (r < 5.0f && wp.y >= 17.0f && wp.y <= 19.2f) continue;

            // Clear floor inside room
            if (r < 10.5f && wp.y <= 0.05f) continue;

            newTris.Add(i0);
            newTris.Add(i1);
            newTris.Add(i2);
        }

        cleanMesh = new Mesh();
        cleanMesh.name = "museum_rotunda_masterpiece_clean";
        cleanMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        cleanMesh.vertices = verts;
        cleanMesh.normals = normals;
        cleanMesh.uv = uvs;
        cleanMesh.triangles = newTris.ToArray();
        cleanMesh.RecalculateBounds();

        AssetDatabase.CreateAsset(cleanMesh, cleanAssetPath);
        AssetDatabase.SaveAssets();
        return cleanMesh;
    }
#endif
}
