using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// XENOASIS — UFOBulderEditor.cs
/// Generates production-grade 3D Wavefront OBJ models for:
/// 1. UFOSpaceship_Exterior.obj: Complete aerodynamic alien saucer with multi-tier hull,
///    bridge observation dome, perimeter plasma ring, reactor core, thrusters & landing pads.
/// 2. UFOCockpit_Interior.obj: Detailed interior command bridge with wrap-around pilot console,
///    3 angled MFD screens, holographic radar dish, ergonomic pilot command chair,
///    structural canopy arch ribs, overhead avionics, and rear airlock pressure bulkhead.
/// </summary>
public class UFOBulderEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XENOASIS/Generate Real UFO 3D Models")]
    public static void GenerateAllUFOModels()
    {
        Debug.Log("[XENOASIS] Generating production-grade 3D UFO Models...");

        string modelsDir = "Assets/Models/UFO";
        if (!Directory.Exists(modelsDir)) Directory.CreateDirectory(modelsDir);

        string extPath = Path.Combine(modelsDir, "UFOSpaceship_Exterior.obj");
        string intPath = Path.Combine(modelsDir, "UFOCockpit_Interior.obj");

        BuildExteriorUFO(extPath);
        BuildCockpitInterior(intPath);

        AssetDatabase.Refresh();

        // Configure imported models
        ConfigureImportedModel(extPath);
        ConfigureImportedModel(intPath);

        // Build Prefabs with PBR materials
        CreateUFOPrefabs();

        Debug.Log($"[XENOASIS] ✔ Successfully generated real UFO 3D Models and Prefabs:\n- {extPath}\n- {intPath}\n- Assets/Prefabs/UFO_Mothership.prefab\n- Assets/Prefabs/UFO_CockpitBridge.prefab");
    }

    public static void CreateUFOPrefabs()
    {
        string prefabsDir = "Assets/Prefabs";
        if (!Directory.Exists(prefabsDir)) Directory.CreateDirectory(prefabsDir);

        Material scifiWallDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_Dark.mat");
        Material scifiWallWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_White.mat");
        Material glassViewportMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        Material cyanGlowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        Material goldGlowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");

        Material cockpitHullDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat") ?? scifiWallDark;
        Material cockpitScreenGlass = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_ScreenGlass.mat") ?? scifiWallDark;
        Material cockpitHullWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullWhite.mat") ?? scifiWallWhite;

        // 1. Exterior Mothership Prefab (Tripo AI High-Poly PBR Model)
        GameObject extGLB = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_exterior.glb");
        GameObject extModel = extGLB ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/UFO/UFOSpaceship_Exterior.obj");
        if (extModel != null)
        {
            GameObject extInstance = new GameObject("UFO_Mothership");
            GameObject modelChild = Instantiate(extModel, extInstance.transform);
            modelChild.name = "UFO_HullModel";
            modelChild.transform.localPosition = Vector3.zero;
            modelChild.transform.localRotation = Quaternion.identity;
            modelChild.transform.localScale = (extGLB != null) ? new Vector3(18f, 18f, 18f) : Vector3.one;

            if (extGLB == null)
            {
                foreach (var mr in modelChild.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (mr.name.Contains("Hull_Primary")) mr.sharedMaterial = scifiWallDark;
                    else if (mr.name.Contains("Hull_Accent")) mr.sharedMaterial = scifiWallWhite;
                    else if (mr.name.Contains("Bridge_Glass")) mr.sharedMaterial = glassViewportMat;
                    else if (mr.name.Contains("Plasma_Ring")) mr.sharedMaterial = cyanGlowMat;
                    else if (mr.name.Contains("Reactor_Core")) mr.sharedMaterial = goldGlowMat;
                }
            }

            // Central tractor beam spotlight pointing downward
            GameObject spotObj = new GameObject("TractorSpotlight");
            spotObj.transform.SetParent(extInstance.transform);
            spotObj.transform.localPosition = new Vector3(0, -2.5f, 0);
            spotObj.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            var spot = spotObj.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.color = new Color(0f, 0.9f, 1f);
            spot.intensity = 5.0f;
            spot.range = 70f;
            spot.spotAngle = 65f;

            // Warm golden reactor core glow light
            GameObject coreObj = new GameObject("ReactorCoreGlow");
            coreObj.transform.SetParent(extInstance.transform);
            coreObj.transform.localPosition = new Vector3(0, -1.5f, 0);
            var coreLight = coreObj.AddComponent<Light>();
            coreLight.type = LightType.Point;
            coreLight.color = new Color(1.0f, 0.75f, 0.25f);
            coreLight.intensity = 3.5f;
            coreLight.range = 20f;

            string prefabPath = "Assets/Prefabs/UFO_Mothership.prefab";
            PrefabUtility.SaveAsPrefabAsset(extInstance, prefabPath);
            DestroyImmediate(extInstance);
            Debug.Log("[XENOASIS] Created Tripo UFO Mothership prefab: " + prefabPath);
        }

        // 2. Interior Cockpit Bridge Prefab (Full 360° Enclosed Alien UFO Bridge + Tripo PBR Module)
        GameObject intObjModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/UFO/UFOCockpit_Interior.obj");
        GameObject tripoModule = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_module_tripo.glb") 
                              ?? AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_interior.glb");

        if (intObjModel != null)
        {
            GameObject intInstance = new GameObject("UFO_CockpitBridge");
            
            // 2A. Primary Enclosed Alien Hull (Left/Right Walls, Side Viewports, Floor, Roof, Bulkhead)
            GameObject hullChild = Instantiate(intObjModel, intInstance.transform);
            hullChild.name = "UFO_Cockpit_Enclosure";
            hullChild.transform.localPosition = Vector3.zero;
            hullChild.transform.localRotation = Quaternion.identity;
            hullChild.transform.localScale = Vector3.one;

            Material leftScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Left_EarthScan.mat") ?? cockpitScreenGlass;
            Material rightScreenMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen_Right_EarthOrbit.mat") ?? cockpitScreenGlass;

            if (cockpitHullDark != null) { cockpitHullDark.SetFloat("_Cull", 0f); cockpitHullDark.doubleSidedGI = true; }
            if (cockpitHullWhite != null) { cockpitHullWhite.SetFloat("_Cull", 0f); cockpitHullWhite.doubleSidedGI = true; }
            if (leftScreenMat != null) { leftScreenMat.SetFloat("_Cull", 0f); leftScreenMat.doubleSidedGI = true; }
            if (rightScreenMat != null) { rightScreenMat.SetFloat("_Cull", 0f); rightScreenMat.doubleSidedGI = true; }

            foreach (var mr in hullChild.GetComponentsInChildren<MeshRenderer>(true))
            {
                // OBJ coordinate inversion: Screen_RightPanoramic is physical Left (-1.78m X), Screen_LeftPanoramic is physical Right (+1.78m X)
                if (mr.name.Contains("Screen_RightPanoramic")) mr.sharedMaterial = leftScreenMat; // Left: Earth Biosphere & Scan
                else if (mr.name.Contains("Screen_LeftPanoramic")) mr.sharedMaterial = rightScreenMat; // Right: Sol-3 Orbit Dynamics
                else if (mr.name.Contains("Cockpit_Screen")) mr.sharedMaterial = cockpitScreenGlass;
                else if (mr.name.Contains("Cockpit_Dark")) mr.sharedMaterial = cockpitHullDark;
                else if (mr.name.Contains("Cockpit_Light")) mr.sharedMaterial = cockpitHullWhite;
                else if (mr.name.Contains("GlassViewport") || mr.name.Contains("Cockpit_Glass")) mr.sharedMaterial = glassViewportMat;
                else if (mr.name.Contains("Glow_Cyan")) mr.sharedMaterial = cyanGlowMat;
                else if (mr.name.Contains("Glow_Gold")) mr.sharedMaterial = goldGlowMat;
            }

            // 2B. Tripo AI High-Poly PBR Command Module (Mounted at front command desk)
            if (tripoModule != null)
            {
                GameObject tripoChild = Instantiate(tripoModule, intInstance.transform);
                tripoChild.name = "Tripo_PBR_CommandDesk";
                tripoChild.transform.localPosition = new Vector3(0f, 0.44f, 1.38f);
                tripoChild.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                tripoChild.transform.localScale = Vector3.one * 1.35f;
            }

            // 2C. Secondary Tripo Sci-Fi Console & Tactical Station in Rear Bridge
            GameObject tripoInterior = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/ufo_cockpit_interior.glb");
            if (tripoInterior != null)
            {
                GameObject rearTripo = Instantiate(tripoInterior, intInstance.transform);
                rearTripo.name = "Tripo_Rear_AvionicsNexus";
                rearTripo.transform.localPosition = new Vector3(0f, 0.22f, -1.90f);
                rearTripo.transform.localRotation = Quaternion.identity;
                rearTripo.transform.localScale = Vector3.one * 1.15f;
            }

            // 2D. Ambient Forward Cockpit Illumination (Cyan/Blue Sci-Fi Glow)
            GameObject lightObj = new GameObject("Cockpit_InternalLight");
            lightObj.transform.SetParent(intInstance.transform);
            lightObj.transform.localPosition = new Vector3(0f, 1.85f, 0.4f);
            var pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(0.12f, 0.85f, 1.0f);
            pLight.intensity = 2.2f;
            pLight.range = 5.5f;

            // 2E. Ambient Rear Cabin Illumination (Soft warm cyan fill for solid pilot chair, crew seats & rear nexus)
            GameObject rearLightObj = new GameObject("Cockpit_RearLight");
            rearLightObj.transform.SetParent(intInstance.transform);
            rearLightObj.transform.localPosition = new Vector3(0f, 1.95f, -1.20f);
            var rLight = rearLightObj.AddComponent<Light>();
            rLight.type = LightType.Point;
            rLight.color = new Color(0.20f, 0.85f, 1.0f);
            rLight.intensity = 2.0f;
            rLight.range = 6.0f;

            string prefabPath = "Assets/Prefabs/UFO_CockpitBridge.prefab";
            PrefabUtility.SaveAsPrefabAsset(intInstance, prefabPath);
            DestroyImmediate(intInstance);
            Debug.Log("[XENOASIS] ✔ Created complete 360° Enclosed UFO Cockpit Bridge prefab with Tripo PBR: " + prefabPath);

        }

        AssetDatabase.SaveAssets();
    }

    // =========================================================================
    // 1. EXTERIOR UFO MOTHERSHIP / SAUCER (18m Diameter)
    // =========================================================================
    static void BuildExteriorUFO(string filePath)
    {
        var obj = new ObjWriter();

        // 19-point profile curve (r, y, group)
        var profile = new (float r, float y, string group)[]
        {
            (0.00f,  3.20f, "Hull_Accent"),   // 0: Spire tip
            (0.12f,  2.70f, "Hull_Accent"),   // 1: Spire neck
            (0.35f,  2.45f, "Hull_Accent"),   // 2: Spire base
            (1.80f,  2.25f, "Hull_Primary"),  // 3: Bridge dome crown
            (2.45f,  1.90f, "Bridge_Glass"),  // 4: Bridge window upper rim
            (2.65f,  1.55f, "Bridge_Glass"),  // 5: Bridge window lower rim
            (3.80f,  1.45f, "Hull_Accent"),   // 6: Upper deck tier 1 step
            (5.50f,  1.10f, "Hull_Primary"),  // 7: Main upper saucer slope
            (7.50f,  0.60f, "Hull_Primary"),  // 8: Outer upper slope
            (8.75f,  0.20f, "Hull_Accent"),   // 9: Rim bevel upper
            (9.00f,  0.00f, "Hull_Accent"),   // 10: Equator knife-edge
            (8.80f, -0.15f, "Hull_Accent"),   // 11: Rim bevel lower
            (8.20f, -0.30f, "Hull_Primary"),  // 12: Underbelly outer lip
            (7.20f, -0.45f, "Plasma_Ring"),   // 13: Propulsion trench outer
            (6.50f, -0.38f, "Plasma_Ring"),   // 14: Propulsion trench inner
            (4.80f, -0.75f, "Hull_Primary"),  // 15: Underbelly inner curve
            (2.80f, -1.15f, "Hull_Accent"),   // 16: Reactor housing rim
            (1.60f, -1.40f, "Reactor_Core"),  // 17: Reactor emitter bowl
            (0.00f, -1.55f, "Reactor_Core")   // 18: Center tractor lens
        };

        int radialSegments = 48;
        int numP = profile.Length;

        // Generate vertices grid: [segment, profileIndex]
        int[,] grid = new int[radialSegments, numP];

        for (int s = 0; s < radialSegments; s++)
        {
            float angleDeg = (float)s / radialSegments * 360f;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);

            for (int p = 0; p < numP; p++)
            {
                float r = profile[p].r;
                float y = profile[p].y;
                Vector3 pos = new Vector3(sin * r, y, cos * r);

                // Approximate surface normal from neighboring profile points
                Vector3 tangentY;
                if (p == 0) tangentY = new Vector3(sin * (profile[1].r - profile[0].r), profile[1].y - profile[0].y, cos * (profile[1].r - profile[0].r));
                else if (p == numP - 1) tangentY = new Vector3(sin * (profile[p].r - profile[p - 1].r), profile[p].y - profile[p - 1].y, cos * (profile[p].r - profile[p - 1].r));
                else tangentY = new Vector3(sin * (profile[p + 1].r - profile[p - 1].r), profile[p + 1].y - profile[p - 1].y, cos * (profile[p + 1].r - profile[p - 1].r));

                Vector3 tangentPhi = new Vector3(cos, 0, -sin);
                Vector3 normal = Vector3.Cross(tangentPhi, tangentY).normalized;
                if (normal.y < 0 && p < 10) normal = -normal;
                if (normal.y > 0 && p >= 11) normal = -normal;

                Vector2 uv = new Vector2((float)s / radialSegments, (float)p / (numP - 1));
                grid[s, p] = obj.AddVertex(pos, uv, normal);
            }
        }

        // Connect quads between profile rings
        for (int p = 0; p < numP - 1; p++)
        {
            string groupName = profile[p + 1].group;
            obj.SetGroup(groupName);

            for (int s = 0; s < radialSegments; s++)
            {
                int nextS = (s + 1) % radialSegments;

                int v0 = grid[s, p];
                int v1 = grid[nextS, p];
                int v2 = grid[nextS, p + 1];
                int v3 = grid[s, p + 1];

                if (p == 0) // Apex fan
                {
                    obj.AddTriangle(v0, v2, v3);
                }
                else if (p == numP - 2) // Bottom center fan
                {
                    obj.AddTriangle(v0, v1, v2);
                }
                else
                {
                    obj.AddQuad(v0, v1, v2, v3);
                }
            }
        }

        // Add 4 Underbelly Landing / Docking Struts
        obj.SetGroup("Hull_Accent");
        for (int i = 0; i < 4; i++)
        {
            float legAngle = (i * 90f + 45f) * Mathf.Deg2Rad;
            Vector3 legCenter = new Vector3(Mathf.Sin(legAngle) * 5.6f, -0.6f, Mathf.Cos(legAngle) * 5.6f);
            AddBox(obj, legCenter, new Vector3(0.7f, 0.4f, 1.4f), Quaternion.Euler(0, i * 90f + 45f, 0));
        }

        // Add 4 Perimeter Directional Thrusters
        obj.SetGroup("Plasma_Ring");
        for (int i = 0; i < 4; i++)
        {
            float thAngle = (i * 90f) * Mathf.Deg2Rad;
            Vector3 thCenter = new Vector3(Mathf.Sin(thAngle) * 8.9f, 0.0f, Mathf.Cos(thAngle) * 8.9f);
            AddBox(obj, thCenter, new Vector3(0.9f, 0.15f, 0.35f), Quaternion.Euler(0, i * 90f, 0));
        }

        obj.Save(filePath);
    }

    // =========================================================================
    // 2. INTERIOR UFO COCKPIT BRIDGE (Detailed Command Room)
    // =========================================================================
    static void BuildCockpitInterior(string filePath)
    {
        var obj = new ObjWriter();

        // 2A. Cockpit Floor Dais (Octagonal high-tech platform, radius 2.6m, height 0.16m)
        obj.SetGroup("Cockpit_Dark");
        int floorSegs = 24;
        int[] floorTopVerts = new int[floorSegs];
        int[] floorBotVerts = new int[floorSegs];
        int centerTop = obj.AddVertex(new Vector3(0, 0.16f, 0), new Vector2(0.5f, 0.5f), Vector3.up);

        for (int i = 0; i < floorSegs; i++)
        {
            float ang = (float)i / floorSegs * Mathf.PI * 2f;
            float x = Mathf.Sin(ang) * 2.6f;
            float z = Mathf.Cos(ang) * 2.6f;
            Vector2 uv = new Vector2(Mathf.Sin(ang) * 0.5f + 0.5f, Mathf.Cos(ang) * 0.5f + 0.5f);
            floorTopVerts[i] = obj.AddVertex(new Vector3(x, 0.16f, z), uv, Vector3.up);
            floorBotVerts[i] = obj.AddVertex(new Vector3(x, 0f, z), uv, new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang)));
        }

        for (int i = 0; i < floorSegs; i++)
        {
            int next = (i + 1) % floorSegs;
            obj.AddTriangle(centerTop, floorTopVerts[i], floorTopVerts[next]);
            obj.AddQuad(floorTopVerts[i], floorTopVerts[next], floorBotVerts[next], floorBotVerts[i]);
        }

        // Subfloor Base Plate (Radius 3.0m, height 0.02m to 0.16m, sealing 100% of floor underside)
        AddCurvedWallStrip(obj, -180f, 180f, 32, 2.95f, 0.05f, 0.02f, 0.02f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -180f, 180f, 32, 2.95f, 2.95f, 0.02f, 0.16f, "Cockpit_Dark", true);

        // Floor Illuminated Circuit Trim Ring
        obj.SetGroup("Glow_Cyan");
        for (int i = 0; i < floorSegs; i++)
        {
            float ang1 = (float)i / floorSegs * Mathf.PI * 2f;
            float ang2 = (float)(i + 1) / floorSegs * Mathf.PI * 2f;
            Vector3 p1 = new Vector3(Mathf.Sin(ang1) * 2.45f, 0.165f, Mathf.Cos(ang1) * 2.45f);
            Vector3 p2 = new Vector3(Mathf.Sin(ang2) * 2.45f, 0.165f, Mathf.Cos(ang2) * 2.45f);
            Vector3 p3 = new Vector3(Mathf.Sin(ang2) * 2.50f, 0.165f, Mathf.Cos(ang2) * 2.50f);
            Vector3 p4 = new Vector3(Mathf.Sin(ang1) * 2.50f, 0.165f, Mathf.Cos(ang1) * 2.50f);
            int v1 = obj.AddVertex(p1, Vector2.zero, Vector3.up);
            int v2 = obj.AddVertex(p2, Vector2.one, Vector3.up);
            int v3 = obj.AddVertex(p3, Vector2.one, Vector3.up);
            int v4 = obj.AddVertex(p4, Vector2.zero, Vector3.up);
            obj.AddQuad(v1, v2, v3, v4);
        }

        // 2B. Wrap-Around Pilot Command Console (Arc -70° to +70°, radius 1.5m to 2.1m)
        obj.SetGroup("Cockpit_Dark");
        int consoleSegs = 16;
        float startDeg = -65f;
        float endDeg = 65f;
        int[,] conGrid = new int[consoleSegs + 1, 4]; // 0=base-inner, 1=base-outer, 2=desk-inner, 3=desk-outer

        for (int i = 0; i <= consoleSegs; i++)
        {
            float t = (float)i / consoleSegs;
            float deg = Mathf.Lerp(startDeg, endDeg, t);
            float rad = deg * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);

            Vector3 pBaseIn = new Vector3(sin * 1.55f, 0.16f, cos * 1.55f);
            Vector3 pBaseOut = new Vector3(sin * 2.15f, 0.16f, cos * 2.15f);
            Vector3 pDeskIn = new Vector3(sin * 1.50f, 0.75f, cos * 1.50f);
            Vector3 pDeskOut = new Vector3(sin * 2.10f, 0.98f, cos * 2.10f); // 25° forward rise

            conGrid[i, 0] = obj.AddVertex(pBaseIn, new Vector2(t, 0), -new Vector3(sin, 0, cos));
            conGrid[i, 1] = obj.AddVertex(pBaseOut, new Vector2(t, 0), new Vector3(sin, 0, cos));
            conGrid[i, 2] = obj.AddVertex(pDeskIn, new Vector2(t, 1), Vector3.up);
            conGrid[i, 3] = obj.AddVertex(pDeskOut, new Vector2(t, 1), Vector3.up);
        }

        for (int i = 0; i < consoleSegs; i++)
        {
            // Front console face facing pilot (CCW: 0, 2, next2, next0)
            obj.AddQuad(conGrid[i, 0], conGrid[i, 2], conGrid[i + 1, 2], conGrid[i + 1, 0]);
            // Console desktop surface (CCW: 2, 3, next3, next2)
            obj.AddQuad(conGrid[i, 2], conGrid[i, 3], conGrid[i + 1, 3], conGrid[i + 1, 2]);
            // Outer console face facing glass (CCW: 3, 1, next1, next3)
            obj.AddQuad(conGrid[i, 3], conGrid[i, 1], conGrid[i + 1, 1], conGrid[i + 1, 3]);
            // Console bottom face sealing underside
            obj.AddQuad(conGrid[i, 1], conGrid[i, 0], conGrid[i + 1, 0], conGrid[i + 1, 1]);
        }

        // Console Side Endcaps
        obj.AddQuad(conGrid[0, 0], conGrid[0, 2], conGrid[0, 3], conGrid[0, 1]);
        obj.AddQuad(conGrid[consoleSegs, 1], conGrid[consoleSegs, 3], conGrid[consoleSegs, 2], conGrid[consoleSegs, 0]);

        // 2C. 3 Angled Multi-Function Displays (MFDs) with High-Contrast Dark Screens & Cyan Bezels
        AddBezelScreen(obj, 0f, 1.78f, 0.88f, 0.88f, 0.40f, 22f);
        AddBezelScreen(obj, -35f, 1.76f, 0.88f, 0.78f, 0.37f, 22f);
        AddBezelScreen(obj, 35f, 1.76f, 0.88f, 0.78f, 0.37f, 22f);

        // Center Holographic Radar Emitter Lens (recessed low on lower console)
        obj.SetGroup("Glow_Gold");
        Vector3 dishCenter = new Vector3(0, 0.35f, 1.05f);
        AddCylinderMesh(obj, dishCenter, 0.10f, 0.015f, 16);

        // 2D. Pilot Command Chair & Cockpit Seating (Ergonomic luxury alien flight command throne)
        // Mounted at base Z = 0.00m, Y = 0.16m. Seating cushion at Z = 0.02m, Y = 0.42m.
        // Sleek shoulder-level backrest (stops cleanly at Y = 0.73m) ensures 100% open sightlines into the rear cabin.
        
        // Pedestal Mount & Swivel Base
        obj.SetGroup("Cockpit_Dark");
        AddCylinderMesh(obj, new Vector3(0, 0.22f, 0.00f), 0.24f, 0.22f, 16);
        AddCylinderMesh(obj, new Vector3(0, 0.33f, 0.00f), 0.32f, 0.04f, 16);
        obj.SetGroup("Glow_Cyan");
        AddRingMesh(obj, new Vector3(0, 0.25f, 0.00f), 0.22f, 0.26f, 0.02f, 16);

        // Bucket Seat Base Frame & Thigh Bolsters
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(0, 0.38f, 0.00f), new Vector3(0.70f, 0.12f, 0.58f), Quaternion.identity);
        AddBox(obj, new Vector3(-0.33f, 0.44f, 0.00f), new Vector3(0.10f, 0.12f, 0.54f), Quaternion.Euler(0, 0, 15f));
        AddBox(obj, new Vector3(0.33f, 0.44f, 0.00f), new Vector3(0.10f, 0.12f, 0.54f), Quaternion.Euler(0, 0, -15f));

        // Seat Cushion (Light Platinum Ergonomic Flight Leather)
        obj.SetGroup("Cockpit_Light");
        AddBox(obj, new Vector3(0, 0.42f, 0.02f), new Vector3(0.54f, 0.08f, 0.50f), Quaternion.identity);
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(0, 0.43f, 0.02f), new Vector3(0.56f, 0.015f, 0.52f), Quaternion.identity);

        // Ergonomic Lumbar Backrest (Tilted back 12°, stops cleanly at shoulder level Y = 0.73m)
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(0, 0.56f, -0.28f), new Vector3(0.62f, 0.34f, 0.12f), Quaternion.Euler(-12f, 0, 0));
        AddBox(obj, new Vector3(-0.31f, 0.56f, -0.26f), new Vector3(0.08f, 0.30f, 0.14f), Quaternion.Euler(-12f, 14f, 0));
        AddBox(obj, new Vector3(0.31f, 0.56f, -0.26f), new Vector3(0.08f, 0.30f, 0.14f), Quaternion.Euler(-12f, -14f, 0));

        // Lumbar Leather Cushion (Platinum leather)
        obj.SetGroup("Cockpit_Light");
        AddBox(obj, new Vector3(0, 0.56f, -0.23f), new Vector3(0.50f, 0.30f, 0.07f), Quaternion.Euler(-12f, 0, 0));
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(0, 0.56f, -0.22f), new Vector3(0.52f, 0.32f, 0.015f), Quaternion.Euler(-12f, 0, 0));

        // Solid Armored Rear Spine (Covers the entire back of the seat, 100% opaque and sealed)
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(0, 0.48f, -0.32f), new Vector3(0.56f, 0.44f, 0.10f), Quaternion.Euler(-12f, 0, 0));
        AddBox(obj, new Vector3(0, 0.28f, -0.22f), new Vector3(0.36f, 0.22f, 0.16f), Quaternion.identity);

        // Rear Power Core Node & Cyan Trim
        obj.SetGroup("Glow_Gold");
        AddCylinderMesh(obj, new Vector3(0, 0.56f, -0.38f), 0.08f, 0.03f, 16);
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(0, 0.68f, -0.35f), new Vector3(0.48f, 0.02f, 0.02f), Quaternion.Euler(-12f, 0, 0));
        AddBox(obj, new Vector3(0, 0.38f, -0.24f), new Vector3(0.52f, 0.02f, 0.02f), Quaternion.Euler(-12f, 0, 0));

        // Armrests with Flight Throttle & Joystick Controls
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(-0.40f, 0.50f, 0.05f), new Vector3(0.12f, 0.08f, 0.44f), Quaternion.identity);
        AddBox(obj, new Vector3(0.40f, 0.50f, 0.05f), new Vector3(0.12f, 0.08f, 0.44f), Quaternion.identity);
        obj.SetGroup("Cockpit_Light");
        AddBox(obj, new Vector3(-0.40f, 0.545f, 0.03f), new Vector3(0.10f, 0.02f, 0.34f), Quaternion.identity);
        AddBox(obj, new Vector3(0.40f, 0.545f, 0.03f), new Vector3(0.10f, 0.02f, 0.34f), Quaternion.identity);
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(-0.40f, 0.58f, 0.20f), new Vector3(0.04f, 0.09f, 0.04f), Quaternion.Euler(14f, 0, 0));
        AddBox(obj, new Vector3(0.40f, 0.58f, 0.20f), new Vector3(0.04f, 0.09f, 0.04f), Quaternion.Euler(14f, 0, 0));


        // Secondary Crew / Navigator Seats in Rear Cabin (-X and +X at Z = -1.20m)
        for (int side = -1; side <= 1; side += 2)
        {
            Vector3 seatPos = new Vector3(side * 1.35f, 0.16f, -1.20f);
            Quaternion seatRot = Quaternion.Euler(0, side * -25f, 0);

            obj.SetGroup("Cockpit_Dark");
            AddCylinderMesh(obj, seatPos + new Vector3(0, 0.14f, 0), 0.14f, 0.18f, 12);
            AddBox(obj, seatPos + seatRot * new Vector3(0, 0.28f, 0), new Vector3(0.52f, 0.10f, 0.48f), seatRot);
            AddBox(obj, seatPos + seatRot * new Vector3(0, 0.65f, -0.20f), new Vector3(0.48f, 0.62f, 0.12f), seatRot * Quaternion.Euler(-10f, 0, 0));
            obj.SetGroup("Cockpit_Light");
            AddBox(obj, seatPos + seatRot * new Vector3(0, 0.65f, -0.13f), new Vector3(0.40f, 0.54f, 0.06f), seatRot * Quaternion.Euler(-10f, 0, 0));
            obj.SetGroup("Glow_Cyan");
            AddBox(obj, seatPos + seatRot * new Vector3(0, 0.98f, -0.24f), new Vector3(0.32f, 0.025f, 0.025f), seatRot * Quaternion.Euler(-10f, 0, 0));
        }

        // 2E. LEFT WALL & SIDE AVIONICS CONSOLE (-24° to -115°)
        // 100% Solid curved aerodynamic titanium hull wall (zero transparency on left side)
        // Lower Hull Wall (Y = 0.16m to 1.10m)
        AddCurvedWallStrip(obj, -24f, -72f, 10, 2.65f, 2.65f, 0.16f, 1.10f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -24f, -72f, 10, 2.78f, 2.78f, 0.16f, 1.10f, "Cockpit_Dark", false);

        // Mid Wall (-24° to -72°: Solid Bulkhead housing Left Panoramic Infogram Screen on pilot's left)
        // Positioned cleanly at r = 2.62m without any foreground obstructions
        AddCurvedWallStrip(obj, -24f, -72f, 12, 2.62f, 2.62f, 1.10f, 2.10f, "Screen_LeftPanoramic", true);
        AddCurvedWallStrip(obj, -24f, -72f, 12, 2.78f, 2.78f, 1.10f, 2.10f, "Cockpit_Dark", false);

        // Mid Wall (-72° to -98°: Left Lateral Observation Viewport)
        // Window Sill Lower (Y = 0.16m to 1.25m)
        AddCurvedWallStrip(obj, -72f, -98f, 6, 2.65f, 2.65f, 0.16f, 1.25f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -72f, -98f, 6, 2.78f, 2.78f, 0.16f, 1.25f, "Cockpit_Dark", false);
        // Window Glass Pane (Y = 1.25m to 1.95m)
        AddCurvedWallStrip(obj, -72f, -98f, 6, 2.66f, 2.66f, 1.25f, 1.95f, "GlassViewport", true);
        // Window Sill Upper (Y = 1.95m to 2.10f)
        AddCurvedWallStrip(obj, -72f, -98f, 6, 2.65f, 2.65f, 1.95f, 2.10f, "Cockpit_Light", true);
        AddCurvedWallStrip(obj, -72f, -98f, 6, 2.78f, 2.78f, 1.95f, 2.10f, "Cockpit_Dark", false);

        // Mid Wall (-98° to -115°: Solid Wall)
        AddCurvedWallStrip(obj, -98f, -115f, 4, 2.65f, 2.65f, 0.16f, 2.10f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -98f, -115f, 4, 2.78f, 2.78f, 0.16f, 2.10f, "Cockpit_Dark", false);

        // Upper Shoulder Wall (Y = 2.10m to 2.65m, curving inward to r = 2.20m)
        AddCurvedWallStrip(obj, -24f, -115f, 16, 2.65f, 2.20f, 2.10f, 2.65f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -24f, -115f, 16, 2.78f, 2.30f, 2.10f, 2.65f, "Cockpit_Dark", false);

        // Left Avionics Console Desks and Glowing Trim (Mounted in rear cabin at -78° to -112°, behind panoramic screen)
        int leftSegs = 4;
        for (int i = 0; i < leftSegs; i++)
        {
            float midA = Mathf.Lerp(-78f, -112f, (float)(i + 0.5f) / leftSegs);
            float midRad = midA * Mathf.Deg2Rad;
            Vector3 posDesk = new Vector3(Mathf.Sin(midRad) * 2.15f, 0.82f, Mathf.Cos(midRad) * 2.15f);
            Quaternion rotDesk = Quaternion.LookRotation(new Vector3(Mathf.Sin(midRad), 0, Mathf.Cos(midRad)), Vector3.up);

            obj.SetGroup("Cockpit_Dark");
            AddBox(obj, posDesk, new Vector3(0.55f, 0.40f, 0.60f), rotDesk);

            obj.SetGroup("Glow_Cyan");
            Vector3 lipPos = posDesk - rotDesk * new Vector3(0, -0.21f, 0.31f);
            AddBox(obj, lipPos, new Vector3(0.52f, 0.02f, 0.04f), rotDesk);

            // Side Display Screen
            obj.SetGroup("Cockpit_ScreenGlass");
            Vector3 scrPos = posDesk + rotDesk * new Vector3(0, 0.35f, 0.20f);
            AddBox(obj, scrPos, new Vector3(0.46f, 0.28f, 0.03f), rotDesk * Quaternion.Euler(-18f, 0, 0));
        }

        // 2F. RIGHT WALL & TACTICAL NAVIGATION CONSOLE (+24° to +115°)
        // 100% Solid curved aerodynamic titanium hull wall (zero transparency on right side)
        // Lower Hull Wall (Y = 0.16m to 1.10m)
        AddCurvedWallStrip(obj, 24f, 72f, 10, 2.65f, 2.65f, 0.16f, 1.10f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, 24f, 72f, 10, 2.78f, 2.78f, 0.16f, 1.10f, "Cockpit_Dark", false);

        // Mid Wall (+24° to +72°: Solid Bulkhead housing Right Panoramic Infogram Screen on pilot's right)
        // Positioned cleanly at r = 2.62m without any foreground obstructions
        AddCurvedWallStrip(obj, 24f, 72f, 12, 2.62f, 2.62f, 1.10f, 2.10f, "Screen_RightPanoramic", true);
        AddCurvedWallStrip(obj, 24f, 72f, 12, 2.78f, 2.78f, 1.10f, 2.10f, "Cockpit_Dark", false);

        // Mid Wall (+72° to +98°: Right Lateral Observation Viewport)
        // Window Sill Lower (Y = 0.16m to 1.25m)
        AddCurvedWallStrip(obj, 72f, 98f, 6, 2.65f, 2.65f, 0.16f, 1.25f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, 72f, 98f, 6, 2.78f, 2.78f, 0.16f, 1.25f, "Cockpit_Dark", false);
        // Window Glass Pane (Y = 1.25m to 1.95m)
        AddCurvedWallStrip(obj, 72f, 98f, 6, 2.66f, 2.66f, 1.25f, 1.95f, "GlassViewport", true);
        // Window Sill Upper (Y = 1.95m to 2.10f)
        AddCurvedWallStrip(obj, 72f, 98f, 6, 2.65f, 2.65f, 1.95f, 2.10f, "Cockpit_Light", true);
        AddCurvedWallStrip(obj, 72f, 98f, 6, 2.78f, 2.78f, 1.95f, 2.10f, "Cockpit_Dark", false);

        // Mid Wall (+98° to +115°: Solid Wall)
        AddCurvedWallStrip(obj, 98f, 115f, 4, 2.65f, 2.65f, 0.16f, 2.10f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, 98f, 115f, 4, 2.78f, 2.78f, 0.16f, 2.10f, "Cockpit_Dark", false);

        // Upper Shoulder Wall (Y = 2.10m to 2.65m, curving inward to r = 2.20m)
        AddCurvedWallStrip(obj, 24f, 115f, 16, 2.65f, 2.20f, 2.10f, 2.65f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, 24f, 115f, 16, 2.78f, 2.30f, 2.10f, 2.65f, "Cockpit_Dark", false);

        // Right Tactical Desks and Glowing Trim (Mounted in rear cabin at 78° to 112°, behind panoramic screen)
        int rightSegs = 4;
        for (int i = 0; i < rightSegs; i++)
        {
            float midA = Mathf.Lerp(78f, 112f, (float)(i + 0.5f) / rightSegs);
            float midRad = midA * Mathf.Deg2Rad;
            Vector3 posDesk = new Vector3(Mathf.Sin(midRad) * 2.15f, 0.82f, Mathf.Cos(midRad) * 2.15f);
            Quaternion rotDesk = Quaternion.LookRotation(new Vector3(Mathf.Sin(midRad), 0, Mathf.Cos(midRad)), Vector3.up);

            obj.SetGroup("Cockpit_Dark");
            AddBox(obj, posDesk, new Vector3(0.55f, 0.40f, 0.60f), rotDesk);

            obj.SetGroup("Glow_Cyan");
            Vector3 lipPos = posDesk - rotDesk * new Vector3(0, -0.21f, 0.31f);
            AddBox(obj, lipPos, new Vector3(0.52f, 0.02f, 0.04f), rotDesk);

            // Side Display Screen
            obj.SetGroup("Cockpit_ScreenGlass");
            Vector3 scrPos = posDesk + rotDesk * new Vector3(0, 0.35f, 0.20f);
            AddBox(obj, scrPos, new Vector3(0.46f, 0.28f, 0.03f), rotDesk * Quaternion.Euler(-18f, 0, 0));
        }

        // 2G. Canopy Structural Arch Ribs (Framing canopy and rear cabin, completely clear of panoramic screens)
        float[] ribAngles = { -105f, -24f, 24f, 105f };
        for (int r = 0; r < ribAngles.Length; r++)
        {
            obj.SetGroup("Cockpit_Dark");
            float rad = ribAngles[r] * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            int ribSteps = 8;
            Vector3[] ribPts = new Vector3[ribSteps + 1];
            for (int s = 0; s <= ribSteps; s++)
            {
                float t = (float)s / ribSteps;
                float curR = Mathf.Lerp(2.65f, 0.35f, t);
                float curY = Mathf.Lerp(0.16f, 2.75f, Mathf.Sin(t * Mathf.PI * 0.5f));
                ribPts[s] = new Vector3(sin * curR, curY, cos * curR);
            }

            for (int s = 0; s < ribSteps; s++)
            {
                Vector3 mid = (ribPts[s] + ribPts[s + 1]) * 0.5f;
                Vector3 dir = (ribPts[s + 1] - ribPts[s]).normalized;
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                float len = Vector3.Distance(ribPts[s], ribPts[s + 1]);
                AddBox(obj, mid, new Vector3(0.14f, 0.18f, len * 1.05f), rot);
            }

            // Inner illuminated cyan light conduit
            obj.SetGroup("Glow_Cyan");
            for (int s = 0; s < ribSteps - 1; s++)
            {
                Vector3 mid = (ribPts[s] + ribPts[s + 1]) * 0.5f - new Vector3(sin, 0, cos) * 0.06f;
                Vector3 dir = (ribPts[s + 1] - ribPts[s]).normalized;
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                float len = Vector3.Distance(ribPts[s], ribPts[s + 1]);
                AddBox(obj, mid, new Vector3(0.04f, 0.04f, len * 0.95f), rot);
            }
        }

        // 2H. Forward Panoramic Canopy Glass Viewport (-24° to +24°, Y = 0.98m to 2.20m)
        // Glass Transparex Shield framing Planet Earth
        AddCurvedWallStrip(obj, -24f, 24f, 12, 2.25f, 2.25f, 0.98f, 2.20f, "GlassViewport", true);
        // Upper Canopy Brow (Y = 2.20m to 2.65m, curving inward to r = 2.20m)
        AddCurvedWallStrip(obj, -24f, 24f, 12, 2.25f, 2.20f, 2.20f, 2.65f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -24f, 24f, 12, 2.35f, 2.30f, 2.20f, 2.65f, "Cockpit_Dark", false);

        // Lower Hull Wall under front desk (-24° to +24°, Y = 0.16m to 0.75m)
        AddCurvedWallStrip(obj, -24f, 24f, 8, 2.65f, 2.65f, 0.16f, 0.75f, "Cockpit_Dark", true);

        // 2I. Continuous Ceiling Roof Dome (Sealing the entire top, 360°)
        // Arches inward from r = 2.20m at Y = 2.65m to r = 0.35m at Y = 2.78m
        AddCurvedWallStrip(obj, -180f, 180f, 32, 2.20f, 0.35f, 2.65f, 2.78f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, -180f, 180f, 32, 2.30f, 0.35f, 2.65f, 2.82f, "Cockpit_Dark", false);

        // Central Ceiling Medallion & Cabin Downlight Core
        obj.SetGroup("Cockpit_Light");
        AddCylinderMesh(obj, new Vector3(0, 2.76f, 0), 0.38f, 0.04f, 16);
        obj.SetGroup("Glow_Cyan");
        AddRingMesh(obj, new Vector3(0, 2.75f, 0), 0.28f, 0.36f, 0.015f, 16);

        // Overhead Avionics Console (Suspended directly below the ceiling roof)
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(0, 2.60f, 0.55f), new Vector3(1.30f, 0.18f, 1.50f), Quaternion.Euler(14f, 0, 0));
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(0, 2.50f, 0.55f), new Vector3(1.05f, 0.02f, 1.25f), Quaternion.Euler(14f, 0, 0));

        // 2J. Rear Bulkhead Wall & Airlock Door (115° to 245° / -115°, sealing behind pilot)
        // 100% Solid curved rear bulkhead
        AddCurvedWallStrip(obj, 115f, 245f, 16, 2.65f, 2.20f, 0.16f, 2.65f, "Cockpit_Dark", true);
        AddCurvedWallStrip(obj, 115f, 245f, 16, 2.78f, 2.30f, 0.16f, 2.65f, "Cockpit_Dark", false);

        // Heavy Airlock Frame at Z = -2.45m
        obj.SetGroup("Cockpit_Light");
        AddBox(obj, new Vector3(-0.95f, 1.45f, -2.45f), new Vector3(0.35f, 2.30f, 0.30f), Quaternion.identity);
        AddBox(obj, new Vector3(0.95f, 1.45f, -2.45f), new Vector3(0.35f, 2.30f, 0.30f), Quaternion.identity);
        AddBox(obj, new Vector3(0f, 2.55f, -2.45f), new Vector3(2.25f, 0.35f, 0.30f), Quaternion.identity);

        // Sliding Pressure Door Panel
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, new Vector3(0f, 1.35f, -2.48f), new Vector3(1.55f, 2.05f, 0.12f), Quaternion.identity);

        // Door Lock Wheel Hub
        obj.SetGroup("Glow_Gold");
        AddCylinderMesh(obj, new Vector3(0f, 1.35f, -2.41f), 0.24f, 0.05f, 12);

        obj.Save(filePath);
    }

    // =========================================================================
    // 3. THE TERRAN EMBASSY ROOM (First Contact Archive & Observatory)
    // =========================================================================
    [MenuItem("XENOASIS/Generate Terran Embassy Room Model")]
    public static void GenerateTerranEmbassyRoomModel()
    {
        Debug.Log("[XENOASIS] Generating production-grade 3D Terran Embassy Room Model...");
        string modelsDir = "Assets/Models/Chamber";
        if (!Directory.Exists(modelsDir)) Directory.CreateDirectory(modelsDir);

        string roomPath = Path.Combine(modelsDir, "TerranEmbassy_Room.obj");
        BuildTerranEmbassyRoom(roomPath);

        AssetDatabase.Refresh();
        ConfigureImportedModel(roomPath);
        CreateChamberPrefab();

        Debug.Log("[XENOASIS] ✔ Successfully generated Terran Embassy Room: Assets/Prefabs/TerranEmbassy_Room.prefab");
    }

    public static void CreateChamberPrefab()
    {
        string prefabsDir = "Assets/Prefabs";
        if (!Directory.Exists(prefabsDir)) Directory.CreateDirectory(prefabsDir);

        Material scifiWallDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_Dark.mat");
        Material scifiWallWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiWall_White.mat");
        Material scifiFloorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SciFiFloor.mat");
        Material glassViewportMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/GlassViewport.mat");
        Material cyanGlowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        Material goldGlowMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveGold.mat");

        GameObject roomModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Chamber/TerranEmbassy_Room.obj");
        if (roomModel != null)
        {
            GameObject roomInstance = Instantiate(roomModel);
            roomInstance.name = "TerranEmbassy_Room";

            foreach (var mr in roomInstance.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr.name.Contains("Chamber_Floor")) mr.sharedMaterial = scifiFloorMat;
                else if (mr.name.Contains("Chamber_Dark")) mr.sharedMaterial = scifiWallDark;
                else if (mr.name.Contains("Chamber_White")) mr.sharedMaterial = scifiWallWhite;
                else if (mr.name.Contains("Chamber_Glass")) mr.sharedMaterial = glassViewportMat;
                else if (mr.name.Contains("Glow_Cyan")) mr.sharedMaterial = cyanGlowMat;
                else if (mr.name.Contains("Glow_Gold")) mr.sharedMaterial = goldGlowMat;
            }

            string prefabPath = "Assets/Prefabs/TerranEmbassy_Room.prefab";
            PrefabUtility.SaveAsPrefabAsset(roomInstance, prefabPath);
            DestroyImmediate(roomInstance);
            Debug.Log("[XENOASIS] Created prefab: " + prefabPath);
        }
        AssetDatabase.SaveAssets();
    }

    public static void BuildTerranEmbassyRoom(string filePath)
    {
        var obj = new ObjWriter();

        // 3A. Main Chamber Floor (Octagonal / Cylindrical disc r=8.8m)
        obj.SetGroup("Chamber_Floor");
        AddCylinderMesh(obj, new Vector3(0, -0.05f, 0), 8.8f, 0.10f, 36);

        // Floor Runway Light Strips from Airlock (Z=-8.5) to Central Dais (Z=-1.8)
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(-1.25f, 0.02f, -4.8f), new Vector3(0.08f, 0.03f, 6.4f), Quaternion.identity);
        AddBox(obj, new Vector3(1.25f, 0.02f, -4.8f), new Vector3(0.08f, 0.03f, 6.4f), Quaternion.identity);

        // Perimeter Floor Guide Track (Curved glow ribbon at r=8.4m)
        int trackSegs = 28;
        for (int i = 0; i < trackSegs; i++)
        {
            float a1 = Mathf.Lerp(-85f, 265f, (float)i / trackSegs) * Mathf.Deg2Rad;
            float a2 = Mathf.Lerp(-85f, 265f, (float)(i + 1) / trackSegs) * Mathf.Deg2Rad;
            Vector3 p1 = new Vector3(Mathf.Sin(a1) * 8.4f, 0.02f, Mathf.Cos(a1) * 8.4f);
            Vector3 p2 = new Vector3(Mathf.Sin(a2) * 8.4f, 0.02f, Mathf.Cos(a2) * 8.4f);
            Vector3 mid = (p1 + p2) * 0.5f;
            Vector3 dir = (p2 - p1).normalized;
            float len = Vector3.Distance(p1, p2);
            AddBox(obj, mid, new Vector3(0.06f, 0.02f, len * 1.05f), Quaternion.LookRotation(dir, Vector3.up));
        }

        // 3B. Central Landing Dais & Holo-Table Plinth
        // Elevated Central Landing Pad for Alien Traveler (Z = 0)
        obj.SetGroup("Chamber_Dark");
        AddCylinderMesh(obj, new Vector3(0, 0.05f, 0f), 2.20f, 0.10f, 32);
        obj.SetGroup("Glow_Cyan");
        AddRingMesh(obj, new Vector3(0, 0.101f, 0f), 2.05f, 2.20f, 0.012f, 32);
        AddRingMesh(obj, new Vector3(0, 0.101f, 0f), 1.15f, 1.25f, 0.012f, 24);

        // Dais Floor Alignment Inlays (Compass crosshairs pointing N, S, E, W)
        AddBox(obj, new Vector3(0, 0.102f, 0), new Vector3(0.06f, 0.005f, 4.0f), Quaternion.identity);
        AddBox(obj, new Vector3(0, 0.102f, 0), new Vector3(4.0f, 0.005f, 0.06f), Quaternion.identity);

        // Plinth for Civilization & DNA Holo-Table (Forward at Z = 2.4f)
        obj.SetGroup("Chamber_Dark");
        AddCylinderMesh(obj, new Vector3(0, 0.08f, 2.4f), 1.50f, 0.16f, 24);
        obj.SetGroup("Glow_Cyan");
        AddRingMesh(obj, new Vector3(0, 0.161f, 2.4f), 1.40f, 1.50f, 0.012f, 24);

        // 3C. Sample Pod Exhibition Plinths (3 Hexagonal Plinths)
        Vector3[] plinthPos = {
            new Vector3(-4.8f, 0.15f, 4.2f),  // Hydrosphere (Left)
            new Vector3(0f, 0.15f, 6.2f),     // Geosphere (Center)
            new Vector3(4.8f, 0.15f, 4.2f)    // Biosphere (Right)
        };
        for (int p = 0; p < 3; p++)
        {
            obj.SetGroup("Chamber_Dark");
            AddCylinderMesh(obj, plinthPos[p], 1.25f, 0.30f, 16);
            obj.SetGroup("Glow_Cyan");
            AddRingMesh(obj, plinthPos[p] + Vector3.up * 0.151f, 1.16f, 1.27f, 0.015f, 16);
        }

        // 3D. Curved Rear Bulkhead Walls (Radius 8.5m, arc 95° to 265°, height 5.2m)
        int wallPanels = 12;
        for (int i = 0; i < wallPanels; i++)
        {
            float a1 = Mathf.Lerp(95f, 265f, (float)i / wallPanels);
            float a2 = Mathf.Lerp(95f, 265f, (float)(i + 1) / wallPanels);
            float midDeg = (a1 + a2) * 0.5f;
            float midRad = midDeg * Mathf.Deg2Rad;
            Vector3 panelPos = new Vector3(Mathf.Sin(midRad) * 8.5f, 2.6f, Mathf.Cos(midRad) * 8.5f);
            Quaternion panelRot = Quaternion.Euler(0, midDeg + 180f, 0);

            // Alternating dark titanium and luminous white composite panels
            obj.SetGroup((i % 2 == 0) ? "Chamber_Dark" : "Chamber_White");
            float panelWidth = 2f * 8.5f * Mathf.Sin((a2 - a1) * 0.5f * Mathf.Deg2Rad) * 1.05f;
            AddBox(obj, panelPos, new Vector3(panelWidth, 5.2f, 0.25f), panelRot);

            // Recessed vertical cyan accent light conduit between panels
            obj.SetGroup("Glow_Cyan");
            Vector3 seamPos = new Vector3(Mathf.Sin(a1 * Mathf.Deg2Rad) * 8.42f, 2.6f, Mathf.Cos(a1 * Mathf.Deg2Rad) * 8.42f);
            AddBox(obj, seamPos, new Vector3(0.06f, 4.8f, 0.08f), Quaternion.Euler(0, a1 + 180f, 0));
        }

        // 3E. Rear Architectural Bulkhead & Diplomatic Airlock Portal (Z = -8.5m)
        obj.SetGroup("Chamber_Dark");
        // Heavy Jambs and Lintel
        AddBox(obj, new Vector3(-1.85f, 2.4f, -8.5f), new Vector3(0.65f, 4.8f, 0.65f), Quaternion.identity);
        AddBox(obj, new Vector3(1.85f, 2.4f, -8.5f), new Vector3(0.65f, 4.8f, 0.65f), Quaternion.identity);
        AddBox(obj, new Vector3(0f, 4.85f, -8.5f), new Vector3(4.35f, 0.70f, 0.65f), Quaternion.identity);

        // Status Indicator Bar over Airlock
        obj.SetGroup("Glow_Cyan");
        AddBox(obj, new Vector3(0f, 4.45f, -8.45f), new Vector3(2.6f, 0.10f, 0.05f), Quaternion.identity);

        // Sealed Rear Diplomatic Portal Panels
        obj.SetGroup("Chamber_White");
        AddBox(obj, new Vector3(-0.75f, 2.2f, -8.55f), new Vector3(1.45f, 4.3f, 0.12f), Quaternion.identity);
        AddBox(obj, new Vector3(0.75f, 2.2f, -8.55f), new Vector3(1.45f, 4.3f, 0.12f), Quaternion.identity);

        // 3F. Front Panoramic Observation Bay (Arc from -85° to +85° at radius 8.5m)
        int obsSegs = 9;
        float[] mullionAngles = { -80f, -60f, -40f, -20f, 0f, 20f, 40f, 60f, 80f };

        // Lower Balustrade Sill (Y = 0 to 0.75m)
        for (int i = 0; i < obsSegs; i++)
        {
            float a1 = Mathf.Lerp(-85f, 85f, (float)i / obsSegs);
            float a2 = Mathf.Lerp(-85f, 85f, (float)(i + 1) / obsSegs);
            float midDeg = (a1 + a2) * 0.5f;
            float midRad = midDeg * Mathf.Deg2Rad;
            Vector3 sillPos = new Vector3(Mathf.Sin(midRad) * 8.5f, 0.38f, Mathf.Cos(midRad) * 8.5f);
            Quaternion sillRot = Quaternion.Euler(0, midDeg, 0);
            float sillWidth = 2f * 8.5f * Mathf.Sin((a2 - a1) * 0.5f * Mathf.Deg2Rad) * 1.05f;

            // Sill Body
            obj.SetGroup("Chamber_Dark");
            AddBox(obj, sillPos, new Vector3(sillWidth, 0.75f, 0.35f), sillRot);

            // Sill Glowing Lip
            obj.SetGroup("Glow_Cyan");
            Vector3 lipPos = new Vector3(Mathf.Sin(midRad) * 8.45f, 0.76f, Mathf.Cos(midRad) * 8.45f);
            AddBox(obj, lipPos, new Vector3(sillWidth, 0.04f, 0.38f), sillRot);

            // Upper Window Header / Architrave (Y = 4.7m to 5.2m)
            obj.SetGroup("Chamber_Dark");
            Vector3 headerPos = new Vector3(Mathf.Sin(midRad) * 8.5f, 4.95f, Mathf.Cos(midRad) * 8.5f);
            AddBox(obj, headerPos, new Vector3(sillWidth, 0.50f, 0.35f), sillRot);

            // Panoramic Glass Panes (Seated neatly between sill and header)
            obj.SetGroup("Chamber_Glass");
            Vector3 glassPos = new Vector3(Mathf.Sin(midRad) * 8.5f, 2.72f, Mathf.Cos(midRad) * 8.5f);
            AddBox(obj, glassPos, new Vector3(sillWidth * 0.98f, 3.92f, 0.04f), sillRot);
        }

        // Aerodynamic Window Mullions / Columns
        obj.SetGroup("Chamber_Dark");
        for (int m = 0; m < mullionAngles.Length; m++)
        {
            float mRad = mullionAngles[m] * Mathf.Deg2Rad;
            Vector3 mPos = new Vector3(Mathf.Sin(mRad) * 8.5f, 2.6f, Mathf.Cos(mRad) * 8.5f);
            AddBox(obj, mPos, new Vector3(0.18f, 5.2f, 0.40f), Quaternion.Euler(0, mullionAngles[m], 0));
        }

        // 3G. Cantilevered Architectural Ceiling & Crystalline Glass Skylight Dome
        // Outer dark roof deck (r = 4.2m to 9.0m)
        obj.SetGroup("Chamber_Dark");
        AddRingMesh(obj, new Vector3(0, 5.25f, 0), 4.20f, 9.0f, 0.25f, 36);

        // Skylight Bezel Ring & Glow Rim (r = 4.05m to 4.25m)
        obj.SetGroup("Glow_Cyan");
        AddRingMesh(obj, new Vector3(0, 5.14f, 0), 4.05f, 4.25f, 0.04f, 36);

        // Crystalline Glass Skylight Dome Disc (r = 0 to 4.18m)
        obj.SetGroup("Chamber_Glass");
        AddCylinderMesh(obj, new Vector3(0, 5.22f, 0), 4.18f, 0.05f, 36);

        // Radial Skylight Struts / Ribs (8 sleek dark titanium mullions)
        obj.SetGroup("Chamber_Dark");
        for (int s = 0; s < 8; s++)
        {
            float spokeDeg = s * 45f;
            float spokeRad = spokeDeg * Mathf.Deg2Rad;
            Vector3 spokePos = new Vector3(Mathf.Sin(spokeRad) * 2.1f, 5.22f, Mathf.Cos(spokeRad) * 2.1f);
            Quaternion spokeRot = Quaternion.Euler(0, spokeDeg, 0);
            AddBox(obj, spokePos, new Vector3(0.10f, 0.08f, 4.2f), spokeRot);
        }

        obj.Save(filePath);
    }

    // =========================================================================
    // HELPER GEOMETRY PRIMITIVES FOR OBJ WRITER
    // =========================================================================
    static void AddBox(ObjWriter obj, Vector3 center, Vector3 size, Quaternion rotation)
    {
        Vector3 h = size * 0.5f;
        Vector3[] localVerts = new Vector3[]
        {
            new Vector3(-h.x, -h.y, -h.z), new Vector3( h.x, -h.y, -h.z),
            new Vector3( h.x,  h.y, -h.z), new Vector3(-h.x,  h.y, -h.z),
            new Vector3(-h.x, -h.y,  h.z), new Vector3( h.x, -h.y,  h.z),
            new Vector3( h.x,  h.y,  h.z), new Vector3(-h.x,  h.y,  h.z)
        };

        Vector3[] worldVerts = new Vector3[8];
        for (int i = 0; i < 8; i++) worldVerts[i] = center + rotation * localVerts[i];

        int[][] faces = new int[][]
        {
            new int[]{ 0, 3, 2, 1 }, // Front (-Z, CCW outward)
            new int[]{ 5, 6, 7, 4 }, // Back (+Z, CCW outward)
            new int[]{ 4, 7, 3, 0 }, // Left (-X, CCW outward)
            new int[]{ 1, 2, 6, 5 }, // Right (+X, CCW outward)
            new int[]{ 3, 7, 6, 2 }, // Top (+Y, CCW outward)
            new int[]{ 0, 1, 5, 4 }  // Bottom (-Y, CCW outward)
        };

        Vector3[] normals = new Vector3[]
        {
            rotation * Vector3.back, rotation * Vector3.forward,
            rotation * Vector3.left, rotation * Vector3.right,
            rotation * Vector3.up,   rotation * Vector3.down
        };

        for (int f = 0; f < 6; f++)
        {
            int v0 = obj.AddVertex(worldVerts[faces[f][0]], new Vector2(0, 0), normals[f]);
            int v1 = obj.AddVertex(worldVerts[faces[f][1]], new Vector2(1, 0), normals[f]);
            int v2 = obj.AddVertex(worldVerts[faces[f][2]], new Vector2(1, 1), normals[f]);
            int v3 = obj.AddVertex(worldVerts[faces[f][3]], new Vector2(0, 1), normals[f]);
            obj.AddQuad(v0, v1, v2, v3);
        }
    }

    static void AddCylinderMesh(ObjWriter obj, Vector3 center, float radius, float height, int segments)
    {
        float h = height * 0.5f;
        int[] topVerts = new int[segments];
        int[] botVerts = new int[segments];

        int topCenter = obj.AddVertex(center + Vector3.up * h, new Vector2(0.5f, 0.5f), Vector3.up);
        int botCenter = obj.AddVertex(center - Vector3.up * h, new Vector2(0.5f, 0.5f), Vector3.down);

        for (int i = 0; i < segments; i++)
        {
            float ang = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Sin(ang) * radius;
            float z = Mathf.Cos(ang) * radius;
            Vector3 norm = new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
            Vector2 uv = new Vector2(Mathf.Sin(ang) * 0.5f + 0.5f, Mathf.Cos(ang) * 0.5f + 0.5f);

            topVerts[i] = obj.AddVertex(center + new Vector3(x, h, z), uv, Vector3.up);
            botVerts[i] = obj.AddVertex(center + new Vector3(x, -h, z), uv, Vector3.down);
        }

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            obj.AddTriangle(topCenter, topVerts[i], topVerts[next]); // Top cap CCW outward
            obj.AddTriangle(botCenter, botVerts[next], botVerts[i]); // Bot cap CCW outward
            obj.AddQuad(topVerts[i], botVerts[i], botVerts[next], topVerts[next]); // Side CCW outward
        }
    }

    static void AddRingMesh(ObjWriter obj, Vector3 center, float innerRadius, float outerRadius, float height, int segments)
    {
        float h = height * 0.5f;
        int[] topIn = new int[segments];
        int[] topOut = new int[segments];
        int[] botIn = new int[segments];
        int[] botOut = new int[segments];

        for (int i = 0; i < segments; i++)
        {
            float ang = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(ang);
            float sin = Mathf.Sin(ang);

            topIn[i] = obj.AddVertex(center + new Vector3(sin * innerRadius, h, cos * innerRadius), new Vector2(0, 0), Vector3.up);
            topOut[i] = obj.AddVertex(center + new Vector3(sin * outerRadius, h, cos * outerRadius), new Vector2(1, 0), Vector3.up);
            botIn[i] = obj.AddVertex(center + new Vector3(sin * innerRadius, -h, cos * innerRadius), new Vector2(0, 1), Vector3.down);
            botOut[i] = obj.AddVertex(center + new Vector3(sin * outerRadius, -h, cos * outerRadius), new Vector2(1, 1), Vector3.down);
        }

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            obj.AddQuad(topIn[i], topIn[next], topOut[next], topOut[i]);
            obj.AddQuad(topOut[i], topOut[next], botOut[next], botOut[i]);
            obj.AddQuad(botIn[i], botIn[next], topIn[next], topIn[i]);
        }
    }

    static void AddCurvedWallStrip(ObjWriter obj, float startDeg, float endDeg, int segs, float rBot, float rTop, float yBot, float yTop, string groupName, bool facingInward = true)
    {
        obj.SetGroup(groupName);
        for (int i = 0; i < segs; i++)
        {
            float a1 = Mathf.Lerp(startDeg, endDeg, (float)i / segs) * Mathf.Deg2Rad;
            float a2 = Mathf.Lerp(startDeg, endDeg, (float)(i + 1) / segs) * Mathf.Deg2Rad;

            Vector3 v1_b = new Vector3(Mathf.Sin(a1) * rBot, yBot, Mathf.Cos(a1) * rBot);
            Vector3 v2_b = new Vector3(Mathf.Sin(a2) * rBot, yBot, Mathf.Cos(a2) * rBot);
            Vector3 v1_t = new Vector3(Mathf.Sin(a1) * rTop, yTop, Mathf.Cos(a1) * rTop);
            Vector3 v2_t = new Vector3(Mathf.Sin(a2) * rTop, yTop, Mathf.Cos(a2) * rTop);

            float midA = (a1 + a2) * 0.5f;
            Vector3 n = facingInward ? -new Vector3(Mathf.Sin(midA), 0, Mathf.Cos(midA)) : new Vector3(Mathf.Sin(midA), 0, Mathf.Cos(midA));

            float u1 = (float)i / segs;
            float u2 = (float)(i + 1) / segs;

            if (facingInward)
            {
                int p1 = obj.AddVertex(v1_b, new Vector2(u1, 0), n);
                int p2 = obj.AddVertex(v2_b, new Vector2(u2, 0), n);
                int p3 = obj.AddVertex(v2_t, new Vector2(u2, 1), n);
                int p4 = obj.AddVertex(v1_t, new Vector2(u1, 1), n);
                obj.AddQuad(p1, p2, p3, p4);
            }
            else
            {
                int p1 = obj.AddVertex(v2_b, new Vector2(u2, 0), n);
                int p2 = obj.AddVertex(v1_b, new Vector2(u1, 0), n);
                int p3 = obj.AddVertex(v1_t, new Vector2(u1, 1), n);
                int p4 = obj.AddVertex(v2_t, new Vector2(u2, 1), n);
                obj.AddQuad(p1, p2, p3, p4);
            }
        }
    }

    static void AddScreenPanel(ObjWriter obj, float angleDeg, float radius, float y, float width, float height, float tiltDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, y, Mathf.Cos(rad) * radius);
        Quaternion rot = Quaternion.Euler(-tiltDeg, angleDeg, 0);
        AddBox(obj, pos, new Vector3(width, height, 0.02f), rot);
    }

    static void AddBezelScreen(ObjWriter obj, float angleDeg, float radius, float y, float width, float height, float tiltDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, y, Mathf.Cos(rad) * radius);
        Quaternion rot = Quaternion.Euler(-tiltDeg, angleDeg, 0);

        // 1. Structural Backing Housing Frame (Cockpit_Dark) - sits slightly recessed behind the screen
        obj.SetGroup("Cockpit_Dark");
        AddBox(obj, pos + rot * new Vector3(0, 0, 0.012f), new Vector3(width + 0.04f, height + 0.04f, 0.020f), rot);

        // 2. High-Contrast Obsidian Glass Screen Surface (Cockpit_Screen)
        obj.SetGroup("Cockpit_Screen");
        AddBox(obj, pos, new Vector3(width, height, 0.008f), rot);

        // 3. Precision Glowing Bezel Border Lines (Glow_Cyan)
        obj.SetGroup("Glow_Cyan");
        float bw = 0.006f;
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;
        // Top and Bottom Trim
        AddBox(obj, pos + rot * new Vector3(0, halfH, -0.005f), new Vector3(width + 0.012f, bw, 0.004f), rot);
        AddBox(obj, pos + rot * new Vector3(0, -halfH, -0.005f), new Vector3(width + 0.012f, bw, 0.004f), rot);
        // Left and Right Trim
        AddBox(obj, pos + rot * new Vector3(-halfW, 0, -0.005f), new Vector3(bw, height, 0.004f), rot);
        AddBox(obj, pos + rot * new Vector3(halfW, 0, -0.005f), new Vector3(bw, height, 0.004f), rot);
    }

    // =========================================================================
    // MODEL IMPORTER CONFIGURATION
    // =========================================================================
    static void ConfigureImportedModel(string path)
    {
        ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
        if (mi != null)
        {
            mi.globalScale = 1.0f;
            mi.importNormals = ModelImporterNormals.Calculate;
            mi.normalSmoothingAngle = 65f;
            mi.importBlendShapes = false;
            mi.importAnimation = false;
            mi.addCollider = true;
            mi.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            mi.SaveAndReimport();
        }
    }

    // =========================================================================
    // SIMPLE INTERNAL WAVEFRONT OBJ WRITER
    // =========================================================================
    class ObjWriter
    {
        private List<Vector3> verts = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector3> normals = new List<Vector3>();
        private Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
        private string curGroup = "Default";

        public void SetGroup(string name)
        {
            curGroup = name;
            if (!groups.ContainsKey(curGroup)) groups[curGroup] = new List<string>();
        }

        public int AddVertex(Vector3 v, Vector2 uv, Vector3 n)
        {
            verts.Add(v);
            uvs.Add(uv);
            normals.Add(n);
            return verts.Count;
        }

        public void AddTriangle(int v1, int v2, int v3)
        {
            if (!groups.ContainsKey(curGroup)) groups[curGroup] = new List<string>();
            groups[curGroup].Add($"f {v1}/{v1}/{v1} {v2}/{v2}/{v2} {v3}/{v3}/{v3}");
        }

        public void AddQuad(int v1, int v2, int v3, int v4)
        {
            AddTriangle(v1, v2, v3);
            AddTriangle(v1, v3, v4);
        }

        public void Save(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# XENOASIS Real 3D UFO Spaceship Model");
            sb.AppendLine($"# Generated {System.DateTime.Now}");

            foreach (var v in verts)
                sb.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "v {0:F4} {1:F4} {2:F4}", v.x, v.y, v.z));
            foreach (var uv in uvs)
                sb.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "vt {0:F4} {1:F4}", uv.x, uv.y));
            foreach (var n in normals)
                sb.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "vn {0:F4} {1:F4} {2:F4}", n.x, n.y, n.z));

            foreach (var kvp in groups)
            {
                sb.AppendLine($"g {kvp.Key}");
                sb.AppendLine($"usemtl {kvp.Key}");
                foreach (var face in kvp.Value)
                    sb.AppendLine(face);
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
#endif
}
