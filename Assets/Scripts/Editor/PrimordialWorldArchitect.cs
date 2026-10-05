#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using Xenoasis.Core;
using Xenoasis.Player;

namespace Xenoasis.EditorTools
{
    /// <summary>
    /// Procedurally constructs and populates the World 1: The Primordial Cradle scene.
    /// Integrates WorldLabs 360 atmosphere, 6 Tripo 3D model assets, water, lighting, and return portal.
    /// </summary>
    public static class PrimordialWorldArchitect
    {
        private const string ScenePath = "Assets/Scenes/World1_PrimordialCradle.unity";

        [MenuItem("XENOASIS/World 1/Build Primordial Cradle Scene", false, 10)]
        public static void BuildWorld1Scene()
        {
            Debug.Log("[PrimordialWorldArchitect] ✦ Initiating assembly of World 1: The Primordial Cradle...");

            // Ensure directory exists
            string sceneDir = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
            }

            Scene currentScene = EditorSceneManager.GetActiveScene();
            Scene scene;
            bool wasUntitled = string.IsNullOrEmpty(currentScene.path);

            if (wasUntitled)
            {
                // In batchmode or fresh editor session with untitled scene, just use it
                scene = currentScene;
            }
            else
            {
                EditorSceneManager.SaveOpenScenes();
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }

            // Clear old hierarchy
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var go in rootObjects)
            {
                Object.DestroyImmediate(go);
            }

            // Materials & Prefabs
            Material goldMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LuxuryGold_Inlay.mat");
            Material plinthMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_DaisPlinth.mat");
            Material obsidianMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ObsidianGlass.mat");
            Material glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassDome.mat") 
                             ?? AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
            Material glowCyan = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
            Material glowGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");
            Material waterMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WaterRefraction.mat");
            Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
            Material darkHullMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat");

            // Audio clips
            AudioClip oceanClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/water_drops.wav");
            AudioClip droneClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");
            AudioClip chimeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
            AudioClip snapClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");

            // Hierarchy Root Containers
            GameObject worldRoot = new GameObject("--- PRIMORDIAL CRADLE ---");
            GameObject envRoot = new GameObject("[Environment]");
            envRoot.transform.SetParent(worldRoot.transform, false);
            GameObject relicsRoot = new GameObject("[Interactive_Relics]");
            relicsRoot.transform.SetParent(worldRoot.transform, false);
            GameObject vfxRoot = new GameObject("[VFX_Particles]");
            vfxRoot.transform.SetParent(worldRoot.transform, false);
            GameObject audioRoot = new GameObject("[Spatial_Audio]");
            audioRoot.transform.SetParent(worldRoot.transform, false);
            GameObject managersRoot = new GameObject("[Managers]");
            managersRoot.transform.SetParent(worldRoot.transform, false);

            // =========================================================================
            // 1. LIGHTING, ATMOSPHERE & SKYBOX
            // =========================================================================
            SetupAtmosphereAndSkybox();

            // Archean Faint Young Sun (Warm amber-violet directional light)
            GameObject sunGo = new GameObject("Sun_ArcheanFaintSun");
            sunGo.transform.SetParent(envRoot.transform, false);
            sunGo.transform.rotation = Quaternion.Euler(22f, -38f, 0f);
            var sunLight = sunGo.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.65f, 0.32f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Fill Light (Soft teal-green reflection from primordial ocean)
            GameObject fillLightGo = new GameObject("Sun_OceanBioluminescentFill");
            fillLightGo.transform.SetParent(envRoot.transform, false);
            fillLightGo.transform.rotation = Quaternion.Euler(-65f, 140f, 0f);
            var fillLight = fillLightGo.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.12f, 0.55f, 0.52f);
            fillLight.intensity = 0.45f;
            fillLight.shadows = LightShadows.None;

            // =========================================================================
            // 2. BASALT BEACH, ROCK FORMATIONS & LAGOON WATER
            // =========================================================================
            // Try loading WorldLabs collider mesh if generated
            GameObject colliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/WorldLabs/primordial_cradle_collider.glb");
            if (colliderPrefab != null)
            {
                GameObject meshColliderGo = Object.Instantiate(colliderPrefab, envRoot.transform);
                meshColliderGo.name = "WorldLabs_Terrain_Collider";
                meshColliderGo.transform.localPosition = Vector3.zero;
                meshColliderGo.transform.localScale = Vector3.one * 8.0f;
            }

            // Basalt Coast Platform (Where player walks)
            GameObject coastPlatform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coastPlatform.name = "Basalt_LagoonTerrace_Main";
            coastPlatform.transform.SetParent(envRoot.transform, false);
            coastPlatform.transform.localPosition = new Vector3(0f, -0.45f, 0f);
            coastPlatform.transform.localScale = new Vector3(26.0f, 0.45f, 26.0f);
            if (obsidianMat != null) coastPlatform.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat;

            // Hexagonal Basalt Column Clusters (Procedural basalt stepping rocks)
            CreateBasaltColumns(envRoot.transform, darkHullMat ?? obsidianMat);

            // Shimmering Turquoise Geothermal Lagoon Water Plane
            GameObject lagoonWater = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lagoonWater.name = "Water_LagoonSurface";
            lagoonWater.transform.SetParent(envRoot.transform, false);
            lagoonWater.transform.localPosition = new Vector3(0f, -0.05f, 8.0f);
            lagoonWater.transform.localScale = new Vector3(32.0f, 0.02f, 32.0f);
            var waterRend = lagoonWater.GetComponent<MeshRenderer>();
            if (waterMat != null) waterRend.sharedMaterial = waterMat;
            Object.DestroyImmediate(lagoonWater.GetComponent<Collider>());

            // =========================================================================
            // 3. TRIPO 3D INTERACTIVE RELICS & PROPS
            // =========================================================================
            // Prop 1: Hydrothermal Chimneys (Steaming volcanic rock chimneys)
            GameObject chimney1 = SpawnTripoProp("Assets/Models/Tripo/primordial_hydrothermal_chimney.glb",
                "Tripo_HydrothermalChimney_West", relicsRoot.transform,
                new Vector3(-6.2f, 0.0f, 7.8f), Quaternion.Euler(0f, 25f, 0f), Vector3.one * 3.4f);

            GameObject chimney2 = SpawnTripoProp("Assets/Models/Tripo/primordial_hydrothermal_chimney.glb",
                "Tripo_HydrothermalChimney_East", relicsRoot.transform,
                new Vector3(7.4f, 0.0f, 9.2f), Quaternion.Euler(0f, -65f, 0f), Vector3.one * 3.8f);

            // Add steam particle systems to the chimneys
            ParticleSystem steamPs1 = AddVentSteam(chimney1 != null ? chimney1.transform : relicsRoot.transform, new Vector3(0f, 2.8f, 0f));
            ParticleSystem steamPs2 = AddVentSteam(chimney2 != null ? chimney2.transform : relicsRoot.transform, new Vector3(0f, 3.2f, 0f));

            // Prop 2: Stromatolite Colonies (Fossilized microbial dome colonies)
            GameObject stromatolite1 = SpawnTripoProp("Assets/Models/Tripo/primordial_stromatolite_colony.glb",
                "Tripo_StromatoliteColony_ShallowsWest", relicsRoot.transform,
                new Vector3(-3.8f, -0.20f, 4.2f), Quaternion.Euler(0f, 40f, 0f), Vector3.one * 2.0f);

            GameObject stromatolite2 = SpawnTripoProp("Assets/Models/Tripo/primordial_stromatolite_colony.glb",
                "Tripo_StromatoliteColony_ShallowsEast", relicsRoot.transform,
                new Vector3(4.5f, -0.22f, 4.8f), Quaternion.Euler(0f, -80f, 0f), Vector3.one * 2.2f);

            // Prop 3: Giant Ammonite Shell Rock
            GameObject ammonite = SpawnTripoProp("Assets/Models/Tripo/primordial_giant_ammonite_rock.glb",
                "Tripo_GiantAmmonite_FossilRock", relicsRoot.transform,
                new Vector3(6.5f, 0.40f, 2.5f), Quaternion.Euler(15f, -120f, -10f), Vector3.one * 2.4f);

            // Add localized spotlight on ammonite
            if (ammonite != null)
            {
                var ammoSpot = ammonite.AddComponent<Light>();
                ammoSpot.type = LightType.Spot;
                ammoSpot.color = new Color(0.4f, 0.9f, 1.0f);
                ammoSpot.intensity = 3.5f;
                ammoSpot.range = 5.0f;
                ammoSpot.spotAngle = 45f;
            }

            // Prop 4: Levitating Prebiotic Crystal Spire (Centerpiece of Lagoon)
            GameObject crystalSpire = SpawnTripoProp("Assets/Models/Tripo/primordial_prebiotic_crystal_spire.glb",
                "Tripo_PrebioticCrystalSpire_FloatingCore", relicsRoot.transform,
                new Vector3(0f, 2.2f, 10.5f), Quaternion.identity, Vector3.one * 2.6f);

            if (crystalSpire != null)
            {
                var rotator = crystalSpire.AddComponent<ExhibitRotator>();
                rotator.rotationSpeed = 6.0f;
                rotator.bobAmplitude = 0.12f;
                rotator.bobSpeed = 1.0f;

                var crystalLight = crystalSpire.AddComponent<Light>();
                crystalLight.type = LightType.Point;
                crystalLight.color = new Color(0.15f, 0.95f, 1.0f);
                crystalLight.intensity = 5.0f;
                crystalLight.range = 14.0f;
            }

            // Prop 5: Bioluminescent Water Lotuses (Floating on lagoon)
            GameObject lotus1 = SpawnTripoProp("Assets/Models/Tripo/primordial_bioluminescent_lotus.glb",
                "Tripo_BioLotus_01", relicsRoot.transform,
                new Vector3(-1.6f, -0.02f, 3.2f), Quaternion.Euler(0f, 15f, 0f), Vector3.one * 0.95f);

            GameObject lotus2 = SpawnTripoProp("Assets/Models/Tripo/primordial_bioluminescent_lotus.glb",
                "Tripo_BioLotus_02", relicsRoot.transform,
                new Vector3(2.4f, -0.02f, 3.6f), Quaternion.Euler(0f, 75f, 0f), Vector3.one * 0.85f);

            GameObject lotus3 = SpawnTripoProp("Assets/Models/Tripo/primordial_bioluminescent_lotus.glb",
                "Tripo_BioLotus_03", relicsRoot.transform,
                new Vector3(0.5f, -0.02f, 5.5f), Quaternion.Euler(0f, -45f, 0f), Vector3.one * 1.1f);

            // Prop 6: Return Monolith & Portal Altar (For returning to Museum)
            GameObject monolith = SpawnTripoProp("Assets/Models/Tripo/primordial_return_monolith.glb",
                "Tripo_ReturnMonolith_Altar", relicsRoot.transform,
                new Vector3(0f, 0f, -4.8f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 2.8f);

            // Altar Dais Plinth
            var monolithBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            monolithBase.name = "Monolith_BasaltPlinth";
            monolithBase.transform.SetParent(relicsRoot.transform, false);
            monolithBase.transform.localPosition = new Vector3(0f, 0.15f, -4.8f);
            monolithBase.transform.localScale = new Vector3(3.4f, 0.15f, 3.4f);
            if (plinthMat != null) monolithBase.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

            // Concentric Gold & Cyan Altar Rings
            CreateMeshRing(relicsRoot.transform, "Monolith_RingGold", 1.85f, 0.05f, goldMat, 0.31f, new Vector3(0f, 0f, -4.8f));
            CreateMeshRing(relicsRoot.transform, "Monolith_RingCyan", 1.60f, 0.03f, glowCyan, 0.315f, new Vector3(0f, 0f, -4.8f));

            // Miniature Museum Microcosm Sphere (Held in monolith altar)
            GameObject returnSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            returnSphere.name = "ReturnSphere_MuseumMicrocosm";
            returnSphere.transform.SetParent(relicsRoot.transform, false);
            returnSphere.transform.localPosition = new Vector3(0f, 2.45f, -4.8f);
            returnSphere.transform.localScale = Vector3.one * 0.70f;
            if (glassMat != null) returnSphere.GetComponent<MeshRenderer>().sharedMaterial = glassMat;

            var returnSphereRot = returnSphere.AddComponent<ExhibitRotator>();
            returnSphereRot.rotationSpeed = 18.0f;
            returnSphereRot.bobAmplitude = 0.04f;
            returnSphereRot.bobSpeed = 1.4f;

            var returnSphereLight = returnSphere.AddComponent<Light>();
            returnSphereLight.type = LightType.Point;
            returnSphereLight.color = new Color(1.0f, 0.85f, 0.35f);
            returnSphereLight.intensity = 3.5f;
            returnSphereLight.range = 6.0f;

            var returnSphereCol = returnSphere.GetComponent<SphereCollider>();
            if (returnSphereCol != null) returnSphereCol.isTrigger = true;

            // Swirling Portal Particle System around Return Monolith
            GameObject portalVfx = new GameObject("Portal_SwirlingGoldParticles");
            portalVfx.transform.SetParent(relicsRoot.transform, false);
            portalVfx.transform.localPosition = new Vector3(0f, 2.45f, -4.8f);
            var portalPs = portalVfx.AddComponent<ParticleSystem>();
            var pMain = portalPs.main;
            pMain.startLifetime = 2.5f;
            pMain.startSpeed = 0.25f;
            pMain.startSize = 0.04f;
            pMain.startColor = new Color(1.0f, 0.88f, 0.3f, 0.9f);
            pMain.maxParticles = 120;
            pMain.simulationSpace = ParticleSystemSimulationSpace.Local;
            var pShape = portalPs.shape;
            pShape.shapeType = ParticleSystemShapeType.Sphere;
            pShape.radius = 0.75f;
            var pEmission = portalPs.emission;
            pEmission.rateOverTime = 30f;
            var pRend = portalVfx.GetComponent<ParticleSystemRenderer>();
            if (stardustMat != null) pRend.sharedMaterial = stardustMat;

            // =========================================================================
            // 4. ATMOSPHERIC PARTICLES
            // =========================================================================
            // Mineral sparkles drifting across the air
            GameObject mineralSparklesGo = new GameObject("VFX_MineralDustSparkles");
            mineralSparklesGo.transform.SetParent(vfxRoot.transform, false);
            mineralSparklesGo.transform.localPosition = new Vector3(0f, 2.5f, 4.0f);
            var minPs = mineralSparklesGo.AddComponent<ParticleSystem>();
            var minMain = minPs.main;
            minMain.startLifetime = 6.0f;
            minMain.startSpeed = 0.08f;
            minMain.startSize = 0.025f;
            minMain.startColor = new Color(0.2f, 0.9f, 0.85f, 0.75f);
            minMain.maxParticles = 150;
            var minShape = minPs.shape;
            minShape.shapeType = ParticleSystemShapeType.Box;
            minShape.scale = new Vector3(20f, 6f, 20f);
            var minEmission = minPs.emission;
            minEmission.rateOverTime = 25f;
            var minPsr = mineralSparklesGo.GetComponent<ParticleSystemRenderer>();
            if (stardustMat != null) minPsr.sharedMaterial = stardustMat;

            // =========================================================================
            // 5. SPATIAL AUDIO SYSTEM
            // =========================================================================
            var oceanAudio = audioRoot.AddComponent<AudioSource>();
            oceanAudio.clip = oceanClip;
            oceanAudio.loop = true;
            oceanAudio.volume = 0.65f;
            oceanAudio.playOnAwake = true;

            var droneAudio = audioRoot.AddComponent<AudioSource>();
            droneAudio.clip = droneClip;
            droneAudio.loop = true;
            droneAudio.volume = 0.40f;
            droneAudio.playOnAwake = true;

            // =========================================================================
            // 6. PLAYER RIG & SPAWN POINT
            // =========================================================================
            GameObject playerSpawn = new GameObject("Player_SpawnPoint");
            playerSpawn.transform.SetParent(worldRoot.transform, false);
            playerSpawn.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            playerSpawn.transform.localRotation = Quaternion.identity;

            // Camera Rig
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(playerSpawn.transform, false);
            camGo.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            var cam = camGo.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 1000f;
            camGo.AddComponent<AudioListener>();

            // Attach SphereDivingTransition to Camera for return capability
            var divingTrans = camGo.AddComponent<SphereDivingTransition>();

            // Setup Transition Overlay Canvas
            GameObject overlayCanvasGo = new GameObject("Diving_OverlayCanvas");
            overlayCanvasGo.transform.SetParent(camGo.transform, false);
            var canvas = overlayCanvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasGroup = overlayCanvasGo.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            overlayCanvasGo.SetActive(false);

            // =========================================================================
            // 7. PRIMORDIAL WORLD MANAGER
            // =========================================================================
            var manager = managersRoot.AddComponent<PrimordialWorldManager>();
            
            // Setup Serialized Fields via SerializedObject for clean editor assignment
            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("sunLight").objectReferenceValue = sunLight;
            so.FindProperty("geothermalSteam").objectReferenceValue = steamPs1;
            so.FindProperty("mineralSparkles").objectReferenceValue = minPs;
            so.FindProperty("waterDroplets").objectReferenceValue = portalPs;
            so.FindProperty("waterSurface").objectReferenceValue = lagoonWater.transform;
            so.FindProperty("waterMaterial").objectReferenceValue = waterMat;
            so.FindProperty("ambientOceanAudio").objectReferenceValue = oceanAudio;
            so.FindProperty("windAudio").objectReferenceValue = droneAudio;
            so.FindProperty("returnMonolith").objectReferenceValue = monolith != null ? monolith.transform : null;
            so.FindProperty("returnSphere").objectReferenceValue = returnSphere.transform;
            so.FindProperty("portalParticles").objectReferenceValue = portalPs;
            so.FindProperty("playerSpawnPoint").objectReferenceValue = playerSpawn.transform;

            // Interactive relics array
            SerializedProperty relicsProp = so.FindProperty("interactiveRelics");
            Transform[] relicTransforms = new Transform[]
            {
                chimney1 != null ? chimney1.transform : null,
                stromatolite1 != null ? stromatolite1.transform : null,
                ammonite != null ? ammonite.transform : null,
                crystalSpire != null ? crystalSpire.transform : null,
                lotus1 != null ? lotus1.transform : null,
                returnSphere.transform
            };
            relicsProp.arraySize = relicTransforms.Length;
            for (int i = 0; i < relicTransforms.Length; i++)
            {
                relicsProp.GetArrayElementAtIndex(i).objectReferenceValue = relicTransforms[i];
            }

            // Relic Info Curatorial Data
            SerializedProperty infoProp = so.FindProperty("relicInfos");
            infoProp.arraySize = 6;
            SetRelicData(infoProp.GetArrayElementAtIndex(0), "Hydrothermal Smoker", 
                "Deep mineral chimney venting hydrogen sulfide and methane. The thermal catalytic womb where primeval peptides folded.",
                new Color(1f, 0.5f, 0.1f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(1), "Stromatolite Dome", 
                "Cyanobacteria reef matrix. The pioneering builders who spent 2 billion years generating Earth's breathable atmosphere.",
                new Color(0.2f, 0.9f, 0.7f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(2), "Ammonite Fossil Relic", 
                "Spiral nautilus shell crystallizing the sacred Fibonacci spiral of marine life, embedded in Archean basalt.",
                new Color(0.3f, 0.8f, 1.0f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(3), "Prebiotic Crystal Core", 
                "Levitating kyanite cluster sealing a primordial water droplet with Archean chirality memory.",
                new Color(0.1f, 1.0f, 0.9f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(4), "Bioluminescent Lotus", 
                "Primeval aquatic flora glowing in the twilight tide pool, sensitive to human presence.",
                new Color(0.0f, 0.95f, 0.8f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(5), "Return Portal Monolith", 
                "Terran Embassy anchor. Touch the miniature museum sphere to reverse the sensory dive and return to the archive.",
                new Color(1f, 0.85f, 0.3f), snapClip);

            so.ApplyModifiedProperties();

            // Mark Scene Dirty and Save to ScenePath
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            // Register in Build Settings
            AddSceneToBuildSettings(ScenePath);
            AddSceneToBuildSettings("Assets/Scenes/WelcomeChamber.unity");

            Debug.Log("[PrimordialWorldArchitect] ✦ World 1: The Primordial Cradle 100% COMPLETE & SAVED TO ASSETS!");
        }

        private static void SetupAtmosphereAndSkybox()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.08f, 0.32f, 0.38f);
            RenderSettings.ambientEquatorColor = new Color(0.18f, 0.28f, 0.24f);
            RenderSettings.ambientGroundColor = new Color(0.04f, 0.08f, 0.06f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.12f, 0.35f, 0.38f);
            RenderSettings.fogDensity = 0.018f;

            // Load or Create Skybox
            string panoPath = "Assets/Textures/Skybox/WorldLabs_PrimordialCradle_Pano.png";
            Texture2D panoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(panoPath);
            if (panoTex != null)
            {
                Material skyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_WorldLabs_PrimordialCradle.mat");
                if (skyMat == null)
                {
                    Shader skyShader = Shader.Find("Skybox/Panoramic");
                    if (skyShader != null)
                    {
                        skyMat = new Material(skyShader);
                        skyMat.SetTexture("_MainTex", panoTex);
                        skyMat.SetFloat("_Exposure", 1.15f);
                        AssetDatabase.CreateAsset(skyMat, "Assets/Materials/Skybox_WorldLabs_PrimordialCradle.mat");
                    }
                }
                if (skyMat != null)
                {
                    RenderSettings.skybox = skyMat;
                }
            }
        }

        private static GameObject SpawnTripoProp(string path, string goName, Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                GameObject obj = Object.Instantiate(prefab, parent);
                obj.name = goName;
                obj.transform.localPosition = localPos;
                obj.transform.localRotation = localRot;
                obj.transform.localScale = scale;

                // Add collider if none exists
                if (obj.GetComponentInChildren<Collider>() == null)
                {
                    var col = obj.AddComponent<BoxCollider>();
                }
                return obj;
            }
            else
            {
                Debug.LogWarning($"[PrimordialWorldArchitect] Asset not found at path: {path}. Spawning placeholder.");
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.name = goName + "_Placeholder";
                placeholder.transform.SetParent(parent, false);
                placeholder.transform.localPosition = localPos;
                placeholder.transform.localRotation = localRot;
                placeholder.transform.localScale = scale;
                return placeholder;
            }
        }

        private static ParticleSystem AddVentSteam(Transform parent, Vector3 localOffset)
        {
            GameObject steamGo = new GameObject("Steam_GeothermalPlume");
            steamGo.transform.SetParent(parent, false);
            steamGo.transform.localPosition = localOffset;

            var ps = steamGo.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 3.5f;
            main.startSpeed = 1.6f;
            main.startSize = 0.35f;
            main.startColor = new Color(0.85f, 0.95f, 1.0f, 0.55f);
            main.maxParticles = 60;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 12f;
            shape.radius = 0.25f;

            var emission = ps.emission;
            emission.rateOverTime = 18f;

            var sol = ps.sizeOverLifetime;
            sol.enabled = true;
            sol.size = new ParticleSystem.MinMaxCurve(1.0f, AnimationCurve.Linear(0f, 0.4f, 1f, 2.5f));

            var col = ps.colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.2f, 0.8f, 0.9f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.6f, 0.25f), new GradientAlphaKey(0f, 1f) }
            );
            col.color = grad;

            Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
            if (stardustMat != null)
            {
                steamGo.GetComponent<ParticleSystemRenderer>().sharedMaterial = stardustMat;
            }

            return ps;
        }

        private static void CreateBasaltColumns(Transform parent, Material mat)
        {
            Vector3[] positions = new Vector3[]
            {
                new Vector3(-4.5f, 0.1f, 2.2f),
                new Vector3(-3.8f, 0.25f, 1.6f),
                new Vector3(-5.2f, 0.35f, 2.8f),
                new Vector3(4.8f, 0.15f, 1.8f),
                new Vector3(5.5f, 0.30f, 2.4f),
                new Vector3(3.9f, 0.12f, 1.2f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                var col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                col.name = "BasaltColumn_" + (i + 1);
                col.transform.SetParent(parent, false);
                col.transform.localPosition = positions[i];
                col.transform.localScale = new Vector3(0.85f, 0.65f, 0.85f);
                if (mat != null) col.GetComponent<MeshRenderer>().sharedMaterial = mat;
            }
        }

        private static void CreateMeshRing(Transform parent, string name, float radius, float width, Material mat, float yOffset, Vector3 centerOffset)
        {
            GameObject ring = new GameObject(name);
            ring.transform.SetParent(parent, false);
            ring.transform.localPosition = centerOffset + new Vector3(0f, yOffset, 0f);

            MeshFilter mf = ring.AddComponent<MeshFilter>();
            MeshRenderer mr = ring.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            Mesh mesh = new Mesh();
            mesh.name = name + "_Mesh";

            int segments = 48;
            Vector3[] vertices = new Vector3[segments * 2];
            int[] triangles = new int[segments * 6];

            float rInner = radius - width * 0.5f;
            float rOuter = radius + width * 0.5f;

            for (int i = 0; i < segments; i++)
            {
                float a = (float)i / segments * Mathf.PI * 2f;
                float cos = Mathf.Cos(a);
                float sin = Mathf.Sin(a);

                vertices[i * 2] = new Vector3(cos * rInner, 0f, sin * rInner);
                vertices[i * 2 + 1] = new Vector3(cos * rOuter, 0f, sin * rOuter);

                int next = (i + 1) % segments;
                int idx = i * 6;
                triangles[idx + 0] = i * 2;
                triangles[idx + 1] = next * 2;
                triangles[idx + 2] = i * 2 + 1;

                triangles[idx + 3] = i * 2 + 1;
                triangles[idx + 4] = next * 2;
                triangles[idx + 5] = next * 2 + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;
        }

        private static void SetRelicData(SerializedProperty prop, string name, string desc, Color glow, AudioClip sound)
        {
            prop.FindPropertyRelative("relicName").stringValue = name;
            prop.FindPropertyRelative("description").stringValue = desc;
            prop.FindPropertyRelative("glowColor").colorValue = glow;
            prop.FindPropertyRelative("proximitySound").objectReferenceValue = sound;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var s in scenes)
            {
                if (s.path == scenePath) return; // Already present
            }
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            System.Array.Copy(scenes, newScenes, scenes.Length);
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
        }

        [MenuItem("XENOASIS/World 1/Capture World 1 Screenshot")]
        public static void CaptureWorld1Screenshot()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            Camera cam = Camera.main;
            if (cam == null) cam = Object.FindObjectOfType<Camera>();
            if (cam == null)
            {
                Debug.LogError("No camera found in scene");
                return;
            }

            cam.transform.position = new Vector3(0f, 1.8f, -1.5f);
            cam.transform.LookAt(new Vector3(0f, 2.2f, 10.5f));

            int width = 1920;
            int height = 1080;
            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            string outDir = "Assets/Screenshots";
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "world1_primordial_cradle_overview.png");
            File.WriteAllBytes(outPath, bytes);
            Debug.Log("[PrimordialWorldArchitect] ✦ Screenshot saved: " + outPath);
        }

        [MenuItem("XENOASIS/World 1/Capture Monolith Screenshot")]
        public static void CaptureMonolithScreenshot()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            Camera cam = Camera.main;
            if (cam == null) cam = Object.FindObjectOfType<Camera>();
            if (cam == null) return;

            cam.transform.position = new Vector3(0f, 1.8f, -1.8f);
            cam.transform.LookAt(new Vector3(0f, 2.2f, -4.8f));

            int width = 1920;
            int height = 1080;
            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            string outDir = "Assets/Screenshots";
            string outPath = Path.Combine(outDir, "world1_primordial_return_monolith.png");
            File.WriteAllBytes(outPath, bytes);
            Debug.Log("[PrimordialWorldArchitect] ✦ Monolith screenshot saved: " + outPath);
        }
    }
}
#endif
