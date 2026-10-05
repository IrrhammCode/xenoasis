using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// XENOASIS — GrandMuseumArchitect.cs
/// Complete Master Redesign of the Welcome Chamber Grand Museum Pavilion:
/// 1. Floor Architecture:
///    - Mirror-finish deep obsidian & carbon terrazzo floor (URP Lit, Metallic 0.70, Smoothness 0.94)
///    - Floor top surface precisely at Y = 0.000m
///    - Dedicated Floor Inlays at absolute world Y (zero z-fighting, raised 8-15mm)
///    - 3 Concentric Luxury Gold Inlay Rings (Inner R=5.4m, Mid R=11.5m, Outer R=17.5m)
///    - 3 Concentric Cyan Stardust Guide Rings (Inner R=5.2m, Mid R=11.2m, Outer R=17.2m)
///    - 16 Radial Illuminated Guide Tracks leading from central dais to the 16 wall bays
///    - Elevated Landing Dais Plinth in brushed dark titanium with gold perimeter rim & step glow
/// 2. Colonnade & Column Architecture:
///    - 16 Monumental Columns using Tripo AI high-poly model (museum_sci_fi_column.glb)
///    - Plinth bases with gold rings, capitals, and exterior reinforcing buttress pylons with gold spine fins
///    - Columns located at (k + 0.5) * 22.5° to perfectly frame the 16 bays
/// 3. Wall Architecture & 16 Perimeter Bays:
///    - Bay 08 (South, Z = -18.5m, X = 0.0m): Monumental Entrance Portal with Tripo Curatorial Portal
///      (museum_curatorial_portal.glb) sitting firmly at ground level Y = 0.0m to 6.4m
///    - Bays 00, 04, 12 (North, East, West): Panoramic Observation Window Bays with floor-to-ceiling glass,
///      gold balustrade lips, cyan glow, and Tripo luxury lounge benches (museum_luxury_lounge_bench.glb)
///    - 12 Solid Acoustic Gallery Bays with recessed curatorial display alcoves, gold framing, and 3200K spots
///    - 6 Hero Tripo Vitrines (museum_artifact_vitrine.glb) housing illuminated prehistoric & futuristic relics
///    - 6 Hero Tripo Pedestals (museum_pedestal_display_v3.glb) displaying curated humanity artifacts
///      (Rosetta Stone, Apollo 11 Plaque, Voyager Golden Record, Prometheus Flame, Svalbard Seed Vault, Ambassador Statue)
/// 4. Roof & Ceiling Architecture:
///    - 16 Arched Structural Vault Ribs spanning from column capitals to central collar with gold conduits
///    - Multi-tiered coffered ceiling soffits with indirect warm ambient cove uplighting
///    - Central titanium iris aperture collar with gold inner bezel & cyan beacon ring
///    - Crowning crystal glass skylight dome
///    - Hero Central Tripo Chandelier (museum_ceiling_chandelier.glb) hanging below the dome with warm crystalline lighting
/// </summary>
public static class GrandMuseumArchitect
{
    [MenuItem("XENOASIS/Redesign Luxury Museum Pavilion")]
    public static void RedesignMuseum()
    {
        Debug.Log("[GrandMuseumArchitect] ✦ Initiating complete luxury architectural redesign of Welcome Chamber with Tripo AI assets...");

        GameObject chamberRoot = GameObject.Find("--- XENOASIS ---/[Environment]/TerranLivingArchive");
        if (chamberRoot == null)
        {
            Debug.LogError("[GrandMuseumArchitect] Could not find TerranLivingArchive in active scene!");
            return;
        }

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) urpLit = Shader.Find("Standard");

        // =========================================================================
        // 1. PBR MATERIALS SETUP (Deep Obsidian, Brushed Titanium, Luxury Gold, Cyan Glow)
        // =========================================================================
        // A. Mirror-Finish Obsidian Luxury Floor
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_LuxuryFloor.mat");
        if (floorMat == null)
        {
            floorMat = new Material(urpLit);
            AssetDatabase.CreateAsset(floorMat, "Assets/Materials/Museum_LuxuryFloor.mat");
        }
        floorMat.shader = urpLit;
        floorMat.SetColor("_BaseColor", new Color(0.045f, 0.055f, 0.075f, 1.0f)); // Deep cosmic obsidian
        floorMat.SetFloat("_Metallic", 0.70f);
        floorMat.SetFloat("_Smoothness", 0.94f); // Glossy, mirror-like reflections
        floorMat.SetFloat("_Cull", 0f);
        floorMat.doubleSidedGI = true;
        EditorUtility.SetDirty(floorMat);

        // B. Dark Titanium Dais & Plinth
        Material daisMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_DaisPlinth.mat");
        if (daisMat == null)
        {
            daisMat = new Material(urpLit);
            AssetDatabase.CreateAsset(daisMat, "Assets/Materials/Museum_DaisPlinth.mat");
        }
        daisMat.shader = urpLit;
        daisMat.SetColor("_BaseColor", new Color(0.11f, 0.12f, 0.15f, 1.0f));
        daisMat.SetFloat("_Metallic", 0.85f);
        daisMat.SetFloat("_Smoothness", 0.88f);
        EditorUtility.SetDirty(daisMat);

        // C. Solid Dark Titanium Wall Bulkhead
        Material wallDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_Dark.mat");
        if (wallDark == null)
        {
            wallDark = new Material(urpLit);
            AssetDatabase.CreateAsset(wallDark, "Assets/Materials/SciFiWall_Dark.mat");
        }
        wallDark.shader = urpLit;
        wallDark.SetColor("_BaseColor", new Color(0.07f, 0.08f, 0.10f, 1.0f));
        wallDark.SetFloat("_Metallic", 0.75f);
        wallDark.SetFloat("_Smoothness", 0.85f);
        wallDark.SetFloat("_Cull", 0f);
        wallDark.doubleSidedGI = true;
        EditorUtility.SetDirty(wallDark);

        // D. Alabaster Pearl Wall Accent
        Material wallWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_White.mat");
        if (wallWhite == null)
        {
            wallWhite = new Material(urpLit);
            AssetDatabase.CreateAsset(wallWhite, "Assets/Materials/SciFiWall_White.mat");
        }
        wallWhite.shader = urpLit;
        wallWhite.SetColor("_BaseColor", new Color(0.82f, 0.84f, 0.88f, 1.0f));
        wallWhite.SetFloat("_Metallic", 0.35f);
        wallWhite.SetFloat("_Smoothness", 0.80f);
        EditorUtility.SetDirty(wallWhite);

        // E. Luxury Gold Inlay
        Material goldMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LuxuryGold_Inlay.mat");
        if (goldMat == null)
        {
            goldMat = new Material(urpLit);
            AssetDatabase.CreateAsset(goldMat, "Assets/Materials/LuxuryGold_Inlay.mat");
        }
        goldMat.shader = urpLit;
        goldMat.SetColor("_BaseColor", new Color(0.94f, 0.78f, 0.35f, 1.0f));
        goldMat.SetFloat("_Metallic", 0.95f);
        goldMat.SetFloat("_Smoothness", 0.90f);
        EditorUtility.SetDirty(goldMat);

        // F. Emissive Cyan Conduit
        Material glowCyan = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        if (glowCyan == null)
        {
            glowCyan = new Material(urpLit);
            AssetDatabase.CreateAsset(glowCyan, "Assets/Materials/EmissiveCyan.mat");
        }
        glowCyan.shader = urpLit;
        glowCyan.SetColor("_BaseColor", new Color(0f, 0.88f, 1.0f, 1.0f));
        glowCyan.EnableKeyword("_EMISSION");
        glowCyan.SetColor("_EmissionColor", new Color(0f, 0.88f, 1.0f) * 2.6f);
        EditorUtility.SetDirty(glowCyan);

        // G. Emissive Gold Beacon
        Material glowGold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");
        if (glowGold == null)
        {
            glowGold = new Material(urpLit);
            AssetDatabase.CreateAsset(glowGold, "Assets/Materials/EmissiveGold.mat");
        }
        glowGold.shader = urpLit;
        glowGold.SetColor("_BaseColor", new Color(1.0f, 0.78f, 0.25f, 1.0f));
        glowGold.EnableKeyword("_EMISSION");
        glowGold.SetColor("_EmissionColor", new Color(1.0f, 0.78f, 0.25f) * 2.8f);
        EditorUtility.SetDirty(glowGold);

        // H. Glass Viewport
        Material glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        if (glassMat != null)
        {
            glassMat.SetFloat("_GridEmission", 0f);
            glassMat.SetFloat("_GridLineWidth", 0f);
            EditorUtility.SetDirty(glassMat);
        }

        // =========================================================================
        // 2. ROOT ARCHITECTURE CONTAINER SETUP
        // =========================================================================
        Transform oldPavilion = chamberRoot.transform.Find("GrandMuseumPavilion_Architecture");
        if (oldPavilion != null) Object.DestroyImmediate(oldPavilion.gameObject);

        Transform oldFloor = chamberRoot.transform.Find("Museum_Floor_Foundation");
        if (oldFloor != null) Object.DestroyImmediate(oldFloor.gameObject);

        Transform oldDais = chamberRoot.transform.Find("Landing_Dais_Plinth");
        if (oldDais != null) Object.DestroyImmediate(oldDais.gameObject);

        GameObject pavilion = new GameObject("GrandMuseumPavilion_Architecture");
        pavilion.transform.SetParent(chamberRoot.transform, false);
        pavilion.transform.localPosition = Vector3.zero;
        pavilion.transform.localRotation = Quaternion.identity;

        float rotundaRadius = 18.5f;
        float wallHeight = 8.0f;
        float foundationRadius = 22.5f;
        float apertureRadius = 5.5f;

        // =========================================================================
        // 3. FLOOR & FOUNDATION ARCHITECTURE
        // =========================================================================
        BuildFloorAndFoundation(chamberRoot.transform, pavilion.transform, foundationRadius, rotundaRadius,
                                daisMat, floorMat, goldMat, glowCyan);

        // =========================================================================
        // 4. COLONNADE & WALL ARCHITECTURE (16 BAYS) WITH TRIPO COLUMNS, PORTAL & VITRINES
        // =========================================================================
        BuildColonnadeAndWalls(pavilion.transform, rotundaRadius, wallHeight,
                              wallDark, goldMat, glowCyan, glassMat);

        // =========================================================================
        // 5. ROOF VAULT RIBS & CEILING SOFFIT WITH TRIPO CHANDELIER
        // =========================================================================
        BuildRoofAndCeiling(pavilion.transform, rotundaRadius, wallHeight, apertureRadius,
                           wallDark, goldMat, glowCyan, glassMat);

        // =========================================================================
        // 6. FURNISHINGS & CURATORIAL LIGHTING
        // =========================================================================
        SetupMuseumFurnishingsAndLighting(chamberRoot.transform, pavilion.transform, rotundaRadius, wallDark, goldMat);

        // Mark scene dirty and save
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(chamberRoot.scene);
        Debug.Log("[GrandMuseumArchitect] ✦ Grand Museum Pavilion Redesign with Tripo AI Assets 100% COMPLETE!");
    }

    private static void BuildFloorAndFoundation(Transform chamberRoot, Transform pavilion, float foundRadius, float floorRadius,
                                                Material daisMat, Material floorMat, Material goldMat, Material glowCyan)
    {
        // 1. Tiered Foundation Base (Anchors building to lake plinth)
        GameObject foundation = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        foundation.name = "Pavilion_TieredFoundation";
        foundation.transform.SetParent(pavilion, false);
        foundation.transform.localPosition = new Vector3(0f, -0.40f, 0f);
        foundation.transform.localScale = new Vector3(foundRadius * 2f, 0.40f, foundRadius * 2f);
        foundation.GetComponent<MeshRenderer>().sharedMaterial = daisMat;

        // Gold Trim on Foundation Outer Lip
        CreateMeshRing(foundation.transform, "Foundation_GoldRim", 0.99f, 0.02f, goldMat, 0.51f);
        CreateMeshRing(foundation.transform, "Foundation_CyanBeacon", 0.97f, 0.015f, glowCyan, 0.515f);

        // 2. Primary Obsidian Mirror Floor Slab (37m diameter)
        // Cylinder default height is 2.0. Scale Y = 0.05 gives height 0.10m (half-height 0.05m).
        // Position at Y = -0.05m means top surface is EXACTLY at Y = 0.000m!
        GameObject floorObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floorObj.name = "Museum_Floor_Foundation";
        floorObj.transform.SetParent(chamberRoot, false);
        floorObj.transform.localPosition = new Vector3(0f, -0.05f, 0f);
        floorObj.transform.localScale = new Vector3(floorRadius * 2.02f, 0.05f, floorRadius * 2.02f);
        floorObj.GetComponent<MeshRenderer>().sharedMaterial = floorMat;

        // 3. Dedicated Floor Inlay Group (Parented to pavilion with scale (1,1,1) for ZERO z-fighting)
        GameObject inlaysGroup = new GameObject("Floor_LuxuryInlays");
        inlaysGroup.transform.SetParent(pavilion, false);
        inlaysGroup.transform.localPosition = Vector3.zero;

        // Concentric Luxury Gold Inlay Rings (Crisp, raised by 8mm - 12mm above floor)
        CreateMeshRing(inlaysGroup.transform, "Floor_Inlay_OuterRing", 17.5f, 0.16f, goldMat, 0.008f);
        CreateMeshRing(inlaysGroup.transform, "Floor_Inlay_MidRing", 11.5f, 0.14f, goldMat, 0.010f);
        CreateMeshRing(inlaysGroup.transform, "Floor_Inlay_InnerRing", 5.4f, 0.12f, goldMat, 0.012f);

        // Concentric Cyan Stardust Guide Rings (Raised 13mm - 15mm)
        CreateMeshRing(inlaysGroup.transform, "Floor_GlowRing_Outer", 17.2f, 0.04f, glowCyan, 0.013f);
        CreateMeshRing(inlaysGroup.transform, "Floor_GlowRing_Mid", 11.2f, 0.035f, glowCyan, 0.014f);
        CreateMeshRing(inlaysGroup.transform, "Floor_GlowRing_Inner", 5.2f, 0.03f, glowCyan, 0.015f);

        // 4. Elevated Central Landing Dais Plinth (Y=0.0m to 0.24m, Diameter 10.5m)
        GameObject daisObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisObj.name = "Landing_Dais_Plinth";
        daisObj.transform.SetParent(chamberRoot, false);
        daisObj.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        daisObj.transform.localScale = new Vector3(10.5f, 0.24f, 10.5f);
        daisObj.GetComponent<MeshRenderer>().sharedMaterial = daisMat;

        // Dais Upper Rim Bevel
        CreateMeshRing(daisObj.transform, "Dais_GoldBevelRim", 0.98f, 0.04f, goldMat, 0.51f);
        CreateMeshRing(daisObj.transform, "Dais_StepGlowRim", 0.95f, 0.025f, glowCyan, 0.515f);

        // Center Dais Core Inlay Disc (Inner sanctum where the Quantum Gyro stands)
        GameObject daisCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisCore.name = "Dais_ObsidianCore";
        daisCore.transform.SetParent(daisObj.transform, false);
        daisCore.transform.localPosition = new Vector3(0f, 0.52f, 0f);
        daisCore.transform.localScale = new Vector3(0.68f, 0.04f, 0.68f);
        daisCore.GetComponent<MeshRenderer>().sharedMaterial = floorMat;
        Object.DestroyImmediate(daisCore.GetComponent<Collider>());
        CreateMeshRing(daisCore.transform, "Dais_CoreGoldRing", 0.98f, 0.04f, goldMat, 0.52f);

        // 5. 16 Radial Illuminated Guide Tracks leading from central dais to perimeter bays
        GameObject tracksRoot = new GameObject("Floor_TerranRadialTracks");
        tracksRoot.transform.SetParent(inlaysGroup.transform, false);
        tracksRoot.transform.localPosition = Vector3.zero;

        int totalTracks = 16;
        float trackLen = 12.0f; // from R=5.4 to R=17.4
        float trackCenterR = (5.4f + 17.4f) * 0.5f;

        for (int t = 0; t < totalTracks; t++)
        {
            float deg = t * (360f / totalTracks);
            float rad = deg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * trackCenterR, 0.008f, Mathf.Cos(rad) * trackCenterR);
            Quaternion rot = Quaternion.Euler(0f, deg, 0f);

            GameObject track = GameObject.CreatePrimitive(PrimitiveType.Cube);
            track.name = $"RadialTrack_{t:D2}";
            track.transform.SetParent(tracksRoot.transform, false);
            track.transform.localPosition = pos;
            track.transform.localRotation = rot;
            track.transform.localScale = new Vector3(0.16f, 0.006f, trackLen);
            track.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(track.GetComponent<Collider>());

            // Center cyan conduit
            GameObject conduit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            conduit.name = "Conduit";
            conduit.transform.SetParent(track.transform, false);
            conduit.transform.localPosition = new Vector3(0f, 0.60f, 0f);
            conduit.transform.localScale = new Vector3(0.40f, 0.50f, 0.98f);
            conduit.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
            Object.DestroyImmediate(conduit.GetComponent<Collider>());
        }
    }

    private static void BuildColonnadeAndWalls(Transform parent, float radius, float wallHeight,
                                               Material wallDark, Material goldMat, Material glowCyan, Material glassMat)
    {
        int segments = 16;
        float stepDeg = 360f / segments; // 22.5 deg per bay
        float chordWidth = 2f * radius * Mathf.Sin((stepDeg * 0.5f) * Mathf.Deg2Rad) * 1.02f;

        GameObject colonnade = new GameObject("Pavilion_Colonnade");
        colonnade.transform.SetParent(parent, false);

        // Load Hero Tripo AI Models
        GameObject columnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_sci_fi_column.glb");
        GameObject portalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_curatorial_portal.glb");
        GameObject vitrinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_artifact_vitrine.glb");
        GameObject pedestalV3Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_pedestal_display_v3.glb");

        // Curated Artifact Prefabs for Alcoves
        string[] alcoveArtifacts = new string[] {
            "Assets/Models/Tripo/museum_rosetta_stone.glb",
            "Assets/Models/Tripo/museum_apollo11_moon_plaque.glb",
            "Assets/Models/Tripo/museum_voyager_golden_record.glb",
            "Assets/Models/Tripo/museum_prometheus_fire_and_chip.glb",
            "Assets/Models/Tripo/museum_svalbard_seed_vault.glb",
            "Assets/Models/Tripo/museum_human_ambassador_statue.glb"
        };
        int artifactIdx = 0;

        // Vitrine Relic Prefabs
        string[] vitrineRelics = new string[] {
            "Assets/Models/Tripo/primordial_prebiotic_crystal_spire.glb",
            "Assets/Models/Tripo/water_lotus.glb",
            "Assets/Models/Tripo/resonant_crystal.glb",
            "Assets/Models/Tripo/humanity_dna_genome_crystal_spire.glb",
            "Assets/Models/Tripo/primordial_bioluminescent_lotus.glb",
            "Assets/Models/Tripo/glass_chime.glb"
        };
        int vitrineIdx = 0;

        for (int i = 0; i < segments; i++)
        {
            // Bays are centered at i * stepDeg (0 deg = North, 90 deg = East, 180 deg = South, 270 deg = West)
            float bayDeg = i * stepDeg;
            float bayRad = bayDeg * Mathf.Deg2Rad;
            Vector3 bayPos = new Vector3(Mathf.Sin(bayRad) * radius, wallHeight * 0.5f, Mathf.Cos(bayRad) * radius);
            Quaternion bayRot = Quaternion.Euler(0f, bayDeg, 0f);

            // Columns are located at the edges between bays: (i + 0.5) * stepDeg
            float colDeg = (i + 0.5f) * stepDeg;
            float colRad = colDeg * Mathf.Deg2Rad;
            Vector3 colPos = new Vector3(Mathf.Sin(colRad) * radius, wallHeight * 0.5f, Mathf.Cos(colRad) * radius);
            Quaternion colRot = Quaternion.Euler(0f, colDeg + 180f, 0f);

            // =========================================================================
            // A. MONUMENTAL ARCHITECTURAL COLUMN (Tripo museum_sci_fi_column.glb)
            // =========================================================================
            GameObject colRoot = new GameObject($"Column_{i:D2}");
            colRoot.transform.SetParent(colonnade.transform, false);
            colRoot.transform.localPosition = colPos;
            colRoot.transform.localRotation = colRot;

            // Column Plinth Base (Octagonal / Tiered Cylinder)
            GameObject colBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            colBase.name = "PlinthBase";
            colBase.transform.SetParent(colRoot.transform, false);
            colBase.transform.localPosition = new Vector3(0f, -wallHeight * 0.46f, 0f);
            colBase.transform.localScale = new Vector3(1.35f, 0.35f, 1.35f);
            colBase.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
            Object.DestroyImmediate(colBase.GetComponent<Collider>());
            CreateMeshRing(colBase.transform, "BaseGoldRing", 0.98f, 0.05f, goldMat, 0.52f);

            // Column Shaft: Use Hero Tripo Sci-Fi Column!
            if (columnPrefab != null)
            {
                GameObject pillarInst = Object.Instantiate(columnPrefab, colRoot.transform);
                pillarInst.name = "Tripo_SciFiColumn";
                pillarInst.transform.localPosition = new Vector3(0f, 0f, 0f);
                pillarInst.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                pillarInst.transform.localScale = new Vector3(3.2f, 7.8f, 3.2f);
            }
            else
            {
                GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                shaft.name = "Shaft";
                shaft.transform.SetParent(colRoot.transform, false);
                shaft.transform.localPosition = Vector3.zero;
                shaft.transform.localScale = new Vector3(0.95f, wallHeight * 0.42f, 0.95f);
                shaft.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(shaft.GetComponent<Collider>());
            }

            // Column Capital (Top)
            GameObject capital = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            capital.name = "Capital";
            capital.transform.SetParent(colRoot.transform, false);
            capital.transform.localPosition = new Vector3(0f, wallHeight * 0.46f, 0f);
            capital.transform.localScale = new Vector3(1.45f, 0.35f, 1.45f);
            capital.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
            Object.DestroyImmediate(capital.GetComponent<Collider>());
            CreateMeshRing(capital.transform, "CapitalGoldRing", 0.98f, 0.06f, goldMat, -0.52f);

            // Exterior Reinforcing Buttress Pylon (Extends outward to ground)
            GameObject buttress = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttress.name = "ExteriorButtress";
            buttress.transform.SetParent(colRoot.transform, false);
            buttress.transform.localPosition = new Vector3(0f, -wallHeight * 0.12f, 1.25f);
            buttress.transform.localScale = new Vector3(0.65f, wallHeight * 0.75f, 1.55f);
            buttress.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
            Object.DestroyImmediate(buttress.GetComponent<Collider>());

            // Gold Fin on Buttress Spine
            GameObject bFin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bFin.name = "ButtressGoldSpine";
            bFin.transform.SetParent(buttress.transform, false);
            bFin.transform.localPosition = new Vector3(0f, 0f, 0.52f);
            bFin.transform.localScale = new Vector3(0.08f, 0.98f, 0.05f);
            bFin.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(bFin.GetComponent<Collider>());

            // =========================================================================
            // B. BAY ENCLOSURE
            // =========================================================================
            bool isSouthEntrance = (i == 8); // Exact South (180 deg, X = 0, Z = -18.5m)
            bool isObservationWindow = (i == 0 || i == 4 || i == 12); // North, East, West

            if (isSouthEntrance && portalPrefab != null)
            {
                // Grand Curatorial Entrance Portal Bay (Framing the South Arrival Promenade)
                GameObject portalBay = new GameObject($"GrandCuratorialPortalBay_{i:D2}");
                portalBay.transform.SetParent(colonnade.transform, false);
                portalBay.transform.localPosition = bayPos;
                portalBay.transform.localRotation = bayRot;

                // Portal Frame Model from Tripo sitting firmly at floor level (Y = 0.0m to 6.4m)
                GameObject portalInst = Object.Instantiate(portalPrefab, portalBay.transform);
                portalInst.name = "Tripo_CuratorialPortal";
                portalInst.transform.localPosition = new Vector3(0f, -wallHeight * 0.5f + 3.2f, 0.15f);
                portalInst.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                portalInst.transform.localScale = new Vector3(2.2f, 6.4f, 6.2f);

                // Flanking Door Jamb Bulkheads (Left & Right to seal the wall between columns)
                float sideWidth = (chordWidth - 5.8f) * 0.5f;
                if (sideWidth > 0.1f)
                {
                    var jambLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    jambLeft.name = "PortalJamb_Left";
                    jambLeft.transform.SetParent(portalBay.transform, false);
                    jambLeft.transform.localPosition = new Vector3(-chordWidth * 0.5f + sideWidth * 0.5f, 0f, 0f);
                    jambLeft.transform.localScale = new Vector3(sideWidth, wallHeight, 0.40f);
                    jambLeft.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                    Object.DestroyImmediate(jambLeft.GetComponent<Collider>());

                    var jambRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    jambRight.name = "PortalJamb_Right";
                    jambRight.transform.SetParent(portalBay.transform, false);
                    jambRight.transform.localPosition = new Vector3(chordWidth * 0.5f - sideWidth * 0.5f, 0f, 0f);
                    jambRight.transform.localScale = new Vector3(sideWidth, wallHeight, 0.40f);
                    jambRight.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                    Object.DestroyImmediate(jambRight.GetComponent<Collider>());
                }

                // Upper Lintel Header above the portal arch (Y = 6.4m to 8.0m)
                var pHeader = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pHeader.name = "PortalLintelHeader";
                pHeader.transform.SetParent(portalBay.transform, false);
                pHeader.transform.localPosition = new Vector3(0f, wallHeight * 0.5f - 0.8f, 0f);
                pHeader.transform.localScale = new Vector3(chordWidth * 0.98f, 1.6f, 0.55f);
                pHeader.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(pHeader.GetComponent<Collider>());

                var pHeaderLip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pHeaderLip.name = "HeaderGoldLip";
                pHeaderLip.transform.SetParent(pHeader.transform, false);
                pHeaderLip.transform.localPosition = new Vector3(0f, -0.51f, 0f);
                pHeaderLip.transform.localScale = new Vector3(1.0f, 0.08f, 0.22f);
                pHeaderLip.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(pHeaderLip.GetComponent<Collider>());

                // Upper Arch Welcome Spotlights
                GameObject pLightL = new GameObject("PortalSpotlight_L");
                pLightL.transform.SetParent(portalBay.transform, false);
                pLightL.transform.localPosition = new Vector3(-2.2f, wallHeight * 0.5f - 1.2f, -0.6f);
                var pl1 = pLightL.AddComponent<Light>();
                pl1.type = LightType.Spot;
                pl1.color = new Color(1.0f, 0.92f, 0.78f);
                pl1.intensity = 2.4f;
                pl1.range = 10.0f;
                pl1.spotAngle = 60f;

                GameObject pLightR = new GameObject("PortalSpotlight_R");
                pLightR.transform.SetParent(portalBay.transform, false);
                pLightR.transform.localPosition = new Vector3(2.2f, wallHeight * 0.5f - 1.2f, -0.6f);
                var pl2 = pLightR.AddComponent<Light>();
                pl2.type = LightType.Spot;
                pl2.color = new Color(1.0f, 0.92f, 0.78f);
                pl2.intensity = 2.4f;
                pl2.range = 10.0f;
                pl2.spotAngle = 60f;
            }
            else if (isObservationWindow)
            {
                // Panoramic Observation Window Bay
                GameObject winBay = new GameObject($"PanoramicWindowBay_{i:D2}");
                winBay.transform.SetParent(colonnade.transform, false);
                winBay.transform.localPosition = bayPos;
                winBay.transform.localRotation = bayRot;

                // 1. Lower Balustrade Sill (Y = 0 to 0.75m)
                GameObject sill = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sill.name = "WindowSill";
                sill.transform.SetParent(winBay.transform, false);
                sill.transform.localPosition = new Vector3(0f, -wallHeight * 0.5f + 0.40f, 0f);
                sill.transform.localScale = new Vector3(chordWidth * 0.98f, 0.80f, 0.65f);
                sill.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(sill.GetComponent<Collider>());

                // Gold Balustrade Lip
                GameObject sillLip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sillLip.name = "SillGoldLip";
                sillLip.transform.SetParent(sill.transform, false);
                sillLip.transform.localPosition = new Vector3(0f, 0.51f, 0f);
                sillLip.transform.localScale = new Vector3(1.0f, 0.06f, 0.22f);
                sillLip.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(sillLip.GetComponent<Collider>());

                // Cyan Balustrade Glow
                GameObject sillGlow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sillGlow.name = "SillCyanGlow";
                sillGlow.transform.SetParent(sill.transform, false);
                sillGlow.transform.localPosition = new Vector3(0f, 0.52f, -0.25f);
                sillGlow.transform.localScale = new Vector3(0.98f, 0.02f, 0.04f);
                sillGlow.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
                Object.DestroyImmediate(sillGlow.GetComponent<Collider>());

                // 2. Crystal Clear Panoramic Glass Viewport Pane (Y = 0.75m to 7.2m)
                GameObject glass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                glass.name = "ViewportGlass";
                glass.transform.SetParent(winBay.transform, false);
                glass.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                glass.transform.localScale = new Vector3(chordWidth * 0.96f, wallHeight - 1.6f, 0.06f);
                glass.GetComponent<MeshRenderer>().sharedMaterial = glassMat;
                Object.DestroyImmediate(glass.GetComponent<Collider>());

                // 3. Upper Architectural Header / Lintel (Y = 7.2m to 8.0m)
                GameObject header = GameObject.CreatePrimitive(PrimitiveType.Cube);
                header.name = "WindowHeader";
                header.transform.SetParent(winBay.transform, false);
                header.transform.localPosition = new Vector3(0f, wallHeight * 0.5f - 0.40f, 0f);
                header.transform.localScale = new Vector3(chordWidth * 0.98f, 0.80f, 0.70f);
                header.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(header.GetComponent<Collider>());

                GameObject headerLip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                headerLip.name = "HeaderGoldLip";
                headerLip.transform.SetParent(header.transform, false);
                headerLip.transform.localPosition = new Vector3(0f, -0.51f, 0f);
                headerLip.transform.localScale = new Vector3(1.0f, 0.06f, 0.22f);
                headerLip.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
                Object.DestroyImmediate(headerLip.GetComponent<Collider>());

                // Window gallery downlight illuminating balustrade
                GameObject wLight = new GameObject("WindowDownlight");
                wLight.transform.SetParent(winBay.transform, false);
                wLight.transform.localPosition = new Vector3(0f, wallHeight * 0.5f - 0.9f, -0.4f);
                var wl = wLight.AddComponent<Light>();
                wl.type = LightType.Spot;
                wl.color = new Color(1.0f, 0.92f, 0.80f);
                wl.intensity = 1.6f;
                wl.range = 8.5f;
                wl.spotAngle = 65f;
            }
            else
            {
                // Solid Architectural Acoustic Gallery Bay
                GameObject wallBay = new GameObject($"GalleryWallBay_{i:D2}");
                wallBay.transform.SetParent(colonnade.transform, false);
                wallBay.transform.localPosition = bayPos;
                wallBay.transform.localRotation = bayRot;

                // 1. Primary Solid Titanium Bulkhead Wall (35cm thick)
                GameObject bulkhead = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bulkhead.name = "BulkheadSlab";
                bulkhead.transform.SetParent(wallBay.transform, false);
                bulkhead.transform.localPosition = Vector3.zero;
                bulkhead.transform.localScale = new Vector3(chordWidth * 0.98f, wallHeight, 0.35f);
                bulkhead.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(bulkhead.GetComponent<Collider>());

                // 2. Recessed Curatorial Display Alcove / Vitrine Niche (Inside face at Z = -0.16m)
                GameObject alcove = GameObject.CreatePrimitive(PrimitiveType.Cube);
                alcove.name = "DisplayAlcove_Backing";
                alcove.transform.SetParent(wallBay.transform, false);
                alcove.transform.localPosition = new Vector3(0f, 0.25f, -0.14f);
                alcove.transform.localScale = new Vector3(chordWidth * 0.72f, wallHeight * 0.65f, 0.08f);
                alcove.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
                Object.DestroyImmediate(alcove.GetComponent<Collider>());

                // Gold Framing border around alcove
                BuildAlcoveBevelFrame(alcove.transform, goldMat);

                // Horizontal Cyan Groove on wall
                GameObject groove = GameObject.CreatePrimitive(PrimitiveType.Cube);
                groove.name = "CyanGroove";
                groove.transform.SetParent(wallBay.transform, false);
                groove.transform.localPosition = new Vector3(0f, -wallHeight * 0.28f, -0.19f);
                groove.transform.localScale = new Vector3(chordWidth * 0.92f, 0.035f, 0.04f);
                groove.GetComponent<MeshRenderer>().sharedMaterial = glowCyan;
                Object.DestroyImmediate(groove.GetComponent<Collider>());

                // Upper Gallery Downlight illuminating the wall niche
                GameObject aLight = new GameObject("CuratorialDownlight");
                aLight.transform.SetParent(wallBay.transform, false);
                aLight.transform.localPosition = new Vector3(0f, wallHeight * 0.42f, -0.55f);
                var al = aLight.AddComponent<Light>();
                al.type = LightType.Spot;
                al.color = new Color(1.0f, 0.90f, 0.75f); // Warm 3200K gallery pin spot
                al.intensity = 2.2f;
                al.range = 8.0f;
                al.spotAngle = 55f;

                // 3. CURATED EXHIBIT: Alternating Vitrine vs Pedestal Displays
                bool isVitrine = (i % 2 == 1);
                if (isVitrine && vitrinePrefab != null)
                {
                    // Hero Tripo Vitrine
                    GameObject vitrineInst = Object.Instantiate(vitrinePrefab, wallBay.transform);
                    vitrineInst.name = $"Tripo_ArtifactVitrine_{i:D2}";
                    vitrineInst.transform.localPosition = new Vector3(0f, -wallHeight * 0.5f + 1.25f, -0.65f);
                    vitrineInst.transform.localRotation = Quaternion.identity;
                    vitrineInst.transform.localScale = new Vector3(2.0f, 2.4f, 2.0f);

                    // Place an internal glowing relic inside the vitrine
                    if (vitrineIdx < vitrineRelics.Length)
                    {
                        var relicPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(vitrineRelics[vitrineIdx % vitrineRelics.Length]);
                        if (relicPrefab != null)
                        {
                            GameObject rInst = Object.Instantiate(relicPrefab, vitrineInst.transform);
                            rInst.name = "CuratedRelic";
                            rInst.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                            rInst.transform.localScale = Vector3.one * 0.45f;
                        }
                        vitrineIdx++;
                    }
                }
                else if (pedestalV3Prefab != null)
                {
                    // Hero Tripo Display Pedestal V3
                    GameObject pedInst = Object.Instantiate(pedestalV3Prefab, wallBay.transform);
                    pedInst.name = $"Tripo_PedestalDisplay_{i:D2}";
                    pedInst.transform.localPosition = new Vector3(0f, -wallHeight * 0.5f + 0.48f, -0.65f);
                    pedInst.transform.localRotation = Quaternion.identity;
                    pedInst.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                    // Place a curated humanity artifact on the pedestal
                    if (artifactIdx < alcoveArtifacts.Length)
                    {
                        var artPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(alcoveArtifacts[artifactIdx % alcoveArtifacts.Length]);
                        if (artPrefab != null)
                        {
                            GameObject aInst = Object.Instantiate(artPrefab, wallBay.transform);
                            aInst.name = "CuratedHumanityArtifact";
                            aInst.transform.localPosition = new Vector3(0f, -wallHeight * 0.5f + 1.15f, -0.65f);
                            aInst.transform.localScale = Vector3.one * 0.55f;
                        }
                        artifactIdx++;
                    }
                }
            }
        }
    }

    private static void BuildAlcoveBevelFrame(Transform alcove, Material goldMat)
    {
        float borderThick = 0.045f;
        // Top
        var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
        t.name = "Frame_Top";
        t.transform.SetParent(alcove, false);
        t.transform.localPosition = new Vector3(0f, 0.50f, -0.52f);
        t.transform.localScale = new Vector3(1.0f, borderThick, 0.1f);
        t.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(t.GetComponent<Collider>());

        // Bottom
        var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Frame_Bottom";
        b.transform.SetParent(alcove, false);
        b.transform.localPosition = new Vector3(0f, -0.50f, -0.52f);
        b.transform.localScale = new Vector3(1.0f, borderThick, 0.1f);
        b.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(b.GetComponent<Collider>());

        // Left
        var l = GameObject.CreatePrimitive(PrimitiveType.Cube);
        l.name = "Frame_Left";
        l.transform.SetParent(alcove, false);
        l.transform.localPosition = new Vector3(-0.50f, 0f, -0.52f);
        l.transform.localScale = new Vector3(borderThick, 1.0f, 0.1f);
        l.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(l.GetComponent<Collider>());

        // Right
        var r = GameObject.CreatePrimitive(PrimitiveType.Cube);
        r.name = "Frame_Right";
        r.transform.SetParent(alcove, false);
        r.transform.localPosition = new Vector3(0.50f, 0f, -0.52f);
        r.transform.localScale = new Vector3(borderThick, 1.0f, 0.1f);
        r.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
        Object.DestroyImmediate(r.GetComponent<Collider>());
    }

    private static void BuildRoofAndCeiling(Transform parent, float rotundaRadius, float wallHeight, float apertureRadius,
                                           Material wallDark, Material goldMat, Material glowCyan, Material glassMat)
    {
        GameObject roofRoot = new GameObject("Pavilion_Roof_Architecture");
        roofRoot.transform.SetParent(parent, false);

        int ribCount = 16;
        float stepDeg = 360f / ribCount;

        // 1. 16 Structural Arched Vault Ribs spanning from Column Capitals to Central Collar
        // Column capitals are at (r + 0.5) * stepDeg, so ribs connect directly to columns!
        GameObject ribsRoot = new GameObject("Vault_StructuralRibs");
        ribsRoot.transform.SetParent(roofRoot.transform, false);

        float r1 = rotundaRadius;
        float y1 = wallHeight;
        float r2 = apertureRadius;
        float y2 = wallHeight + 0.85f;

        float ribSpan = Mathf.Sqrt(Mathf.Pow(r1 - r2, 2) + Mathf.Pow(y2 - y1, 2));
        float ribAngle = Mathf.Atan2(y2 - y1, r1 - r2) * Mathf.Rad2Deg;

        for (int r = 0; r < ribCount; r++)
        {
            float deg = (r + 0.5f) * stepDeg;
            float rad = deg * Mathf.Deg2Rad;

            float midR = (r1 + r2) * 0.5f;
            float midY = (y1 + y2) * 0.5f;
            Vector3 ribPos = new Vector3(Mathf.Sin(rad) * midR, midY, Mathf.Cos(rad) * midR);
            Quaternion ribRot = Quaternion.Euler(-ribAngle, deg + 180f, 0f);

            GameObject rib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rib.name = $"VaultRib_{r:D2}";
            rib.transform.SetParent(ribsRoot.transform, false);
            rib.transform.localPosition = ribPos;
            rib.transform.localRotation = ribRot;
            rib.transform.localScale = new Vector3(0.35f, 0.55f, ribSpan);
            rib.GetComponent<MeshRenderer>().sharedMaterial = wallDark;
            Object.DestroyImmediate(rib.GetComponent<Collider>());

            // Underside gold conduit
            GameObject rConduit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rConduit.name = "UndersideGoldConduit";
            rConduit.transform.SetParent(rib.transform, false);
            rConduit.transform.localPosition = new Vector3(0f, -0.52f, 0f);
            rConduit.transform.localScale = new Vector3(0.08f, 0.04f, 0.98f);
            rConduit.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(rConduit.GetComponent<Collider>());
        }

        // 2. Multi-Tiered Coffered Ceiling Soffit Rings (Dark titanium with indirect cove lighting)
        GameObject soffitRoot = new GameObject("Ceiling_CofferedSoffits");
        soffitRoot.transform.SetParent(roofRoot.transform, false);

        // Outer Soffit Ring (R=18.5m down to 14.5m)
        Create3DSlabRing(soffitRoot.transform, "Soffit_OuterRing", 14.5f, 18.6f, 0.16f, wallHeight + 0.15f, wallDark);
        CreateMeshRing(soffitRoot.transform, "Soffit_OuterGoldLip", 14.5f, 0.08f, goldMat, wallHeight + 0.07f);

        // Mid Soffit Ring (R=14.5m down to 10.0m)
        Create3DSlabRing(soffitRoot.transform, "Soffit_MidRing", 10.0f, 14.5f, 0.16f, wallHeight + 0.45f, wallDark);
        CreateMeshRing(soffitRoot.transform, "Soffit_MidGoldLip", 10.0f, 0.08f, goldMat, wallHeight + 0.37f);

        // Inner Soffit Ring (R=10.0m down to 5.5m)
        Create3DSlabRing(soffitRoot.transform, "Soffit_InnerRing", 5.5f, 10.0f, 0.16f, wallHeight + 0.75f, wallDark);
        CreateMeshRing(soffitRoot.transform, "Soffit_InnerGoldLip", 5.5f, 0.08f, goldMat, wallHeight + 0.67f);

        // 3. Central Roof Aperture Collar (R=5.5m at Y=8.85m)
        Create3DSlabRing(roofRoot.transform, "Roof_ApertureCollar_Outer", 5.4f, 5.8f, 0.35f, wallHeight + 0.85f, wallDark);
        CreateMeshRing(roofRoot.transform, "Roof_ApertureGoldBezel", 5.5f, 0.14f, goldMat, wallHeight + 0.85f + 0.18f);
        CreateMeshRing(roofRoot.transform, "Roof_ApertureCyanBeacon", 5.45f, 0.06f, glowCyan, wallHeight + 0.85f - 0.18f);

        // 4. Crowning Glass Skylight Dome above the aperture
        GameObject dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dome.name = "Pavilion_CrowningGlassDome";
        dome.transform.SetParent(roofRoot.transform, false);
        dome.transform.localPosition = new Vector3(0f, wallHeight + 0.65f, 0f);
        dome.transform.localScale = new Vector3(apertureRadius * 2.2f, 3.2f, apertureRadius * 2.2f);
        dome.GetComponent<MeshRenderer>().sharedMaterial = glassMat;
        Object.DestroyImmediate(dome.GetComponent<Collider>());

        // 5. HERO TRIPO CEILING CHANDELIER (museum_ceiling_chandelier.glb)
        GameObject chandelierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_ceiling_chandelier.glb");
        if (chandelierPrefab != null)
        {
            GameObject chanRoot = new GameObject("Pavilion_CentralChandelier");
            chanRoot.transform.SetParent(roofRoot.transform, false);
            chanRoot.transform.localPosition = new Vector3(0f, wallHeight - 1.25f, 0f); // Y = 6.75m

            GameObject chanInst = Object.Instantiate(chandelierPrefab, chanRoot.transform);
            chanInst.name = "Tripo_Chandelier_Model";
            chanInst.transform.localPosition = Vector3.zero;
            chanInst.transform.localRotation = Quaternion.identity;
            chanInst.transform.localScale = new Vector3(4.2f, 4.2f, 4.2f);

            // 4 Crystalline Point Lights radiating warm luxury light downward
            for (int cl = 0; cl < 4; cl++)
            {
                float angle = cl * 90f * Mathf.Deg2Rad;
                Vector3 lPos = new Vector3(Mathf.Sin(angle) * 1.5f, -0.6f, Mathf.Cos(angle) * 1.5f);
                GameObject cLight = new GameObject($"ChandelierLight_{cl}");
                cLight.transform.SetParent(chanRoot.transform, false);
                cLight.transform.localPosition = lPos;
                var l = cLight.AddComponent<Light>();
                l.type = LightType.Point;
                l.color = new Color(1.0f, 0.92f, 0.78f);
                l.intensity = 2.0f;
                l.range = 14.0f;
            }
        }

        // 6. Indirect Ambient Cove Uplighting
        for (int c = 0; c < 8; c++)
        {
            float cDeg = c * 45f;
            float cRad = cDeg * Mathf.Deg2Rad;
            Vector3 cPos = new Vector3(Mathf.Sin(cRad) * 12.5f, wallHeight + 0.25f, Mathf.Cos(cRad) * 12.5f);

            GameObject coveLight = new GameObject($"CeilingCoveLight_{c}");
            coveLight.transform.SetParent(soffitRoot.transform, false);
            coveLight.transform.localPosition = cPos;
            var cl = coveLight.AddComponent<Light>();
            cl.type = LightType.Point;
            cl.color = new Color(1.0f, 0.95f, 0.88f); // Soft warm white 3600K
            cl.intensity = 2.0f;
            cl.range = 14.0f;
        }
    }

    private static void SetupMuseumFurnishingsAndLighting(Transform chamberRoot, Transform pavilion, float rotundaRadius,
                                                         Material wallDark, Material goldMat)
    {
        // 1. Luxury Observation Benches facing inward toward the rotunda
        GameObject benchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_luxury_lounge_bench.glb");
        if (benchPrefab != null)
        {
            GameObject loungesRoot = new GameObject("Museum_Observation_Lounges");
            loungesRoot.transform.SetParent(pavilion, false);

            float[] windowDegs = { 0f, 90f, 270f }; // North, East, West observation bays
            for (int w = 0; w < windowDegs.Length; w++)
            {
                float rad = windowDegs[w] * Mathf.Deg2Rad;
                Vector3 benchPos = new Vector3(Mathf.Sin(rad) * (rotundaRadius - 2.8f), 0.05f, Mathf.Cos(rad) * (rotundaRadius - 2.8f));
                Quaternion benchRot = Quaternion.Euler(0f, windowDegs[w] + 180f, 0f);

                GameObject bench = Object.Instantiate(benchPrefab, loungesRoot.transform);
                bench.name = $"LoungeBench_Window_{w}";
                bench.transform.localPosition = benchPos;
                bench.transform.localRotation = benchRot;
                bench.transform.localScale = Vector3.one * 1.25f;
            }
        }

        // 2. Central Chamber Fill Lighting (Balanced warm museum illumination)
        Transform oldFill = GameObject.Find("ChamberFillLight")?.transform;
        if (oldFill != null)
        {
            var l = oldFill.GetComponent<Light>();
            if (l != null)
            {
                l.color = new Color(1.0f, 0.95f, 0.90f);
                l.intensity = 1.15f;
            }
        }

        Transform sun = GameObject.Find("TerranSunLight")?.transform;
        if (sun != null)
        {
            var sl = sun.GetComponent<Light>();
            if (sl != null)
            {
                sl.color = new Color(1.0f, 0.96f, 0.90f);
                sl.intensity = 1.25f;
            }
        }
    }

    // =========================================================================
    // PROCEDURAL MESH GENERATION HELPERS
    // =========================================================================
    private static void CreateMeshRing(Transform parent, string name, float radius, float width, Material mat, float yPos)
    {
        GameObject ringObj = new GameObject(name);
        ringObj.transform.SetParent(parent, false);
        ringObj.transform.localPosition = new Vector3(0f, yPos, 0f);

        MeshFilter mf = ringObj.AddComponent<MeshFilter>();
        MeshRenderer mr = ringObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        int segments = 64;
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

    private static void Create3DSlabRing(Transform parent, string name, float rInner, float rOuter, float height, float yPos, Material mat)
    {
        GameObject slabObj = new GameObject(name);
        slabObj.transform.SetParent(parent, false);
        slabObj.transform.localPosition = new Vector3(0f, yPos, 0f);

        MeshFilter mf = slabObj.AddComponent<MeshFilter>();
        MeshRenderer mr = slabObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        int segments = 64;
        Mesh mesh = new Mesh { name = name + "_3DSlabMesh" };

        int vertCount = (segments + 1) * 4; // top 2, bottom 2
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[segments * 24]; // 4 faces per segment (top, bottom, outer wall, inner wall)

        float hHalf = height * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            int b = i * 4;
            // Top inner, top outer
            vertices[b] = new Vector3(cos * rInner, hHalf, sin * rInner);
            vertices[b + 1] = new Vector3(cos * rOuter, hHalf, sin * rOuter);
            // Bottom inner, bottom outer
            vertices[b + 2] = new Vector3(cos * rInner, -hHalf, sin * rInner);
            vertices[b + 3] = new Vector3(cos * rOuter, -hHalf, sin * rOuter);

            uvs[b] = new Vector2((float)i / segments, 0f);
            uvs[b + 1] = new Vector2((float)i / segments, 1f);
            uvs[b + 2] = new Vector2((float)i / segments, 0f);
            uvs[b + 3] = new Vector2((float)i / segments, 1f);

            if (i < segments)
            {
                int t = i * 24;
                int curr = i * 4;
                int next = (i + 1) * 4;

                // 1. Top face
                triangles[t] = curr;
                triangles[t + 1] = curr + 1;
                triangles[t + 2] = next;

                triangles[t + 3] = curr + 1;
                triangles[t + 4] = next + 1;
                triangles[t + 5] = next;

                // 2. Bottom face (reverse winding)
                triangles[t + 6] = curr + 2;
                triangles[t + 7] = next + 2;
                triangles[t + 8] = curr + 3;

                triangles[t + 9] = curr + 3;
                triangles[t + 10] = next + 2;
                triangles[t + 11] = next + 3;

                // 3. Outer wall
                triangles[t + 12] = curr + 1;
                triangles[t + 13] = curr + 3;
                triangles[t + 14] = next + 1;

                triangles[t + 15] = curr + 3;
                triangles[t + 16] = next + 3;
                triangles[t + 17] = next + 1;

                // 4. Inner wall (reverse winding)
                triangles[t + 18] = curr;
                triangles[t + 19] = next;
                triangles[t + 20] = curr + 2;

                triangles[t + 21] = curr + 2;
                triangles[t + 22] = next;
                triangles[t + 23] = next + 2;
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
