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
    /// Procedurally constructs and populates the World 2: The Forge of Civilization scene.
    /// Integrates WorldLabs 360 atmosphere, 6 Tripo v3.1 3D hero models, stone forge terraces,
    /// dynamic firelight, floating embers, and return portal to Station II in Welcome Chamber.
    /// </summary>
    public static class CivilizationWorldArchitect
    {
        private const string ScenePath = "Assets/Scenes/World2_ForgeOfCivilization.unity";

        [MenuItem("XENOASIS/World 2/Build Forge of Civilization Scene", false, 11)]
        public static void BuildWorld2Scene()
        {
            Debug.Log("[CivilizationWorldArchitect] ✦ Initiating assembly of World 2: The Forge of Civilization...");

            // Ensure directory exists
            string sceneDir = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
            }

            Scene scene;
            if (File.Exists(ScenePath))
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
            else
            {
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
            Material stardustMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_Stardust.mat");
            Material particleGlowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_GlowingAdditive.mat") ?? stardustMat;
            Material particleSmokeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Particle_SmokeAlpha.mat") ?? stardustMat;
            Material darkHullMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat");

            // Audio clips
            AudioClip bonfireClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");
            AudioClip windClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/water_drops.wav");
            AudioClip chimeClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
            AudioClip snapClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");

            // Hierarchy Root Containers
            GameObject worldRoot = new GameObject("--- FORGE OF CIVILIZATION ---");
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

            // Prehistoric Sunset Sun (Warm amber-crimson directional light)
            GameObject sunGo = new GameObject("Sun_PrehistoricDuskSun");
            sunGo.transform.SetParent(envRoot.transform, false);
            sunGo.transform.rotation = Quaternion.Euler(26f, -42f, 0f);
            var sunLight = sunGo.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.48f, 0.22f);
            sunLight.intensity = 1.45f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Warm Fill Light (Reflected glow from embers & forge fire)
            GameObject fillLightGo = new GameObject("Sun_ForgeEmberFill");
            fillLightGo.transform.SetParent(envRoot.transform, false);
            fillLightGo.transform.rotation = Quaternion.Euler(-55f, 135f, 0f);
            var fillLight = fillLightGo.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.48f, 0.18f, 0.12f);
            fillLight.intensity = 0.60f;
            fillLight.shadows = LightShadows.None;

            // =========================================================================
            // 2. WORLDLABS COLLIDER & MOUNTAIN FORGE TERRACE
            // =========================================================================
            GameObject colliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/WorldLabs/forge_of_civilization_collider.glb");
            if (colliderPrefab != null)
            {
                GameObject meshColliderGo = Object.Instantiate(colliderPrefab, envRoot.transform);
                meshColliderGo.name = "WorldLabs_Terrain_Collider";
                meshColliderGo.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                meshColliderGo.transform.localScale = Vector3.one * 8.5f;

                // Disable renderers on collider so it functions as physical collision without occluding skybox
                foreach (var rend in meshColliderGo.GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = false;
                }
            }

            // Central Sacred Hearth Terrace Platform
            GameObject forgePlatform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            forgePlatform.name = "Terrace_PrometheusPlaza_Main";
            forgePlatform.transform.SetParent(envRoot.transform, false);
            forgePlatform.transform.localPosition = new Vector3(0f, -0.45f, 4.0f);
            forgePlatform.transform.localScale = new Vector3(30.0f, 0.45f, 30.0f);
            if (obsidianMat != null) forgePlatform.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat;

            // Concentric Megalithic Basalt Pillars around the perimeter
            CreateBasaltForgeRing(envRoot.transform, darkHullMat ?? obsidianMat);

            // =========================================================================
            // 3. TRIPO 3D HERO INTERACTIVE RELICS
            // =========================================================================
            // Relic 1: Promethean Ceremonial Stone Hearth (Centerpiece)
            GameObject hearthGo = SpawnTripoProp("Assets/Models/Tripo/civilization_prometheus_hearth.glb",
                "Tripo_PrometheusHearth_Center", relicsRoot.transform,
                new Vector3(0f, 0.0f, 5.0f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 3.0f);

            var hearthLight = AddFlickerFireLight(hearthGo != null ? hearthGo.transform : relicsRoot.transform,
                new Vector3(0f, 1.4f, 5.0f), new Color(1.0f, 0.55f, 0.15f), 5.5f, 14f);

            // Relic 2: Paleolithic Flint Anvil & Knapping Station (West)
            GameObject anvilGo = SpawnTripoProp("Assets/Models/Tripo/civilization_paleolithic_flint_anvil.glb",
                "Tripo_PaleolithicFlintAnvil_West", relicsRoot.transform,
                new Vector3(-4.5f, 0.0f, 4.2f), Quaternion.Euler(0f, 45f, 0f), Vector3.one * 2.2f);

            var anvilLight = AddFlickerFireLight(anvilGo != null ? anvilGo.transform : relicsRoot.transform,
                new Vector3(-4.5f, 1.2f, 4.2f), new Color(1.0f, 0.65f, 0.25f), 3.2f, 7f);

            // Relic 3: Bronze Armillary Sphere & Astrolabe (North-West)
            GameObject astrolabeGo = SpawnTripoProp("Assets/Models/Tripo/civilization_bronze_astrolabe_armillary.glb",
                "Tripo_BronzeAstrolabeArmillary_NW", relicsRoot.transform,
                new Vector3(-3.2f, 0.0f, 7.8f), Quaternion.Euler(0f, -25f, 0f), Vector3.one * 2.4f);

            if (astrolabeGo != null)
            {
                var rotator = astrolabeGo.AddComponent<ExhibitRotator>();
                rotator.rotationSpeed = 8.0f;
                rotator.bobAmplitude = 0.05f;
                rotator.bobSpeed = 0.8f;
            }

            var astrolabeSpot = astrolabeGo != null ? astrolabeGo.AddComponent<Light>() : relicsRoot.AddComponent<Light>();
            astrolabeSpot.type = LightType.Spot;
            astrolabeSpot.color = new Color(1.0f, 0.85f, 0.45f);
            astrolabeSpot.intensity = 4.2f;
            astrolabeSpot.range = 8.0f;
            astrolabeSpot.spotAngle = 50f;

            // Relic 4: Gutenberg Printing Press (North-East)
            GameObject pressGo = SpawnTripoProp("Assets/Models/Tripo/civilization_gutenberg_printing_press.glb",
                "Tripo_GutenbergPrintingPress_NE", relicsRoot.transform,
                new Vector3(3.2f, 0.0f, 7.8f), Quaternion.Euler(0f, -65f, 0f), Vector3.one * 2.4f);

            var pressLight = AddFlickerFireLight(pressGo != null ? pressGo.transform : relicsRoot.transform,
                new Vector3(3.2f, 1.5f, 7.8f), new Color(1.0f, 0.70f, 0.35f), 3.4f, 8f);

            // Relic 5: Silicon Quantum Monolith (East)
            GameObject siliconGo = SpawnTripoProp("Assets/Models/Tripo/civilization_silicon_quantum_monolith.glb",
                "Tripo_SiliconQuantumMonolith_East", relicsRoot.transform,
                new Vector3(4.5f, 0.0f, 4.2f), Quaternion.Euler(0f, -90f, 0f), Vector3.one * 2.5f);

            if (siliconGo != null)
            {
                var siliconLight = siliconGo.AddComponent<Light>();
                siliconLight.type = LightType.Point;
                siliconLight.color = new Color(0.2f, 0.85f, 1.0f); // High-tech cyan-amber pulse
                siliconLight.intensity = 5.0f;
                siliconLight.range = 10f;
            }

            // Relic 6: Chrono-Return Monolith & Portal Altar (South, behind player spawn)
            GameObject returnMonolithGo = SpawnTripoProp("Assets/Models/Tripo/civilization_return_monolith.glb",
                "Tripo_ReturnMonolith_South", relicsRoot.transform,
                new Vector3(0f, 0f, -4.8f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 2.8f);

            // Altar Dais Plinth for Monolith
            var monolithBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            monolithBase.name = "Monolith_BasaltPlinth";
            monolithBase.transform.SetParent(relicsRoot.transform, false);
            monolithBase.transform.localPosition = new Vector3(0f, 0.15f, -4.8f);
            monolithBase.transform.localScale = new Vector3(3.4f, 0.15f, 3.4f);
            if (plinthMat != null) monolithBase.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

            // Concentric Gold & Amber Altar Rings
            CreateMeshRing(relicsRoot.transform, "Monolith_RingGold", 1.85f, 0.05f, goldMat, 0.31f, new Vector3(0f, 0f, -4.8f));
            CreateMeshRing(relicsRoot.transform, "Monolith_RingAmber", 1.60f, 0.03f, glowGold ?? glowCyan, 0.315f, new Vector3(0f, 0f, -4.8f));

            // Miniature Museum Microcosm Sphere
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
            returnSphereLight.intensity = 3.8f;
            returnSphereLight.range = 6.0f;

            // =========================================================================
            // 4. PARTICLE SYSTEMS (Embers, Smoke, Hearth Flames, Quantum Sparks)
            // =========================================================================
            ParticleSystem embersPs = CreateFloatingEmbers(vfxRoot.transform, particleGlowMat);
            ParticleSystem smokePs = CreateForgeSmoke(vfxRoot.transform, new Vector3(0f, 1.8f, 5.5f), particleSmokeMat);
            ParticleSystem hearthFlamesPs = CreateHearthFlames(vfxRoot.transform, new Vector3(0f, 0.8f, 5.5f), particleGlowMat);
            ParticleSystem siliconSparksPs = CreateQuantumSparks(vfxRoot.transform, new Vector3(7.2f, 1.6f, 4.5f), particleGlowMat);

            // Return Portal Swirling Particles
            GameObject portalPsGo = new GameObject("VFX_ReturnPortal_Embers");
            portalPsGo.transform.SetParent(vfxRoot.transform, false);
            portalPsGo.transform.localPosition = new Vector3(0f, 2.45f, -4.8f);
            var portalPs = portalPsGo.AddComponent<ParticleSystem>();
            var portalRend = portalPsGo.GetComponent<ParticleSystemRenderer>();
            if (portalRend != null) portalRend.sharedMaterial = particleGlowMat;
            var portalMain = portalPs.main;
            portalMain.startLifetime = 1.8f;
            portalMain.startSpeed = 0.6f;
            portalMain.startSize = 0.06f;
            portalMain.startColor = new Color(1.0f, 0.85f, 0.3f, 0.9f);
            portalMain.maxParticles = 80;
            portalMain.simulationSpace = ParticleSystemSimulationSpace.World;
            var portalShape = portalPs.shape;
            portalShape.shapeType = ParticleSystemShapeType.Sphere;
            portalShape.radius = 0.55f;
            var portalEmission = portalPs.emission;
            portalEmission.rateOverTime = 25f;

            // =========================================================================
            // 5. SPATIAL AUDIO SOURCES
            // =========================================================================
            AudioSource forgeAudio = audioRoot.AddComponent<AudioSource>();
            forgeAudio.clip = bonfireClip;
            forgeAudio.loop = true;
            forgeAudio.spatialBlend = 0.85f;
            forgeAudio.volume = 0.65f;
            forgeAudio.transform.localPosition = new Vector3(0f, 1.0f, 5.5f);

            AudioSource windAudio = audioRoot.AddComponent<AudioSource>();
            windAudio.clip = windClip;
            windAudio.loop = true;
            windAudio.spatialBlend = 0.5f;
            windAudio.volume = 0.40f;

            AudioSource relicAudio = audioRoot.AddComponent<AudioSource>();
            relicAudio.spatialBlend = 0.80f;
            relicAudio.playOnAwake = false;

            // =========================================================================
            // 6. PLAYER SPAWN RIG & CAMERA
            // =========================================================================
            GameObject playerSpawn = new GameObject("PlayerSpawnPoint");
            playerSpawn.transform.SetParent(envRoot.transform, false);
            playerSpawn.transform.localPosition = new Vector3(0f, 1.1f, -0.5f);
            playerSpawn.transform.rotation = Quaternion.identity; // Looking north toward Prometheus Hearth

            GameObject mainCamGo = new GameObject("Main Camera");
            mainCamGo.tag = "MainCamera";
            mainCamGo.transform.SetParent(playerSpawn.transform, false);
            mainCamGo.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            mainCamGo.transform.localRotation = Quaternion.identity;
            var cam = mainCamGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            mainCamGo.AddComponent<AudioListener>();

            // SphereDivingTransition on Camera Rig for Return Trip
            var divingTrans = mainCamGo.AddComponent<SphereDivingTransition>();

            // =========================================================================
            // 7. CIVILIZATION WORLD MANAGER WIRING
            // =========================================================================
            GameObject managerGo = new GameObject("CivilizationWorldManager");
            managerGo.transform.SetParent(managersRoot.transform, false);
            var worldMgr = managerGo.AddComponent<CivilizationWorldManager>();

            SerializedObject so = new SerializedObject(worldMgr);
            so.FindProperty("sunLight").objectReferenceValue = sunLight;
            so.FindProperty("floatingEmbers").objectReferenceValue = embersPs;
            so.FindProperty("forgeSmoke").objectReferenceValue = smokePs;
            so.FindProperty("hearthFlames").objectReferenceValue = hearthFlamesPs;
            so.FindProperty("siliconSparks").objectReferenceValue = siliconSparksPs;
            so.FindProperty("ambientForgeAudio").objectReferenceValue = forgeAudio;
            so.FindProperty("ambientWindAudio").objectReferenceValue = windAudio;
            so.FindProperty("relicInteractionAudio").objectReferenceValue = relicAudio;
            so.FindProperty("returnMonolith").objectReferenceValue = returnMonolithGo != null ? returnMonolithGo.transform : null;
            so.FindProperty("returnSphere").objectReferenceValue = returnSphere.transform;
            so.FindProperty("returnPortalParticles").objectReferenceValue = portalPs;
            so.FindProperty("playerSpawnPoint").objectReferenceValue = playerSpawn.transform;

            // Flickering Firelights Array
            SerializedProperty fireLightsProp = so.FindProperty("forgeFireLights");
            Light[] fireLightList = new Light[] { hearthLight, anvilLight, pressLight };
            fireLightsProp.arraySize = fireLightList.Length;
            for (int i = 0; i < fireLightList.Length; i++)
            {
                fireLightsProp.GetArrayElementAtIndex(i).objectReferenceValue = fireLightList[i];
            }

            // Interactive Relics Array (6 items)
            SerializedProperty relicsProp = so.FindProperty("interactiveRelics");
            Transform[] relicTransforms = new Transform[]
            {
                hearthGo != null ? hearthGo.transform : null,
                anvilGo != null ? anvilGo.transform : null,
                astrolabeGo != null ? astrolabeGo.transform : null,
                pressGo != null ? pressGo.transform : null,
                siliconGo != null ? siliconGo.transform : null,
                returnSphere.transform
            };
            relicsProp.arraySize = relicTransforms.Length;
            for (int i = 0; i < relicTransforms.Length; i++)
            {
                relicsProp.GetArrayElementAtIndex(i).objectReferenceValue = relicTransforms[i];
            }

            // Relic Curatorial Data
            SerializedProperty infoProp = so.FindProperty("relicInfos");
            infoProp.arraySize = 6;
            SetRelicData(infoProp.GetArrayElementAtIndex(0), "The Promethean Hearth", "PALEOLITHIC // 1.500.000 B.C.",
                "Eternal ember forge. Controlling thermal energy doubled hominid brain caloric intake and birthed human hearth culture.",
                new Color(1f, 0.55f, 0.15f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(1), "Flint Knapping Anvil", "ACHEULEAN // 300.000 B.C.",
                "Percussion-flaked obsidian and flint biface hand-axes. The first intentional physical embodiment of mental foresight.",
                new Color(1f, 0.70f, 0.25f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(2), "Bronze Armillary Astrolabe", "ANTIQUITY & RENAISSANCE // 150 B.C.",
                "Interlocking brass rings modeling celestial mechanics. Turning observation into navigation and cartography.",
                new Color(1f, 0.85f, 0.35f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(3), "Gutenberg Movable Press", "PRINTING REVOLUTION // 1440 A.D.",
                "Cast lead types and screw press. Decoupling human memory from biological mortality and democratizing empirical wisdom.",
                new Color(0.95f, 0.65f, 0.30f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(4), "Quantum Silicon Monolith", "COMPUTATIONAL AGE // 1947 - 2026 A.D.",
                "Purified quartz crystal etched with billion-transistor circuits. Transforming fire and sand into machine cognition.",
                new Color(0.20f, 0.85f, 1.0f), chimeClip);
            SetRelicData(infoProp.GetArrayElementAtIndex(5), "Chrono-Return Monolith", "TERRAN EMBASSY ANCHOR",
                "Touch the golden brazier's microcosm sphere to reverse the sensory dive and return to the Museum Welcome Chamber.",
                new Color(1.0f, 0.85f, 0.35f), snapClip);

            so.ApplyModifiedProperties();

            // Mark Scene Dirty and Save to ScenePath
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            // Register in Build Settings
            AddSceneToBuildSettings(ScenePath);
            AddSceneToBuildSettings("Assets/Scenes/WelcomeChamber.unity");

            Debug.Log("[CivilizationWorldArchitect] ✦ World 2: The Forge of Civilization 100% COMPLETE & SAVED!");
        }

        private static void SetupAtmosphereAndSkybox()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.25f, 0.12f, 0.14f);
            RenderSettings.ambientEquatorColor = new Color(0.35f, 0.18f, 0.12f);
            RenderSettings.ambientGroundColor = new Color(0.10f, 0.06f, 0.05f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.22f, 0.10f, 0.12f);
            RenderSettings.fogDensity = 0.014f;

            // Load and configure Skybox
            string panoPath = "Assets/Textures/Skybox/WorldLabs_ForgeOfCivilization_Pano.png";
            AssetDatabase.ImportAsset(panoPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(panoPath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 4096;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }

            Texture2D panoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(panoPath);
            if (panoTex != null)
            {
                Material skyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_WorldLabs_ForgeOfCivilization.mat");
                if (skyMat == null)
                {
                    Shader skyShader = Shader.Find("Skybox/Panoramic");
                    if (skyShader != null)
                    {
                        skyMat = new Material(skyShader);
                        AssetDatabase.CreateAsset(skyMat, "Assets/Materials/Skybox_WorldLabs_ForgeOfCivilization.mat");
                    }
                }
                if (skyMat != null)
                {
                    skyMat.SetTexture("_MainTex", panoTex);
                    skyMat.SetFloat("_Exposure", 1.35f);
                    skyMat.SetFloat("_Mapping", 1.0f); // Latitude Longitude Layout
                    skyMat.SetFloat("_ImageType", 0.0f); // 360 Degrees
                    skyMat.EnableKeyword("_MAPPING_LATITUDE_LONGITUDE_LAYOUT");
                    EditorUtility.SetDirty(skyMat);
                    RenderSettings.skybox = skyMat;
                }
            }
        }

        private static GameObject SpawnTripoProp(string assetPath, string name, Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[CivilizationWorldArchitect] Asset not found at {assetPath}. Spawning placeholder.");
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.name = name + "_Placeholder";
                placeholder.transform.SetParent(parent, false);
                placeholder.transform.localPosition = localPos;
                placeholder.transform.localRotation = localRot;
                placeholder.transform.localScale = scale;
                return placeholder;
            }

            GameObject instance = Object.Instantiate(prefab, parent);
            instance.name = name;
            instance.transform.localPosition = localPos;
            instance.transform.localRotation = localRot;
            instance.transform.localScale = scale;

            // Ensure colliders exist for VR raycast
            MeshFilter[] filters = instance.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in filters)
            {
                if (mf.GetComponent<Collider>() == null && mf.sharedMesh != null)
                {
                    var mc = mf.gameObject.AddComponent<MeshCollider>();
                    mc.convex = false;
                }
            }

            // Align bottom of model to localPos.y
            Bounds b = new Bounds();
            bool hasBounds = false;
            foreach (var r in instance.GetComponentsInChildren<Renderer>())
            {
                if (!hasBounds) { b = r.bounds; hasBounds = true; }
                else { b.Encapsulate(r.bounds); }
            }
            if (hasBounds)
            {
                float bottomY = b.min.y;
                float desiredY = localPos.y;
                instance.transform.position += Vector3.up * (desiredY - bottomY);
            }

            return instance;
        }

        private static Light AddFlickerFireLight(Transform parent, Vector3 pos, Color color, float intensity, float range)
        {
            GameObject lightGo = new GameObject("FireLight_" + parent.name);
            lightGo.transform.SetParent(parent, false);
            lightGo.transform.position = pos;
            var l = lightGo.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.Soft;
            return l;
        }

        private static ParticleSystem CreateFloatingEmbers(Transform parent, Material mat)
        {
            GameObject psGo = new GameObject("VFX_FloatingEmbers");
            psGo.transform.SetParent(parent, false);
            psGo.transform.localPosition = new Vector3(0f, 1.2f, 4.0f);

            var ps = psGo.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 3.5f;
            main.startSpeed = 0.8f;
            main.startSize = 0.05f;
            main.startColor = new Color(1.0f, 0.55f, 0.15f, 0.95f);
            main.maxParticles = 250;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(20f, 0.5f, 20f);

            var emission = ps.emission;
            emission.rateOverTime = 40f;

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.y = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);

            var rend = psGo.GetComponent<ParticleSystemRenderer>();
            if (mat != null) rend.sharedMaterial = mat;

            return ps;
        }

        private static ParticleSystem CreateForgeSmoke(Transform parent, Vector3 pos, Material mat)
        {
            GameObject psGo = new GameObject("VFX_ForgeSmoke");
            psGo.transform.SetParent(parent, false);
            psGo.transform.position = pos;

            var ps = psGo.AddComponent<ParticleSystem>();
            var rend = psGo.GetComponent<ParticleSystemRenderer>();
            if (mat != null && rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 4.0f;
            main.startSpeed = 1.2f;
            main.startSize = 0.35f;
            main.startColor = new Color(0.2f, 0.18f, 0.18f, 0.35f);
            main.maxParticles = 80;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 0.4f;

            var emission = ps.emission;
            emission.rateOverTime = 12f;

            return ps;
        }

        private static ParticleSystem CreateHearthFlames(Transform parent, Vector3 pos, Material mat)
        {
            GameObject psGo = new GameObject("VFX_HearthFlames");
            psGo.transform.SetParent(parent, false);
            psGo.transform.position = pos;

            var ps = psGo.AddComponent<ParticleSystem>();
            var rend = psGo.GetComponent<ParticleSystemRenderer>();
            if (mat != null && rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 0.85f;
            main.startSpeed = 1.8f;
            main.startSize = 0.40f;
            main.startColor = new Color(1.0f, 0.55f, 0.1f, 0.85f);
            main.maxParticles = 120;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 18f;
            shape.radius = 0.6f;

            var emission = ps.emission;
            emission.rateOverTime = 60f;

            return ps;
        }

        private static ParticleSystem CreateQuantumSparks(Transform parent, Vector3 pos, Material mat)
        {
            GameObject psGo = new GameObject("VFX_QuantumSiliconSparks");
            psGo.transform.SetParent(parent, false);
            psGo.transform.position = pos;

            var ps = psGo.AddComponent<ParticleSystem>();
            var rend = psGo.GetComponent<ParticleSystemRenderer>();
            if (mat != null && rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 1.2f;
            main.startSpeed = 0.9f;
            main.startSize = 0.04f;
            main.startColor = new Color(0.2f, 0.85f, 1.0f, 0.95f);
            main.maxParticles = 60;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.8f;

            var emission = ps.emission;
            emission.rateOverTime = 20f;

            return ps;
        }

        private static void CreateBasaltForgeRing(Transform parent, Material mat)
        {
            int count = 16;
            float radius = 13.5f;
            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2f / count;
                Vector3 pos = new Vector3(Mathf.Sin(angle) * radius, 0.8f, Mathf.Cos(angle) * radius + 4.0f);

                GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                col.name = $"BasaltForgeColumn_{i:D2}";
                col.transform.SetParent(parent, false);
                col.transform.localPosition = pos;
                float height = 2.4f + Mathf.Sin(i * 1.5f) * 0.8f;
                col.transform.localScale = new Vector3(1.1f, height, 1.1f);
                if (mat != null) col.GetComponent<MeshRenderer>().sharedMaterial = mat;
            }
        }

        private static void CreateMeshRing(Transform parent, string name, float radius, float width, Material mat, float yOffset, Vector3 center)
        {
            GameObject ringGo = new GameObject(name);
            ringGo.transform.SetParent(parent, false);
            ringGo.transform.localPosition = new Vector3(center.x, center.y + yOffset, center.z);

            MeshFilter mf = ringGo.AddComponent<MeshFilter>();
            MeshRenderer mr = ringGo.AddComponent<MeshRenderer>();
            if (mat != null) mr.sharedMaterial = mat;

            Mesh mesh = new Mesh();
            int segments = 48;
            Vector3[] verts = new Vector3[(segments + 1) * 2];
            int[] tris = new int[segments * 6];

            float rInner = radius - width * 0.5f;
            float rOuter = radius + width * 0.5f;

            for (int i = 0; i <= segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                float sin = Mathf.Sin(a);
                float cos = Mathf.Cos(a);

                verts[i * 2 + 0] = new Vector3(sin * rInner, 0f, cos * rInner);
                verts[i * 2 + 1] = new Vector3(sin * rOuter, 0f, cos * rOuter);

                if (i < segments)
                {
                    int root = i * 2;
                    tris[i * 6 + 0] = root;
                    tris[i * 6 + 1] = root + 1;
                    tris[i * 6 + 2] = root + 2;

                    tris[i * 6 + 3] = root + 1;
                    tris[i * 6 + 4] = root + 3;
                    tris[i * 6 + 5] = root + 2;
                }
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;
        }

        private static void SetRelicData(SerializedProperty prop, string name, string era, string desc, Color glow, AudioClip sfx)
        {
            prop.FindPropertyRelative("relicName").stringValue = name;
            prop.FindPropertyRelative("eraLabel").stringValue = era;
            prop.FindPropertyRelative("description").stringValue = desc;
            prop.FindPropertyRelative("glowColor").colorValue = glow;
            if (sfx != null) prop.FindPropertyRelative("proximitySound").objectReferenceValue = sfx;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var existingScenes = EditorBuildSettings.scenes;
            foreach (var s in existingScenes)
            {
                if (s.path == scenePath) return;
            }

            var newScenes = new EditorBuildSettingsScene[existingScenes.Length + 1];
            for (int i = 0; i < existingScenes.Length; i++)
            {
                newScenes[i] = existingScenes[i];
            }
            newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log($"[CivilizationWorldArchitect] Registered '{scenePath}' in EditorBuildSettings.");
        }
    }
}
#endif
