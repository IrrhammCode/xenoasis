using UnityEngine;
using UnityEditor;

/// <summary>
/// XENOASIS — SceneSetupEditor.cs
/// Unity Editor utility to auto-assemble the Welcome Chamber scene hierarchy.
/// Run from Unity menu: XENOASIS > Setup Scene.
/// </summary>
public class SceneSetupEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XENOASIS/Setup Scene — Welcome Chamber")]
    public static void SetupWelcomeChamber()
    {
        // ===== ROOT OBJECTS =====
        GameObject root = CreateOrFind("--- XENOASIS ---");

        // ===== MANAGERS =====
        GameObject managers = CreateChildOrFind(root, "[Managers]");
        AddComponentIfMissing<GameManager>(managers);
        AddComponentIfMissing<OfferingManager>(managers);
        AddComponentIfMissing<AudioManager>(managers);

        // ===== ENVIRONMENT =====
        GameObject environment = CreateChildOrFind(root, "[Environment]");

        // Floor Platform
        GameObject floor = CreateChildOrFind(environment, "FloorPlatform");
        if (floor.GetComponent<MeshFilter>() == null)
        {
            var mf = floor.AddComponent<MeshFilter>();
            mf.sharedMesh = CreateCircleMesh(6f, 64); // 12m diameter
            floor.AddComponent<MeshRenderer>();
            floor.AddComponent<MeshCollider>();
        }
        floor.transform.position = Vector3.zero;

        // Pillars (4 arching obsidian pillars)
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f;
            Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * 5f;
            GameObject pillar = CreateChildOrFind(environment, $"ObsidianPillar_{i + 1}");
            pillar.transform.position = pos;
            pillar.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        // ===== OFFERINGS =====
        GameObject offerings = CreateChildOrFind(root, "[Offerings]");

        // Offering 1: Water Basin (center)
        GameObject waterBasin = CreateChildOrFind(offerings, "Offering1_WaterBasin");
        waterBasin.transform.position = new Vector3(0, 0.5f, 0);
        AddComponentIfMissing<HandProximityDetector>(waterBasin);
        AddComponentIfMissing<WaterSphereController>(waterBasin);

        GameObject waterSphere = CreateChildOrFind(waterBasin, "WaterSphere");
        if (waterSphere.GetComponent<MeshFilter>() == null)
        {
            waterSphere.AddComponent<MeshFilter>().sharedMesh =
                Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
            waterSphere.AddComponent<MeshRenderer>();
            waterSphere.AddComponent<SphereCollider>();
        }
        waterSphere.transform.localPosition = new Vector3(0, 0.5f, 0);
        waterSphere.transform.localScale = Vector3.one * 0.6f;

        // Offering 2: Crystal (left)
        GameObject crystal = CreateChildOrFind(offerings, "Offering2_Crystal");
        crystal.transform.position = new Vector3(-3f, 1f, 0);
        AddComponentIfMissing<HandProximityDetector>(crystal);
        AddComponentIfMissing<CrystalResonance>(crystal);

        GameObject crystalVis = CreateChildOrFind(crystal, "CrystalVisualizer");
        AddComponentIfMissing<SoundWaveVisualizer>(crystalVis);

        // Offering 3: Flora (right)
        GameObject flora = CreateChildOrFind(offerings, "Offering3_Flora");
        flora.transform.position = new Vector3(3f, 0.5f, 0);
        AddComponentIfMissing<HandProximityDetector>(flora);
        AddComponentIfMissing<FloraBloom>(flora);

        GameObject energyBeam = CreateChildOrFind(flora, "EnergyBeam");
        AddComponentIfMissing<EnergyBeamVFX>(energyBeam);

        // Offering Pedestals
        for (int i = 0; i < 3; i++)
        {
            string name = i == 0 ? "Pedestal_Water" : i == 1 ? "Pedestal_Crystal" : "Pedestal_Flora";
            GameObject pedestal = CreateChildOrFind(offerings, name);
            Vector3 pos = i == 0 ? new Vector3(0, 0, 0)
                        : i == 1 ? new Vector3(-3f, 0, 0)
                        : new Vector3(3f, 0, 0);
            pedestal.transform.position = pos;
        }

        // ===== VFX =====
        GameObject vfx = CreateChildOrFind(root, "[VFX]");

        GameObject stardust = CreateChildOrFind(vfx, "CosmicStardust");
        AddComponentIfMissing<StardustController>(stardust);
        if (stardust.GetComponent<ParticleSystem>() == null)
            stardust.AddComponent<ParticleSystem>();
        stardust.transform.position = Vector3.zero;

        GameObject beacon = CreateChildOrFind(vfx, "ClimaxBeacon");
        AddComponentIfMissing<BeaconClimaxVFX>(beacon);
        beacon.transform.position = new Vector3(0, 0.5f, 0);

        // ===== CAMERA & FADE =====
        GameObject cameraRig = CreateChildOrFind(root, "[CameraRig]");

        GameObject fadeCanvas = CreateChildOrFind(cameraRig, "FadeCanvas");
        AddComponentIfMissing<FadeController>(fadeCanvas);
        if (fadeCanvas.GetComponent<Canvas>() == null)
        {
            var canvas = fadeCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            fadeCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        }

        GameObject passthroughCtrl = CreateChildOrFind(cameraRig, "PassthroughController");
        AddComponentIfMissing<PassthroughTransition>(passthroughCtrl);

        // ===== LIGHTING =====
        GameObject lighting = CreateChildOrFind(root, "[Lighting]");

        GameObject dirLight = CreateChildOrFind(lighting, "DistantStarlight");
        if (dirLight.GetComponent<Light>() == null)
        {
            var light = dirLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.1f, 0.1f, 0.24f); // Cold blue #1A1A3E
            light.intensity = 0.15f;
        }
        dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        // ===== END SCREEN =====
        GameObject endScreen = CreateChildOrFind(root, "EndScreenCanvas");
        if (endScreen.GetComponent<Canvas>() == null)
        {
            var canvas = endScreen.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 998;
        }
        endScreen.SetActive(false);

        Debug.Log("[XENOASIS] Welcome Chamber scene hierarchy assembled successfully!");
        EditorUtility.DisplayDialog("XENOASIS", "Welcome Chamber scene setup complete!\n\nRemember to:\n1. Import Tripo .glb models into Assets/Models/Tripo/\n2. Assign materials to renderers\n3. Wire up SerializeField references in Inspector", "Got it!");
    }

    // ===== HELPERS =====

    static GameObject CreateOrFind(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static GameObject CreateChildOrFind(GameObject parent, string name)
    {
        Transform t = parent.transform.Find(name);
        if (t != null) return t.gameObject;
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    static T AddComponentIfMissing<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    static Mesh CreateCircleMesh(float radius, int segments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CirclePlatform";

        int vertCount = segments + 1;
        Vector3[] verts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] tris = new int[segments * 3];

        verts[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            verts[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            uvs[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);
        }

        for (int i = 0; i < segments; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
#endif
}
