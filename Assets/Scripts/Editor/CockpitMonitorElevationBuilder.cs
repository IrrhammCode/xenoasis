using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CockpitMonitorElevationBuilder
{
    [MenuItem("Xenoasis/Elevate Cockpit Monitors")]
    public static string ElevateMonitors()
    {
        var enc = GameObject.Find("UFO_Cockpit_Enclosure");
        if (enc == null) return "UFO_Cockpit_Enclosure not found in scene!";

        var darkMf = enc.transform.Find("Cockpit_Dark")?.GetComponent<MeshFilter>();
        var glowMf = enc.transform.Find("Glow_Cyan")?.GetComponent<MeshFilter>();
        if (darkMf == null || glowMf == null) return "MeshFilters not found!";

        // Load base meshes
        Mesh darkMesh = darkMf.sharedMesh;
        Mesh glowMesh = glowMf.sharedMesh;

        // Elevation offset: +14cm UP in Y, 8cm CLOSER in Z towards pilot
        Vector3 offset = new Vector3(0f, 0.14f, -0.08f);

        // 1. Split Glow_Cyan into: non-monitor trim, Center bezel, Left bezel, Right bezel
        Vector3[] gVerts = glowMesh.vertices;
        int[] gTris = glowMesh.triangles;
        Vector3[] gNormals = glowMesh.normals;
        Vector2[] gUVs = glowMesh.uv;

        List<int> keepGlowTris = new List<int>();
        List<int> centerBezelTris = new List<int>();
        List<int> leftBezelTris = new List<int>();
        List<int> rightBezelTris = new List<int>();

        for (int i = 0; i < gTris.Length; i += 3)
        {
            Vector3 v0 = gVerts[gTris[i]];
            Vector3 v1 = gVerts[gTris[i + 1]];
            Vector3 v2 = gVerts[gTris[i + 2]];
            Vector3 c = (v0 + v1 + v2) / 3f;

            if (c.z > 1.1f && c.y > 0.6f && c.y < 1.3f && Mathf.Abs(c.x) < 1.6f)
            {
                if (Mathf.Abs(c.x) < 0.55f)
                {
                    centerBezelTris.Add(gTris[i]); centerBezelTris.Add(gTris[i + 1]); centerBezelTris.Add(gTris[i + 2]);
                }
                else if (c.x < -0.55f)
                {
                    leftBezelTris.Add(gTris[i]); leftBezelTris.Add(gTris[i + 1]); leftBezelTris.Add(gTris[i + 2]);
                }
                else
                {
                    rightBezelTris.Add(gTris[i]); rightBezelTris.Add(gTris[i + 1]); rightBezelTris.Add(gTris[i + 2]);
                }
            }
            else
            {
                keepGlowTris.Add(gTris[i]); keepGlowTris.Add(gTris[i + 1]); keepGlowTris.Add(gTris[i + 2]);
            }
        }

        // 2. Split Cockpit_Dark into: keep (hull/table), Center backplate (276..299), Right (300..323), Left (324..347)
        Vector3[] dVerts = darkMesh.vertices;
        int[] dTris = darkMesh.triangles;
        Vector3[] dNormals = darkMesh.normals;
        Vector2[] dUVs = darkMesh.uv;

        List<int> keepDarkTris = new List<int>();
        List<int> centerBackTris = new List<int>();
        List<int> rightBackTris = new List<int>();
        List<int> leftBackTris = new List<int>();

        for (int i = 0; i < dTris.Length; i += 3)
        {
            int a = dTris[i], b = dTris[i + 1], c = dTris[i + 2];
            bool isCenterBack = (a >= 276 && a <= 299) || (b >= 276 && b <= 299) || (c >= 276 && c <= 299);
            bool isRightBack = (a >= 300 && a <= 323) || (b >= 300 && b <= 323) || (c >= 300 && c <= 323);
            bool isLeftBack = (a >= 324 && a <= 347) || (b >= 324 && b <= 347) || (c >= 324 && c <= 347);

            if (isCenterBack) { centerBackTris.Add(a); centerBackTris.Add(b); centerBackTris.Add(c); }
            else if (isRightBack) { rightBackTris.Add(a); rightBackTris.Add(b); rightBackTris.Add(c); }
            else if (isLeftBack) { leftBackTris.Add(a); leftBackTris.Add(b); leftBackTris.Add(c); }
            else { keepDarkTris.Add(a); keepDarkTris.Add(b); keepDarkTris.Add(c); }
        }

        // Clean hull and glow meshes (without old sunken monitor parts)
        Mesh cleanDarkMesh = ExtractSubmesh(dVerts, dNormals, dUVs, keepDarkTris, Vector3.zero);
        cleanDarkMesh.name = "Cockpit_Dark_Clean";
        SaveOrUpdateAsset(cleanDarkMesh, "Assets/Models/UFO/Cockpit_Dark_Clean.asset");

        Mesh cleanGlowMesh = ExtractSubmesh(gVerts, gNormals, gUVs, keepGlowTris, Vector3.zero);
        cleanGlowMesh.name = "Cockpit_Glow_Cyan_Clean";
        SaveOrUpdateAsset(cleanGlowMesh, "Assets/Models/UFO/Cockpit_Glow_Cyan_Clean.asset");

        darkMf.sharedMesh = cleanDarkMesh;
        glowMf.sharedMesh = cleanGlowMesh;

        // Elevated Bezel meshes
        Mesh centerBezel = ExtractSubmesh(gVerts, gNormals, gUVs, centerBezelTris, offset);
        Mesh leftBezel = ExtractSubmesh(gVerts, gNormals, gUVs, leftBezelTris, offset);
        Mesh rightBezel = ExtractSubmesh(gVerts, gNormals, gUVs, rightBezelTris, offset);

        // Elevated Backplate meshes
        Mesh centerBack = ExtractSubmesh(dVerts, dNormals, dUVs, centerBackTris, offset);
        Mesh leftBack = ExtractSubmesh(dVerts, dNormals, dUVs, leftBackTris, offset);
        Mesh rightBack = ExtractSubmesh(dVerts, dNormals, dUVs, rightBackTris, offset);

        // Combine Bezel + Backplate into unified Housing meshes (2 submeshes: 0 = Dark casing, 1 = Cyan glow bezel)
        Mesh centerHousing = CombineHousing(centerBack, centerBezel, "Cockpit_Monitor_Center_Housing");
        Mesh leftHousing = CombineHousing(leftBack, leftBezel, "Cockpit_Monitor_Left_Housing");
        Mesh rightHousing = CombineHousing(rightBack, rightBezel, "Cockpit_Monitor_Right_Housing");

        SaveOrUpdateAsset(centerHousing, "Assets/Models/UFO/Cockpit_Monitor_Center_Housing.asset");
        SaveOrUpdateAsset(leftHousing, "Assets/Models/UFO/Cockpit_Monitor_Left_Housing.asset");
        SaveOrUpdateAsset(rightHousing, "Assets/Models/UFO/Cockpit_Monitor_Right_Housing.asset");

        // Re-generate base screen quad meshes with offset baked in
        Mesh centerScreen = BuildScreenMesh(offset, 0);
        Mesh leftScreen = BuildScreenMesh(offset, 1);
        Mesh rightScreen = BuildScreenMesh(offset, 2);

        SaveOrUpdateAsset(centerScreen, "Assets/Models/UFO/Cockpit_Monitor_Center_Mesh.asset");
        SaveOrUpdateAsset(leftScreen, "Assets/Models/UFO/Cockpit_Monitor_Left_Mesh.asset");
        SaveOrUpdateAsset(rightScreen, "Assets/Models/UFO/Cockpit_Monitor_Right_Mesh.asset");

        // Generate Mounting Struts Mesh
        Mesh mountsMesh = BuildMountingStrutsMesh(enc.transform, offset);
        SaveOrUpdateAsset(mountsMesh, "Assets/Models/UFO/Cockpit_Monitor_Mounts.asset");

        // Mount everything in UFO_Cockpit_Enclosure
        Material darkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_HullDark.mat");
        Material cyanMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EmissiveCyan.mat");
        Material leftMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Left.mat");
        Material centerMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Center.mat");
        Material rightMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cockpit_Monitor_Right.mat");

        SetupMonitorObject(enc.transform, "Cockpit_Monitor_Center", centerScreen, centerMat, centerHousing, darkMat, cyanMat);
        SetupMonitorObject(enc.transform, "Cockpit_Monitor_Left", leftScreen, leftMat, leftHousing, darkMat, cyanMat);
        SetupMonitorObject(enc.transform, "Cockpit_Monitor_Right", rightScreen, rightMat, rightHousing, darkMat, cyanMat);

        // Mounting Struts GameObject
        Transform mountsT = enc.transform.Find("Monitor_Mounting_Brackets");
        if (mountsT == null)
        {
            var mountsObj = new GameObject("Monitor_Mounting_Brackets");
            mountsObj.transform.SetParent(enc.transform, false);
            mountsT = mountsObj.transform;
        }
        mountsT.localPosition = Vector3.zero;
        mountsT.localRotation = Quaternion.identity;
        mountsT.localScale = Vector3.one;
        var mountsMf = mountsT.GetComponent<MeshFilter>();
        if (mountsMf == null) mountsMf = mountsT.gameObject.AddComponent<MeshFilter>();
        var mountsMr = mountsT.GetComponent<MeshRenderer>();
        if (mountsMr == null) mountsMr = mountsT.gameObject.AddComponent<MeshRenderer>();
        mountsMf.sharedMesh = mountsMesh;
        mountsMr.sharedMaterials = new Material[] { darkMat, cyanMat };

        AssetDatabase.SaveAssets();
        return "Elevated cockpit monitors built and configured successfully!";
    }

    private static Mesh ExtractSubmesh(Vector3[] srcV, Vector3[] srcN, Vector2[] srcUV, List<int> triIndices, Vector3 vOffset)
    {
        Mesh m = new Mesh();
        Dictionary<int, int> remap = new Dictionary<int, int>();
        List<Vector3> nV = new List<Vector3>();
        List<Vector3> nN = new List<Vector3>();
        List<Vector2> nUV = new List<Vector2>();
        List<int> nT = new List<int>();

        for (int i = 0; i < triIndices.Count; i++)
        {
            int oldIdx = triIndices[i];
            if (!remap.TryGetValue(oldIdx, out int newIdx))
            {
                newIdx = nV.Count;
                remap[oldIdx] = newIdx;
                nV.Add(srcV[oldIdx] + vOffset);
                if (srcN != null && srcN.Length > oldIdx) nN.Add(srcN[oldIdx]);
                if (srcUV != null && srcUV.Length > oldIdx) nUV.Add(srcUV[oldIdx]);
            }
            nT.Add(newIdx);
        }
        m.SetVertices(nV);
        if (nN.Count == nV.Count) m.SetNormals(nN);
        if (nUV.Count == nV.Count) m.SetUVs(0, nUV);
        m.SetTriangles(nT, 0);
        m.RecalculateBounds();
        return m;
    }

    private static Mesh CombineHousing(Mesh back, Mesh bez, string name)
    {
        Mesh h = new Mesh();
        h.name = name;
        List<Vector3> verts = new List<Vector3>(back.vertices);
        List<Vector3> norms = new List<Vector3>(back.normals);
        List<Vector2> uvs = new List<Vector2>(back.uv);
        int backVertCount = verts.Count;

        verts.AddRange(bez.vertices);
        norms.AddRange(bez.normals);
        uvs.AddRange(bez.uv);

        h.SetVertices(verts);
        h.SetNormals(norms);
        h.SetUVs(0, uvs);
        h.subMeshCount = 2;

        h.SetTriangles(back.triangles, 0);

        int[] bezTris = bez.triangles;
        int[] offsetBezTris = new int[bezTris.Length];
        for (int i = 0; i < bezTris.Length; i++) offsetBezTris[i] = bezTris[i] + backVertCount;
        h.SetTriangles(offsetBezTris, 1);

        h.RecalculateBounds();
        return h;
    }

    private static Mesh BuildScreenMesh(Vector3 offset, int monitorIndex)
    {
        // monitorIndex: 0 = Center, 1 = Left, 2 = Right
        Mesh m = new Mesh();
        Vector3[] v = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] tris = new int[] { 0, 1, 2, 0, 2, 3 };

        if (monitorIndex == 0)
        {
            // Center monitor: bottom edge at Y=0.693, Z=1.851; top at Y=1.064, Z=1.701
            v[0] = new Vector3(-0.440f, 0.693f, 1.851f) + offset; // bottom-left
            v[1] = new Vector3(-0.440f, 1.064f, 1.701f) + offset; // top-left
            v[2] = new Vector3( 0.440f, 1.064f, 1.701f) + offset; // top-right
            v[3] = new Vector3( 0.440f, 0.693f, 1.851f) + offset; // bottom-right
        }
        else if (monitorIndex == 1)
        {
            // Left monitor (X < 0): OBJ coords are inverted in X or angled
            // From Cockpit_Screen verts: (-0.728, 0.707, 1.719) to (-1.367, 0.707, 1.272)
            v[0] = new Vector3(-1.367f, 0.707f, 1.272f) + offset; // outer-bottom
            v[1] = new Vector3(-1.287f, 1.050f, 1.158f) + offset; // outer-top
            v[2] = new Vector3(-0.648f, 1.050f, 1.606f) + offset; // inner-top
            v[3] = new Vector3(-0.728f, 0.707f, 1.719f) + offset; // inner-bottom
        }
        else
        {
            // Right monitor (X > 0)
            v[0] = new Vector3( 0.728f, 0.707f, 1.719f) + offset; // inner-bottom
            v[1] = new Vector3( 0.648f, 1.050f, 1.606f) + offset; // inner-top
            v[2] = new Vector3( 1.287f, 1.050f, 1.158f) + offset; // outer-top
            v[3] = new Vector3( 1.367f, 0.707f, 1.272f) + offset; // outer-bottom
        }

        uv[0] = new Vector2(0f, 0f);
        uv[1] = new Vector2(0f, 1f);
        uv[2] = new Vector2(1f, 1f);
        uv[3] = new Vector2(1f, 0f);

        m.vertices = v;
        m.uv = uv;
        m.triangles = tris;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    private static Mesh BuildMountingStrutsMesh(Transform enc, Vector3 offset)
    {
        // Build dual mounting struts under each of the 3 monitors
        // 2 submeshes: 0 = Dark metal pillars, 1 = Cyan glowing collars
        Mesh m = new Mesh();
        m.name = "Cockpit_Monitor_Mounts";

        List<Vector3> pillarVerts = new List<Vector3>();
        List<int> pillarTris = new List<int>();
        List<Vector3> collarVerts = new List<Vector3>();
        List<int> collarTris = new List<int>();

        Vector3[] tops = new Vector3[] {
            new Vector3(-0.25f, 0.693f + offset.y, 1.851f + offset.z),
            new Vector3( 0.25f, 0.693f + offset.y, 1.851f + offset.z),
            new Vector3(-1.15f, 0.707f + offset.y, 1.380f + offset.z),
            new Vector3(-0.80f, 0.707f + offset.y, 1.620f + offset.z),
            new Vector3( 0.80f, 0.707f + offset.y, 1.620f + offset.z),
            new Vector3( 1.15f, 0.707f + offset.y, 1.380f + offset.z)
        };
        Vector3[] bottoms = new Vector3[] {
            new Vector3(-0.25f, 0.745f, 1.76f),
            new Vector3( 0.25f, 0.745f, 1.76f),
            new Vector3(-1.15f, 0.745f, 1.35f),
            new Vector3(-0.80f, 0.745f, 1.58f),
            new Vector3( 0.80f, 0.745f, 1.58f),
            new Vector3( 1.15f, 0.745f, 1.35f)
        };

        for (int i = 0; i < tops.Length; i++)
        {
            AddCylinder(pillarVerts, pillarTris, bottoms[i], tops[i], 0.016f, 8);
            // Glowing collar ring at 75% height
            Vector3 cBottom = Vector3.Lerp(bottoms[i], tops[i], 0.70f);
            Vector3 cTop = Vector3.Lerp(bottoms[i], tops[i], 0.85f);
            AddCylinder(collarVerts, collarTris, cBottom, cTop, 0.022f, 8);
        }

        List<Vector3> allVerts = new List<Vector3>(pillarVerts);
        int pillarVertCount = allVerts.Count;
        allVerts.AddRange(collarVerts);

        int[] offsetCollarTris = new int[collarTris.Count];
        for (int i = 0; i < collarTris.Count; i++) offsetCollarTris[i] = collarTris[i] + pillarVertCount;

        m.SetVertices(allVerts);
        m.subMeshCount = 2;
        m.SetTriangles(pillarTris.ToArray(), 0);
        m.SetTriangles(offsetCollarTris, 1);
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    private static void AddCylinder(List<Vector3> verts, List<int> tris, Vector3 p0, Vector3 p1, float radius, int segments)
    {
        Vector3 dir = (p1 - p0).normalized;
        Vector3 side = Vector3.Cross(dir, Vector3.up).normalized;
        if (side.sqrMagnitude < 0.001f) side = Vector3.Cross(dir, Vector3.forward).normalized;
        Vector3 up = Vector3.Cross(side, dir).normalized;

        int baseIdx = verts.Count;
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector3 radial = (side * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
            verts.Add(p0 + radial);
            verts.Add(p1 + radial);
        }

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            int b0 = baseIdx + i * 2;
            int t0 = baseIdx + i * 2 + 1;
            int b1 = baseIdx + next * 2;
            int t1 = baseIdx + next * 2 + 1;

            tris.Add(b0); tris.Add(t0); tris.Add(b1);
            tris.Add(b1); tris.Add(t0); tris.Add(t1);
        }
    }

    private static void SetupMonitorObject(Transform encT, string monName, Mesh screenMesh, Material screenMat, Mesh housingMesh, Material darkMat, Material cyanMat)
    {
        Transform monT = encT.Find(monName);
        if (monT == null)
        {
            var monObj = new GameObject(monName);
            monObj.transform.SetParent(encT, false);
            monT = monObj.transform;
        }
        monT.localPosition = Vector3.zero;
        monT.localRotation = Quaternion.identity;
        monT.localScale = Vector3.one;

        var mf = monT.GetComponent<MeshFilter>();
        if (mf == null) mf = monT.gameObject.AddComponent<MeshFilter>();
        var mr = monT.GetComponent<MeshRenderer>();
        if (mr == null) mr = monT.gameObject.AddComponent<MeshRenderer>();
        mf.sharedMesh = screenMesh;
        mr.sharedMaterial = screenMat;

        Transform hT = monT.Find("Housing");
        if (hT == null)
        {
            var hObj = new GameObject("Housing");
            hObj.transform.SetParent(monT, false);
            hT = hObj.transform;
        }
        hT.localPosition = Vector3.zero;
        hT.localRotation = Quaternion.identity;
        hT.localScale = Vector3.one;

        var hmf = hT.GetComponent<MeshFilter>();
        if (hmf == null) hmf = hT.gameObject.AddComponent<MeshFilter>();
        var hmr = hT.GetComponent<MeshRenderer>();
        if (hmr == null) hmr = hT.gameObject.AddComponent<MeshRenderer>();
        hmf.sharedMesh = housingMesh;
        hmr.sharedMaterials = new Material[] { darkMat, cyanMat };
    }

    private static void SaveOrUpdateAsset(Mesh mesh, string path)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing == null)
        {
            AssetDatabase.CreateAsset(mesh, path);
        }
        else
        {
            existing.Clear();
            EditorUtility.CopySerialized(mesh, existing);
            AssetDatabase.SaveAssets();
        }
    }
}
