using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// XENOASIS — FinalMuseumBuilder.cs
/// Completes the Welcome Chamber Museum with world-class architectural design:
/// 1. Floor Design:
///    - Deep mirror-polished cosmic obsidian floor using URP Lit PBR (Zero cyan grid lines)
///    - Elevated central dais plinth (Y=0.24m) in brushed dark titanium with gold inlay rim
/// 2. Wall Design:
///    - 16 architectural bays framed by Tripo Sculpted Obsidian Pillars
///    - Solid bays feature recessed dark titanium acoustic panels with gold bevel trim and warm gallery downlights
///    - 4 panoramic window bays with crystal clear glass overlooking alpine mountains
/// 3. Roof & Ceiling Design:
///    - 16 arched structural vault ribs in titanium spanning from pillars to central aperture
///    - Multi-tiered coffered ceiling soffit with warm ambient cove uplighting
///    - Central skylight iris aperture with 12 titanium shutter blades that animate open upon descent
/// 4. Tripo 3D Interior:
///    - Quantum Gyroscope Centerpiece rotating above the central dais
///    - 4 Obsidian & Gold Plinths displaying Humanity's Artifacts
///    - 2 Luxury Ergonomic Marble & Gold Lounge Benches facing panoramic mountain windows
///    - Interactive Hologram Terminals
/// </summary>
public static class FinalMuseumBuilder
{
    [MenuItem("XENOASIS/Build Final Tripo Museum Experience")]
    public static void BuildFinalMuseum()
    {
        Debug.Log("[FinalMuseumBuilder] ✦ Starting complete architectural museum enhancement...");

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) urpLit = Shader.Find("Standard");

        // 1. PBR Materials Setup
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_LuxuryFloor.mat");
        if (floorMat == null) floorMat = new Material(urpLit);
        floorMat.shader = urpLit;
        floorMat.SetColor("_BaseColor", new Color(0.06f, 0.07f, 0.09f, 1.0f));
        floorMat.SetFloat("_Metallic", 0.45f);
        floorMat.SetFloat("_Smoothness", 0.90f);
        EditorUtility.SetDirty(floorMat);

        Material daisMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Museum_DaisPlinth.mat");
        if (daisMat == null) daisMat = new Material(urpLit);
        daisMat.shader = urpLit;
        daisMat.SetColor("_BaseColor", new Color(0.14f, 0.15f, 0.18f, 1.0f));
        daisMat.SetFloat("_Metallic", 0.70f);
        daisMat.SetFloat("_Smoothness", 0.85f);
        EditorUtility.SetDirty(daisMat);

        Material wallDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_Dark.mat");
        if (wallDark == null) wallDark = new Material(urpLit);
        wallDark.shader = urpLit;
        wallDark.SetColor("_BaseColor", new Color(0.12f, 0.13f, 0.16f, 1.0f));
        wallDark.SetFloat("_Metallic", 0.65f);
        wallDark.SetFloat("_Smoothness", 0.80f);
        EditorUtility.SetDirty(wallDark);

        Material wallWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_White.mat");
        if (wallWhite == null) wallWhite = new Material(urpLit);
        wallWhite.shader = urpLit;
        wallWhite.SetColor("_BaseColor", new Color(0.88f, 0.89f, 0.91f, 1.0f));
        wallWhite.SetFloat("_Metallic", 0.15f);
        wallWhite.SetFloat("_Smoothness", 0.70f);
        EditorUtility.SetDirty(wallWhite);

        Material goldInlayMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LuxuryGold_Inlay.mat");
        if (goldInlayMat == null)
        {
            goldInlayMat = new Material(urpLit);
            goldInlayMat.name = "LuxuryGold_Inlay";
            AssetDatabase.CreateAsset(goldInlayMat, "Assets/Materials/LuxuryGold_Inlay.mat");
        }
        goldInlayMat.SetColor("_BaseColor", new Color(0.92f, 0.76f, 0.38f, 1.0f));
        goldInlayMat.SetFloat("_Metallic", 0.90f);
        goldInlayMat.SetFloat("_Smoothness", 0.88f);
        EditorUtility.SetDirty(goldInlayMat);

        Material glassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        if (glassMat != null)
        {
            glassMat.SetFloat("_GridEmission", 0f);
            glassMat.SetFloat("_GridLineWidth", 0f);
            EditorUtility.SetDirty(glassMat);
        }

        GameObject tla = GameObject.Find("TerranLivingArchive");
        if (tla == null)
        {
            tla = new GameObject("TerranLivingArchive");
            GameObject env = GameObject.Find("[Environment]");
            if (env != null) tla.transform.SetParent(env.transform);
        }

        GameObject arch = GameObject.Find("GrandMuseumPavilion_Architecture");
        if (arch == null)
        {
            arch = new GameObject("GrandMuseumPavilion_Architecture");
            arch.transform.SetParent(tla.transform, false);
        }

        // =========================================================================
        // 1. FLOOR & DAIS DESIGN (Clean luxury mirror obsidian + gold dais rim + radial inlays)
        // =========================================================================
        GameObject floorObj = GameObject.Find("Museum_Floor_Foundation");
        if (floorObj != null)
        {
            floorObj.transform.position = new Vector3(0f, -0.02f, 0f);
            floorObj.transform.localScale = new Vector3(37.37f, 0.05f, 37.37f);
            var mr = floorObj.GetComponent<MeshRenderer>();
            if (mr && floorMat) mr.sharedMaterial = floorMat;
        }

        GameObject dais = GameObject.Find("Landing_Dais_Plinth");
        if (dais != null)
        {
            dais.transform.position = new Vector3(0f, 0.12f, 0f);
            dais.transform.localScale = new Vector3(10.5f, 0.24f, 10.5f);
            var mr = dais.GetComponent<MeshRenderer>();
            if (mr && daisMat) mr.sharedMaterial = daisMat;
        }

        // Gold Inlay Ring on Dais Edge & Radial Compass Floor Strips
        BuildDaisGoldInlay(arch.transform, goldInlayMat);

        // Update Colonnade Gallery Wall Bays to luxury dark titanium
        GameObject colonnade = GameObject.Find("Pavilion_Colonnade");
        if (colonnade != null)
        {
            for (int i = 0; i < colonnade.transform.childCount; i++)
            {
                var bay = colonnade.transform.GetChild(i);
                if (bay.name.StartsWith("GalleryWallBay"))
                {
                    var mr = bay.GetComponent<MeshRenderer>();
                    if (mr && wallDark) mr.sharedMaterial = wallDark;
                }
            }
        }

        // =========================================================================
        // 2. WALL DESIGN (Recessed Titanium Panels + Slender Gold Molding + Downlights)
        // =========================================================================
        BuildArchitecturalWallDetails(arch.transform, wallDark, goldInlayMat);

        // =========================================================================
        // 3. ROOF DESIGN (Vaulted Ribs + Coffered Soffits + Warm Uplighting)
        // =========================================================================
        BuildMonumentalRoof(arch.transform, wallDark, wallWhite, goldInlayMat, glassMat);

        // =========================================================================
        // 4. TRIPO 3D OBSIDIAN PILLARS
        // =========================================================================
        BuildTripoObsidianColonnade(arch.transform);

        // =========================================================================
        // 5. TRIPO 3D INTERIOR ASSETS (Centerpiece, Plinths, Benches, Terminals)
        // =========================================================================
        BuildTripoInteriorExhibits(tla.transform);

        // Save scene changes
        EditorUtility.SetDirty(arch);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[FinalMuseumBuilder] ✦ Museum architecture & interior transformation 100% COMPLETE!");
    }

    private static void BuildDaisGoldInlay(Transform parent, Material goldMat)
    {
        Transform oldInlays = parent.Find("Floor_ArchitecturalInlays");
        if (oldInlays != null) Object.DestroyImmediate(oldInlays.gameObject);

        GameObject inlays = new GameObject("Floor_ArchitecturalInlays");
        inlays.transform.SetParent(parent, false);

        // Elegant gold ring framing the landing dais
        CreateMeshRing(inlays.transform, "Inlay_DaisGoldRim", 5.25f, 0.12f, goldMat, 0.242f);

        // 16 Radial Compass Inlay Strips on the polished obsidian floor
        int stripCount = 16;
        float rStart = 5.30f;
        float rEnd = 17.50f;
        float len = rEnd - rStart;
        float midR = (rStart + rEnd) * 0.5f;

        for (int i = 0; i < stripCount; i++)
        {
            float angleDeg = i * (360f / stripCount);
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * midR, 0.005f, Mathf.Cos(rad) * midR);
            Quaternion rot = Quaternion.Euler(0f, -angleDeg, 0f);

            GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = $"FloorInlay_Radial_{i:D2}";
            strip.transform.SetParent(inlays.transform, false);
            strip.transform.position = pos;
            strip.transform.rotation = rot;
            strip.transform.localScale = new Vector3(0.08f, 0.006f, len);
            strip.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(strip.GetComponent<Collider>());
        }
    }

    private static void BuildArchitecturalWallDetails(Transform parent, Material darkMat, Material goldMat)
    {
        Transform oldDetails = parent.Find("Wall_ArchitecturalDetails");
        if (oldDetails != null) Object.DestroyImmediate(oldDetails.gameObject);

        GameObject wallDetails = new GameObject("Wall_ArchitecturalDetails");
        wallDetails.transform.SetParent(parent, false);

        int bayCount = 16;
        float wallRadius = 18.0f;

        for (int i = 0; i < bayCount; i++)
        {
            // Skip window bays (0, 4, 8, 12)
            if (i % 4 == 0) continue;

            float angleDeg = i * (360f / bayCount);
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * wallRadius, 4.0f, Mathf.Cos(rad) * wallRadius);
            Quaternion rot = Quaternion.Euler(0, angleDeg + 180f, 0);

            // Inset acoustic titanium panel
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = $"AcousticPanel_Bay_{i:D2}";
            panel.transform.SetParent(wallDetails.transform, false);
            panel.transform.position = pos + rot * Vector3.back * 0.10f;
            panel.transform.rotation = rot;
            panel.transform.localScale = new Vector3(4.8f, 6.4f, 0.12f);
            panel.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
            Object.DestroyImmediate(panel.GetComponent<Collider>());

            // Slender gold architectural framing border (4 border bars around perimeter)
            float pw = 4.8f, ph = 6.4f, borderThick = 0.06f;

            // Top border
            var topB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topB.name = "Trim_Top";
            topB.transform.SetParent(panel.transform, false);
            topB.transform.localPosition = new Vector3(0f, 0.5f - (borderThick * 0.5f) / ph, 0.52f);
            topB.transform.localScale = new Vector3(1.0f, borderThick / ph, 0.04f);
            topB.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(topB.GetComponent<Collider>());

            // Bottom border
            var btmB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btmB.name = "Trim_Bottom";
            btmB.transform.SetParent(panel.transform, false);
            btmB.transform.localPosition = new Vector3(0f, -0.5f + (borderThick * 0.5f) / ph, 0.52f);
            btmB.transform.localScale = new Vector3(1.0f, borderThick / ph, 0.04f);
            btmB.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(btmB.GetComponent<Collider>());

            // Left border
            var leftB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftB.name = "Trim_Left";
            leftB.transform.SetParent(panel.transform, false);
            leftB.transform.localPosition = new Vector3(-0.5f + (borderThick * 0.5f) / pw, 0f, 0.52f);
            leftB.transform.localScale = new Vector3(borderThick / pw, 1.0f, 0.04f);
            leftB.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(leftB.GetComponent<Collider>());

            // Right border
            var rightB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightB.name = "Trim_Right";
            rightB.transform.SetParent(panel.transform, false);
            rightB.transform.localPosition = new Vector3(0.5f - (borderThick * 0.5f) / pw, 0f, 0.52f);
            rightB.transform.localScale = new Vector3(borderThick / pw, 1.0f, 0.04f);
            rightB.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(rightB.GetComponent<Collider>());

            // 5 vertical titanium acoustic slats inside the panel
            for (int s = -2; s <= 2; s++)
            {
                var slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slat.name = $"AcousticSlat_{s + 2}";
                slat.transform.SetParent(panel.transform, false);
                slat.transform.localPosition = new Vector3(s * 0.18f, 0f, 0.51f);
                slat.transform.localScale = new Vector3(0.04f / pw, 0.88f, 0.02f);
                slat.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
                Object.DestroyImmediate(slat.GetComponent<Collider>());
            }

            // Warm gallery downlight softly washing down the bay
            var spotObj = new GameObject($"GalleryDownlight_{i:D2}");
            spotObj.transform.SetParent(panel.transform, false);
            spotObj.transform.localPosition = new Vector3(0f, 0.52f, 0.6f);
            spotObj.transform.localRotation = Quaternion.Euler(78f, 0f, 0f);
            var light = spotObj.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = new Color(1.0f, 0.94f, 0.86f);
            light.intensity = 0.9f;
            light.range = 8.0f;
            light.spotAngle = 70f;
        }
    }

    private static void BuildMonumentalRoof(Transform parent, Material darkMat, Material whiteMat, Material goldMat, Material glassMat)
    {
        // 0. Clean up legacy/obsolete roof parts and cyan glow artifacts
        string[] obsoleteNames = {
            "Pavilion_RoofCanopySlab",
            "Pavilion_Roof_Parapet_OuterRim",
            "Pavilion_Roof_Aperture_CollarBezel",
            "Roof_Aperture_CollarGlow",
            "ApertureGlow_Bottom",
            "Foundation_CyanGlowTrim",
            "Roof_Parapet_BeaconRing"
        };
        foreach (var name in obsoleteNames)
        {
            var old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);
        }

        Transform existingCeiling = parent.Find("Monumental_Roof_Structure");
        if (existingCeiling != null) Object.DestroyImmediate(existingCeiling.gameObject);

        GameObject roofRoot = new GameObject("Monumental_Roof_Structure");
        roofRoot.transform.SetParent(parent, false);

        // 1. Annular Coffered Ceiling Soffit Slab (Inner R=5.8m, Outer R=18.4m, Y=8.10m to 8.55m)
        GameObject cofferedRing = new GameObject("Roof_CofferedCeilingSlab");
        cofferedRing.transform.SetParent(roofRoot.transform, false);
        var mfCoffered = cofferedRing.AddComponent<MeshFilter>();
        mfCoffered.sharedMesh = Create3DSlabRingMesh(5.80f, 18.40f, 8.10f, 8.55f, 64);
        var mrCoffered = cofferedRing.AddComponent<MeshRenderer>();
        mrCoffered.sharedMaterial = darkMat;

        // 2. Central Aperture Bezel Collar (Framing the 11.6m skylight opening)
        GameObject apertureCollar = new GameObject("Roof_CentralApertureBezel");
        apertureCollar.transform.SetParent(roofRoot.transform, false);
        var mfAperture = apertureCollar.AddComponent<MeshFilter>();
        mfAperture.sharedMesh = Create3DSlabRingMesh(5.65f, 6.05f, 8.00f, 8.75f, 64);
        var mrAperture = apertureCollar.AddComponent<MeshRenderer>();
        mrAperture.sharedMaterial = darkMat;

        // Gold Inlay Rim on bottom lip of the skylight aperture
        CreateMeshRing(roofRoot.transform, "Aperture_GoldRim", 5.80f, 0.22f, goldMat, 8.04f);

        // 3. Outer Ring Beam (Soffit Crown framing perimeter wall)
        GameObject outerCrown = new GameObject("Roof_OuterCrownRing");
        outerCrown.transform.SetParent(roofRoot.transform, false);
        var mfOuter = outerCrown.AddComponent<MeshFilter>();
        mfOuter.sharedMesh = Create3DSlabRingMesh(18.00f, 19.20f, 7.90f, 8.65f, 64);
        var mrOuter = outerCrown.AddComponent<MeshRenderer>();
        mrOuter.sharedMaterial = darkMat;

        // 4. 16 Arched Structural Vault Ribs (Dark titanium with gold undercarriage trims)
        GameObject beamsRoot = new GameObject("Roof_StructuralVaultRibs");
        beamsRoot.transform.SetParent(roofRoot.transform, false);

        int ribCount = 16;
        float outerR = 18.2f;
        float innerR = 5.8f;
        float outerY = 7.95f;
        float innerY = 8.55f;

        for (int i = 0; i < ribCount; i++)
        {
            float angle = (float)i / ribCount * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 startP = new Vector3(cos * outerR, outerY, sin * outerR);
            Vector3 endP = new Vector3(cos * innerR, innerY, sin * innerR);
            Vector3 midP = (startP + endP) * 0.5f + Vector3.up * 0.25f;

            Vector3 dir = endP - startP;
            float length = dir.magnitude;

            GameObject rib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rib.name = $"VaultRib_{i:D2}";
            rib.transform.SetParent(beamsRoot.transform, false);
            rib.transform.position = midP;
            rib.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            rib.transform.localScale = new Vector3(0.38f, 0.44f, length);
            rib.GetComponent<MeshRenderer>().sharedMaterial = darkMat;
            Object.DestroyImmediate(rib.GetComponent<Collider>());

            // Gold accent trim on underside of rib
            GameObject ribGold = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ribGold.name = "GoldRibTrim";
            ribGold.transform.SetParent(rib.transform, false);
            ribGold.transform.localPosition = new Vector3(0f, -0.52f, 0f);
            ribGold.transform.localScale = new Vector3(0.18f, 0.06f, 0.98f);
            ribGold.GetComponent<MeshRenderer>().sharedMaterial = goldMat;
            Object.DestroyImmediate(ribGold.GetComponent<Collider>());
        }

        // 5. Grand Crowning Crystal Dome (Over the skylight aperture)
        GameObject domeObj = GameObject.Find("Pavilion_CrowningGlassDome");
        if (domeObj == null)
        {
            domeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            domeObj.name = "Pavilion_CrowningGlassDome";
        }
        domeObj.transform.SetParent(roofRoot.transform, false);
        domeObj.transform.localPosition = new Vector3(0f, 9.20f, 0f);
        domeObj.transform.localScale = new Vector3(11.6f, 4.0f, 11.6f);
        domeObj.GetComponent<MeshRenderer>().sharedMaterial = glassMat;
        Object.DestroyImmediate(domeObj.GetComponent<Collider>());

        // 6. Warm Ceiling Cove Uplight
        var coveLightObj = GameObject.Find("Roof_CoveAmbientLight");
        if (coveLightObj == null)
        {
            coveLightObj = new GameObject("Roof_CoveAmbientLight");
            coveLightObj.transform.SetParent(roofRoot.transform, false);
        }
        coveLightObj.transform.localPosition = new Vector3(0f, 8.20f, 0f);
        var coveLight = coveLightObj.GetComponent<Light>();
        if (coveLight == null) coveLight = coveLightObj.AddComponent<Light>();
        coveLight.type = LightType.Point;
        coveLight.color = new Color(0.98f, 0.92f, 0.82f);
        coveLight.intensity = 2.8f;
        coveLight.range = 24.0f;

        // 7. Mechanical Iris Roof Shutter (12 Titanium Blades with Gold Trims)
        GameObject shutterObj = GameObject.Find("ChamberRoofShutter");
        if (shutterObj == null)
        {
            shutterObj = new GameObject("ChamberRoofShutter");
        }
        shutterObj.transform.SetParent(roofRoot.transform, false);
        shutterObj.transform.localPosition = Vector3.zero;
        var shutter = shutterObj.GetComponent<ChamberRoofShutter>();
        if (shutter == null) shutter = shutterObj.AddComponent<ChamberRoofShutter>();
        shutter.CeilingHeight = 8.45f;
        shutter.BuildIrisBlades();
        shutter.SetShutterStateImmediate(true); // Initially closed for the descent opening sequence

        // Link Shutter to UFODescentSequence
        var ufoSeq = Object.FindObjectOfType<UFODescentSequence>();
        if (ufoSeq != null)
        {
            var field = typeof(UFODescentSequence).GetField("chamberRoofShutter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(ufoSeq, shutter);
                EditorUtility.SetDirty(ufoSeq);
            }
        }
    }

    private static void BuildTripoObsidianColonnade(Transform archTransform)
    {
        GameObject pillarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/obsidian_pillar.glb");
        if (pillarPrefab == null) return;

        Transform existingPillars = archTransform.Find("Tripo_Obsidian_Colonnade");
        if (existingPillars != null) Object.DestroyImmediate(existingPillars.gameObject);

        GameObject pillarsRoot = new GameObject("Tripo_Obsidian_Colonnade");
        pillarsRoot.transform.SetParent(archTransform, false);

        int count = 16;
        float radius = 18.0f;

        for (int i = 0; i < count; i++)
        {
            float angleDeg = i * (360f / count);
            float rad = angleDeg * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, 0.0f, Mathf.Cos(rad) * radius);
            Quaternion rot = Quaternion.Euler(0f, -angleDeg + 90f, 0f);

            GameObject pillarInst = Object.Instantiate(pillarPrefab, pillarsRoot.transform);
            pillarInst.name = $"Tripo_ObsidianPillar_{i:D2}";
            pillarInst.transform.position = pos;
            pillarInst.transform.rotation = rot;
            pillarInst.transform.localScale = new Vector3(3.2f, 8.0f, 3.2f);

            var col = pillarInst.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0, 4.0f, 0);
            col.radius = 0.55f;
            col.height = 8.0f;

            GameObject oldCol = GameObject.Find($"Column_{i:D2}");
            if (oldCol != null) oldCol.SetActive(false);
        }
    }

    private static void BuildTripoInteriorExhibits(Transform tlaTransform)
    {
        Transform interiorRoot = tlaTransform.Find("Final_Tripo_Interior");
        if (interiorRoot != null) Object.DestroyImmediate(interiorRoot.gameObject);

        GameObject interior = new GameObject("Final_Tripo_Interior");
        interior.transform.SetParent(tlaTransform, false);

        // 1. Quantum Gyroscope Centerpiece
        GameObject gyroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_centerpiece_quantum_gyro.glb");
        if (gyroPrefab != null)
        {
            GameObject gyro = Object.Instantiate(gyroPrefab, interior.transform);
            gyro.name = "Tripo_Centerpiece_QuantumGyro";
            gyro.transform.position = new Vector3(0f, 3.85f, 0f);
            gyro.transform.localScale = Vector3.one * 2.2f;

            var rotator = gyro.AddComponent<ExhibitRotator>();
            rotator.rotationSpeed = 12.0f;
            rotator.bobAmplitude = 0.08f;
            rotator.bobSpeed = 1.2f;

            var lightObj = new GameObject("Gyro_HoloCoreLight");
            lightObj.transform.SetParent(gyro.transform, false);
            var l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.1f, 0.85f, 1.0f);
            l.intensity = 2.4f;
            l.range = 6.0f;
        }

        // 2. 4 Obsidian & Gold Plinths with Humanity's Artifacts
        GameObject pedestalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_pedestal_obsidian_gold.glb");
        if (pedestalPrefab != null)
        {
            float pedestalRadius = 9.8f;
            string[] artifactPaths = {
                "Assets/Models/Tripo/museum_rosetta_stone.glb",
                "Assets/Models/Tripo/museum_voyager_golden_record.glb",
                "Assets/Models/Tripo/museum_prometheus_fire_and_chip.glb",
                "Assets/Models/Tripo/museum_svalbard_seed_vault.glb"
            };

            string[] exhibitTitles = {
                "Rosetta Stone (Bilingual Archive)",
                "Voyager Golden Record (Interstellar Greeting)",
                "Prometheus Spark (Silicon & Fire)",
                "Svalbard Global Seed Sanctuary"
            };

            float[] angles = { 45f, 135f, 225f, 315f };

            for (int i = 0; i < 4; i++)
            {
                float rad = angles[i] * Mathf.Deg2Rad;
                Vector3 pPos = new Vector3(Mathf.Sin(rad) * pedestalRadius, 0.03f, Mathf.Cos(rad) * pedestalRadius);
                Quaternion pRot = Quaternion.Euler(0f, angles[i] + 180f, 0f);

                GameObject ped = Object.Instantiate(pedestalPrefab, interior.transform);
                ped.name = $"Tripo_Plinth_{i:D2}_{exhibitTitles[i]}";
                ped.transform.position = pPos;
                ped.transform.rotation = pRot;
                ped.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);

                var pedCol = ped.AddComponent<BoxCollider>();
                pedCol.center = new Vector3(0, 0.45f, 0);
                pedCol.size = new Vector3(1.3f, 0.9f, 1.3f);

                if (i < artifactPaths.Length)
                {
                    GameObject artPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(artifactPaths[i]);
                    if (artPrefab != null)
                    {
                        GameObject art = Object.Instantiate(artPrefab, ped.transform);
                        art.name = "Exhibited_Artifact";
                        art.transform.localPosition = new Vector3(0f, 0.55f, 0f);
                        art.transform.localRotation = Quaternion.identity;
                        art.transform.localScale = Vector3.one * 0.45f;

                        var artRot = art.AddComponent<ExhibitRotator>();
                        artRot.rotationSpeed = 8.0f;
                        artRot.bobAmplitude = 0.04f;
                        artRot.bobSpeed = 1.5f;
                    }
                }

                var spotObj = new GameObject("Pedestal_Spot");
                spotObj.transform.SetParent(ped.transform, false);
                spotObj.transform.localPosition = new Vector3(0f, 4.5f, 0f);
                spotObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                var spot = spotObj.AddComponent<Light>();
                spot.type = LightType.Spot;
                spot.color = new Color(0.98f, 0.95f, 0.88f);
                spot.intensity = 2.2f;
                spot.range = 7.0f;
                spot.spotAngle = 45f;
            }
        }

        // 3. Tripo Luxury Lounge Benches (Facing East & West Panoramic Windows)
        GameObject benchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_luxury_lounge_bench.glb");
        if (benchPrefab != null)
        {
            Vector3[] benchPositions = {
                new Vector3(13.8f, 0.03f, -2.5f),
                new Vector3(-13.8f, 0.03f, 2.5f)
            };
            Quaternion[] benchRotations = {
                Quaternion.Euler(0f, 105f, 0f),
                Quaternion.Euler(0f, -75f, 0f)
            };

            for (int i = 0; i < benchPositions.Length; i++)
            {
                GameObject bench = Object.Instantiate(benchPrefab, interior.transform);
                bench.name = $"Tripo_LuxuryLoungeBench_{i:D2}";
                bench.transform.position = benchPositions[i];
                bench.transform.rotation = benchRotations[i];
                bench.transform.localScale = Vector3.one * 2.8f;

                var box = bench.AddComponent<BoxCollider>();
                box.center = new Vector3(0, 0.45f, 0);
                box.size = new Vector3(1.2f, 0.9f, 2.4f);
            }
        }

        // 4. Tripo Interactive Terminals
        GameObject terminalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/museum_interactive_terminal.glb");
        if (terminalPrefab != null)
        {
            Vector3[] termPositions = {
                new Vector3(0f, 0.03f, 13.5f),
                new Vector3(0f, 0.03f, -13.5f)
            };
            Quaternion[] termRotations = {
                Quaternion.Euler(0f, 180f, 0f),
                Quaternion.Euler(0f, 0f, 0f)
            };

            for (int i = 0; i < termPositions.Length; i++)
            {
                GameObject term = Object.Instantiate(terminalPrefab, interior.transform);
                term.name = $"Tripo_InteractiveTerminal_{i:D2}";
                term.transform.position = termPositions[i];
                term.transform.rotation = termRotations[i];
                term.transform.localScale = Vector3.one * 1.65f;

                var termCol = term.AddComponent<BoxCollider>();
                termCol.center = new Vector3(0, 0.85f, 0);
                termCol.size = new Vector3(1.1f, 1.7f, 0.9f);
            }
        }
    }

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

    private static Mesh Create3DSlabRingMesh(float innerRadius, float outerRadius, float yBottom, float yTop, int segments)
    {
        Mesh mesh = new Mesh { name = "Roof_SlabRingMesh" };
        var verts = new List<Vector3>();
        var norms = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        // 1. Bottom Face (-Y)
        int bStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            verts.Add(new Vector3(s * innerRadius, yBottom, c * innerRadius));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(s * 0.5f + 0.5f, c * 0.5f + 0.5f));

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

        // 2. Top Face (+Y)
        int tStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float norm = (float)i / segments;
            float angle = norm * Mathf.PI * 2f;
            float s = Mathf.Sin(angle), c = Mathf.Cos(angle);
            verts.Add(new Vector3(s * innerRadius, yTop, c * innerRadius));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(s * 0.5f + 0.5f, c * 0.5f + 0.5f));

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

        // 3. Inner Rim (facing aperture center)
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

        // 4. Outer Rim (facing outwards)
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
}
