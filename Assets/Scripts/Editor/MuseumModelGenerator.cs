using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;

/// <summary>
/// XENOASIS — MuseumModelGenerator.cs
/// Procedurally builds the 5 interactive exhibit pavilions for the Museum of Humanity
/// inside the Terran Embassy Chamber:
/// 1. Pavilion 1: The Voyager Golden Record (Levitating disc, needle, pulsar hologram)
/// 2. Pavilion 2: The Rosetta Stone & Floating Art Masterpieces Gallery
/// 3. Pavilion 3: The Spark of Prometheus (Handaxe, Wheel & Silicon Microprocessor)
/// 4. Pavilion 4: The Svalbard Global Seed Vault (Cryo vials & holographic sprouter)
/// 5. Pavilion 5: Apollo 11 Lunar Bootprint & Alien Diplomatic Peace Scanner
/// </summary>
public static class MuseumModelGenerator
{
    private const string PrefabDir = "Assets/Prefabs/Museum";

    [MenuItem("XENOASIS/Build Museum of Humanity Exhibits")]
    public static void BuildAllMuseumExhibits()
    {
        Debug.Log("[XENOASIS] Building 3D Museum of Humanity exhibits...");

        if (!Directory.Exists(PrefabDir)) Directory.CreateDirectory(PrefabDir);
        MuseumTextureGenerator.GenerateAllMuseumTextures();

        BuildExhibit1_GoldenRecord();
        BuildExhibit2_RosettaArt();
        BuildExhibit3_PrometheusSilicon();
        BuildExhibit4_SvalbardSeedVault();
        BuildExhibit5_ApolloLunarPlaque();

        AssetDatabase.Refresh();
        Debug.Log("[XENOASIS] All 5 Museum of Humanity exhibit prefabs built successfully!");
    }

    private static Material GetMat(string path, Color fallbackColor)
    {
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            m.color = fallbackColor;
        }
        return m;
    }

    private static GameObject CreatePedestal(string name, Material darkMat, Material cyanGlowMat, Material goldGlowMat, float radius = 1.35f, float height = 0.85f)
    {
        GameObject root = new GameObject(name);

        // Hexagonal / Cylindrical Base Plinth
        GameObject basePlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePlinth.name = "Plinth_Base";
        basePlinth.transform.SetParent(root.transform);
        basePlinth.transform.localPosition = new Vector3(0, height * 0.5f, 0);
        basePlinth.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
        basePlinth.GetComponent<MeshRenderer>().sharedMaterial = darkMat;

        // Glowing Trim Ring
        GameObject glowRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        glowRing.name = "Plinth_GlowRing";
        glowRing.transform.SetParent(root.transform);
        glowRing.transform.localPosition = new Vector3(0, height + 0.01f, 0);
        glowRing.transform.localScale = new Vector3(radius * 1.95f, 0.02f, radius * 1.95f);
        glowRing.GetComponent<MeshRenderer>().sharedMaterial = cyanGlowMat;

        // Display Table Top
        GameObject topPlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        topPlinth.name = "Plinth_Top";
        topPlinth.transform.SetParent(root.transform);
        topPlinth.transform.localPosition = new Vector3(0, height + 0.04f, 0);
        topPlinth.transform.localScale = new Vector3(radius * 1.8f, 0.04f, radius * 1.8f);
        topPlinth.GetComponent<MeshRenderer>().sharedMaterial = darkMat;

        return root;
    }

    private static void AddPlacard(GameObject exhibit, string title, string era, string desc, Material darkMat, Material cyanGlowMat, float radius = 1.35f, float height = 0.85f)
    {
        GameObject placardStand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placardStand.name = "Placard_Stand";
        placardStand.transform.SetParent(exhibit.transform, false);
        placardStand.transform.localPosition = new Vector3(0, height * 0.72f, -radius * 1.05f);
        placardStand.transform.localRotation = Quaternion.Euler(-30f, 0, 0);
        placardStand.transform.localScale = new Vector3(1.10f, 0.65f, 0.06f);
        placardStand.GetComponent<MeshRenderer>().sharedMaterial = darkMat;

        GameObject textObj = new GameObject("Placard_Text");
        textObj.transform.SetParent(placardStand.transform, false);
        textObj.transform.localPosition = new Vector3(0, 0, -0.52f);
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one * 0.12f;

        var tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = $"<b><color=#00E5FF>{title.ToUpper()}</color></b>\n<size=75%><color=#FFD700>[ {era} ]</color>\n<color=#E0F7FA>{desc}</color></size>";
        tmp.fontSize = 1.3f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.rectTransform.sizeDelta = new Vector2(8.5f, 5.0f);

        // Interaction Prompt indicator
        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(exhibit.transform, false);
        promptObj.transform.localPosition = new Vector3(0, height + 0.85f, -radius * 0.75f);
        promptObj.transform.localScale = Vector3.one * 0.12f;
        var pTmp = promptObj.AddComponent<TextMeshPro>();
        pTmp.text = "<color=#00FF88><b>[ E ] SENTUH / AMATI ARTEFAK</b></color>";
        pTmp.fontSize = 1.6f;
        pTmp.alignment = TextAlignmentOptions.Center;
        pTmp.rectTransform.sizeDelta = new Vector2(10.0f, 2.5f);
    }

    private static Light AddExhibitSpotlight(GameObject exhibit, Color lightColor, float height = 3.6f)
    {
        GameObject spotObj = new GameObject("Exhibit_Spotlight");
        spotObj.transform.SetParent(exhibit.transform);
        spotObj.transform.localPosition = new Vector3(0, height, 0.2f);
        spotObj.transform.localRotation = Quaternion.Euler(80f, 0, 0);
        var spot = spotObj.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.color = lightColor;
        spot.intensity = 2.4f;
        spot.range = 8.0f;
        spot.spotAngle = 60f;
        return spot;
    }

    // =========================================================================
    // 1. EXHIBIT 1: THE VOYAGER GOLDEN RECORD (Tripo 3D AI Model)
    // =========================================================================
    private static void BuildExhibit1_GoldenRecord()
    {
        Material darkMat = GetMat("Assets/Materials/SciFiWall_Dark.mat", Color.black);
        Material cyanGlow = GetMat("Assets/Materials/EmissiveCyan.mat", Color.cyan);
        Material goldGlow = GetMat("Assets/Materials/EmissiveGold.mat", Color.yellow);

        GameObject exhibit = CreatePedestal("Exhibit_1_VoyagerGoldenRecord", darkMat, cyanGlow, goldGlow, 1.4f, 0.85f);

        // Tripo 3D PBR Model: The Voyager Golden Record on Pedestal
        GameObject tripoRecordPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_voyager_golden_record.glb");
        GameObject modelObj = null;
        if (tripoRecordPrefab != null)
        {
            modelObj = Object.Instantiate(tripoRecordPrefab, exhibit.transform);
            modelObj.name = "VoyagerGoldenRecord_Tripo3D";
            modelObj.transform.localPosition = new Vector3(0, 0.85f + 0.68f, 0);
            modelObj.transform.localRotation = Quaternion.Euler(0, -90f, 0);
            modelObj.transform.localScale = Vector3.one * 1.35f;
        }

        // Floating Pulsar Hologram Wireframe Ring
        GameObject holoProjector = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        holoProjector.name = "Holo_PulsarMapProjector";
        holoProjector.transform.SetParent(exhibit.transform);
        holoProjector.transform.localPosition = new Vector3(0, 2.35f, 0);
        holoProjector.transform.localScale = new Vector3(0.95f, 0.01f, 0.95f);
        holoProjector.GetComponent<MeshRenderer>().sharedMaterial = cyanGlow;
        var hCol = holoProjector.GetComponent<Collider>();
        if (hCol != null) Object.DestroyImmediate(hCol);

        // Gold Dust Particles
        GameObject dustObj = new GameObject("GoldDustParticles");
        dustObj.transform.SetParent(exhibit.transform);
        dustObj.transform.localPosition = new Vector3(0, 1.8f, 0);
        var dustPS = dustObj.AddComponent<ParticleSystem>();
        ConfigureDustPS(dustPS, new Color(1f, 0.85f, 0.3f));

        Light spot = AddExhibitSpotlight(exhibit, new Color(1f, 0.90f, 0.5f));
        AddPlacard(exhibit, "The Voyager Golden Record", "1977 M — PESAN ANTARBINTANG", 
            "Piringan fonograf tembaga berlapis emas yang diluncurkan menembus batas heliosfer. Berisi 115 citra, salam dalam 55 bahasa manusia, suara ombak, dan musik Bach & Chuck Berry.",
            darkMat, cyanGlow, 1.4f, 0.85f);

        var controller = exhibit.AddComponent<GoldenRecordTurntable>();
        var so = new SerializedObject(controller);
        if (modelObj != null) so.FindProperty("recordDisc").objectReferenceValue = modelObj.transform;
        so.FindProperty("pulsarHoloProjector").objectReferenceValue = holoProjector;
        so.FindProperty("goldDustVFX").objectReferenceValue = dustPS;
        so.FindProperty("goldenHaloLight").objectReferenceValue = spot;
        so.FindProperty("placardText").objectReferenceValue = exhibit.GetComponentInChildren<TextMeshPro>();
        so.FindProperty("interactionPrompt").objectReferenceValue = exhibit.transform.Find("InteractionPrompt")?.gameObject;
        so.FindProperty("exhibitSpotlight").objectReferenceValue = spot;
        so.ApplyModifiedProperties();

        SavePrefab(exhibit, "Exhibit_1_VoyagerGoldenRecord.prefab");
    }

    // =========================================================================
    // 2. EXHIBIT 2: ROSETTA STONE & ART GALLERY
    // =========================================================================
    private static void BuildExhibit2_RosettaArt()
    {
        Material darkMat = GetMat("Assets/Materials/SciFiWall_Dark.mat", Color.black);
        Material cyanGlow = GetMat("Assets/Materials/EmissiveCyan.mat", Color.cyan);
        Material goldGlow = GetMat("Assets/Materials/EmissiveGold.mat", Color.yellow);
        Material rosettaMat = GetMat("Assets/Materials/Museum/Mat_RosettaStone.mat", Color.black);
        Material monaMat = GetMat("Assets/Materials/Museum/Mat_Artwork_MonaLisa.mat", Color.white);
        Material starryMat = GetMat("Assets/Materials/Museum/Mat_Artwork_StarryNight.mat", Color.blue);
        Material caveMat = GetMat("Assets/Materials/Museum/Mat_Artwork_Lascaux.mat", Color.red);

        GameObject exhibit = CreatePedestal("Exhibit_2_RosettaStoneAndArt", darkMat, cyanGlow, goldGlow, 1.6f, 0.85f);

        // =========================================================================
        // Tripo 3D PBR Model: The Rosetta Stone Stele
        // =========================================================================
        GameObject tripoRosettaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_rosetta_stone.glb");
        GameObject rosettaObj = null;
        if (tripoRosettaPrefab != null)
        {
            rosettaObj = Object.Instantiate(tripoRosettaPrefab, exhibit.transform);
            rosettaObj.name = "RosettaStone_Tripo3D";
            rosettaObj.transform.localPosition = new Vector3(0, 0.85f + 0.65f, -0.15f);
            rosettaObj.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            rosettaObj.transform.localScale = Vector3.one * 1.35f;
        }

        // 3 Floating Holographic Art Frames hovering above
        Material[] artMats = { caveMat, monaMat, starryMat };
        Transform[] frames = new Transform[3];
        float[] frameAngles = { -45f, 0f, 45f };

        GameObject galleryRoot = new GameObject("ArtGallery_FramesGroup");
        galleryRoot.transform.SetParent(exhibit.transform);
        galleryRoot.transform.localPosition = new Vector3(0, 2.35f, 0.45f);

        for (int i = 0; i < 3; i++)
        {
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
            frame.name = $"HoloArtFrame_{i + 1}";
            frame.transform.SetParent(galleryRoot.transform);
            float rad = frameAngles[i] * Mathf.Deg2Rad;
            frame.transform.localPosition = new Vector3(Mathf.Sin(rad) * 1.15f, 0, Mathf.Cos(rad) * 0.45f);
            frame.transform.localRotation = Quaternion.Euler(0, -frameAngles[i] * 0.8f, 0);
            frame.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            frame.GetComponent<MeshRenderer>().sharedMaterial = artMats[i];
            frames[i] = frame.transform;
            var c = frame.GetComponent<Collider>();
            if (c != null) Object.DestroyImmediate(c);
        }

        // Translation Laser Beam
        GameObject transBeam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        transBeam.name = "TranslationLaserBeam";
        transBeam.transform.SetParent(exhibit.transform);
        transBeam.transform.localPosition = new Vector3(0, 1.45f, -0.05f);
        transBeam.transform.localScale = new Vector3(0.72f, 0.015f, 0.25f);
        transBeam.transform.localRotation = Quaternion.Euler(18f, 0, 0);
        transBeam.GetComponent<MeshRenderer>().sharedMaterial = cyanGlow;
        var tbCol = transBeam.GetComponent<Collider>();
        if (tbCol != null) Object.DestroyImmediate(tbCol);

        Light spot = AddExhibitSpotlight(exhibit, new Color(0.2f, 0.85f, 1f));
        AddPlacard(exhibit, "Batu Rosetta & Mahakarya Seni", "196 SM - ABAD KE-20",
            "Dekrit Raja Ptolemaios V dalam 3 aksara (Hieroglif, Demotik, Yunani) yang membuka tabir bahasa Mesir Kuno, dipadukan evolusi kanvas estetika dari gua prasejarah hingga era impresionisme modern.",
            darkMat, cyanGlow, 1.6f, 0.85f);

        var controller = exhibit.AddComponent<MuseumArtworkGallery>();
        var so = new SerializedObject(controller);
        if (rosettaObj != null) so.FindProperty("rosettaStone").objectReferenceValue = rosettaObj.transform;
        so.FindProperty("translationBeam").objectReferenceValue = transBeam;
        var pFrames = so.FindProperty("artFrames");
        pFrames.arraySize = 3;
        for (int i = 0; i < 3; i++) pFrames.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        so.FindProperty("placardText").objectReferenceValue = exhibit.GetComponentInChildren<TextMeshPro>();
        so.FindProperty("interactionPrompt").objectReferenceValue = exhibit.transform.Find("InteractionPrompt")?.gameObject;
        so.FindProperty("exhibitSpotlight").objectReferenceValue = spot;
        so.ApplyModifiedProperties();

        SavePrefab(exhibit, "Exhibit_2_RosettaStoneAndArt.prefab");
    }

    // =========================================================================
    // 3. EXHIBIT 3: PROMETHEUS TOOLS & SILICON (Tripo 3D AI Model)
    // =========================================================================
    private static void BuildExhibit3_PrometheusSilicon()
    {
        Material darkMat = GetMat("Assets/Materials/SciFiWall_Dark.mat", Color.black);
        Material cyanGlow = GetMat("Assets/Materials/EmissiveCyan.mat", Color.cyan);
        Material goldGlow = GetMat("Assets/Materials/EmissiveGold.mat", Color.yellow);

        GameObject exhibit = CreatePedestal("Exhibit_3_PrometheusAndSilicon", darkMat, cyanGlow, goldGlow, 1.5f, 0.85f);

        // Tripo 3D PBR Model: Spark of Prometheus Handaxe evolving to Silicon Wafer & Processor
        GameObject tripoPrometheusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_prometheus_fire_and_chip.glb");
        GameObject modelObj = null;
        if (tripoPrometheusPrefab != null)
        {
            modelObj = Object.Instantiate(tripoPrometheusPrefab, exhibit.transform);
            modelObj.name = "PrometheusSilicon_Tripo3D";
            modelObj.transform.localPosition = new Vector3(0, 0.85f + 0.60f, 0);
            modelObj.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            modelObj.transform.localScale = Vector3.one * 1.35f;
        }

        // Primitive Fire Light & Particles
        GameObject fireObj = new GameObject("FireSparkVFX");
        fireObj.transform.SetParent(exhibit.transform, false);
        fireObj.transform.localPosition = new Vector3(0f, 1.25f, 0.15f);
        var firePS = fireObj.AddComponent<ParticleSystem>();
        ConfigureDustPS(firePS, new Color(1f, 0.45f, 0.1f));

        GameObject fLightObj = new GameObject("FireGlowLight");
        fLightObj.transform.SetParent(fireObj.transform, false);
        fLightObj.transform.localPosition = Vector3.zero;
        var fLight = fLightObj.AddComponent<Light>();
        fLight.type = LightType.Point;
        fLight.color = new Color(1f, 0.55f, 0.15f);
        fLight.intensity = 1.8f;
        fLight.range = 4.0f;

        Light spot = AddExhibitSpotlight(exhibit, new Color(1f, 0.75f, 0.35f));
        AddPlacard(exhibit, "Api, Roda & Silikon", "3.3 JUTA SM - ABAD 21",
            "Jejak lompatan alat Homo sapiens: dari membelah batu dan mengendalikan api purba, penemuan roda penggerak mekanis, hingga memahat miliaran gerbang logika nano-transistor pada wafer silikon fotolitografi.",
            darkMat, cyanGlow, 1.5f, 0.85f);

        var controller = exhibit.AddComponent<PrometheusToolEvolution>();
        var so = new SerializedObject(controller);
        if (modelObj != null) so.FindProperty("stoneHandaxe").objectReferenceValue = modelObj.transform;
        so.FindProperty("fireSparkPS").objectReferenceValue = firePS;
        so.FindProperty("fireGlowLight").objectReferenceValue = fLight;
        so.FindProperty("placardText").objectReferenceValue = exhibit.GetComponentInChildren<TextMeshPro>();
        so.FindProperty("interactionPrompt").objectReferenceValue = exhibit.transform.Find("InteractionPrompt")?.gameObject;
        so.FindProperty("exhibitSpotlight").objectReferenceValue = spot;
        so.ApplyModifiedProperties();

        SavePrefab(exhibit, "Exhibit_3_PrometheusAndSilicon.prefab");
    }

    // =========================================================================
    // 4. EXHIBIT 4: SVALBARD SEED VAULT (Tripo 3D AI Model)
    // =========================================================================
    private static void BuildExhibit4_SvalbardSeedVault()
    {
        Material darkMat = GetMat("Assets/Materials/SciFiWall_Dark.mat", Color.black);
        Material cyanGlow = GetMat("Assets/Materials/EmissiveCyan.mat", Color.cyan);
        Material goldGlow = GetMat("Assets/Materials/EmissiveGold.mat", Color.yellow);

        GameObject exhibit = CreatePedestal("Exhibit_4_SvalbardSeedVault", darkMat, cyanGlow, goldGlow, 1.5f, 0.85f);

        // Tripo 3D PBR Model: Svalbard Cryogenic Preservation Capsule
        GameObject tripoSeedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_svalbard_seed_vault.glb");
        GameObject modelObj = null;
        if (tripoSeedPrefab != null)
        {
            modelObj = Object.Instantiate(tripoSeedPrefab, exhibit.transform);
            modelObj.name = "SvalbardSeedVault_Tripo3D";
            modelObj.transform.localPosition = new Vector3(0, 0.85f + 0.65f, 0);
            modelObj.transform.localRotation = Quaternion.Euler(0, 270f, 0);
            modelObj.transform.localScale = Vector3.one * 1.30f;
        }

        // Holographic Sprouter Plant (Expanding Terran Flora)
        GameObject sproutRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sproutRoot.name = "Holo_PlantSprout";
        sproutRoot.transform.SetParent(exhibit.transform);
        sproutRoot.transform.localPosition = new Vector3(0, 2.35f, 0);
        sproutRoot.transform.localScale = Vector3.one * 0.45f;
        sproutRoot.GetComponent<MeshRenderer>().sharedMaterial = cyanGlow;
        var sCol = sproutRoot.GetComponent<Collider>();
        if (sCol != null) Object.DestroyImmediate(sCol);

        // Bio Growth Particles
        GameObject bioPSObj = new GameObject("BioGrowthParticles");
        bioPSObj.transform.SetParent(exhibit.transform);
        bioPSObj.transform.localPosition = new Vector3(0, 2.35f, 0);
        var bioPS = bioPSObj.AddComponent<ParticleSystem>();
        ConfigureDustPS(bioPS, new Color(0.1f, 1f, 0.6f));

        Light spot = AddExhibitSpotlight(exhibit, new Color(0.15f, 1f, 0.65f));
        AddPlacard(exhibit, "Bank Benih Global Svalbard", "2008 M — BRANKAS KIAMAT BUMI",
            "Terletak di pulau permafrost Arktik Norwegia pada suhu -18°C. Menyimpan lebih dari 1.100.000 sampel benih tanaman pokok dari seluruh penjuru dunia guna menjamin ketahanan pangan hayati Bumi.",
            darkMat, cyanGlow, 1.5f, 0.85f);

        var controller = exhibit.AddComponent<SeedVaultSprouter>();
        var so = new SerializedObject(controller);
        so.FindProperty("holoPlantSprout").objectReferenceValue = sproutRoot.transform;
        so.FindProperty("growthStardustPS").objectReferenceValue = bioPS;
        so.FindProperty("bioGlowLight").objectReferenceValue = spot;
        so.FindProperty("placardText").objectReferenceValue = exhibit.GetComponentInChildren<TextMeshPro>();
        so.FindProperty("interactionPrompt").objectReferenceValue = exhibit.transform.Find("InteractionPrompt")?.gameObject;
        so.FindProperty("exhibitSpotlight").objectReferenceValue = spot;
        so.ApplyModifiedProperties();

        SavePrefab(exhibit, "Exhibit_4_SvalbardSeedVault.prefab");
    }

    // =========================================================================
    // 5. EXHIBIT 5: APOLLO 11 LUNAR BOOTPRINT & DIPLOMATIC SCANNER (Tripo 3D AI Model)
    // =========================================================================
    private static void BuildExhibit5_ApolloLunarPlaque()
    {
        Material darkMat = GetMat("Assets/Materials/SciFiWall_Dark.mat", Color.black);
        Material cyanGlow = GetMat("Assets/Materials/EmissiveCyan.mat", Color.cyan);
        Material goldGlow = GetMat("Assets/Materials/EmissiveGold.mat", Color.yellow);

        GameObject exhibit = CreatePedestal("Exhibit_5_Apollo11AndDiplomaticPeace", darkMat, cyanGlow, goldGlow, 1.5f, 0.85f);

        // Tripo 3D PBR Model: Apollo 11 Lunar Bootprint and Golden Peace Plaque
        GameObject tripoApolloPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_apollo11_moon_plaque.glb");
        GameObject modelObj = null;
        if (tripoApolloPrefab != null)
        {
            modelObj = Object.Instantiate(tripoApolloPrefab, exhibit.transform);
            modelObj.name = "Apollo11Plaque_Tripo3D";
            modelObj.transform.localPosition = new Vector3(-0.35f, 0.85f + 0.62f, 0);
            modelObj.transform.localRotation = Quaternion.Euler(0, -90f, 0);
            modelObj.transform.localScale = Vector3.one * 1.30f;
        }

        // Diplomatic Touch Sensor Pad (Right side of plinth)
        GameObject sensorPad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sensorPad.name = "Diplomatic_ScannerPad";
        sensorPad.transform.SetParent(exhibit.transform);
        sensorPad.transform.localPosition = new Vector3(0.42f, 0.94f, 0);
        sensorPad.transform.localScale = new Vector3(0.55f, 0.035f, 0.55f);
        sensorPad.GetComponent<MeshRenderer>().sharedMaterial = darkMat;

        GameObject sensorGlowRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sensorGlowRing.name = "Sensor_GlowRing";
        sensorGlowRing.transform.SetParent(sensorPad.transform);
        sensorGlowRing.transform.localPosition = new Vector3(0, 0.55f, 0);
        sensorGlowRing.transform.localScale = new Vector3(0.85f, 0.05f, 0.85f);
        sensorGlowRing.GetComponent<MeshRenderer>().sharedMaterial = cyanGlow;

        // Alien Glowing Handprint Visual
        GameObject alienHand = GameObject.CreatePrimitive(PrimitiveType.Quad);
        alienHand.name = "Alien_Handprint_Glow";
        alienHand.transform.SetParent(sensorPad.transform);
        alienHand.transform.localPosition = new Vector3(0, 0.65f, 0);
        alienHand.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        alienHand.transform.localScale = Vector3.one * 0.65f;
        alienHand.GetComponent<MeshRenderer>().sharedMaterial = goldGlow;

        // Scanner Laser Beam
        GameObject scanBeam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        scanBeam.name = "BiometricScanBeam";
        scanBeam.transform.SetParent(exhibit.transform);
        scanBeam.transform.localPosition = new Vector3(0.42f, 0.98f, 0);
        scanBeam.transform.localScale = new Vector3(0.48f, 0.005f, 0.04f);
        scanBeam.GetComponent<MeshRenderer>().sharedMaterial = cyanGlow;
        var sbCol = scanBeam.GetComponent<Collider>();
        if (sbCol != null) Object.DestroyImmediate(sbCol);

        // Peace Harmonics Particles
        GameObject peacePSObj = new GameObject("PeaceHarmonicsParticles");
        peacePSObj.transform.SetParent(exhibit.transform);
        peacePSObj.transform.localPosition = new Vector3(0.42f, 1.25f, 0);
        var peacePS = peacePSObj.AddComponent<ParticleSystem>();
        ConfigureDustPS(peacePS, new Color(0f, 0.9f, 1f));

        Light spot = AddExhibitSpotlight(exhibit, new Color(0.85f, 0.92f, 1f));
        AddPlacard(exhibit, "Jejak Apollo 11 & Pakta Perdamaian", "1969 M — LANGKAH RAKSASA",
            "Jejak sol sepatu astronot Neil Armstrong pada debu regolith Bulan. Di sampingnya terdapat sensor kontak diplomatik tempat Anda dapat meletakkan tangan dan mengabadikan pakta perdamaian antarbintang.",
            darkMat, cyanGlow, 1.5f, 0.85f);

        var controller = exhibit.AddComponent<DiplomaticHandprintScanner>();
        var so = new SerializedObject(controller);
        if (modelObj != null) so.FindProperty("moonRegolithPlaque").objectReferenceValue = modelObj.transform;
        so.FindProperty("alienHandprintVisual").objectReferenceValue = alienHand;
        so.FindProperty("scannerBeam").objectReferenceValue = scanBeam.transform;
        so.FindProperty("peaceHarmonicsPS").objectReferenceValue = peacePS;
        so.FindProperty("diplomaticHaloLight").objectReferenceValue = spot;
        so.FindProperty("placardText").objectReferenceValue = exhibit.GetComponentInChildren<TextMeshPro>();
        so.FindProperty("interactionPrompt").objectReferenceValue = exhibit.transform.Find("InteractionPrompt")?.gameObject;
        so.FindProperty("exhibitSpotlight").objectReferenceValue = spot;
        so.ApplyModifiedProperties();

        SavePrefab(exhibit, "Exhibit_5_Apollo11AndDiplomaticPeace.prefab");
    }

    private static void ConfigureDustPS(ParticleSystem ps, Color color)
    {
        var main = ps.main;
        main.startColor = color;
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.09f);
        main.startLifetime = 2.5f;
        main.startSpeed = 0.25f;
        main.maxParticles = 50;

        var emission = ps.emission;
        emission.rateOverTime = 12f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.45f;
    }

    private static void SavePrefab(GameObject obj, string fileName)
    {
        string path = Path.Combine(PrefabDir, fileName);
        PrefabUtility.SaveAsPrefabAsset(obj, path);
        Object.DestroyImmediate(obj);
        Debug.Log($"[XENOASIS] Saved Exhibit Prefab: {path}");
    }
}
