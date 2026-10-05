using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// XENOASIS — ChamberRoofShutter.cs
/// Controls the animated mechanical iris roof shutter on the Grand Museum Pavilion skylight.
/// - 12 precision-engineered interlocking aerodynamic titanium blades with cyan seal guides.
/// - Initially OPEN (blades smoothly retracted into the 60cm roof slab casing, aperture radius 5.5m completely clear for the tractor beam).
/// - Once the traveler touches down, closes smoothly ("si gedung di tutup atap nya")
///   completely sealing the 11-meter aperture with zero gaps and activating the central hub seal.
/// </summary>
public class ChamberRoofShutter : MonoBehaviour
{
    public static ChamberRoofShutter Instance { get; private set; }

    [Header("Iris Shutter Configuration")]
    [SerializeField] private int bladeCount = 12;
    [SerializeField] private float openRadius = 5.80f;
    [SerializeField] private float closedRadius = 0.0f;
    [SerializeField] private float ceilingHeight = 8.45f;
    [SerializeField] private float closeDuration = 3.2f;

    public float CeilingHeight
    {
        get => ceilingHeight;
        set => ceilingHeight = value;
    }

    [Header("Materials")]
    [SerializeField] private Material shutterMaterial;
    [SerializeField] private Material sealGlowMaterial;

    [Header("Audio")]
    [SerializeField] private AudioSource shutterAudioSource;
    [SerializeField] private AudioClip servoCloseClip;

    [Header("Events")]
    public UnityEvent OnShutterClosingStarted;
    public UnityEvent OnShutterFullyClosed;

    public bool IsClosed { get; private set; } = false;

    private Transform[] blades;
    private Vector3[] openPositions;
    private Vector3[] closedPositions;
    private Quaternion[] bladeRotations;
    private GameObject centralSealCap;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        BuildIrisBlades();
    }

    public void BuildIrisBlades()
    {
        // Clean existing blades if any
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        blades = new Transform[bladeCount];
        openPositions = new Vector3[bladeCount];
        closedPositions = new Vector3[bladeCount];
        bladeRotations = new Quaternion[bladeCount];

        // Ensure materials
        if (shutterMaterial == null)
        {
            shutterMaterial = Resources.Load<Material>("SciFiWall_Dark") 
                ?? new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            shutterMaterial.name = "RoofShutter_PlateMat";
            shutterMaterial.color = new Color(0.12f, 0.13f, 0.16f, 1.0f);
        }

        if (sealGlowMaterial == null)
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            sealGlowMaterial = new Material(litShader);
            sealGlowMaterial.name = "RoofShutter_SealGoldMat";
            sealGlowMaterial.SetColor("_BaseColor", new Color(0.92f, 0.76f, 0.38f, 1.0f));
            sealGlowMaterial.SetFloat("_Metallic", 0.90f);
            sealGlowMaterial.SetFloat("_Smoothness", 0.85f);
            sealGlowMaterial.EnableKeyword("_EMISSION");
            sealGlowMaterial.SetColor("_EmissionColor", new Color(0.92f, 0.76f, 0.38f) * 0.5f);
        }

        // Shared procedural blade mesh: curved wedge leaf spanning 38 degrees (30 deg + 8 deg overlap)
        Mesh bladeMesh = CreateIrisBladeMesh(0.0f, 6.10f, 38f, 0.045f, 10);

        float angleStep = 360f / bladeCount;

        for (int i = 0; i < bladeCount; i++)
        {
            float angleDeg = i * angleStep;
            float rad = angleDeg * Mathf.Deg2Rad;

            Vector3 outwardDir = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            // In closed position, blade tip is at the center (r=0) with alternate slight Y stagger (4mm) to prevent z-fighting
            float yStagger = (i % 2 == 0 ? 0.004f : -0.004f);
            Vector3 inPos = transform.position + new Vector3(0, ceilingHeight + yStagger, 0);
            // In open position, blade is retracted outward by openRadius along its radial orientation
            Vector3 outPos = inPos + outwardDir * openRadius;

            GameObject bladeObj = new GameObject($"IrisBlade_{i:D2}");
            bladeObj.transform.SetParent(transform, false);
            bladeObj.transform.localRotation = Quaternion.Euler(0, angleDeg, 0);

            var mf = bladeObj.AddComponent<MeshFilter>();
            mf.sharedMesh = bladeMesh;
            var mr = bladeObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = shutterMaterial;

            // Glowing cyan interlocking seal conduit along leading edge
            GameObject glowEdge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glowEdge.name = "BladeSealGlow";
            glowEdge.transform.SetParent(bladeObj.transform, false);
            // Position along leading radial seam
            float edgeRad = (19f) * Mathf.Deg2Rad;
            glowEdge.transform.localPosition = new Vector3(Mathf.Sin(edgeRad) * 3.0f, 0.025f, Mathf.Cos(edgeRad) * 3.0f);
            glowEdge.transform.localRotation = Quaternion.Euler(0, 19f, 0);
            glowEdge.transform.localScale = new Vector3(0.045f, 0.015f, 5.8f);
            glowEdge.GetComponent<MeshRenderer>().sharedMaterial = sealGlowMaterial;
            Collider col = glowEdge.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);

            blades[i] = bladeObj.transform;
            openPositions[i] = outPos;
            closedPositions[i] = inPos;
            bladeRotations[i] = bladeObj.transform.rotation;

            // Start in OPEN position (retracted into roof slab casing)
            bladeObj.transform.position = outPos;
        }

        // Central Seal Cap (appears when fully closed to lock the exact center hub)
        centralSealCap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        centralSealCap.name = "CentralSealCap";
        centralSealCap.transform.SetParent(transform, false);
        centralSealCap.transform.localPosition = new Vector3(0, ceilingHeight + 0.035f, 0);
        centralSealCap.transform.localScale = new Vector3(1.35f, 0.04f, 1.35f);
        centralSealCap.GetComponent<MeshRenderer>().sharedMaterial = shutterMaterial;

        GameObject capRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        capRim.name = "CapRim_Glow";
        capRim.transform.SetParent(centralSealCap.transform, false);
        capRim.transform.localPosition = new Vector3(0, 0.52f, 0);
        capRim.transform.localScale = new Vector3(1.08f, 0.08f, 1.08f);
        capRim.GetComponent<MeshRenderer>().sharedMaterial = sealGlowMaterial;
        Collider rcol = capRim.GetComponent<Collider>();
        if (rcol != null) DestroyImmediate(rcol);

        Collider ccol = centralSealCap.GetComponent<Collider>();
        if (ccol != null) DestroyImmediate(ccol);
        centralSealCap.SetActive(false);
    }

    private static Mesh CreateIrisBladeMesh(float innerR, float outerR, float angleSpanDeg, float thickness, int segments)
    {
        Mesh mesh = new Mesh { name = "IrisBlade_ProceduralMesh" };
        var verts = new System.Collections.Generic.List<Vector3>();
        var norms = new System.Collections.Generic.List<Vector3>();
        var uvs = new System.Collections.Generic.List<Vector2>();
        var tris = new System.Collections.Generic.List<int>();

        // Top Face (+Y)
        int topStart = verts.Count;
        for (int s = 0; s <= segments; s++)
        {
            float t = (float)s / segments;
            float rad = (t * angleSpanDeg - angleSpanDeg * 0.5f) * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);
            verts.Add(new Vector3(sin * innerR, thickness * 0.5f, cos * innerR));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(0f, t));
            verts.Add(new Vector3(sin * outerR, thickness * 0.5f, cos * outerR));
            norms.Add(Vector3.up);
            uvs.Add(new Vector2(1f, t));
        }
        for (int s = 0; s < segments; s++)
        {
            int vi = topStart + s * 2;
            tris.Add(vi); tris.Add(vi + 2); tris.Add(vi + 1);
            tris.Add(vi + 1); tris.Add(vi + 2); tris.Add(vi + 3);
        }

        // Bottom Face (-Y)
        int btmStart = verts.Count;
        for (int s = 0; s <= segments; s++)
        {
            float t = (float)s / segments;
            float rad = (t * angleSpanDeg - angleSpanDeg * 0.5f) * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);
            verts.Add(new Vector3(sin * innerR, -thickness * 0.5f, cos * innerR));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(0f, t));
            verts.Add(new Vector3(sin * outerR, -thickness * 0.5f, cos * outerR));
            norms.Add(Vector3.down);
            uvs.Add(new Vector2(1f, t));
        }
        for (int s = 0; s < segments; s++)
        {
            int vi = btmStart + s * 2;
            tris.Add(vi); tris.Add(vi + 1); tris.Add(vi + 2);
            tris.Add(vi + 1); tris.Add(vi + 3); tris.Add(vi + 2);
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Smoothly closes the iris roof shutter.
    /// </summary>
    public void CloseShutter()
    {
        if (IsClosed) return;
        StopAllCoroutines();
        StartCoroutine(AnimateShutter(true));
    }

    /// <summary>
    /// Smoothly opens the iris roof shutter.
    /// </summary>
    public void OpenShutter()
    {
        if (!IsClosed) return;
        StopAllCoroutines();
        StartCoroutine(AnimateShutter(false));
    }

    /// <summary>
    /// Instantly sets the shutter to open or closed state without animation.
    /// Useful for editor setup, tests, and scene state initialization.
    /// </summary>
    public void SetShutterStateImmediate(bool closed)
    {
        if (blades == null || blades.Length == 0) BuildIrisBlades();
        for (int i = 0; i < bladeCount; i++)
        {
            if (blades != null && i < blades.Length && blades[i] != null)
            {
                blades[i].position = closed ? closedPositions[i] : openPositions[i];
            }
        }
        IsClosed = closed;
        if (centralSealCap != null) centralSealCap.SetActive(closed);
    }

    private IEnumerator AnimateShutter(bool close)
    {
        if (close) OnShutterClosingStarted?.Invoke();
        else if (centralSealCap != null) centralSealCap.SetActive(false);

        if (shutterAudioSource != null && servoCloseClip != null)
        {
            shutterAudioSource.PlayOneShot(servoCloseClip, 0.85f);
        }

        float elapsed = 0f;
        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / closeDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < bladeCount; i++)
            {
                if (blades != null && i < blades.Length && blades[i] != null)
                {
                    Vector3 from = close ? openPositions[i] : closedPositions[i];
                    Vector3 to = close ? closedPositions[i] : openPositions[i];
                    blades[i].position = Vector3.Lerp(from, to, smoothT);
                }
            }

            yield return null;
        }

        IsClosed = close;
        if (centralSealCap != null) centralSealCap.SetActive(close);

        if (close)
        {
            OnShutterFullyClosed?.Invoke();
            Debug.Log("[ChamberRoofShutter] ✦ Grand Iris Shutter Sealed 100%. Atmospheric Hermetic Seal Active.");
        }
    }
}
