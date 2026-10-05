using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Xenoasis.Player;

/// <summary>
/// XENOASIS — TerranSpheresArchitect.cs
/// Builds "The Living Terran Spheres: Museum of Being Human"
/// A AAA-grade spatial exhibition designed for extraterrestrial observers:
/// 
/// 1. Grand Terran Rotunda:
///    - Polished obsidian & gold dais plinth with concentric compass inlays and South promenade.
///    - Central anti-gravity dais with rotating 3.2m Quantum Gyro centerpiece (museum_centerpiece_quantum_gyro.glb).
///    - Upward-projecting volumetric beacon spotlight into the UFO tractor bay.
/// 
/// 2. Four Monumental Living Terran Spheres in an Expansive Semi-Circular Amphitheater Arc (R=8.5m):
///    - Station 1 (West / -60°): The Primordial Cradle (Oceanic Biome, Planet Earth Globe & Water Lotus).
///    - Station 2 (North-West / -20°): The Spark of Civilization (Fire, Archimedes Clockwork Gears & Silicon).
///    - Station 3 (North-East / +20°): The Human Archive (Rosetta Stone, Seed Sanctuary & DNA Helix).
///    - Station 4 (East / +60°): The Cosmic Horizon (The Voyager Golden Record & Apollo Lunar Artifacts).
/// 
/// 3. Monumental Scale & Hero Tripo 3D Models:
///    - Tiered shrine dais (4.2m diameter) on each station.
///    - High-poly Tripo Obsidian & Gold pedestals (scale 1.30x, perfect showcase height 1.28m).
///    - 2.0m diameter Living Terran Diorama Spheres (GlassDome.mat with glowing latitude lines).
///    - Hero Tripo models displayed prominently on pedestals: central_basin.glb, water_lotus.glb,
///      museum_prometheus_fire_and_chip.glb, museum_rosetta_stone.glb, museum_svalbard_seed_vault.glb,
///      museum_voyager_golden_record.glb, museum_apollo11_moon_plaque.glb.
///    - Interactive Alien Curator Kiosks (museum_interactive_terminal.glb) on the front-right of each shrine.
///    - Sleek Curator Plaques with readable 3D text in front facing the incoming visitor.
/// 
/// 4. Perimeter Luxury Lounges & Curatorial Lighting:
///    - museum_luxury_lounge_bench.glb on east/west wings and gallery corners (no clipping).
///    - Dedicated warm gallery pin-spots illuminating both the diorama spheres and the physical artifacts.
/// </summary>
public static class TerranSpheresArchitect
{
    [MenuItem("XENOASIS/Build Living Terran Spheres Museum")]
    public static void BuildMuseum()
    {
        Debug.Log("[TerranSpheresArchitect] ✦ Initiating construction of Monumental Living Terran Spheres Museum...");

        string welcomeScenePath = "Assets/Scenes/WelcomeChamber.unity";
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path != welcomeScenePath)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(welcomeScenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
        }

        GameObject archiveRoot = GameObject.Find("--- XENOASIS ---/[Environment]/TerranLivingArchive");
        if (archiveRoot == null)
        {
            Debug.LogError("[TerranSpheresArchitect] Could not find TerranLivingArchive in scene!");
            return;
        }

        // 1. Remove any foreign root objects that don't belong in Welcome Chamber (e.g. World 2 forge)
        foreach (var root in archiveRoot.scene.GetRootGameObjects())
        {
            if (root.name != "--- XENOASIS ---" && root.name != "[CameraRig]")
            {
                Debug.Log($"[TerranSpheresArchitect] Removing foreign root from Welcome Chamber: {root.name}");
                Object.DestroyImmediate(root);
            }
        }

        // 2. Clean out legacy clutter objects so ONLY the 4 Living Terran Spheres exist
        string[] legacyClutter = new string[]
        {
            "HumanAmbassadorStation",
            "AlienBioMirrorStation",
            "CivilizationHoloTable",
            "LivingSamplePods",
            "GlassChimesGroup",
            "MuseumOfHumanity",
            "MuseumInformationTerminals",
            "Landing_Dais_Plinth"
        };
        foreach (string clutterName in legacyClutter)
        {
            Transform t = archiveRoot.transform.Find(clutterName);
            if (t != null)
            {
                Debug.Log($"[TerranSpheresArchitect] Removing legacy clutter: {clutterName}");
                Object.DestroyImmediate(t.gameObject);
            }
        }

        // 3. Clean out any previous container
        Transform oldSpheres = archiveRoot.transform.Find("TheLivingTerranSpheres");
        if (oldSpheres != null) Object.DestroyImmediate(oldSpheres.gameObject);

        GameObject spheresRoot = new GameObject("TheLivingTerranSpheres");
        spheresRoot.transform.SetParent(archiveRoot.transform, false);
        spheresRoot.transform.localPosition = Vector3.zero;
        spheresRoot.transform.localRotation = Quaternion.identity;

        // Load PBR Materials
        Material goldMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LuxuryGold_Inlay.mat");
        Material plinthMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_DaisPlinth.mat");
        Material obsidianMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ObsidianGlass.mat");
        Material hullDarkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat");
        Material glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassDome.mat");
        if (glassMat == null) glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        Material earthMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PlanetEarth_Globe3D.mat");
        Material atmoMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PlanetEarth_AtmosphereHaze.mat");
        Material glowCyan = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        Material glowGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");

        // 1. Build Grand Floor Compass Inlays & South Entrance Promenade
        BuildFloorCompass(spheresRoot.transform, goldMat, glowCyan);

        // 2. Build Central Quantum Gyro Dais & Vertical Beacon Light
        BuildCenterpiece(spheresRoot.transform, goldMat, plinthMat, obsidianMat, glowCyan, glowGold);

        // 3. Build 4 Monumental Living Terran Spheres in Semi-Circular Amphitheater Arc (R=8.5m)
        BuildStation1_PrimordialCradle(spheresRoot.transform, goldMat, plinthMat, obsidianMat, glassMat, earthMat, atmoMat, glowCyan);
        BuildStation2_SparkOfCivilization(spheresRoot.transform, goldMat, plinthMat, obsidianMat, glassMat, glowGold, hullDarkMat);
        BuildStation3_HeartOfHumanity(spheresRoot.transform, goldMat, plinthMat, obsidianMat, glassMat, glowCyan, glowGold);
        BuildStation4_CosmicHorizon(spheresRoot.transform, goldMat, plinthMat, obsidianMat, glassMat, glowCyan, glowGold);

        // 4. Build Perimeter Luxury Lounges (Flanking Wings & Gallery Corners)
        BuildPerimeterLounges(spheresRoot.transform);

        // 5. Build Curatorial Gallery Spotlighting
        BuildGalleryLighting(spheresRoot.transform);

        EditorUtility.SetDirty(spheresRoot);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(archiveRoot.scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(archiveRoot.scene);
        Debug.Log("[TerranSpheresArchitect] ✦ The Monumental Living Terran Spheres Museum 100% COMPLETE!");
    }

    // =========================================================================
    // 1. FLOOR COMPASS, AMPHITHEATER ARCS & ENTRANCE PROMENADE
    // =========================================================================
    private static void BuildFloorCompass(Transform parent, Material goldMat, Material glowCyan)
    {
        GameObject compassRoot = new GameObject("Floor_TerranCompass_Inlays");
        compassRoot.transform.SetParent(parent, false);

        // Concentric Gold Inlay Rings
        CreateMeshRing(compassRoot.transform, "Compass_InnerDaisRing", 4.20f, 0.070f, goldMat, 0.015f);
        CreateMeshRing(compassRoot.transform, "Compass_InnerCyanAccent", 4.35f, 0.025f, glowCyan, 0.018f);

        CreateMeshRing(compassRoot.transform, "Compass_ShrinesOrbitArc", 8.50f, 0.090f, goldMat, 0.015f);
        CreateMeshRing(compassRoot.transform, "Compass_ShrinesCyanGuide", 8.70f, 0.030f, glowCyan, 0.018f);

        CreateMeshRing(compassRoot.transform, "Compass_OuterColonnadeRing", 14.50f, 0.080f, goldMat, 0.015f);

        // Radial Gold Inlay Spokes connecting Center Dais to the 4 Shrines (-60°, -20°, +20°, +60°)
        float[] spokeAngles = new float[] { -60f, -20f, 20f, 60f };
        for (int i = 0; i < spokeAngles.Length; i++)
        {
            float ang = spokeAngles[i] * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Sin(ang), 0f, Mathf.Cos(ang));
            Vector3 spokeCenter = dir * 6.35f;

            var spoke = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spoke.name = "Spoke_To_Station_" + (i + 1);
            spoke.transform.SetParent(compassRoot.transform, false);
            spoke.transform.localPosition = new Vector3(spokeCenter.x, 0.012f, spokeCenter.z);
            spoke.transform.localRotation = Quaternion.Euler(0f, spokeAngles[i], 0f);
            spoke.transform.localScale = new Vector3(0.06f, 0.015f, 3.80f);
            spoke.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(spoke.GetComponent<Collider>());
        }

        // South Grand Entrance Promenade (from Z=-14.0m to Z=-4.5m)
        GameObject promenade = new GameObject("Entrance_GrandPromenade");
        promenade.transform.SetParent(compassRoot.transform, false);

        float runnerX = 1.80f;
        for (int side = -1; side <= 1; side += 2)
        {
            var runner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            runner.name = side < 0 ? "Promenade_Runner_West" : "Promenade_Runner_East";
            runner.transform.SetParent(promenade.transform, false);
            runner.transform.localPosition = new Vector3(side * runnerX, 0.012f, -9.25f);
            runner.transform.localScale = new Vector3(0.06f, 0.015f, 9.50f);
            runner.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(runner.GetComponent<Collider>());
        }

        // Cross-tie inlays along the promenade
        for (float z = -13.0f; z <= -5.5f; z += 2.5f)
        {
            var tie = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tie.name = "Promenade_CrossTie_" + Mathf.RoundToInt(z);
            tie.transform.SetParent(promenade.transform, false);
            tie.transform.localPosition = new Vector3(0f, 0.014f, z);
            tie.transform.localScale = new Vector3(runnerX * 2f - 0.12f, 0.015f, 0.035f);
            tie.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
            Object.DestroyImmediate(tie.GetComponent<Collider>());
        }
    }

    // =========================================================================
    // 2. CENTRAL QUANTUM GYRO DAIS & BEACON
    // =========================================================================
    private static void BuildCenterpiece(Transform parent, Material goldMat, Material plinthMat,
                                         Material obsidianMat, Material glowCyan, Material glowGold)
    {
        GameObject centerRoot = new GameObject("Centerpiece_TerranAxis");
        centerRoot.transform.SetParent(parent, false);
        centerRoot.transform.localPosition = Vector3.zero;

        // Tier 1 Base Platform (Diameter 7.2m, Height 0.10m, on top of landing plinth at Y=0.24m)
        var daisT1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisT1.name = "Dais_Tier1_Base";
        daisT1.transform.SetParent(centerRoot.transform, false);
        daisT1.transform.localPosition = new Vector3(0f, 0.28f, 0f);
        daisT1.transform.localScale = new Vector3(7.20f, 0.05f, 7.20f);
        daisT1.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat ?? plinthMat;
        Object.DestroyImmediate(daisT1.GetComponent<Collider>());

        // Gold Rim on Tier 1
        CreateMeshRing(centerRoot.transform, "Dais_T1_GoldRim", 3.60f, 0.10f, goldMat, 0.33f);

        // Tier 2 Raised Plinth (Diameter 5.2m, Height 0.10m)
        var daisT2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisT2.name = "Dais_Tier2_Plinth";
        daisT2.transform.SetParent(centerRoot.transform, false);
        daisT2.transform.localPosition = new Vector3(0f, 0.38f, 0f);
        daisT2.transform.localScale = new Vector3(5.20f, 0.05f, 5.20f);
        daisT2.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;
        Object.DestroyImmediate(daisT2.GetComponent<Collider>());

        // Gold Rim on Tier 2
        CreateMeshRing(centerRoot.transform, "Dais_T2_GoldRim", 2.60f, 0.08f, goldMat, 0.43f);

        // Core Anti-Gravity Emitter Disk (Diameter 3.2m, Height 0.04m)
        var daisCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisCore.name = "Dais_Tier3_CoreDisk";
        daisCore.transform.SetParent(centerRoot.transform, false);
        daisCore.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        daisCore.transform.localScale = new Vector3(3.20f, 0.02f, 3.20f);
        daisCore.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat ?? plinthMat;
        Object.DestroyImmediate(daisCore.GetComponent<Collider>());

        // Emitter Concentric Inlays
        CreateMeshRing(centerRoot.transform, "Dais_CoreGoldRing", 1.55f, 0.05f, goldMat, 0.47f);
        CreateMeshRing(centerRoot.transform, "Dais_CoreCyanRing", 1.35f, 0.03f, glowCyan, 0.475f);

        // Floating Tripo Quantum Gyro Centerpiece (Monumental 3.2m scale)
        GameObject gyroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_centerpiece_quantum_gyro.glb");
        if (gyroPrefab != null)
        {
            GameObject gyro = Object.Instantiate(gyroPrefab, centerRoot.transform);
            gyro.name = "Tripo_QuantumGyro_Core";
            gyro.transform.localPosition = new Vector3(0f, 2.45f, 0f);
            gyro.transform.localScale = Vector3.one * 3.20f;

            var rot = gyro.AddComponent<ExhibitRotator>();
            rot.rotationSpeed = 8.0f;
            rot.bobAmplitude = 0.05f;
            rot.bobSpeed = 0.9f;
        }

        // Center Dais Ambient Glow Light
        var cLight = centerRoot.AddComponent<Light>();
        cLight.type = LightType.Point;
        cLight.color = new Color(0.20f, 0.85f, 1.0f);
        cLight.intensity = 4.0f;
        cLight.range = 10.0f;

        // Vertical Beacon Spotlight pointing straight UP into Mothership tractor bay (NO solid cylinder!)
        GameObject beaconGo = new GameObject("Center_VerticalBeaconSpot");
        beaconGo.transform.SetParent(centerRoot.transform, false);
        beaconGo.transform.localPosition = new Vector3(0f, 0.50f, 0f);
        beaconGo.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Pointing UP

        var bLight = beaconGo.AddComponent<Light>();
        bLight.type = LightType.Spot;
        bLight.color = new Color(0.25f, 0.90f, 1.0f);
        bLight.intensity = 5.0f;
        bLight.range = 28.0f;
        bLight.spotAngle = 26.0f;
    }

    // =========================================================================
    // 3. STATION 1: THE PRIMORDIAL CRADLE (WATER & EARTH)
    // =========================================================================
    private static void BuildStation1_PrimordialCradle(Transform parent, Material goldMat, Material plinthMat, Material obsidianMat,
                                                      Material glassMat, Material earthMat, Material atmoMat, Material glowCyan)
    {
        // Radius = 8.5m, Angle = -60° (West)
        float ang = -60f * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(ang) * 8.50f, 0f, Mathf.Cos(ang) * 8.50f);
        Quaternion rot = Quaternion.LookRotation(new Vector3(0f, 0f, -2f) - pos);

        GameObject st = CreateStationRoot(parent, "Station_01_PrimordialCradle", pos, rot);
        BuildStationShrineBase(st.transform, goldMat, plinthMat, obsidianMat, glowCyan, "I. THE PRIMORDIAL CRADLE", "EARTH & THE MEMORY OF WATER");

        // Audio Clips
        AudioClip splashClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/water_drops.wav");
        AudioClip bloomClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bloom_chord.wav");
        AudioClip harvestClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");
        AudioClip chimeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
        AudioClip abyssAmbienceClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");

        // 1. Tripo Sapphire Basin on pedestal top (Y=1.28m)
        GameObject basin = null;
        Renderer waterRend = null;
        GameObject basinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/central_basin.glb");
        if (basinPrefab != null)
        {
            basin = Object.Instantiate(basinPrefab, st.transform);
            basin.name = "Tripo_SacredWaterBasin";
            basin.transform.localPosition = new Vector3(0f, 1.45f, 0f);
            basin.transform.localScale = Vector3.one * 1.30f;

            var bCol = basin.AddComponent<BoxCollider>();
            bCol.center = Vector3.zero;
            bCol.size = new Vector3(0.7f, 0.35f, 1.1f);

            // Shimmering Liquid Water Disk inside the basin
            Material waterMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WaterRefraction.mat");
            var waterDisk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            waterDisk.name = "Basin_WaterSurface";
            waterDisk.transform.SetParent(basin.transform, false);
            waterDisk.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            waterDisk.transform.localScale = new Vector3(0.54f, 0.01f, 0.88f);
            waterRend = waterDisk.GetComponent<MeshRenderer>();
            if (waterMat != null) waterRend.sharedMaterial = waterMat;
            Object.DestroyImmediate(waterDisk.GetComponent<Collider>());
        }

        // 2. Tripo Frosted Water Lotus
        GameObject lotus = null;
        GameObject lotusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/water_lotus.glb");
        if (lotusPrefab != null)
        {
            lotus = Object.Instantiate(lotusPrefab, st.transform);
            lotus.name = "Tripo_FrostedWaterLotus";
            lotus.transform.localPosition = new Vector3(0f, 1.58f, 0f);
            lotus.transform.localScale = Vector3.one * 0.55f;

            var lCol = lotus.AddComponent<SphereCollider>();
            lCol.center = new Vector3(0f, 0.15f, 0f);
            lCol.radius = 0.35f;
        }

        // 3. Primordial Memory Droplet (Sacred Pearl revealed upon water ritual)
        GameObject droplet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        droplet.name = "Primordial_MemoryDroplet";
        droplet.transform.SetParent(st.transform, false);
        droplet.transform.localPosition = new Vector3(0f, 1.88f, 0f);
        droplet.transform.localScale = Vector3.one * 0.20f;
        droplet.GetComponent<MeshRenderer>().sharedMaterial = glassMat ?? goldMat;
        var dropCol = droplet.GetComponent<SphereCollider>();
        if (dropCol != null) dropCol.isTrigger = true;

        var dropCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dropCore.name = "Droplet_LuminousCore";
        dropCore.transform.SetParent(droplet.transform, false);
        dropCore.transform.localPosition = Vector3.zero;
        dropCore.transform.localScale = Vector3.one * 0.55f;
        dropCore.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
        Object.DestroyImmediate(dropCore.GetComponent<Collider>());

        var dropRot = droplet.AddComponent<ExhibitRotator>();
        dropRot.rotationSpeed = 25f;
        dropRot.bobAmplitude = 0.025f;
        dropRot.bobSpeed = 2.0f;

        var dropLight = droplet.AddComponent<Light>();
        dropLight.type = LightType.Point;
        dropLight.color = new Color(0.2f, 0.95f, 1.0f);
        dropLight.intensity = 2.2f;
        dropLight.range = 2.0f;

        droplet.SetActive(false); // Dormant until water ritual

        // 4. Monumental Living Terran Sphere hovering gracefully above the basin at Y=2.85m
        GameObject sphere = BuildDioramaSphere(st.transform, "DioramaSphere_WaterEarth", new Vector3(0f, 2.85f, 0f), 1.80f, glassMat, goldMat, glowCyan);

        // Inner Bioluminescent Plankton Particle Field
        GameObject planktonObj = new GameObject("Inner_BioluminescentPlankton");
        planktonObj.transform.SetParent(sphere.transform, false);
        planktonObj.transform.localPosition = Vector3.zero;
        var ps = planktonObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 4.0f;
        main.startSpeed = 0.06f;
        main.startSize = 0.035f;
        main.startColor = new Color(0.15f, 0.90f, 1.0f, 0.85f);
        main.maxParticles = 75;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.85f;
        var emission = ps.emission;
        emission.rateOverTime = 16f;
        var psr = planktonObj.GetComponent<ParticleSystemRenderer>();
        Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
        if (stardustMat != null) psr.sharedMaterial = stardustMat;

        // Inner 3D Earth Globe
        GameObject globe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        globe.name = "Inner_PlanetEarthGlobe";
        globe.transform.SetParent(sphere.transform, false);
        globe.transform.localPosition = Vector3.zero;
        globe.transform.localScale = Vector3.one * 0.85f;
        if (earthMat != null) globe.GetComponent<MeshRenderer>().sharedMaterial = earthMat;
        var gCol = globe.GetComponent<SphereCollider>();
        if (gCol != null) gCol.isTrigger = true;

        var globeRot = globe.AddComponent<ExhibitRotator>();
        globeRot.rotationSpeed = 14.0f;
        globeRot.bobAmplitude = 0.02f;
        globeRot.bobSpeed = 1.3f;

        // Atmosphere Haze Ring
        CreateMeshRing(sphere.transform, "Inner_AtmosphereRing", 0.62f, 0.05f, atmoMat ?? glowCyan, 0f);

        // Internal Bioluminescent Cyan Light
        var sLight = sphere.AddComponent<Light>();
        sLight.type = LightType.Point;
        sLight.color = new Color(0.10f, 0.90f, 1.0f);
        sLight.intensity = 3.5f;
        sLight.range = 5.5f;

        // Setup 6DoF Head-In-Sphere Immersion Trigger
        var immersion = sphere.AddComponent<DioramaImmersionTrigger>();
        immersion.SetupImmersion(ps, sLight, abyssAmbienceClip);

        // 5. Configure Station 1 Terminal Pages
        var termDisplay = st.GetComponentInChildren<MuseumTerminalDisplay>();
        if (termDisplay != null)
        {
            termDisplay.SetCustomPages(new MuseumTerminalDisplay.TerminalPage[]
            {
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "I. THE PRIMORDIAL CRADLE",
                    eraCode = "ARCHEAN EON // 3.8 BILLION B.C.",
                    bodyText = "Water arrived via cosmic cometary bombardment. In boiling hydrothermal tide pools, early RNA polymers catalyzed the memory of life.\n\nTouch the Sacred Basin to initiate the water bloom ritual."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "THE MEMORY OF WATER",
                    eraCode = "ASTROBIOLOGY // PHYSIOLOGY",
                    bodyText = "Every human tear, blood plasma, and cellular fluid maintains the identical electrolyte ratio of Earth's ancient Archean sea.\n\nHumans never left the primordial ocean; they simply evolved membranes to carry the sea within."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "DIVE PROTOCOL // WORLD 1",
                    eraCode = "SENSORY 6DoF IMMERSION READY",
                    bodyText = "Full VR world transition available: The Primordial Cradle.\n\nReach out and grasp the Living Terran Sphere, or press the DIVE PEDESTAL to pull the sphere into your consciousness and enter the Archean lagoon."
                }
            });
        }

        // 6. Interactive DIVE INTO WORLD Pedestal
        GameObject divePlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        divePlinth.name = "DIVE_Button_Pedestal";
        divePlinth.transform.SetParent(st.transform, false);
        divePlinth.transform.localPosition = new Vector3(-1.35f, 0.48f, 0.95f);
        divePlinth.transform.localRotation = Quaternion.Euler(0f, -40f, 0f);
        divePlinth.transform.localScale = new Vector3(0.55f, 0.48f, 0.55f);
        if (plinthMat != null) divePlinth.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

        CreateMeshRing(divePlinth.transform, "DiveButton_GoldRing", 0.28f, 0.035f, goldMat, 1.01f);
        CreateMeshRing(divePlinth.transform, "DiveButton_CyanRing", 0.25f, 0.025f, glowCyan, 1.015f);

        var diveTouchpad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        diveTouchpad.name = "DiveButton_Touchpad";
        diveTouchpad.transform.SetParent(divePlinth.transform, false);
        diveTouchpad.transform.localPosition = new Vector3(0f, 1.03f, 0f);
        diveTouchpad.transform.localScale = new Vector3(0.70f, 0.04f, 0.70f);
        if (glowCyan != null) diveTouchpad.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;

        var diveLight = diveTouchpad.AddComponent<Light>();
        diveLight.type = LightType.Point;
        diveLight.color = new Color(0.1f, 0.9f, 1.0f);
        diveLight.intensity = 2.2f;
        diveLight.range = 2.5f;

        GameObject diveLabel = new GameObject("DiveButton_HoloLabel");
        diveLabel.transform.SetParent(divePlinth.transform, false);
        diveLabel.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        diveLabel.transform.localRotation = Quaternion.Euler(20f, 180f, 0f);
        var tm = diveLabel.AddComponent<TextMesh>();
        tm.text = "✦ DIVE INTO WORLD 1 ✦\n[TOUCH TO ENTER CRADLE]";
        tm.fontSize = 28;
        tm.characterSize = 0.032f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(0.2f, 0.95f, 1.0f);
        tm.fontStyle = FontStyle.Bold;

        // Find or add SphereDivingTransition to Main Camera
        Camera mainCam = Camera.main;
        SphereDivingTransition divingTrans = null;
        if (mainCam != null)
        {
            divingTrans = mainCam.GetComponent<SphereDivingTransition>();
            if (divingTrans == null) divingTrans = mainCam.gameObject.AddComponent<SphereDivingTransition>();
        }

        // 7. Connect Everything to PrimordialCradleExhibit Controller
        var exhibit = st.AddComponent<PrimordialCradleExhibit>();
        var relicSpot = st.transform.Find("Pedestal_RelicSpotlight")?.GetComponent<Light>();

        exhibit.SetupCradle(
            basin != null ? basin.transform : null,
            waterRend,
            lotus != null ? lotus.transform : null,
            droplet,
            relicSpot,
            sphere.transform,
            globe.transform,
            sLight,
            termDisplay,
            splashClip,
            bloomClip,
            harvestClip,
            chimeClip
        );

        exhibit.SetupDiveTransition(divingTrans, diveTouchpad);
    }

    // =========================================================================
    // 4. STATION 2: THE SPARK OF CIVILIZATION (TOOLS, FIRE, & SILICON)
    // =========================================================================
    private static void BuildStation2_SparkOfCivilization(Transform parent, Material goldMat, Material plinthMat, Material obsidianMat,
                                                         Material glassMat, Material glowGold, Material darkMat)
    {
        // Radius = 8.5m, Angle = -20° (North-West)
        float ang = -20f * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(ang) * 8.50f, 0f, Mathf.Cos(ang) * 8.50f);
        Quaternion rot = Quaternion.LookRotation(new Vector3(0f, 0f, -2f) - pos);

        GameObject st = CreateStationRoot(parent, "Station_02_SparkOfCivilization", pos, rot);
        BuildStationShrineBase(st.transform, goldMat, plinthMat, obsidianMat, glowGold, "II. THE SPARK OF INGENUITY", "THE CHRONICLE OF TOOLS, FIRE, & SILICON");

        // 1. Configure Terminal with Station 2 Astrobiological Pages
        var termDisplay = st.GetComponentInChildren<MuseumTerminalDisplay>();
        if (termDisplay != null)
        {
            termDisplay.SetCustomPages(new MuseumTerminalDisplay.TerminalPage[]
            {
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "I. THE METABOLIC LEAP",
                    eraCode = "LOWER PALEOLITHIC // 1.500.000 B.C.",
                    bodyText = "By striking flint against pyrite, early Terrans sparked controlled\nexothermic reactions. Fire externalized digestion, shrinking the gut\nand diverting immense metabolic caloric energy directly into the neocortex."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "II. THE METALLURGIC FORGE",
                    eraCode = "BRONZE & IRON AGE // 3.300 B.C.",
                    bodyText = "Humans tamed the crucible. Smelting copper, tin, and iron ore\nfrom Earth's mantle, they transformed brittle stones into tools\nof agriculture, monumental architecture, and celestial measurement."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "III. THE THINKING STONE",
                    eraCode = "INFORMATION AGE // 2026 A.D.",
                    bodyText = "In a poetic planetary loop, humanity returned to common sand.\nPurifying quartz into monocrystalline silicon and carving nanometer gates,\nTerrans taught the melted rock of their cradle planet how to think."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "DIVE PROTOCOL // WORLD 2",
                    eraCode = "SENSORY 6DoF IMMERSION READY",
                    bodyText = "Full VR world transition available: The Forge of Civilization.\n\nReach out and grasp the Living Terran Sphere, or press the DIVE PEDESTAL to pull the flame into your consciousness and enter the Prehistoric Mountain Forge."
                }
            });
        }

        // 2. Tripo Prometheus Fire & Silicon Chip Relic on Pedestal (Y=1.28m)
        GameObject prom = null;
        GameObject promPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_prometheus_fire_and_chip.glb");
        if (promPrefab != null)
        {
            prom = Object.Instantiate(promPrefab, st.transform);
            prom.name = "Tripo_PrometheusSparkAndChip";
            prom.transform.localPosition = new Vector3(0f, 1.48f, 0f);
            prom.transform.localRotation = Quaternion.Euler(0f, -40f, 0f);
            prom.transform.localScale = Vector3.one * 1.0f;

            var col = prom.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 0.35f, 0f);
            col.size = new Vector3(0.85f, 0.75f, 0.85f);
        }

        // 3. Interactive Sparks Particle System (burst on strike)
        GameObject sparksObj = new GameObject("Prometheus_Sparks_VFX");
        sparksObj.transform.SetParent(st.transform, false);
        sparksObj.transform.localPosition = new Vector3(0f, 1.55f, 0f);
        var sparksPS = sparksObj.AddComponent<ParticleSystem>();
        var sMain = sparksPS.main;
        sMain.startLifetime = 0.65f;
        sMain.startSpeed = 2.2f;
        sMain.startSize = 0.035f;
        sMain.startColor = new Color(1.0f, 0.95f, 0.65f, 1.0f);
        sMain.gravityModifier = 0.85f;
        sMain.playOnAwake = false;
        var sEmission = sparksPS.emission;
        sEmission.rateOverTime = 0f;
        var sShape = sparksPS.shape;
        sShape.shapeType = ParticleSystemShapeType.Cone;
        sShape.angle = 35f;
        sShape.radius = 0.12f;

        Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
        var sPsr = sparksObj.GetComponent<ParticleSystemRenderer>();
        if (stardustMat != null) sPsr.sharedMaterial = stardustMat;

        // 4. Interactive Flame Particle System (dancing golden fire)
        GameObject flameObj = new GameObject("Prometheus_Flame_VFX");
        flameObj.transform.SetParent(st.transform, false);
        flameObj.transform.localPosition = new Vector3(0f, 1.52f, 0f);
        var flamePS = flameObj.AddComponent<ParticleSystem>();
        var fMain = flamePS.main;
        fMain.startLifetime = 0.85f;
        fMain.startSpeed = 0.40f;
        fMain.startSize = 0.12f;
        fMain.startColor = new Color(1.0f, 0.70f, 0.15f, 0.90f);
        fMain.maxParticles = 50;
        fMain.playOnAwake = false;
        var fEmission = flamePS.emission;
        fEmission.rateOverTime = 22f;
        var fShape = flamePS.shape;
        fShape.shapeType = ParticleSystemShapeType.Sphere;
        fShape.radius = 0.16f;

        var fPsr = flameObj.GetComponent<ParticleSystemRenderer>();
        if (stardustMat != null) fPsr.sharedMaterial = stardustMat;

        // 5. Dynamic Fire Light
        GameObject fireLightObj = new GameObject("Prometheus_FireLight");
        fireLightObj.transform.SetParent(st.transform, false);
        fireLightObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        var fLight = fireLightObj.AddComponent<Light>();
        fLight.type = LightType.Point;
        fLight.color = new Color(1.0f, 0.65f, 0.15f);
        fLight.intensity = 0f; // Ignited on strike
        fLight.range = 4.5f;

        // 6. Prometheus Silicon Memory Core (Amber faceted crystal)
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "Prometheus_MemoryCore";
        core.transform.SetParent(st.transform, false);
        core.transform.localPosition = new Vector3(0f, 1.88f, 0f);
        core.transform.localScale = Vector3.one * 0.18f;
        core.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
        if (glowGold != null) core.GetComponent<MeshRenderer>().sharedMaterial = glowGold;
        var coreCol = core.GetComponent<BoxCollider>();
        if (coreCol != null) coreCol.isTrigger = true;

        var coreRot = core.AddComponent<ExhibitRotator>();
        coreRot.rotationSpeed = 30f;
        coreRot.bobAmplitude = 0.025f;
        coreRot.bobSpeed = 2.2f;

        var coreLight = core.AddComponent<Light>();
        coreLight.type = LightType.Point;
        coreLight.color = new Color(1.0f, 0.80f, 0.25f);
        coreLight.intensity = 2.4f;
        coreLight.range = 2.2f;

        core.SetActive(false); // Dormant until strike

        // 7. Monumental Living Terran Sphere hovering gracefully at Y=2.85m
        GameObject sphere = BuildDioramaSphere(st.transform, "DioramaSphere_CivilizationSpark", new Vector3(0f, 2.85f, 0f), 1.80f, glassMat, goldMat, glowGold);

        // Inner Archimedes Gear Rings
        GameObject gearRing1 = new GameObject("Inner_ArchimedesGear_Outer");
        gearRing1.transform.SetParent(sphere.transform, false);
        CreateMeshRing(gearRing1.transform, "GearOuterMesh", 0.58f, 0.07f, goldMat, 0f);
        var rot1 = gearRing1.AddComponent<ExhibitRotator>();
        rot1.rotationSpeed = 16.0f;

        GameObject gearRing2 = new GameObject("Inner_ArchimedesGear_Inner");
        gearRing2.transform.SetParent(sphere.transform, false);
        gearRing2.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
        CreateMeshRing(gearRing2.transform, "GearInnerMesh", 0.40f, 0.05f, goldMat, 0f);
        var rot2 = gearRing2.AddComponent<ExhibitRotator>();
        rot2.rotationSpeed = -22.0f;

        // Inner Glowing Fire Spark Core
        var sparkCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sparkCore.name = "Inner_FirstFireSpark";
        sparkCore.transform.SetParent(sphere.transform, false);
        sparkCore.transform.localPosition = Vector3.zero;
        sparkCore.transform.localScale = Vector3.one * 0.25f;
        sparkCore.GetComponent<MeshRenderer>().sharedMaterial = glowGold;
        Object.DestroyImmediate(sparkCore.GetComponent<Collider>());

        // Internal Warm Amber Light
        var sLight = sphere.AddComponent<Light>();
        sLight.type = LightType.Point;
        sLight.color = new Color(1.0f, 0.75f, 0.20f);
        sLight.intensity = 3.8f;
        sLight.range = 6.0f;

        // Audio Clips
        var strikeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_E.wav");
        var igniteClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bloom_chord.wav");
        var harvestClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");
        var chimeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_G.wav");
        var hearthAmbience = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");

        // 6DoF Immersion Trigger
        var immersion = sphere.AddComponent<DioramaImmersionTrigger>();
        immersion.SetupImmersion(null, sLight, hearthAmbience);

        // 8. Connect to CivilizationSparkExhibit Controller
        var exhibit = st.AddComponent<CivilizationSparkExhibit>();
        var relicSpot = st.transform.Find("Pedestal_RelicSpotlight")?.GetComponent<Light>();

        exhibit.SetupStation(
            prom != null ? prom.transform : st.transform,
            sparksPS,
            flamePS,
            fLight,
            core,
            relicSpot,
            sphere.transform,
            sLight,
            termDisplay,
            strikeClip,
            igniteClip,
            harvestClip,
            chimeClip
        );

        // 9. Interactive DIVE INTO FORGE Pedestal
        GameObject divePlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        divePlinth.name = "DIVE_Button_Pedestal";
        divePlinth.transform.SetParent(st.transform, false);
        divePlinth.transform.localPosition = new Vector3(-1.35f, 0.48f, 0.95f);
        divePlinth.transform.localRotation = Quaternion.Euler(0f, -40f, 0f);
        divePlinth.transform.localScale = new Vector3(0.55f, 0.48f, 0.55f);
        if (plinthMat != null) divePlinth.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

        CreateMeshRing(divePlinth.transform, "DiveButton_GoldRing", 0.28f, 0.035f, goldMat, 1.01f);
        CreateMeshRing(divePlinth.transform, "DiveButton_AmberRing", 0.25f, 0.025f, glowGold, 1.015f);

        var diveTouchpad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        diveTouchpad.name = "DiveButton_Touchpad";
        diveTouchpad.transform.SetParent(divePlinth.transform, false);
        diveTouchpad.transform.localPosition = new Vector3(0f, 1.03f, 0f);
        diveTouchpad.transform.localScale = new Vector3(0.70f, 0.04f, 0.70f);
        if (glowGold != null) diveTouchpad.GetComponent<MeshRenderer>().sharedMaterial = glowGold;

        var diveLight = diveTouchpad.AddComponent<Light>();
        diveLight.type = LightType.Point;
        diveLight.color = new Color(1.0f, 0.65f, 0.15f);
        diveLight.intensity = 2.4f;
        diveLight.range = 2.5f;

        GameObject diveLabel = new GameObject("DiveButton_HoloLabel");
        diveLabel.transform.SetParent(divePlinth.transform, false);
        diveLabel.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        diveLabel.transform.localRotation = Quaternion.Euler(20f, 180f, 0f);
        var tm = diveLabel.AddComponent<TextMesh>();
        tm.text = "✦ DIVE INTO WORLD 2 ✦\n[TOUCH TO ENTER FORGE]";
        tm.fontSize = 28;
        tm.characterSize = 0.032f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1.0f, 0.82f, 0.25f);
        tm.fontStyle = FontStyle.Bold;

        // Find or add SphereDivingTransition to Main Camera
        Camera mainCam = Camera.main;
        SphereDivingTransition divingTrans = null;
        if (mainCam != null)
        {
            divingTrans = mainCam.GetComponent<SphereDivingTransition>();
            if (divingTrans == null) divingTrans = mainCam.gameObject.AddComponent<SphereDivingTransition>();
        }

        exhibit.SetupDiveTransition(divingTrans, diveTouchpad);
    }

    // =========================================================================
    // 5. STATION 3: THE HEART OF HUMANITY (LANGUAGE, SEEDS, & MEMORY)
    // =========================================================================
    private static void BuildStation3_HeartOfHumanity(Transform parent, Material goldMat, Material plinthMat, Material obsidianMat,
                                                     Material glassMat, Material glowCyan, Material glowGold)
    {
        // Radius = 8.5m, Angle = +20° (North-East)
        float ang = 20f * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(ang) * 8.50f, 0f, Mathf.Cos(ang) * 8.50f);
        Quaternion rot = Quaternion.LookRotation(new Vector3(0f, 0f, -2f) - pos);

        GameObject st = CreateStationRoot(parent, "Station_03_HeartOfHumanity", pos, rot);
        BuildStationShrineBase(st.transform, goldMat, plinthMat, obsidianMat, glowCyan, "III. THE HUMAN ARCHIVE", "LANGUAGE, SEEDS, & SACRED MEMORY");

        // 1. Configure Terminal with Station 3 Astrobiological Pages
        var termDisplay = st.GetComponentInChildren<MuseumTerminalDisplay>();
        if (termDisplay != null)
        {
            termDisplay.SetCustomPages(new MuseumTerminalDisplay.TerminalPage[]
            {
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "I. THE WRITTEN WORD",
                    eraCode = "PTOLEMAIC DYNASTY // 196 B.C.",
                    bodyText = "In Memphis, Egypt, priests carved a royal decree across three scripts:\nHieroglyphs for the gods, Demotic for the people, and Greek for the crown.\nTwo millennia later, Terrans used this linguistic triad to unlock lost millennia of voices."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "II. THE ARCTIC SANCTUARY",
                    eraCode = "SPITSBERGEN ARCHIPELAGO // 2008 A.D.",
                    bodyText = "Chiseled 120 meters deep into permafrost sandstone on Svalbard Island,\nhumanity buried over 1.3 million distinct crop seed accessions.\nA planetary backup vault designed to outlive civilizational collapse, floods, and wars."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "III. THE MOLECULAR SCRIPT",
                    eraCode = "THE HUMAN GENOME // 2003 A.D.",
                    bodyText = "Using chemical sequencing and digital supercomputing, humanity decoded\nits own 3.2 billion base-pair blueprint. The species crossed a threshold:\nlife on Earth was no longer just read by natural selection—it could read itself."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "DIVE PROTOCOL // WORLD 3",
                    eraCode = "SENSORY 6DoF IMMERSION READY",
                    bodyText = "Full VR world transition available: The Heart of Humanity.\n\nReach out and cradle the Living Terran Sphere, or press the DIVE PEDESTAL to pull the biological and cultural archive into your consciousness and enter the Arctic Seed & Language Sanctuary."
                }
            });
        }

        // 2. Tripo Rosetta Stone (Language) on Pedestal
        GameObject rosetta = null;
        GameObject rosettaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_rosetta_stone.glb");
        if (rosettaPrefab != null)
        {
            rosetta = Object.Instantiate(rosettaPrefab, st.transform);
            rosetta.name = "Tripo_RosettaStoneArchive";
            rosetta.transform.localPosition = new Vector3(-0.25f, 1.72f, 0f);
            rosetta.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
            rosetta.transform.localScale = Vector3.one * 0.80f;

            var rCol = rosetta.AddComponent<BoxCollider>();
            rCol.center = new Vector3(0f, 0.40f, 0f);
            rCol.size = new Vector3(0.70f, 0.90f, 0.45f);
        }

        // 3. Tripo Svalbard Seed Vault (Life Sanctuary) on Pedestal
        GameObject seedVault = null;
        GameObject seedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_svalbard_seed_vault.glb");
        if (seedPrefab != null)
        {
            seedVault = Object.Instantiate(seedPrefab, st.transform);
            seedVault.name = "Tripo_SvalbardSeedSanctuary";
            seedVault.transform.localPosition = new Vector3(0.28f, 1.66f, -0.05f);
            seedVault.transform.localRotation = Quaternion.Euler(0f, -25f, 0f);
            seedVault.transform.localScale = Vector3.one * 0.70f;

            var sCol = seedVault.AddComponent<BoxCollider>();
            sCol.center = new Vector3(0f, 0.30f, 0f);
            sCol.size = new Vector3(0.75f, 0.65f, 0.75f);
        }

        // 4. Svalbard Sprout Particle System (ascending bio-luminescent genetic spores)
        GameObject sproutObj = new GameObject("Svalbard_Sprout_VFX");
        sproutObj.transform.SetParent(st.transform, false);
        sproutObj.transform.localPosition = new Vector3(0.28f, 1.65f, -0.05f);
        var sproutPS = sproutObj.AddComponent<ParticleSystem>();
        var spMain = sproutPS.main;
        spMain.startLifetime = 1.6f;
        spMain.startSpeed = 0.55f;
        spMain.startSize = 0.055f;
        spMain.startColor = new Color(0.25f, 0.95f, 0.75f, 0.90f);
        spMain.gravityModifier = -0.15f;
        spMain.maxParticles = 50;
        spMain.playOnAwake = false;
        var spEmission = sproutPS.emission;
        spEmission.rateOverTime = 20f;
        var spShape = sproutPS.shape;
        spShape.shapeType = ParticleSystemShapeType.Sphere;
        spShape.radius = 0.18f;

        Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
        var spPsr = sproutObj.GetComponent<ParticleSystemRenderer>();
        if (stardustMat != null) spPsr.sharedMaterial = stardustMat;

        // 5. Cryogenic Vault Glow Light
        GameObject vLightObj = new GameObject("Svalbard_VaultLight");
        vLightObj.transform.SetParent(st.transform, false);
        vLightObj.transform.localPosition = new Vector3(0.28f, 1.75f, -0.05f);
        var vLight = vLightObj.AddComponent<Light>();
        vLight.type = LightType.Point;
        vLight.color = new Color(0.25f, 0.85f, 0.80f);
        vLight.intensity = 0.6f;
        vLight.range = 3.8f;

        // 6. Svalbard Genetic Memory Seed Capsule (Dual Gyroscopic Stasis Rings + Bio-Crystal)
        GameObject memSeed = new GameObject("Svalbard_MemorySeed");
        memSeed.transform.SetParent(st.transform, false);
        memSeed.transform.localPosition = new Vector3(0.02f, 1.88f, 0.12f);

        // Central faceted biological embryo crystal (gold)
        GameObject seedCore = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seedCore.name = "Seed_CoreCrystal";
        seedCore.transform.SetParent(memSeed.transform, false);
        seedCore.transform.localPosition = Vector3.zero;
        seedCore.transform.localScale = Vector3.one * 0.12f;
        seedCore.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
        if (glowGold != null) seedCore.GetComponent<MeshRenderer>().sharedMaterial = glowGold;
        Object.DestroyImmediate(seedCore.GetComponent<Collider>());

        // Orbital Ring 1 (Cyan astrobiological stasis ring)
        GameObject ring1 = new GameObject("Seed_StasisRing_Cyan");
        ring1.transform.SetParent(memSeed.transform, false);
        ring1.transform.localRotation = Quaternion.Euler(30f, 45f, 0f);
        CreateMeshRing(ring1.transform, "RingCyanMesh", 0.18f, 0.015f, glowCyan, 0f);

        // Orbital Ring 2 (Gold bio-containment ring)
        GameObject ring2 = new GameObject("Seed_StasisRing_Gold");
        ring2.transform.SetParent(memSeed.transform, false);
        ring2.transform.localRotation = Quaternion.Euler(-45f, 30f, 0f);
        CreateMeshRing(ring2.transform, "RingGoldMesh", 0.15f, 0.012f, goldMat, 0f);

        var seedCol = memSeed.AddComponent<SphereCollider>();
        seedCol.radius = 0.22f;
        seedCol.isTrigger = true;

        var seedRot = memSeed.AddComponent<ExhibitRotator>();
        seedRot.rotationSpeed = 25f;
        seedRot.bobAmplitude = 0.02f;
        seedRot.bobSpeed = 2.0f;

        var seedLight = memSeed.AddComponent<Light>();
        seedLight.type = LightType.Point;
        seedLight.color = new Color(0.35f, 0.95f, 0.85f);
        seedLight.intensity = 2.8f;
        seedLight.range = 2.5f;

        memSeed.SetActive(false); // Dormant until ritual

        // 7. Monumental Living Terran Sphere hovering gracefully at Y=2.85m
        GameObject sphere = BuildDioramaSphere(st.transform, "DioramaSphere_HumanArchive", new Vector3(0f, 2.85f, 0f), 1.80f, glassMat, goldMat, glowCyan);

        // Inner DNA Double Helix Seed Core (16 nodes, grand scale)
        GameObject helix = new GameObject("Inner_DNA_SeedHelix");
        helix.transform.SetParent(sphere.transform, false);
        for (int i = 0; i < 16; i++)
        {
            float t = (float)i / 16f;
            float angle = t * Mathf.PI * 4f;
            float y = (t - 0.5f) * 0.80f;

            var n1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            n1.transform.SetParent(helix.transform, false);
            n1.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.26f, y, Mathf.Sin(angle) * 0.26f);
            n1.transform.localScale = Vector3.one * 0.075f;
            n1.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
            Object.DestroyImmediate(n1.GetComponent<Collider>());

            var n2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            n2.transform.SetParent(helix.transform, false);
            n2.transform.localPosition = new Vector3(-Mathf.Cos(angle) * 0.26f, y, -Mathf.Sin(angle) * 0.26f);
            n2.transform.localScale = Vector3.one * 0.075f;
            n2.GetComponent<MeshRenderer>().sharedMaterial = glowGold;
            Object.DestroyImmediate(n2.GetComponent<Collider>());
        }
        var hRot = helix.AddComponent<ExhibitRotator>();
        hRot.rotationSpeed = 15.0f;

        // Internal Soft Violet-Cyan Radiance Light
        var sLight = sphere.AddComponent<Light>();
        sLight.type = LightType.Point;
        sLight.color = new Color(0.35f, 0.85f, 1.0f);
        sLight.intensity = 3.5f;
        sLight.range = 5.5f;

        // Audio Clips
        var cryoClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
        var ascendClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bloom_chord.wav");
        var harvestClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");
        var chimeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_G.wav");
        var archiveAmbience = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");

        // 6DoF Immersion Trigger
        var immersion = sphere.AddComponent<DioramaImmersionTrigger>();
        immersion.SetupImmersion(null, sLight, archiveAmbience);

        // 8. Connect to HumanArchiveExhibit Controller
        var exhibit = st.AddComponent<HumanArchiveExhibit>();
        var relicSpot = st.transform.Find("Pedestal_RelicSpotlight")?.GetComponent<Light>();

        exhibit.SetupStation(
            rosetta != null ? rosetta.transform : st.transform,
            seedVault != null ? seedVault.transform : st.transform,
            sproutPS,
            vLight,
            memSeed,
            relicSpot,
            sphere.transform,
            sLight,
            termDisplay,
            cryoClip,
            ascendClip,
            harvestClip,
            chimeClip
        );

        // 9. Interactive DIVE INTO SEED & LANGUAGE SANCTUARY Pedestal
        GameObject divePlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        divePlinth.name = "DIVE_Button_Pedestal";
        divePlinth.transform.SetParent(st.transform, false);
        divePlinth.transform.localPosition = new Vector3(-1.35f, 0.48f, 0.95f);
        divePlinth.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
        divePlinth.transform.localScale = new Vector3(0.55f, 0.48f, 0.55f);
        if (plinthMat != null) divePlinth.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

        CreateMeshRing(divePlinth.transform, "DiveButton_GoldRing", 0.28f, 0.035f, goldMat, 1.01f);
        CreateMeshRing(divePlinth.transform, "DiveButton_CyanRing", 0.25f, 0.025f, glowCyan, 1.015f);

        var diveTouchpad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        diveTouchpad.name = "DiveButton_Touchpad";
        diveTouchpad.transform.SetParent(divePlinth.transform, false);
        diveTouchpad.transform.localPosition = new Vector3(0f, 1.03f, 0f);
        diveTouchpad.transform.localScale = new Vector3(0.70f, 0.04f, 0.70f);
        if (glowCyan != null) diveTouchpad.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;

        var diveLight = diveTouchpad.AddComponent<Light>();
        diveLight.type = LightType.Point;
        diveLight.color = new Color(0.25f, 0.95f, 0.85f);
        diveLight.intensity = 2.4f;
        diveLight.range = 2.5f;

        GameObject diveLabel = new GameObject("DiveButton_HoloLabel");
        diveLabel.transform.SetParent(divePlinth.transform, false);
        diveLabel.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        diveLabel.transform.localRotation = Quaternion.Euler(20f, 180f, 0f);
        var tm = diveLabel.AddComponent<TextMesh>();
        tm.text = "✦ DIVE INTO WORLD 3 ✦\n[TOUCH TO ENTER SANCTUARY]";
        tm.fontSize = 28;
        tm.characterSize = 0.032f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(0.35f, 0.95f, 0.90f);
        tm.fontStyle = FontStyle.Bold;

        // Find or add SphereDivingTransition to Main Camera
        Camera mainCam = Camera.main;
        SphereDivingTransition divingTrans = null;
        if (mainCam != null)
        {
            divingTrans = mainCam.GetComponent<SphereDivingTransition>();
            if (divingTrans == null) divingTrans = mainCam.gameObject.AddComponent<SphereDivingTransition>();
        }

        exhibit.SetupDiveTransition(divingTrans, diveTouchpad);
    }

    // =========================================================================
    // 6. STATION 4: THE COSMIC HORIZON (THE GOLDEN RECORD & VOYAGER)
    // =========================================================================
    private static void BuildStation4_CosmicHorizon(Transform parent, Material goldMat, Material plinthMat, Material obsidianMat,
                                                   Material glassMat, Material glowCyan, Material glowGold)
    {
        // Radius = 8.5m, Angle = +60° (East)
        float ang = 60f * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(ang) * 8.50f, 0f, Mathf.Cos(ang) * 8.50f);
        Quaternion rot = Quaternion.LookRotation(new Vector3(0f, 0f, -2f) - pos);

        GameObject st = CreateStationRoot(parent, "Station_04_CosmicHorizon", pos, rot);
        BuildStationShrineBase(st.transform, goldMat, plinthMat, obsidianMat, glowGold, "IV. THE COSMIC HORIZON", "THE VOYAGER GOLDEN RECORD // 1977");

        // 1. Configure Terminal with Station 4 Astrobiological Pages
        var termDisplay = st.GetComponentInChildren<MuseumTerminalDisplay>();
        if (termDisplay != null)
        {
            termDisplay.SetCustomPages(new MuseumTerminalDisplay.TerminalPage[]
            {
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "I. THE FIRST SHORES",
                    eraCode = "APOLLO 11 // TRANQUILITY BASE // 1969 A.D.",
                    bodyText = "For four billion years, Earth life was bound to its planetary cradle.\nIn July 1969, two Terrans stepped onto the regolith of the Moon.\nLeft upon the ladder of the descent stage was a stainless steel plaque:\n\"Here men from the planet Earth first set foot upon the Moon. We came in peace for all mankind.\""
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "II. THE MESSAGE IN A BOTTLE",
                    eraCode = "VOYAGER 1 & 2 // THE HELIOPAUSE // 1977 A.D.",
                    bodyText = "Carried aboard twin nuclear-powered probes cast toward the stars,\nthe Golden Record contains 115 encoded images, greetings in 55 languages,\nand a 90-minute sonic journey: rainstorms, birdsong, Bach, and human brainwaves.\nConstructed of gold-plated copper, it will endure intact for over a billion years."
                },
                new MuseumTerminalDisplay.TerminalPage
                {
                    title = "III. THE INTERSTELLAR EMBASSY",
                    eraCode = "THE KARDASHEV THRESHOLD // DEEP TIME",
                    bodyText = "Long after Earth's oceans boil and the Sun swells into a red giant,\nthe Voyager records will silently cruise the Orion Arm between the stars.\nEven if Terran civilization ceases to exist, this gold disc testifies forever:\nWe lived, we loved, and we reached outward to find you."
                }
            });
        }

        // 2. Tripo Voyager Golden Record on Pedestal
        GameObject record = null;
        GameObject recordPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_voyager_golden_record.glb");
        if (recordPrefab != null)
        {
            record = Object.Instantiate(recordPrefab, st.transform);
            record.name = "Tripo_VoyagerGoldenRecord";
            record.transform.localPosition = new Vector3(-0.20f, 1.75f, 0f);
            record.transform.localRotation = Quaternion.Euler(0f, 25f, 0f);
            record.transform.localScale = Vector3.one * 0.85f;

            var recCol = record.AddComponent<BoxCollider>();
            recCol.center = new Vector3(0f, 0.35f, 0f);
            recCol.size = new Vector3(0.85f, 0.85f, 0.35f);
        }

        // 3. Tripo Apollo 11 Lunar Plaque on Pedestal
        GameObject apollo = null;
        GameObject apolloPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_apollo11_moon_plaque.glb");
        if (apolloPrefab != null)
        {
            apollo = Object.Instantiate(apolloPrefab, st.transform);
            apollo.name = "Tripo_Apollo11PeacePlaque";
            apollo.transform.localPosition = new Vector3(0.30f, 1.66f, 0f);
            apollo.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
            apollo.transform.localScale = Vector3.one * 0.70f;

            var apCol = apollo.AddComponent<BoxCollider>();
            apCol.center = new Vector3(0f, 0.30f, 0f);
            apCol.size = new Vector3(0.70f, 0.65f, 0.40f);
        }

        // 4. Diamond Stylus Tone Arm Assembly
        GameObject stylusRoot = new GameObject("Voyager_StylusArm");
        stylusRoot.transform.SetParent(st.transform, false);
        stylusRoot.transform.localPosition = new Vector3(-0.48f, 1.62f, 0.12f);

        GameObject stylusBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stylusBase.name = "Stylus_PivotBase";
        stylusBase.transform.SetParent(stylusRoot.transform, false);
        stylusBase.transform.localScale = new Vector3(0.06f, 0.04f, 0.06f);
        if (goldMat != null) stylusBase.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(stylusBase.GetComponent<Collider>());

        GameObject stylusRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stylusRod.name = "Stylus_ToneArm";
        stylusRod.transform.SetParent(stylusRoot.transform, false);
        stylusRod.transform.localPosition = new Vector3(0.12f, 0.08f, -0.05f);
        stylusRod.transform.localRotation = Quaternion.Euler(20f, 40f, 85f);
        stylusRod.transform.localScale = new Vector3(0.015f, 0.14f, 0.015f);
        if (goldMat != null) stylusRod.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(stylusRod.GetComponent<Collider>());

        GameObject needleHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        needleHead.name = "Stylus_NeedleHead";
        needleHead.transform.SetParent(stylusRoot.transform, false);
        needleHead.transform.localPosition = new Vector3(0.24f, 0.05f, -0.10f);
        needleHead.transform.localScale = Vector3.one * 0.035f;
        if (glowCyan != null) needleHead.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
        Object.DestroyImmediate(needleHead.GetComponent<Collider>());

        // 5. Interstellar Pulsar Particle System (radiant golden starbeams)
        GameObject pulsarObj = new GameObject("Voyager_Pulsar_VFX");
        pulsarObj.transform.SetParent(st.transform, false);
        pulsarObj.transform.localPosition = new Vector3(-0.20f, 1.75f, 0f);
        var pulsarPS = pulsarObj.AddComponent<ParticleSystem>();
        var pMain = pulsarPS.main;
        pMain.startLifetime = 1.8f;
        pMain.startSpeed = 0.65f;
        pMain.startSize = 0.045f;
        pMain.startColor = new Color(1.0f, 0.88f, 0.35f, 0.95f);
        pMain.gravityModifier = -0.20f;
        pMain.maxParticles = 50;
        pMain.playOnAwake = false;
        var pEmission = pulsarPS.emission;
        pEmission.rateOverTime = 22f;
        var pShape = pulsarPS.shape;
        pShape.shapeType = ParticleSystemShapeType.Cone;
        pShape.angle = 25f;
        pShape.radius = 0.22f;

        Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
        var pPsr = pulsarObj.GetComponent<ParticleSystemRenderer>();
        if (stardustMat != null) pPsr.sharedMaterial = stardustMat;

        // 6. Turntable Illumination Glow Light
        GameObject tLightObj = new GameObject("Voyager_TurntableLight");
        tLightObj.transform.SetParent(st.transform, false);
        tLightObj.transform.localPosition = new Vector3(-0.20f, 1.80f, 0f);
        var tLight = tLightObj.AddComponent<Light>();
        tLight.type = LightType.Point;
        tLight.color = new Color(1.0f, 0.82f, 0.35f);
        tLight.intensity = 0.5f;
        tLight.range = 4.0f;

        // 7. Voyager Interstellar Memory Disc (Collectable relic)
        GameObject memDisc = new GameObject("Voyager_MemoryDisc");
        memDisc.transform.SetParent(st.transform, false);
        memDisc.transform.localPosition = new Vector3(0.02f, 1.90f, 0.12f);

        // Core Golden Disc
        GameObject discCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        discCore.name = "Disc_GoldRecordCore";
        discCore.transform.SetParent(memDisc.transform, false);
        discCore.transform.localPosition = Vector3.zero;
        discCore.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
        discCore.transform.localScale = new Vector3(0.14f, 0.012f, 0.14f);
        if (glowGold != null) discCore.GetComponent<MeshRenderer>().sharedMaterial = glowGold;
        Object.DestroyImmediate(discCore.GetComponent<Collider>());

        // Pulsar Wave Ring 1 (Cyan)
        GameObject pRing1 = new GameObject("Disc_PulsarRing_Cyan");
        pRing1.transform.SetParent(memDisc.transform, false);
        pRing1.transform.localRotation = Quaternion.Euler(25f, 40f, 0f);
        CreateMeshRing(pRing1.transform, "RingPulsarCyanMesh", 0.18f, 0.015f, glowCyan, 0f);

        // Pulsar Wave Ring 2 (Gold)
        GameObject pRing2 = new GameObject("Disc_PulsarRing_Gold");
        pRing2.transform.SetParent(memDisc.transform, false);
        pRing2.transform.localRotation = Quaternion.Euler(-40f, 25f, 0f);
        CreateMeshRing(pRing2.transform, "RingPulsarGoldMesh", 0.15f, 0.012f, goldMat, 0f);

        var discCol = memDisc.AddComponent<SphereCollider>();
        discCol.radius = 0.22f;
        discCol.isTrigger = true;

        var discRot = memDisc.AddComponent<ExhibitRotator>();
        discRot.rotationSpeed = 35f;
        discRot.bobAmplitude = 0.02f;
        discRot.bobSpeed = 2.2f;

        var discLight = memDisc.AddComponent<Light>();
        discLight.type = LightType.Point;
        discLight.color = new Color(1.0f, 0.88f, 0.40f);
        discLight.intensity = 2.8f;
        discLight.range = 2.5f;

        memDisc.SetActive(false); // Dormant until playback initiated

        // 8. Monumental Living Terran Sphere hovering gracefully at Y=2.85m
        GameObject sphere = BuildDioramaSphere(st.transform, "DioramaSphere_CosmicHorizon", new Vector3(0f, 2.85f, 0f), 1.80f, glassMat, goldMat, glowGold);

        // Inner Spinning Golden Record Miniature
        if (recordPrefab != null)
        {
            GameObject miniRecord = Object.Instantiate(recordPrefab, sphere.transform);
            miniRecord.name = "Inner_MiniatureGoldenRecord";
            miniRecord.transform.localPosition = Vector3.zero;
            miniRecord.transform.localScale = Vector3.one * 0.45f;
            var rRot = miniRecord.AddComponent<ExhibitRotator>();
            rRot.rotationSpeed = 28.0f;
            rRot.bobAmplitude = 0.02f;
            rRot.bobSpeed = 1.8f;
        }

        // Concentric Radio Wave rings
        CreateMeshRing(sphere.transform, "RadioWaveRing_0", 0.55f, 0.035f, glowGold, 0.08f);
        CreateMeshRing(sphere.transform, "RadioWaveRing_1", 0.78f, 0.035f, glowCyan, -0.08f);

        // Internal Bright Gold Star Light
        var sLight = sphere.AddComponent<Light>();
        sLight.type = LightType.Point;
        sLight.color = new Color(1.0f, 0.90f, 0.65f);
        sLight.intensity = 3.8f;
        sLight.range = 6.0f;

        // Audio Clips
        var needleClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_E.wav");
        var pulsarClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_G.wav");
        var musicClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bloom_chord.wav");
        var harvestClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");
        var spaceAmbience = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");

        // 6DoF Immersion Trigger
        var immersion = sphere.AddComponent<DioramaImmersionTrigger>();
        immersion.SetupImmersion(null, sLight, spaceAmbience);

        // 9. Connect to CosmicHorizonExhibit Controller
        var exhibit = st.AddComponent<CosmicHorizonExhibit>();
        var relicSpot = st.transform.Find("Pedestal_RelicSpotlight")?.GetComponent<Light>();

        exhibit.SetupStation(
            record != null ? record.transform : st.transform,
            apollo != null ? apollo.transform : st.transform,
            stylusRoot.transform,
            pulsarPS,
            tLight,
            memDisc,
            relicSpot,
            sphere.transform,
            sLight,
            termDisplay,
            needleClip,
            pulsarClip,
            musicClip,
            harvestClip
        );
    }

    // =========================================================================
    // 7. PERIMETER LUXURY LOUNGES
    // =========================================================================
    private static void BuildPerimeterLounges(Transform parent)
    {
        GameObject loungePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_luxury_lounge_bench.glb");
        if (loungePrefab == null) return;

        GameObject loungeRoot = new GameObject("Perimeter_LuxuryLounges");
        loungeRoot.transform.SetParent(parent, false);

        // West Wing Lounge (facing East toward Rotunda)
        GameObject benchWest = Object.Instantiate(loungePrefab, loungeRoot.transform);
        benchWest.name = "Tripo_LuxuryLounge_WestWing";
        benchWest.transform.localPosition = new Vector3(-12.0f, 0.42f, 0f);
        benchWest.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        benchWest.transform.localScale = Vector3.one * 2.10f;

        // East Wing Lounge (facing West toward Rotunda)
        GameObject benchEast = Object.Instantiate(loungePrefab, loungeRoot.transform);
        benchEast.name = "Tripo_LuxuryLounge_EastWing";
        benchEast.transform.localPosition = new Vector3(12.0f, 0.42f, 0f);
        benchEast.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        benchEast.transform.localScale = Vector3.one * 2.10f;

        // West Gallery Corner Lounge (overlooking amphitheater)
        GameObject benchWestCorner = Object.Instantiate(loungePrefab, loungeRoot.transform);
        benchWestCorner.name = "Tripo_LuxuryLounge_WestCorner";
        benchWestCorner.transform.localPosition = new Vector3(-10.5f, 0.42f, 7.0f);
        benchWestCorner.transform.localRotation = Quaternion.Euler(0f, 135f, 0f);
        benchWestCorner.transform.localScale = Vector3.one * 1.85f;

        // East Gallery Corner Lounge (overlooking amphitheater)
        GameObject benchEastCorner = Object.Instantiate(loungePrefab, loungeRoot.transform);
        benchEastCorner.name = "Tripo_LuxuryLounge_EastCorner";
        benchEastCorner.transform.localPosition = new Vector3(10.5f, 0.42f, 7.0f);
        benchEastCorner.transform.localRotation = Quaternion.Euler(0f, -135f, 0f);
        benchEastCorner.transform.localScale = Vector3.one * 1.85f;
    }

    // =========================================================================
    // 8. CURATORIAL GALLERY LIGHTING
    // =========================================================================
    private static void BuildGalleryLighting(Transform parent)
    {
        GameObject lightsRoot = new GameObject("Curatorial_GalleryLighting");
        lightsRoot.transform.SetParent(parent, false);

        // 4 Overhead Pin-Spotlights aimed at the 4 Living Spheres from the vaulted ceiling (Y=8.0m)
        Vector3[] sphereTargets = new Vector3[] {
            new Vector3(-7.36f, 2.30f, 4.25f),
            new Vector3(-2.91f, 2.30f, 7.99f),
            new Vector3(2.91f, 2.30f, 7.99f),
            new Vector3(7.36f, 2.30f, 4.25f)
        };

        for (int i = 0; i < sphereTargets.Length; i++)
        {
            GameObject spotGo = new GameObject("StationPinSpot_" + (i + 1));
            spotGo.transform.SetParent(lightsRoot.transform, false);

            // Overhead slightly forward from the sphere
            Vector3 spotPos = new Vector3(sphereTargets[i].x * 0.85f, 8.0f, sphereTargets[i].z * 0.75f);
            spotGo.transform.localPosition = spotPos;
            spotGo.transform.LookAt(parent.TransformPoint(sphereTargets[i]));

            var light = spotGo.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = new Color(1.0f, 0.94f, 0.86f); // Warm crisp museum gallery tungsten
            light.intensity = 3.5f;
            light.range = 12.0f;
            light.spotAngle = 46.0f;
        }

        // Center Dais Overhead Gyro Spot
        GameObject centerSpot = new GameObject("Center_GyroOverheadSpot");
        centerSpot.transform.SetParent(lightsRoot.transform, false);
        centerSpot.transform.localPosition = new Vector3(0f, 9.50f, 0f);
        centerSpot.transform.LookAt(parent.TransformPoint(new Vector3(0f, 2.45f, 0f)));

        var cSpot = centerSpot.AddComponent<Light>();
        cSpot.type = LightType.Spot;
        cSpot.color = new Color(1.0f, 0.96f, 0.90f);
        cSpot.intensity = 4.0f;
        cSpot.range = 14.0f;
        cSpot.spotAngle = 40.0f;

        // Soft deep-space museum ambient fill
        GameObject ambientFill = new GameObject("Museum_DeepAtmosphereFill");
        ambientFill.transform.SetParent(lightsRoot.transform, false);
        ambientFill.transform.localPosition = new Vector3(0f, 4.50f, 0f);
        var aLight = ambientFill.AddComponent<Light>();
        aLight.type = LightType.Point;
        aLight.color = new Color(0.12f, 0.18f, 0.28f);
        aLight.intensity = 1.6f;
        aLight.range = 22.0f;
    }

    // =========================================================================
    // HELPER METHODS: SHRINE PLATFORM, PEDESTAL, PLAQUES & SPHERES
    // =========================================================================
    private static GameObject CreateStationRoot(Transform parent, string name, Vector3 pos, Quaternion rot)
    {
        GameObject st = new GameObject(name);
        st.transform.SetParent(parent, false);
        st.transform.localPosition = pos;
        st.transform.localRotation = rot;
        return st;
    }

    private static void BuildStationShrineBase(Transform station, Material goldMat, Material plinthMat, Material obsidianMat,
                                              Material glowMat, string title, string subtitle)
    {
        // 1. Tiered Shrine Dais:
        // Tier 1 Base Plinth (Diameter 4.20m, Height 0.10m)
        var daisT1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisT1.name = "Shrine_DaisTier1_Base";
        daisT1.transform.SetParent(station, false);
        daisT1.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        daisT1.transform.localScale = new Vector3(4.20f, 0.05f, 4.20f);
        daisT1.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat ?? plinthMat;
        Object.DestroyImmediate(daisT1.GetComponent<Collider>());

        CreateMeshRing(station, "Shrine_T1_GoldRim", 2.10f, 0.08f, goldMat, 0.105f);

        // Tier 2 Inner Step (Diameter 3.20m, Height 0.06m)
        var daisT2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisT2.name = "Shrine_DaisTier2_Step";
        daisT2.transform.SetParent(station, false);
        daisT2.transform.localPosition = new Vector3(0f, 0.13f, 0f);
        daisT2.transform.localScale = new Vector3(3.20f, 0.03f, 3.20f);
        daisT2.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;
        Object.DestroyImmediate(daisT2.GetComponent<Collider>());

        CreateMeshRing(station, "Shrine_T2_GoldRim", 1.60f, 0.06f, goldMat, 0.165f);
        CreateMeshRing(station, "Shrine_T2_GlowRim", 1.45f, 0.03f, glowMat, 0.168f);

        // 2. High-Poly Tripo Obsidian & Gold Pedestal (Scale 1.30x, showcase height = 1.28m)
        GameObject pPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_pedestal_obsidian_gold.glb");
        if (pPrefab != null)
        {
            GameObject p = Object.Instantiate(pPrefab, station);
            p.name = "Tripo_ObsidianGold_Pedestal";
            // Height = 0.86 * 1.30 = 1.118m -> Center Y = 0.16m (dais) + 0.559m = 0.72m
            p.transform.localPosition = new Vector3(0f, 0.72f, 0f);
            p.transform.localScale = Vector3.one * 1.30f;

            var col = p.AddComponent<BoxCollider>();
            col.center = Vector3.zero;
            col.size = new Vector3(1.0f, 0.86f, 1.0f);
        }

        // 3. Dedicated Artifact Gallery Downlight (illuminating sacred relics on the pedestal)
        GameObject spotRelic = new GameObject("Pedestal_RelicSpotlight");
        spotRelic.transform.SetParent(station, false);
        spotRelic.transform.localPosition = new Vector3(0f, 2.70f, 1.25f);
        spotRelic.transform.LookAt(station.TransformPoint(new Vector3(0f, 1.55f, 0f)));

        var rLight = spotRelic.AddComponent<Light>();
        rLight.type = LightType.Spot;
        rLight.color = new Color(1.0f, 0.95f, 0.88f);
        rLight.intensity = 3.2f;
        rLight.range = 4.5f;
        rLight.spotAngle = 60.0f;

        // 4. Interactive Alien Curator Kiosk (museum_interactive_terminal.glb) on the front-right of the dais
        GameObject termPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_interactive_terminal.glb");
        if (termPrefab != null)
        {
            GameObject term = Object.Instantiate(termPrefab, station);
            term.name = "Tripo_CuratorInteractiveTerminal";
            term.transform.localPosition = new Vector3(1.35f, 0.70f, 1.05f);
            term.transform.localRotation = Quaternion.Euler(0f, 47f, 0f); // Screen (-X) angled directly facing incoming visitor
            term.transform.localScale = Vector3.one * 1.15f;

            var tCol = term.AddComponent<BoxCollider>();
            tCol.center = Vector3.zero;
            tCol.size = new Vector3(0.55f, 1.05f, 0.65f);

            var termDisplay = term.AddComponent<MuseumTerminalDisplay>();
            var switchClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_E.wav");
            termDisplay.SetupTerminal(switchClip);
        }

        // 5. Curator Plaque mounted on front rim of the shrine dais facing incoming visitor
        CreateStationCuratorPlaque(station, title, subtitle, goldMat, glowMat);
    }

    private static GameObject BuildDioramaSphere(Transform station, string name, Vector3 localPos, float diameter,
                                                 Material glassMat, Material goldMat, Material glowMat)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(station, false);
        sphere.transform.localPosition = localPos;
        sphere.transform.localScale = Vector3.one * diameter;
        if (glassMat != null) sphere.GetComponent<MeshRenderer>().sharedMaterial = glassMat;
        Object.DestroyImmediate(sphere.GetComponent<Collider>());

        var rotator = sphere.AddComponent<ExhibitRotator>();
        rotator.rotationSpeed = 6.0f;
        rotator.bobAmplitude = 0.03f;
        rotator.bobSpeed = 1.1f;

        // Anti-Gravity Halo Containment Ring around Sphere Equator
        CreateMeshRing(sphere.transform, "Halo_EquatorRing", diameter * 0.56f, 0.05f, goldMat, 0f);

        // Tilted Orbital Accent Ring
        GameObject tiltOrbit = new GameObject("Halo_TiltedOrbit");
        tiltOrbit.transform.SetParent(sphere.transform, false);
        tiltOrbit.transform.localRotation = Quaternion.Euler(25f, 0f, 0f);
        CreateMeshRing(tiltOrbit.transform, "Halo_TiltedRingMesh", diameter * 0.60f, 0.035f, glowMat, 0f);

        return sphere;
    }

    private static void CreateStationCuratorPlaque(Transform station, string title, string subtitle, Material goldMat, Material glowMat)
    {
        GameObject plaqueRoot = new GameObject("Curator_StationPlaque");
        plaqueRoot.transform.SetParent(station, false);
        // Positioned at the FRONT of the shrine dais facing incoming visitor
        plaqueRoot.transform.localPosition = new Vector3(0f, 0.45f, 1.65f);
        plaqueRoot.transform.localRotation = Quaternion.Euler(30f, 0f, 0f); // Tilted back towards visitor

        // Gold Bevel Outer Frame
        var outerFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        outerFrame.name = "Plaque_GoldBevelFrame";
        outerFrame.transform.SetParent(plaqueRoot.transform, false);
        outerFrame.transform.localScale = new Vector3(1.18f, 0.32f, 0.04f);
        outerFrame.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(outerFrame.GetComponent<Collider>());

        // Dark Obsidian Backing Plate
        Material plinthMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_DaisPlinth.mat");
        var backPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backPlate.name = "Plaque_ObsidianPlate";
        backPlate.transform.SetParent(plaqueRoot.transform, false);
        backPlate.transform.localPosition = new Vector3(0f, 0f, -0.015f);
        backPlate.transform.localScale = new Vector3(1.12f, 0.28f, 0.02f);
        backPlate.GetComponent<MeshRenderer>().sharedMaterial = plinthMat ?? goldMat;
        Object.DestroyImmediate(backPlate.GetComponent<Collider>());

        // Emissive Holographic Glass Inlay
        var glowStrip = GameObject.CreatePrimitive(PrimitiveType.Quad);
        glowStrip.name = "Plaque_HoloEmissiveStrip";
        glowStrip.transform.SetParent(plaqueRoot.transform, false);
        glowStrip.transform.localPosition = new Vector3(0f, 0f, 0.022f);
        glowStrip.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        glowStrip.transform.localScale = new Vector3(1.06f, 0.22f, 1f);
        glowStrip.GetComponent<MeshRenderer>().sharedMaterial = glowMat;
        Object.DestroyImmediate(glowStrip.GetComponent<Collider>());

        // 3D TextMesh Title & Subtitle for Authentic Curatorial Display
        GameObject textObj = new GameObject("Plaque_TextMesh");
        textObj.transform.SetParent(plaqueRoot.transform, false);
        textObj.transform.localPosition = new Vector3(0f, 0.01f, 0.035f);
        textObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        var tm = textObj.AddComponent<TextMesh>();
        tm.text = $"{title}\n<size=22>{subtitle}</size>";
        tm.fontSize = 28;
        tm.characterSize = 0.022f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.fontStyle = FontStyle.Bold;

        // Subtle warm glow on the plaque text
        var pLight = plaqueRoot.AddComponent<Light>();
        pLight.type = LightType.Point;
        pLight.color = new Color(1.0f, 0.95f, 0.85f);
        pLight.intensity = 0.8f;
        pLight.range = 1.8f;
    }

    private static void CreateMeshRing(Transform parent, string name, float radius, float width, Material mat, float yPos)
    {
        GameObject ringObj = new GameObject(name);
        ringObj.transform.SetParent(parent, false);
        ringObj.transform.localPosition = new Vector3(0f, yPos, 0f);

        MeshFilter mf = ringObj.AddComponent<MeshFilter>();
        MeshRenderer mr = ringObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        int segments = 48;
        Mesh mesh = new Mesh { name = name + "_Mesh" };

        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        Vector2[] uvs = new Vector2[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        float rInner = radius - width * 0.5f;
        float rOuter = radius + width * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            vertices[i * 2] = new Vector3(cos * rInner, 0f, sin * rInner);
            vertices[i * 2 + 1] = new Vector3(cos * rOuter, 0f, sin * rOuter);

            uvs[i * 2] = new Vector2((float)i / segments, 0f);
            uvs[i * 2 + 1] = new Vector2((float)i / segments, 1f);

            if (i < segments)
            {
                int baseIdx = i * 2;
                int triIdx = i * 6;

                triangles[triIdx] = baseIdx;
                triangles[triIdx + 1] = baseIdx + 1;
                triangles[triIdx + 2] = baseIdx + 2;

                triangles[triIdx + 3] = baseIdx + 1;
                triangles[triIdx + 4] = baseIdx + 3;
                triangles[triIdx + 5] = baseIdx + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.sharedMesh = mesh;
    }
}
