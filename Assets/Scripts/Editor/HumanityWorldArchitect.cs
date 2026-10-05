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
    /// Procedurally constructs and populates the World 3: The Heart of Humanity scene.
    /// Integrates WorldLabs 360 Arctic Aurora atmosphere, 6 Tripo v3.1 3D hero models,
    /// glacial marble & obsidian sanctuary terraces, cryogenic stasis glow, dancing auroral ribbons,
    /// and return portal back to Station III in the Welcome Chamber.
    /// </summary>
    public static class HumanityWorldArchitect
    {
        private const string ScenePath = "Assets/Scenes/World3_HeartOfHumanity.unity";

        [MenuItem("XENOASIS/World 3/Build Heart of Humanity Scene", false, 12)]
        public static void BuildWorld3Scene()
        {
            Debug.Log("[HumanityWorldArchitect] ✦ Initiating assembly of World 3: The Heart of Humanity...");

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
                EditorSceneManager.SaveOpenScenes();
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
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
            AudioClip polarDrone = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/cosmic_drone.wav");
            AudioClip waterEcho = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambient/water_drops.wav");
            AudioClip toneC = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_C.wav");
            AudioClip toneE = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/crystal_tone_E.wav");
            AudioClip snapClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/offering_complete.wav");

            // Hierarchy Root Containers
            GameObject worldRoot = new GameObject("--- HEART OF HUMANITY ---");
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

            // Polar Midnight Moon (Cool silver-cyan directional light)
            GameObject moonGo = new GameObject("Moon_PolarMidnightMoon");
            moonGo.transform.SetParent(envRoot.transform, false);
            moonGo.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            var moonLight = moonGo.AddComponent<Light>();
            moonLight.type = LightType.Directional;
            moonLight.color = new Color(0.65f, 0.88f, 1.0f);
            moonLight.intensity = 1.35f;
            moonLight.shadows = LightShadows.Soft;

            // Ambient Emerald & Violet Aurora Fill Light
            GameObject auroraFillGo = new GameObject("Light_AuroraEmeraldFill");
            auroraFillGo.transform.SetParent(envRoot.transform, false);
            auroraFillGo.transform.rotation = Quaternion.Euler(-50f, 145f, 0f);
            var auroraLight = auroraFillGo.AddComponent<Light>();
            auroraLight.type = LightType.Directional;
            auroraLight.color = new Color(0.15f, 0.85f, 0.65f);
            auroraLight.intensity = 0.55f;
            auroraLight.shadows = LightShadows.None;

            // =========================================================================
            // 2. WORLDLABS COLLIDER & SANCTUARY PLAZA TERRACE
            // =========================================================================
            GameObject colliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/WorldLabs/humanity_sanctuary_collider.glb");
            if (colliderPrefab != null)
            {
                GameObject meshColliderGo = Object.Instantiate(colliderPrefab, envRoot.transform);
                meshColliderGo.name = "WorldLabs_Terrain_Collider";
                meshColliderGo.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                meshColliderGo.transform.localScale = Vector3.one * 8.5f;

                foreach (var rend in meshColliderGo.GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = false;
                }
            }

            // Central Glacial Marble & Obsidian Platform
            GameObject sanctuaryPlatform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sanctuaryPlatform.name = "Terrace_SanctuaryPlaza_Main";
            sanctuaryPlatform.transform.SetParent(envRoot.transform, false);
            sanctuaryPlatform.transform.localPosition = new Vector3(0f, -0.45f, 3.5f);
            sanctuaryPlatform.transform.localScale = new Vector3(30.0f, 0.45f, 30.0f);
            if (obsidianMat != null) sanctuaryPlatform.GetComponent<MeshRenderer>().sharedMaterial = obsidianMat;

            // Perimeter Glacial & Neoclassical Columns
            CreatePermafrostColonnade(envRoot.transform, plinthMat ?? obsidianMat, goldMat, glowCyan);

            // =========================================================================
            // 3. TRIPO 3D HERO INTERACTIVE RELICS
            // =========================================================================
            // Relic 1: Svalbard Seed Vault Arctic Wedge Entrance (North Centerpiece)
            GameObject seedVaultGo = SpawnTripoProp("Assets/Models/Tripo/humanity_svalbard_seed_vault_entrance.glb",
                "Tripo_SvalbardSeedVault_North", relicsRoot.transform,
                new Vector3(0f, 0.0f, 6.5f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 3.2f);

            var vaultLight = AddCryoGlowLight(seedVaultGo != null ? seedVaultGo.transform : relicsRoot.transform,
                new Vector3(0f, 1.8f, 6.5f), new Color(0.2f, 0.95f, 0.85f), 5.5f, 14f);

            // Relic 2: Rosetta Universal Script Stele (North-West)
            GameObject rosettaGo = SpawnTripoProp("Assets/Models/Tripo/humanity_rosetta_stone_stele.glb",
                "Tripo_RosettaStele_NW", relicsRoot.transform,
                new Vector3(-4.5f, 0.0f, 4.2f), Quaternion.Euler(0f, 40f, 0f), Vector3.one * 2.2f);

            var rosettaLight = AddCryoGlowLight(rosettaGo != null ? rosettaGo.transform : relicsRoot.transform,
                new Vector3(-4.5f, 1.4f, 4.2f), new Color(1.0f, 0.85f, 0.45f), 3.5f, 8f);

            // Relic 3: Library of Alexandria Scroll & Parchment Archive (West)
            GameObject alexandriaGo = SpawnTripoProp("Assets/Models/Tripo/humanity_library_alexandria_scroll_archive.glb",
                "Tripo_AlexandriaScrollArchive_West", relicsRoot.transform,
                new Vector3(-5.5f, 0.0f, 0.2f), Quaternion.Euler(0f, 75f, 0f), Vector3.one * 2.3f);

            var alexandriaLight = AddCryoGlowLight(alexandriaGo != null ? alexandriaGo.transform : relicsRoot.transform,
                new Vector3(-5.5f, 1.5f, 0.2f), new Color(1.0f, 0.75f, 0.35f), 3.2f, 8f);

            // Relic 4: Human Genome DNA Biocrystal Spire (North-East)
            GameObject dnaSpireGo = SpawnTripoProp("Assets/Models/Tripo/humanity_dna_genome_crystal_spire.glb",
                "Tripo_DNA_GenomeCrystalSpire_NE", relicsRoot.transform,
                new Vector3(4.5f, 0.0f, 4.2f), Quaternion.Euler(0f, -40f, 0f), Vector3.one * 2.6f);

            if (dnaSpireGo != null)
            {
                var dnaRot = dnaSpireGo.AddComponent<ExhibitRotator>();
                dnaRot.rotationSpeed = 10.0f;
                dnaRot.bobAmplitude = 0.04f;
                dnaRot.bobSpeed = 1.0f;
            }

            var dnaLight = AddCryoGlowLight(dnaSpireGo != null ? dnaSpireGo.transform : relicsRoot.transform,
                new Vector3(4.5f, 1.8f, 4.2f), new Color(0.25f, 0.90f, 1.0f), 5.0f, 12f);

            // Relic 5: Classical Violoncello Instrument of Emotion (East)
            GameObject celloGo = SpawnTripoProp("Assets/Models/Tripo/humanity_grand_violoncello_instrument.glb",
                "Tripo_ClassicalVioloncello_East", relicsRoot.transform,
                new Vector3(5.5f, 0.0f, 0.2f), Quaternion.Euler(0f, -75f, 0f), Vector3.one * 2.2f);

            var celloSpot = celloGo != null ? celloGo.AddComponent<Light>() : relicsRoot.AddComponent<Light>();
            celloSpot.type = LightType.Spot;
            celloSpot.color = new Color(1.0f, 0.82f, 0.45f);
            celloSpot.intensity = 4.5f;
            celloSpot.range = 8.0f;
            celloSpot.spotAngle = 55f;

            // Relic 6: Return Monolith & Welcome Chamber Portal (South, behind spawn)
            GameObject returnMonolithGo = SpawnTripoProp("Assets/Models/Tripo/humanity_return_monolith.glb",
                "Tripo_ReturnMonolith_South", relicsRoot.transform,
                new Vector3(0f, 0f, -4.8f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 2.8f);

            // Altar Dais Plinth for Monolith
            var monolithBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            monolithBase.name = "Monolith_AltarPlinth";
            monolithBase.transform.SetParent(relicsRoot.transform, false);
            monolithBase.transform.localPosition = new Vector3(0f, 0.15f, -4.8f);
            monolithBase.transform.localScale = new Vector3(3.4f, 0.15f, 3.4f);
            if (plinthMat != null) monolithBase.GetComponent<MeshRenderer>().sharedMaterial = plinthMat;

            // Concentric Gold & Cyan Altar Rings
            CreateMeshRing(relicsRoot.transform, "Monolith_RingGold", 1.85f, 0.05f, goldMat, 0.31f, new Vector3(0f, 0f, -4.8f));
            CreateMeshRing(relicsRoot.transform, "Monolith_RingCyan", 1.60f, 0.03f, glowCyan, 0.315f, new Vector3(0f, 0f, -4.8f));

            // Miniature Museum Microcosm Sphere
            GameObject returnSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            returnSphere.name = "ReturnSphere_MuseumMicrocosm";
            returnSphere.transform.SetParent(relicsRoot.transform, false);
            returnSphere.transform.localPosition = new Vector3(0f, 2.45f, -4.8f);
            returnSphere.transform.localScale = Vector3.one * 0.70f;
            if (glassMat != null) returnSphere.GetComponent<MeshRenderer>().sharedMaterial = glassMat;

            var returnSphereRot = returnSphere.AddComponent<ExhibitRotator>();
            returnSphereRot.rotationSpeed = 16.0f;
            returnSphereRot.bobAmplitude = 0.04f;
            returnSphereRot.bobSpeed = 1.2f;

            var returnSphereLight = returnSphere.AddComponent<Light>();
            returnSphereLight.type = LightType.Point;
            returnSphereLight.color = new Color(0.35f, 0.95f, 0.90f);
            returnSphereLight.intensity = 3.8f;
            returnSphereLight.range = 6.0f;

            // =========================================================================
            // 4. PARTICLE SYSTEMS (Aurora, Seed Spores, Whispering Glyphs, Diamond Dust)
            // =========================================================================
            ParticleSystem auroraPs = CreateAuroraRibbons(vfxRoot.transform, particleGlowMat);
            ParticleSystem sporesPs = CreateSeedSproutSpores(vfxRoot.transform, new Vector3(0f, 1.2f, 6.5f), particleGlowMat);
            ParticleSystem glyphsPs = CreateWhisperingGlyphs(vfxRoot.transform, new Vector3(-4.5f, 1.2f, 4.2f), particleGlowMat);
            ParticleSystem dustPs = CreateGlacialDiamondDust(vfxRoot.transform, particleGlowMat);

            // Return Portal Swirling Particles
            GameObject portalPsGo = new GameObject("VFX_ReturnPortal_CyanMatrix");
            portalPsGo.transform.SetParent(vfxRoot.transform, false);
            portalPsGo.transform.localPosition = new Vector3(0f, 2.45f, -4.8f);
            var portalPs = portalPsGo.AddComponent<ParticleSystem>();
            var portalRend = portalPsGo.GetComponent<ParticleSystemRenderer>();
            if (portalRend != null) portalRend.sharedMaterial = particleGlowMat;
            var portalMain = portalPs.main;
            portalMain.startLifetime = 1.8f;
            portalMain.startSpeed = 0.6f;
            portalMain.startSize = 0.07f;
            portalMain.startColor = new Color(0.2f, 0.95f, 0.85f, 0.85f);
            portalMain.gravityModifier = -0.05f;
            var portalEmission = portalPs.emission;
            portalEmission.rateOverTime = 30f;
            var portalShape = portalPs.shape;
            portalShape.shapeType = ParticleSystemShapeType.Circle;
            portalShape.radius = 0.55f;

            // =========================================================================
            // 5. SPATIAL AUDIO SOUNDSCAPE
            // =========================================================================
            var polarAudioSrc = AddLoopingAudio(audioRoot.transform, "Audio_AmbientPolar", polarDrone, 0.45f);
            var langAudioSrc = AddLoopingAudio(audioRoot.transform, "Audio_WhisperedLanguages", waterEcho, 0.35f);
            var celloAudioSrc = AddLoopingAudio(audioRoot.transform, "Audio_CelloMelody", toneC, 0.38f);
            var relicAudioSrc = audioRoot.AddComponent<AudioSource>();
            relicAudioSrc.playOnAwake = false;
            relicAudioSrc.spatialBlend = 0.8f;

            // =========================================================================
            // 6. PLAYER SPAWN POINT & TRANSITION CAMERA
            // =========================================================================
            GameObject spawnGo = new GameObject("PlayerSpawnPoint");
            spawnGo.transform.SetParent(managersRoot.transform, false);
            spawnGo.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            spawnGo.transform.localRotation = Quaternion.identity;

            // Setup Main Camera for Scene (desktop + VR fallback)
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camRig = new GameObject("[CameraRig]");
                camRig.transform.position = spawnGo.transform.position;
                camRig.transform.rotation = spawnGo.transform.rotation;
                GameObject camGo = new GameObject("Main Camera");
                camGo.transform.SetParent(camRig.transform, false);
                camGo.transform.localPosition = new Vector3(0f, 1.7f, 0f);
                mainCam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }

            // Ensure SphereDivingTransition is present for returning
            var divingTrans = mainCam.GetComponent<SphereDivingTransition>();
            if (divingTrans == null) divingTrans = mainCam.gameObject.AddComponent<SphereDivingTransition>();
            divingTrans.SetTargetScene("WelcomeChamber");

            // =========================================================================
            // 7. HUMANITY WORLD MANAGER WIRING
            // =========================================================================
            GameObject managerGo = new GameObject("HumanityWorldManager");
            managerGo.transform.SetParent(managersRoot.transform, false);
            var manager = managerGo.AddComponent<HumanityWorldManager>();

            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("moonLight").objectReferenceValue = moonLight;
            so.FindProperty("auroraRibbons").objectReferenceValue = auroraPs;
            so.FindProperty("seedSproutSpores").objectReferenceValue = sporesPs;
            so.FindProperty("whisperingGlyphs").objectReferenceValue = glyphsPs;
            so.FindProperty("glacialDiamondDust").objectReferenceValue = dustPs;

            so.FindProperty("ambientPolarAudio").objectReferenceValue = polarAudioSrc;
            so.FindProperty("whisperedLanguagesAudio").objectReferenceValue = langAudioSrc;
            so.FindProperty("celloMelodyAudio").objectReferenceValue = celloAudioSrc;
            so.FindProperty("relicInteractionAudio").objectReferenceValue = relicAudioSrc;

            so.FindProperty("returnMonolith").objectReferenceValue = returnMonolithGo != null ? returnMonolithGo.transform : null;
            so.FindProperty("returnSphere").objectReferenceValue = returnSphere.transform;
            so.FindProperty("returnPortalParticles").objectReferenceValue = portalPs;
            so.FindProperty("playerSpawnPoint").objectReferenceValue = spawnGo.transform;

            // Cryogenic lights array
            var cryoProp = so.FindProperty("cryogenicGlowLights");
            cryoProp.arraySize = 4;
            cryoProp.GetArrayElementAtIndex(0).objectReferenceValue = vaultLight;
            cryoProp.GetArrayElementAtIndex(1).objectReferenceValue = rosettaLight;
            cryoProp.GetArrayElementAtIndex(2).objectReferenceValue = alexandriaLight;
            cryoProp.GetArrayElementAtIndex(3).objectReferenceValue = dnaLight;

            // Interactive Relics array (6 items)
            var relicsProp = so.FindProperty("interactiveRelics");
            relicsProp.arraySize = 6;
            relicsProp.GetArrayElementAtIndex(0).objectReferenceValue = seedVaultGo != null ? seedVaultGo.transform : null;
            relicsProp.GetArrayElementAtIndex(1).objectReferenceValue = rosettaGo != null ? rosettaGo.transform : null;
            relicsProp.GetArrayElementAtIndex(2).objectReferenceValue = alexandriaGo != null ? alexandriaGo.transform : null;
            relicsProp.GetArrayElementAtIndex(3).objectReferenceValue = dnaSpireGo != null ? dnaSpireGo.transform : null;
            relicsProp.GetArrayElementAtIndex(4).objectReferenceValue = celloGo != null ? celloGo.transform : null;
            relicsProp.GetArrayElementAtIndex(5).objectReferenceValue = returnMonolithGo != null ? returnMonolithGo.transform : null;

            // Relic Info Descriptions
            var infoProp = so.FindProperty("relicInfos");
            infoProp.arraySize = 6;
            SetRelicData(infoProp.GetArrayElementAtIndex(0),
                "Svalbard Global Seed Vault", "ARCTIC SANCTUARY // 2008 A.D.",
                "120 meters deep inside Svalbard sandstone permafrost, humanity secured over 1.3 million distinct seed accessions. The ultimate planetary insurance for Earth's biosphere.",
                new Color(0.2f, 0.95f, 0.85f), toneE);

            SetRelicData(infoProp.GetArrayElementAtIndex(1),
                "The Rosetta Stone", "PTOLEMAIC DYNASTY // 196 B.C.",
                "A decree carved in Hieroglyphs, Demotic, and Greek. The linguistic catalyst that allowed modern humans to resurrect lost millennia of ancestral civilization.",
                new Color(1.0f, 0.85f, 0.45f), toneC);

            SetRelicData(infoProp.GetArrayElementAtIndex(2),
                "The Library of Alexandria", "HELLENISTIC ARCHIVE // 300 B.C.",
                "The legendary scroll repository where humanity first attempted to gather all knowledge under one roof: geometry, astronomy, poetry, and medicine.",
                new Color(1.0f, 0.75f, 0.35f), toneC);

            SetRelicData(infoProp.GetArrayElementAtIndex(3),
                "The Human Genome DNA Spire", "MOLECULAR THRESHOLD // 2003 A.D.",
                "The 3.2 billion chemical base-pair code of Homo sapiens fully mapped. The moment life on Earth stopped being merely written by natural selection and learned to read itself.",
                new Color(0.25f, 0.90f, 1.0f), toneE);

            SetRelicData(infoProp.GetArrayElementAtIndex(4),
                "The Violoncello of Human Emotion", "SACRED ACOUSTICS // 1700 A.D.",
                "Carved from spruce and maple, vibrating at frequencies that bypass intellect to stir unconditional tears, serenity, and longing across all language barriers.",
                new Color(1.0f, 0.82f, 0.45f), toneC);

            SetRelicData(infoProp.GetArrayElementAtIndex(5),
                "Chronos Return Monolith", "WELCOME CHAMBER GATEWAY",
                "Reach out and touch the hovering miniature sphere to cradle the memory of humanity and return to Station III in the XENOASIS Welcome Chamber.",
                new Color(0.35f, 0.95f, 0.90f), snapClip);

            so.ApplyModifiedProperties();

            // Save Scene
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            // Register in Build Settings
            RegisterSceneInBuildSettings(ScenePath);

            Debug.Log("[HumanityWorldArchitect] ✦ World 3: The Heart of Humanity 100% COMPLETE!");
        }

        #region Helpers & Subsystems
        private static void SetupAtmosphereAndSkybox()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.08f, 0.15f, 0.22f);
            RenderSettings.ambientEquatorColor = new Color(0.05f, 0.12f, 0.18f);
            RenderSettings.ambientGroundColor = new Color(0.02f, 0.04f, 0.08f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.04f, 0.10f, 0.16f);
            RenderSettings.fogDensity = 0.009f;

            // Load or create skybox
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skybox_WorldLabs_HeartOfHumanity.mat");
            Texture2D panoTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Skybox/WorldLabs_HeartOfHumanity_Pano.png")
                             ?? AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Skybox/CosmicVoid_Pano.png");

            if (skyboxMat == null)
            {
                Shader skyShader = Shader.Find("Skybox/Panoramic");
                if (skyShader != null)
                {
                    skyboxMat = new Material(skyShader);
                    if (panoTex != null) skyboxMat.SetTexture("_MainTex", panoTex);
                    skyboxMat.SetFloat("_Exposure", 1.15f);
                    AssetDatabase.CreateAsset(skyboxMat, "Assets/Materials/Skybox_WorldLabs_HeartOfHumanity.mat");
                }
            }
            else if (panoTex != null)
            {
                skyboxMat.SetTexture("_MainTex", panoTex);
                skyboxMat.SetFloat("_Exposure", 1.15f);
            }

            if (skyboxMat != null)
            {
                RenderSettings.skybox = skyboxMat;
            }
        }

        private static GameObject SpawnTripoProp(string assetPath, string name, Transform parent, Vector3 localPos, Quaternion localRot, Vector3 localScale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject go;
            if (prefab != null)
            {
                go = Object.Instantiate(prefab, parent);
                go.name = name;
                go.transform.localPosition = localPos;
                go.transform.localRotation = localRot;
                go.transform.localScale = localScale;
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = name + "_Fallback";
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localPos;
                go.transform.localRotation = localRot;
                go.transform.localScale = localScale;
                var mr = go.GetComponent<MeshRenderer>();
                Material obsidianMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ObsidianGlass.mat");
                if (obsidianMat != null) mr.sharedMaterial = obsidianMat;
            }

            // Ensure it has collider for VR touch interaction
            if (go.GetComponent<Collider>() == null && go.GetComponentInChildren<Collider>() == null)
            {
                var col = go.AddComponent<BoxCollider>();
                col.size = Vector3.one * 1.2f;
                col.center = new Vector3(0f, 0.6f, 0f);
            }

            return go;
        }

        private static Light AddCryoGlowLight(Transform parent, Vector3 localPos, Color color, float intensity, float range)
        {
            GameObject lightGo = new GameObject("Light_CryoGlow");
            lightGo.transform.SetParent(parent, false);
            lightGo.transform.localPosition = localPos;
            var l = lightGo.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.None;
            return l;
        }

        private static ParticleSystem CreateAuroraRibbons(Transform parent, Material mat)
        {
            GameObject go = new GameObject("VFX_AuroraRibbons");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 15f, 20f);
            var ps = go.AddComponent<ParticleSystem>();
            var rend = go.GetComponent<ParticleSystemRenderer>();
            if (rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 8.0f;
            main.startSpeed = 1.2f;
            main.startSize = 3.5f;
            main.startColor = new Color(0.15f, 0.95f, 0.70f, 0.45f);
            main.maxParticles = 120;

            var emission = ps.emission;
            emission.rateOverTime = 18f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(45f, 8f, 12f);

            return ps;
        }

        private static ParticleSystem CreateSeedSproutSpores(Transform parent, Vector3 pos, Material mat)
        {
            GameObject go = new GameObject("VFX_SeedSproutSpores");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var ps = go.AddComponent<ParticleSystem>();
            var rend = go.GetComponent<ParticleSystemRenderer>();
            if (rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 3.5f;
            main.startSpeed = 0.55f;
            main.startSize = 0.065f;
            main.startColor = new Color(0.25f, 0.95f, 0.85f, 0.85f);
            main.gravityModifier = -0.1f;
            main.maxParticles = 150;

            var emission = ps.emission;
            emission.rateOverTime = 25f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.2f;

            return ps;
        }

        private static ParticleSystem CreateWhisperingGlyphs(Transform parent, Vector3 pos, Material mat)
        {
            GameObject go = new GameObject("VFX_WhisperingGlyphs");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var ps = go.AddComponent<ParticleSystem>();
            var rend = go.GetComponent<ParticleSystemRenderer>();
            if (rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 4.0f;
            main.startSpeed = 0.40f;
            main.startSize = 0.08f;
            main.startColor = new Color(1.0f, 0.85f, 0.35f, 0.75f);
            main.gravityModifier = -0.05f;
            main.maxParticles = 80;

            var emission = ps.emission;
            emission.rateOverTime = 15f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.95f;

            return ps;
        }

        private static ParticleSystem CreateGlacialDiamondDust(Transform parent, Material mat)
        {
            GameObject go = new GameObject("VFX_GlacialDiamondDust");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 3.5f, 2.0f);
            var ps = go.AddComponent<ParticleSystem>();
            var rend = go.GetComponent<ParticleSystemRenderer>();
            if (rend != null) rend.sharedMaterial = mat;

            var main = ps.main;
            main.startLifetime = 6.0f;
            main.startSpeed = 0.25f;
            main.startSize = 0.045f;
            main.startColor = new Color(0.85f, 0.95f, 1.0f, 0.65f);
            main.maxParticles = 250;

            var emission = ps.emission;
            emission.rateOverTime = 35f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(25f, 6f, 25f);

            return ps;
        }

        private static AudioSource AddLoopingAudio(Transform parent, string name, AudioClip clip, float volume)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = true;
            src.volume = volume;
            src.spatialBlend = 0.5f;
            src.playOnAwake = true;
            if (clip != null) src.Play();
            return src;
        }

        private static void CreatePermafrostColonnade(Transform parent, Material colMat, Material goldMat, Material glowCyan)
        {
            float radius = 14.5f;
            int count = 14;
            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2f;
                // Leave an opening for entrance/portal at South (angle around PI)
                if (Mathf.Abs(angle - Mathf.PI) < 0.35f) continue;

                Vector3 pos = new Vector3(Mathf.Sin(angle) * radius, 1.2f, Mathf.Cos(angle) * radius + 3.5f);

                GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                col.name = $"GlacialColumn_{i:D2}";
                col.transform.SetParent(parent, false);
                col.transform.localPosition = pos;
                float height = 3.2f + Mathf.Sin(i * 1.8f) * 0.9f;
                col.transform.localScale = new Vector3(1.2f, height, 1.2f);
                if (colMat != null) col.GetComponent<MeshRenderer>().sharedMaterial = colMat;

                // Gold and Cyan Ring capitals
                CreateMeshRing(col.transform, "Col_RingGold", 0.65f, 0.06f, goldMat, 0.98f, Vector3.zero);
                CreateMeshRing(col.transform, "Col_RingCyan", 0.60f, 0.03f, glowCyan, 0.99f, Vector3.zero);
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
            prop.FindPropertyRelative("proximitySound").objectReferenceValue = sfx;
        }

        private static void RegisterSceneInBuildSettings(string path)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == path)
                {
                    scenes[i].enabled = true;
                    EditorBuildSettings.scenes = scenes;
                    return;
                }
            }

            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++)
            {
                newScenes[i] = scenes[i];
            }
            newScenes[scenes.Length] = new EditorBuildSettingsScene(path, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log($"[HumanityWorldArchitect] Registered scene '{path}' in EditorBuildSettings (Index {scenes.Length}).");
        }
        #endregion
    }
}
#endif
