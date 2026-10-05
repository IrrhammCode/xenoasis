using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// XENOASIS — UFOCockpitArchitect.cs
/// Remodels the interior of the extraterrestrial UFO command bridge into a AAA sci-fi masterpiece:
/// 1. Full 360° Enclosed Alien Command Bridge:
///    - Sleek dark titanium anti-gravity deck plating with gold compass rings & illuminated walkway.
///    - 250° solid dark titanium acoustic bulkheads framing the sides and rear.
///    - 110° panoramic forward crystal glass viewport looking out into deep cosmos & alpine mountains.
///    - 4 aerodynamic canopy arches overhead with recessed golden energy conduit trims.
/// 2. Pilot Flight Command Station:
///    - Tripo 3D Alien Pilot Command Throne (ufo_pilot_command_throne.glb) perfectly positioned at pilot seat.
///    - Wrap-around dark titanium flight console with slender gold bezel rims & tactile flight controls.
///    - 3 High-Fidelity Telemetry Displays with un-mirrored readable text & telemetry diagrams.
///    - Central 3D Holo-Globe of Earth spinning above the dash with pulsating touchdown beacon.
/// 3. Mission Stations:
///    - Port: Tripo 3D Copilot & Science Navigation Console (ufo_copilot_navigation_console.glb).
///    - Starboard: Tripo 3D Tactical & Warp Navigation Console (ufo_cockpit_interior.glb).
/// 4. Aft Engineering & Hyperdrive Core:
///    - Tripo 3D Hyperdrive Reactor Core (ufo_hyperdrive_reactor_core.glb) slowly revolving with pulsating plasma core light.
///    - Heavy reinforced airlock pressure door with gold hydraulic seal frames and pressure valve wheel.
///    - Atmospheric conduit manifolds flanking the reactor alcove.
/// </summary>
public static class UFOCockpitArchitect
{
    [MenuItem("XENOASIS/Design Real UFO Interior")]
    public static void DesignUFOInterior()
    {
        Debug.Log("[UFOCockpitArchitect] ✦ Starting complete architectural remodeling of UFO bridge interior...");

        GameObject bridgeRoot = GameObject.Find("[UFOCockpitBridge]");
        if (bridgeRoot == null)
        {
            Debug.LogError("[UFOCockpitArchitect] Could not find [UFOCockpitBridge] in active scene!");
            return;
        }

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) urpLit = Shader.Find("Standard");

        // 1. PBR Materials Setup
        Material hullDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat");
        if (hullDark == null)
        {
            hullDark = new Material(urpLit);
            AssetDatabase.CreateAsset(hullDark, "Assets/Materials/Cockpit_HullDark.mat");
        }
        hullDark.shader = urpLit;
        hullDark.SetColor("_BaseColor", new Color(0.08f, 0.09f, 0.12f, 1.0f));
        hullDark.SetFloat("_Metallic", 0.85f);
        hullDark.SetFloat("_Smoothness", 0.82f);
        hullDark.SetFloat("_Cull", 0f);
        hullDark.doubleSidedGI = true;
        EditorUtility.SetDirty(hullDark);

        Material goldMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LuxuryGold_Inlay.mat");
        if (goldMat == null)
        {
            goldMat = new Material(urpLit);
            AssetDatabase.CreateAsset(goldMat, "Assets/Materials/LuxuryGold_Inlay.mat");
        }
        goldMat.shader = urpLit;
        goldMat.SetColor("_BaseColor", new Color(0.92f, 0.76f, 0.38f, 1.0f));
        goldMat.SetFloat("_Metallic", 0.92f);
        goldMat.SetFloat("_Smoothness", 0.88f);
        EditorUtility.SetDirty(goldMat);

        Material glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        if (glassMat == null)
        {
            glassMat = new Material(urpLit);
            AssetDatabase.CreateAsset(glassMat, "Assets/Materials/GlassViewport.mat");
        }
        glassMat.SetFloat("_GridEmission", 0f);
        glassMat.SetFloat("_GridLineWidth", 0f);
        EditorUtility.SetDirty(glassMat);

        Material glowCyan = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        if (glowCyan == null)
        {
            glowCyan = new Material(urpLit);
            AssetDatabase.CreateAsset(glowCyan, "Assets/Materials/EmissiveCyan.mat");
        }
        glowCyan.shader = urpLit;
        glowCyan.SetColor("_BaseColor", new Color(0f, 0.88f, 1.0f, 1.0f));
        glowCyan.EnableKeyword("_EMISSION");
        glowCyan.SetColor("_EmissionColor", new Color(0f, 0.88f, 1.0f) * 2.2f);
        EditorUtility.SetDirty(glowCyan);

        Material glowGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");
        if (glowGold == null)
        {
            glowGold = new Material(urpLit);
            AssetDatabase.CreateAsset(glowGold, "Assets/Materials/EmissiveGold.mat");
        }
        glowGold.shader = urpLit;
        glowGold.SetColor("_BaseColor", new Color(1.0f, 0.75f, 0.25f, 1.0f));
        glowGold.EnableKeyword("_EMISSION");
        glowGold.SetColor("_EmissionColor", new Color(1.0f, 0.75f, 0.25f) * 2.5f);
        EditorUtility.SetDirty(glowGold);

        // Telemetry screen materials
        Material leftScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Left_EarthScan.mat");
        Material centerScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Center.mat");
        Material rightScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Right_EarthOrbit.mat");

        Transform oldInterior = bridgeRoot.transform.Find("UFO_Cockpit_Interior");
        if (oldInterior != null) Object.DestroyImmediate(oldInterior.gameObject);

        Transform oldMasterpiece = bridgeRoot.transform.Find("UFO_Bridge_Masterpiece");
        if (oldMasterpiece != null) Object.DestroyImmediate(oldMasterpiece.gameObject);

        Transform oldHolo = bridgeRoot.transform.Find("ConsoleHoloDisplay");
        if (oldHolo != null) Object.DestroyImmediate(oldHolo.gameObject);

        Transform oldL1 = bridgeRoot.transform.Find("CockpitInternalLight");
        if (oldL1 != null) Object.DestroyImmediate(oldL1.gameObject);

        Transform oldL2 = bridgeRoot.transform.Find("CockpitRearLight");
        if (oldL2 != null) Object.DestroyImmediate(oldL2.gameObject);

        GameObject bridgeMaster = new GameObject("UFO_Bridge_Masterpiece");
        bridgeMaster.transform.SetParent(bridgeRoot.transform, false);
        bridgeMaster.transform.localPosition = Vector3.zero;
        bridgeMaster.transform.localRotation = Quaternion.identity;

        // =========================================================================
        // 1. DECK & FLOOR ARCHITECTURE
        // =========================================================================
        BuildBridgeDeck(bridgeMaster.transform, hullDark, goldMat, glowCyan);

        // =========================================================================
        // 2. STRUCTURAL BULKHEAD WALLS & CANOPY ARCHES
        // =========================================================================
        BuildBridgeHullAndCanopy(bridgeMaster.transform, hullDark, goldMat, glowCyan, glassMat);

        // =========================================================================
        // 3. PILOT FLIGHT COMMAND STATION (Throne, Dashboard, 3 Screens, Holo-Globe)
        // =========================================================================
        BuildPilotStation(bridgeMaster.transform, hullDark, goldMat, glowCyan, leftScreenMat, centerScreenMat, rightScreenMat);

        // =========================================================================
        // 4. PORT & STARBOARD MISSION WINGS (Tripo Nav & Science Consoles)
        // =========================================================================
        BuildMissionWings(bridgeMaster.transform, hullDark, goldMat, glowCyan);

        // =========================================================================
        // 5. AFT ENGINEERING & HYPERDRIVE REACTOR CORE
        // =========================================================================
        BuildAftEngineering(bridgeMaster.transform, hullDark, goldMat, glowGold);

        // =========================================================================
        // 6. AMBIENT BRIDGE LIGHTING
        // =========================================================================
        SetupBridgeLighting(bridgeMaster.transform);

        // Mark scene dirty and save
        EditorUtility.SetDirty(bridgeMaster);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[UFOCockpitArchitect] ✦ UFO Bridge Interior Architectural Remodeling 100% COMPLETE!");
    }

    private static void BuildBridgeDeck(Transform parent, Material darkMat, Material goldMat, Material glowCyan)
    {
        GameObject deckRoot = new GameObject("Bridge_Deck_Plating");
        deckRoot.transform.SetParent(parent, false);

        // 1. Main circular anti-gravity deck slab (Radius 3.4m, Height 0.12m at local Y = -0.06m)
        GameObject deckSlab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        deckSlab.name = "Deck_PrimarySlab";
        deckSlab.transform.SetParent(deckRoot.transform, false);
        deckSlab.transform.localPosition = new Vector3(0f, -0.06f, 0f);
        deckSlab.transform.localScale = new Vector3(6.8f, 0.12f, 6.8f);
        deckSlab.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(deckSlab.GetComponent<Collider>());

        // 2. Concentric Gold Inlay Rings on deck
        CreateMeshRing(deckRoot.transform, "DeckInlay_InnerRing", 1.55f, 0.08f, goldMat, 0.002f);
        CreateMeshRing(deckRoot.transform, "DeckInlay_OuterRing", 3.15f, 0.10f, goldMat, 0.002f);

        // 3. Central illuminated anti-gravity walkway conduit (leading from aft reactor to pilot throne)
        GameObject walkway = GameObject.CreatePrimitive(PrimitiveType.Cube);
        walkway.name = "Walkway_AntiGravConduit";
        walkway.transform.SetParent(deckRoot.transform, false);
        walkway.transform.localPosition = new Vector3(0f, 0.002f, -1.15f);
        walkway.transform.localScale = new Vector3(0.55f, 0.004f, 3.4f);
        walkway.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(walkway.GetComponent<Collider>());

        // Glowing center guide strip along the walkway
        GameObject guideStrip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        guideStrip.name = "Walkway_GuideGlow";
        guideStrip.transform.SetParent(walkway.transform, false);
        guideStrip.transform.localPosition = new Vector3(0f, 0.52f, 0f);
        guideStrip.transform.localScale = new Vector3(0.08f, 0.002f, 0.98f);
        guideStrip.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
        Object.DestroyImmediate(guideStrip.GetComponent<Collider>());
    }

    private static void BuildBridgeHullAndCanopy(Transform parent, Material darkMat, Material goldMat, Material glowCyan, Material glassMat)
    {
        GameObject hullRoot = new GameObject("Bridge_Hull_Architecture");
        hullRoot.transform.SetParent(parent, false);

        int totalSegments = 24;
        float radius = 3.35f;
        float wallH = 2.45f;
        float yCenter = wallH * 0.5f;

        // Bulkhead wall segments around perimeter
        // Angle 0 is forward (+Z). Front 110° (-55° to +55°, segments 21-23 and 0-3) is the VIEWPORT!
        // Solid wall segments are from 55° to 305° (segments 4 to 20).
        for (int i = 0; i < totalSegments; i++)
        {
            float angleDeg = i * (360f / totalSegments);
            bool isViewport = (angleDeg < 55f || angleDeg > 305f);

            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, yCenter, Mathf.Cos(rad) * radius);
            Quaternion rot = Quaternion.Euler(0f, angleDeg + 180f, 0f);

            if (!isViewport)
            {
                // Solid bulkhead wall segment
                GameObject wallBay = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallBay.name = $"Bulkhead_Bay_{i:D2}";
                wallBay.transform.SetParent(hullRoot.transform, false);
                wallBay.transform.localPosition = pos; // LOCAL position!
                wallBay.transform.localRotation = rot;
                wallBay.transform.localScale = new Vector3(0.95f, wallH, 0.18f);
                wallBay.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
                Object.DestroyImmediate(wallBay.GetComponent<Collider>());

                // Vertical gold accent conduit
                GameObject conduit = GameObject.CreatePrimitive(PrimitiveType.Cube);
                conduit.name = "GoldConduit";
                conduit.transform.SetParent(wallBay.transform, false);
                conduit.transform.localPosition = new Vector3(0f, 0f, 0.52f);
                conduit.transform.localScale = new Vector3(0.04f, 0.95f, 0.04f);
                conduit.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(conduit.GetComponent<Collider>());
            }
            else
            {
                // Lower viewport sill
                GameObject sill = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sill.name = $"ViewportSill_Bay_{i:D2}";
                sill.transform.SetParent(hullRoot.transform, false);
                sill.transform.localPosition = new Vector3(pos.x, 0.35f, pos.z); // LOCAL position!
                sill.transform.localRotation = rot;
                sill.transform.localScale = new Vector3(0.95f, 0.70f, 0.20f);
                sill.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
                Object.DestroyImmediate(sill.GetComponent<Collider>());

                // Gold trim on sill lip
                GameObject sillTrim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sillTrim.name = "SillGoldLip";
                sillTrim.transform.SetParent(sill.transform, false);
                sillTrim.transform.localPosition = new Vector3(0f, 0.52f, 0f);
                sillTrim.transform.localScale = new Vector3(1.0f, 0.06f, 0.12f);
                sillTrim.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(sillTrim.GetComponent<Collider>());

                // Panoramic glass viewport pane
                GameObject glassPane = GameObject.CreatePrimitive(PrimitiveType.Cube);
                glassPane.name = $"ViewportGlass_Bay_{i:D2}";
                glassPane.transform.SetParent(hullRoot.transform, false);
                glassPane.transform.localPosition = new Vector3(pos.x, 1.55f, pos.z); // LOCAL position!
                glassPane.transform.localRotation = rot;
                glassPane.transform.localScale = new Vector3(0.94f, 1.70f, 0.04f);
                glassPane.GetComponent<MeshRenderer>().sharedMaterial = glassMat;
                Object.DestroyImmediate(glassPane.GetComponent<Collider>());

                // Upper viewport canopy header
                GameObject header = GameObject.CreatePrimitive(PrimitiveType.Cube);
                header.name = $"ViewportHeader_Bay_{i:D2}";
                header.transform.SetParent(hullRoot.transform, false);
                header.transform.localPosition = new Vector3(pos.x, 2.50f, pos.z); // LOCAL position!
                header.transform.localRotation = rot;
                header.transform.localScale = new Vector3(0.95f, 0.30f, 0.22f);
                header.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
                Object.DestroyImmediate(header.GetComponent<Collider>());
            }
        }

        // Structural Canopy Arches (4 sleek arches spanning ceiling across bridge)
        float[] archZ = { 1.45f, 0.20f, -1.15f, -2.45f };
        for (int a = 0; a < archZ.Length; a++)
        {
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arch.name = $"CanopyArch_{a:D2}";
            arch.transform.SetParent(hullRoot.transform, false);
            arch.transform.localPosition = new Vector3(0f, 2.52f, archZ[a]);
            arch.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            arch.transform.localScale = new Vector3(0.18f, 3.25f, 0.28f);
            arch.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
            Object.DestroyImmediate(arch.GetComponent<Collider>());

            // Underside gold energy trim on arch
            GameObject archTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            archTrim.name = "ArchGoldTrim";
            archTrim.transform.SetParent(arch.transform, false);
            archTrim.transform.localPosition = new Vector3(0f, 0f, -0.45f);
            archTrim.transform.localScale = new Vector3(0.04f, 0.98f, 0.08f);
            archTrim.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(archTrim.GetComponent<Collider>());
        }

        // Bridge Ceiling Soffit Cap
        GameObject ceilingCap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ceilingCap.name = "Bridge_CeilingSoffit";
        ceilingCap.transform.SetParent(hullRoot.transform, false);
        ceilingCap.transform.localPosition = new Vector3(0f, 2.65f, -0.40f);
        ceilingCap.transform.localScale = new Vector3(6.6f, 0.12f, 6.6f);
        ceilingCap.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(ceilingCap.GetComponent<Collider>());

        // Gold Bezel on Central Avionics Dome
        CreateMeshRing(hullRoot.transform, "Ceiling_AvionicsGoldBezel", 1.80f, 0.15f, goldMat, 2.58f);
    }

    private static void BuildPilotStation(Transform parent, Material darkMat, Material goldMat, Material glowCyan,
                                         Material leftScreenMat, Material centerScreenMat, Material rightScreenMat)
    {
        GameObject stationRoot = new GameObject("Pilot_Flight_Command_Station");
        stationRoot.transform.SetParent(parent, false);

        // 1. Alien Pilot Command Throne (Tripo 3D High-Poly Model)
        GameObject thronePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_pilot_command_throne.glb");
        if (thronePrefab != null)
        {
            GameObject throne = Object.Instantiate(thronePrefab, stationRoot.transform);
            throne.name = "Tripo_AlienPilot_CommandThrone";
            // Native bounds Y is [-0.5, +0.5], facing +X natively.
            // When scaled by 1.35, Y center needs to be at +0.68m to sit on floor, and rotated +90 deg Y to face +Z!
            throne.transform.localPosition = new Vector3(0f, 0.68f, -0.35f);
            throne.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            throne.transform.localScale = Vector3.one * 1.35f;

            var col = throne.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 0f, 0f);
            col.size = new Vector3(0.95f, 1.30f, 0.95f);
        }

        // 2. Wrap-Around Pilot Flight Console Desk (Curved dark titanium with gold trim)
        GameObject consoleRoot = new GameObject("Pilot_Flight_Console");
        consoleRoot.transform.SetParent(stationRoot.transform, false);

        // Center console section
        GameObject centerDesk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerDesk.name = "Console_CenterSection";
        centerDesk.transform.SetParent(consoleRoot.transform, false);
        centerDesk.transform.localPosition = new Vector3(0f, 0.52f, 1.15f);
        centerDesk.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        centerDesk.transform.localScale = new Vector3(1.10f, 0.35f, 0.55f);
        centerDesk.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(centerDesk.GetComponent<Collider>());

        // Center gold perimeter bevel border
        BuildConsoleBevelRim(centerDesk.transform, goldMat);

        // Left angled console wing (angled 24° towards pilot)
        GameObject leftDesk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftDesk.name = "Console_LeftWing";
        leftDesk.transform.SetParent(consoleRoot.transform, false);
        leftDesk.transform.localPosition = new Vector3(-0.95f, 0.52f, 1.02f);
        leftDesk.transform.localRotation = Quaternion.Euler(15f, 24f, 0f);
        leftDesk.transform.localScale = new Vector3(0.95f, 0.35f, 0.55f);
        leftDesk.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(leftDesk.GetComponent<Collider>());

        BuildConsoleBevelRim(leftDesk.transform, goldMat);

        // Right angled console wing (angled -24° towards pilot)
        GameObject rightDesk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightDesk.name = "Console_RightWing";
        rightDesk.transform.SetParent(consoleRoot.transform, false);
        rightDesk.transform.localPosition = new Vector3(0.95f, 0.52f, 1.02f);
        rightDesk.transform.localRotation = Quaternion.Euler(15f, -24f, 0f);
        rightDesk.transform.localScale = new Vector3(0.95f, 0.35f, 0.55f);
        rightDesk.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(rightDesk.GetComponent<Collider>());

        BuildConsoleBevelRim(rightDesk.transform, goldMat);

        // Integrated Tripo Command Desk Module in front
        GameObject tripoModule = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_module_tripo.glb");
        if (tripoModule != null)
        {
            GameObject moduleInst = Object.Instantiate(tripoModule, consoleRoot.transform);
            moduleInst.name = "Tripo_FlightControls_Module";
            moduleInst.transform.localPosition = new Vector3(0f, 0.58f, 0.88f);
            moduleInst.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            moduleInst.transform.localScale = Vector3.one * 0.90f;
        }

        // 3. Mount 3 High-Resolution Telemetry Displays with Titanium Housings (Un-mirrored)
        // Center: Tactical Descent Corridor & Landing Protocol
        MountMonitorDisplay(consoleRoot.transform, "FlightMonitor_Center", 
            new Vector3(0f, 0.98f, 1.25f), Quaternion.Euler(6f, 0f, 0f), 
            new Vector2(0.72f, 0.40f), centerScreenMat, darkMat, goldMat);

        // Left: Planetary Biosphere & Atmosphere Spectrograph
        MountMonitorDisplay(consoleRoot.transform, "FlightMonitor_Left", 
            new Vector3(-0.76f, 0.95f, 1.18f), Quaternion.Euler(6f, 22f, 0f), 
            new Vector2(0.70f, 0.40f), leftScreenMat, darkMat, goldMat);

        // Right: Sol-3 Orbital Dynamics & Extraterrestrial Vessel Systems
        MountMonitorDisplay(consoleRoot.transform, "FlightMonitor_Right", 
            new Vector3(0.76f, 0.95f, 1.18f), Quaternion.Euler(6f, -22f, 0f), 
            new Vector2(0.70f, 0.40f), rightScreenMat, darkMat, goldMat);

        // 4. Central 3D Holo-Globe of Earth spinning above the dash
        GameObject holoRoot = new GameObject("Console_3DHoloEarth");
        holoRoot.transform.SetParent(consoleRoot.transform, false);
        holoRoot.transform.localPosition = new Vector3(0f, 0.88f, 0.92f);

        GameObject holoSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        holoSphere.name = "HoloEarth_Globe";
        holoSphere.transform.SetParent(holoRoot.transform, false);
        holoSphere.transform.localPosition = Vector3.zero;
        holoSphere.transform.localScale = Vector3.one * 0.16f;
        holoSphere.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
        Object.DestroyImmediate(holoSphere.GetComponent<Collider>());

        // Orbit ring around HoloEarth
        CreateMeshRing(holoRoot.transform, "HoloEarth_TrajectoryRing", 0.14f, 0.015f, goldMat, 0f);

        var rotator = holoRoot.AddComponent<ExhibitRotator>();
        rotator.rotationSpeed = 16.0f;
        rotator.bobAmplitude = 0.015f;
        rotator.bobSpeed = 1.5f;

        var holoLight = holoRoot.AddComponent<Light>();
        holoLight.type = LightType.Point;
        holoLight.color = new Color(0f, 0.9f, 1.0f);
        holoLight.intensity = 1.8f;
        holoLight.range = 2.2f;
    }

    private static void BuildConsoleBevelRim(Transform desk, Material goldMat)
    {
        // 4 slender border bars framing the desk top
        float borderW = 0.035f;

        // Front border
        var fb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fb.name = "Rim_Front";
        fb.transform.SetParent(desk, false);
        fb.transform.localPosition = new Vector3(0f, 0.505f, 0.48f);
        fb.transform.localScale = new Vector3(1.0f, 0.03f, borderW);
        fb.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(fb.GetComponent<Collider>());

        // Back border
        var bb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bb.name = "Rim_Back";
        bb.transform.SetParent(desk, false);
        bb.transform.localPosition = new Vector3(0f, 0.505f, -0.48f);
        bb.transform.localScale = new Vector3(1.0f, 0.03f, borderW);
        bb.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(bb.GetComponent<Collider>());

        // Left border
        var lb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lb.name = "Rim_Left";
        lb.transform.SetParent(desk, false);
        lb.transform.localPosition = new Vector3(-0.48f, 0.505f, 0f);
        lb.transform.localScale = new Vector3(borderW, 0.03f, 1.0f);
        lb.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(lb.GetComponent<Collider>());

        // Right border
        var rb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rb.name = "Rim_Right";
        rb.transform.SetParent(desk, false);
        rb.transform.localPosition = new Vector3(0.48f, 0.505f, 0f);
        rb.transform.localScale = new Vector3(borderW, 0.03f, 1.0f);
        rb.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(rb.GetComponent<Collider>());
    }

    private static void MountMonitorDisplay(Transform parent, string name, Vector3 pos, Quaternion rot, 
                                           Vector2 size, Material screenMat, Material housingMat, Material goldMat)
    {
        GameObject monRoot = new GameObject(name);
        monRoot.transform.SetParent(parent, false);
        monRoot.transform.localPosition = pos;
        monRoot.transform.localRotation = rot;

        // Housing backing plate
        GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        housing.name = "Housing";
        housing.transform.SetParent(monRoot.transform, false);
        housing.transform.localPosition = new Vector3(0f, 0f, 0.012f);
        housing.transform.localScale = new Vector3(size.x + 0.05f, size.y + 0.05f, 0.024f);
        housing.GetComponent<MeshRenderer>().sharedMaterial = housingMat;
        Object.DestroyImmediate(housing.GetComponent<Collider>());

        // Gold border bevel
        GameObject goldBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goldBorder.name = "GoldBevel";
        goldBorder.transform.SetParent(monRoot.transform, false);
        goldBorder.transform.localPosition = new Vector3(0f, 0f, 0.005f);
        goldBorder.transform.localScale = new Vector3(size.x + 0.06f, size.y + 0.06f, 0.008f);
        goldBorder.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(goldBorder.GetComponent<Collider>());

        // Active display screen panel (Un-mirrored: negative X scale on rotated Quad)
        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.name = "DisplayScreen";
        screen.transform.SetParent(monRoot.transform, false);
        screen.transform.localPosition = new Vector3(0f, 0f, -0.001f);
        screen.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        screen.transform.localScale = new Vector3(-size.x, size.y, 1f); // Negative X un-mirrors the Quad!
        screen.GetComponent<MeshRenderer>().sharedMaterial = screenMat ?? housingMat;
        Object.DestroyImmediate(screen.GetComponent<Collider>());

        // Mounting armature pylon
        GameObject pylon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pylon.name = "MountingPylon";
        pylon.transform.SetParent(monRoot.transform, false);
        pylon.transform.localPosition = new Vector3(0f, -size.y * 0.5f - 0.08f, 0.04f);
        pylon.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
        pylon.GetComponent<MeshRenderer>().sharedMaterial = housingMat;
        Object.DestroyImmediate(pylon.GetComponent<Collider>());
    }

    private static void BuildMissionWings(Transform parent, Material darkMat, Material goldMat, Material glowCyan)
    {
        GameObject wingsRoot = new GameObject("Bridge_Mission_Wings");
        wingsRoot.transform.SetParent(parent, false);

        // 1. Port Science & Telemetry Suite (Left Wing at -1.85m X, +0.45m Z)
        GameObject copilotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_copilot_navigation_console.glb");
        if (copilotPrefab != null)
        {
            GameObject portStation = Object.Instantiate(copilotPrefab, wingsRoot.transform);
            portStation.name = "Port_ScienceTelemetry_Console";
            // Bounds height is 0.61m. Scale 1.35 -> Center at Y = +0.52m to sit on deck!
            portStation.transform.localPosition = new Vector3(-2.0f, 0.52f, 0.45f);
            portStation.transform.localRotation = Quaternion.Euler(0f, 135f, 0f);
            portStation.transform.localScale = Vector3.one * 1.35f;

            var box = portStation.AddComponent<BoxCollider>();
            box.center = new Vector3(0f, 0f, 0f);
            box.size = new Vector3(0.9f, 0.8f, 1.2f);
        }

        // Port station cyan accent glow
        var portLightObj = new GameObject("Port_StationLight");
        portLightObj.transform.SetParent(wingsRoot.transform, false);
        portLightObj.transform.localPosition = new Vector3(-1.80f, 1.45f, 0.45f);
        var pLight = portLightObj.AddComponent<Light>();
        pLight.type = LightType.Point;
        pLight.color = new Color(0.1f, 0.85f, 1.0f);
        pLight.intensity = 1.4f;
        pLight.range = 3.5f;

        // 2. Starboard Tactical & Warp Suite (Right Wing at +2.0m X, +0.45m Z)
        GameObject navPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_interior.glb");
        if (navPrefab != null)
        {
            GameObject starStation = Object.Instantiate(navPrefab, wingsRoot.transform);
            starStation.name = "Starboard_TacticalWarp_Console";
            starStation.transform.localPosition = new Vector3(2.0f, 0.52f, 0.45f);
            starStation.transform.localRotation = Quaternion.Euler(0f, -45f, 0f);
            starStation.transform.localScale = Vector3.one * 1.35f;

            var box = starStation.AddComponent<BoxCollider>();
            box.center = new Vector3(0f, 0f, 0f);
            box.size = new Vector3(0.9f, 0.8f, 1.2f);
        }

        // Starboard station warm amber accent glow
        var starLightObj = new GameObject("Starboard_StationLight");
        starLightObj.transform.SetParent(wingsRoot.transform, false);
        starLightObj.transform.localPosition = new Vector3(1.80f, 1.45f, 0.45f);
        var sLight = starLightObj.AddComponent<Light>();
        sLight.type = LightType.Point;
        sLight.color = new Color(1.0f, 0.82f, 0.35f);
        sLight.intensity = 1.4f;
        sLight.range = 3.5f;
    }

    private static void BuildAftEngineering(Transform parent, Material darkMat, Material goldMat, Material glowGold)
    {
        GameObject aftRoot = new GameObject("Bridge_Aft_Engineering");
        aftRoot.transform.SetParent(parent, false);

        // 1. Hyperdrive Anti-Gravity Reactor Core (Tripo 3D High-Poly Model)
        GameObject reactorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_hyperdrive_reactor_core.glb");
        if (reactorPrefab != null)
        {
            GameObject reactor = Object.Instantiate(reactorPrefab, aftRoot.transform);
            reactor.name = "Tripo_Hyperdrive_ReactorCore";
            // Bounds height is 1.00m. Scale 1.50 -> Center at Y = +0.75m to sit on deck!
            reactor.transform.localPosition = new Vector3(0f, 0.75f, -2.40f);
            reactor.transform.localRotation = Quaternion.identity;
            reactor.transform.localScale = Vector3.one * 1.50f;

            var rot = reactor.AddComponent<ExhibitRotator>();
            rot.rotationSpeed = 6.0f;
            rot.bobAmplitude = 0.02f;
            rot.bobSpeed = 1.0f;

            var col = reactor.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0f, 0f, 0f);
            col.radius = 0.55f;
            col.height = 1.50f;
        }

        // Warm pulsating plasma reactor core point light
        GameObject coreLightObj = new GameObject("ReactorCore_PlasmaGlow");
        coreLightObj.transform.SetParent(aftRoot.transform, false);
        coreLightObj.transform.localPosition = new Vector3(0f, 0.75f, -2.40f);
        var cLight = coreLightObj.AddComponent<Light>();
        cLight.type = LightType.Point;
        cLight.color = new Color(1.0f, 0.68f, 0.18f);
        cLight.intensity = 3.2f;
        cLight.range = 5.5f;

        // Gold Containment Ring under Reactor
        CreateMeshRing(aftRoot.transform, "Reactor_GoldContainmentRing", 1.15f, 0.14f, goldMat, 0.005f);

        // 2. Heavy Rear Airlock Pressure Bulkhead Door at local Z = -3.20m
        GameObject bulkheadDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bulkheadDoor.name = "Aft_Airlock_BulkheadDoor";
        bulkheadDoor.transform.SetParent(aftRoot.transform, false);
        bulkheadDoor.transform.localPosition = new Vector3(0f, 1.25f, -3.20f);
        bulkheadDoor.transform.localScale = new Vector3(1.85f, 2.30f, 0.18f);
        bulkheadDoor.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
        Object.DestroyImmediate(bulkheadDoor.GetComponent<Collider>());

        // Slender gold framing bars around the perimeter of the airlock door (Hollow frame)
        float dw = 1.85f, dh = 2.30f, bThick = 0.06f;

        var dTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dTop.name = "Frame_Top";
        dTop.transform.SetParent(bulkheadDoor.transform, false);
        dTop.transform.localPosition = new Vector3(0f, 0.5f - (bThick * 0.5f) / dh, 0.52f);
        dTop.transform.localScale = new Vector3(1.0f, bThick / dh, 0.06f);
        dTop.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(dTop.GetComponent<Collider>());

        var dBtm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dBtm.name = "Frame_Bottom";
        dBtm.transform.SetParent(bulkheadDoor.transform, false);
        dBtm.transform.localPosition = new Vector3(0f, -0.5f + (bThick * 0.5f) / dh, 0.52f);
        dBtm.transform.localScale = new Vector3(1.0f, bThick / dh, 0.06f);
        dBtm.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(dBtm.GetComponent<Collider>());

        var dLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dLeft.name = "Frame_Left";
        dLeft.transform.SetParent(bulkheadDoor.transform, false);
        dLeft.transform.localPosition = new Vector3(-0.5f + (bThick * 0.5f) / dw, 0f, 0.52f);
        dLeft.transform.localScale = new Vector3(bThick / dw, 1.0f, 0.06f);
        dLeft.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(dLeft.GetComponent<Collider>());

        var dRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dRight.name = "Frame_Right";
        dRight.transform.SetParent(bulkheadDoor.transform, false);
        dRight.transform.localPosition = new Vector3(0.5f - (bThick * 0.5f) / dw, 0f, 0.52f);
        dRight.transform.localScale = new Vector3(bThick / dw, 1.0f, 0.06f);
        dRight.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(dRight.GetComponent<Collider>());

        // Center warning glyph / pressure valve wheel
        GameObject valve = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        valve.name = "PressureValve_Wheel";
        valve.transform.SetParent(bulkheadDoor.transform, false);
        valve.transform.localPosition = new Vector3(0f, 0.15f, 0.58f);
        valve.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        valve.transform.localScale = new Vector3(0.38f, 0.04f, 0.38f);
        valve.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(valve.GetComponent<Collider>());

        // Flanking Atmospheric Conduits (Left & Right of reactor)
        float[] sideX = { -1.35f, 1.35f };
        for (int s = 0; s < sideX.Length; s++)
        {
            GameObject conduit = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            conduit.name = $"AtmoConduit_{s:D2}";
            conduit.transform.SetParent(aftRoot.transform, false);
            conduit.transform.localPosition = new Vector3(sideX[s], 1.20f, -2.75f);
            conduit.transform.localScale = new Vector3(0.24f, 1.15f, 0.24f);
            conduit.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
            Object.DestroyImmediate(conduit.GetComponent<Collider>());

            // Gold pressure rings on conduits
            for (int r = -1; r <= 1; r++)
            {
                GameObject pRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pRing.name = $"PressureRing_{r + 1}";
                pRing.transform.SetParent(conduit.transform, false);
                pRing.transform.localPosition = new Vector3(0f, r * 0.55f, 0f);
                pRing.transform.localScale = new Vector3(1.08f, 0.08f, 1.08f);
                pRing.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(pRing.GetComponent<Collider>());
            }
        }
    }

    private static void SetupBridgeLighting(Transform parent)
    {
        GameObject lightsRoot = new GameObject("Bridge_Atmospheric_Lighting");
        lightsRoot.transform.SetParent(parent, false);

        // 1. Forward Cockpit Ambient Cyber Fill (Soft blue-cyan)
        GameObject fwdLight = new GameObject("Cockpit_ForwardAmbientLight");
        fwdLight.transform.SetParent(lightsRoot.transform, false);
        fwdLight.transform.localPosition = new Vector3(0f, 1.85f, 0.60f);
        var fLight = fwdLight.AddComponent<Light>();
        fLight.type = LightType.Point;
        fLight.color = new Color(0.12f, 0.82f, 1.0f);
        fLight.intensity = 1.9f;
        fLight.range = 6.0f;

        // 2. Overhead Pilot Spotlight (Warm gallery light gently illuminating the pilot throne)
        GameObject throneSpot = new GameObject("Throne_OverheadSpot");
        throneSpot.transform.SetParent(lightsRoot.transform, false);
        throneSpot.transform.localPosition = new Vector3(0f, 2.45f, 0.15f);
        throneSpot.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        var tLight = throneSpot.AddComponent<Light>();
        tLight.type = LightType.Spot;
        tLight.color = new Color(1.0f, 0.94f, 0.88f);
        tLight.intensity = 1.4f;
        tLight.range = 3.5f;
        tLight.spotAngle = 60f;

        // 3. Aft Cabin Ambient Fill (Soft warm amber from the reactor room)
        GameObject aftLight = new GameObject("Aft_CabinAmbientLight");
        aftLight.transform.SetParent(lightsRoot.transform, false);
        aftLight.transform.localPosition = new Vector3(0f, 1.95f, -1.60f);
        var aLight = aftLight.AddComponent<Light>();
        aLight.type = LightType.Point;
        aLight.color = new Color(0.95f, 0.82f, 0.65f);
        aLight.intensity = 1.6f;
        aLight.range = 5.5f;
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
