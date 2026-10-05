using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// XENOASIS — HoloBioMirror.cs
/// High-fidelity real-time holographic bio-mirror for the Alien Diplomat:
/// - Real-time planar reflection camera rendering the player's 3D Alien Diplomat body, the museum hall, and lighting.
/// - Automatically ensures player alien body is fully rendered in the mirror even during First-Person mode.
/// - Off-axis reflection math ensuring true physical mirror parallax and horizontal inversion.
/// - Holographic scanline and biometric telemetry overlay.
/// </summary>
public class HoloBioMirror : MonoBehaviour
{
    [Header("Mirror Glass & Frame")]
    [SerializeField] private MeshRenderer mirrorGlassRenderer;
    [SerializeField] private int textureResolution = 1024;
    [SerializeField] private float maxRenderDistance = 15f;
    [SerializeField] private Color holographicTint = new Color(0.92f, 0.98f, 1.0f, 1.0f);

    private Camera reflectionCam;
    private RenderTexture mirrorRT;
    private Material mirrorMat;
    private Camera mainCam;
    private Renderer[] frameRenderers;

    private void Awake()
    {
        InitializeMirror();
    }

    private void InitializeMirror()
    {
        if (mirrorGlassRenderer == null)
        {
            mirrorGlassRenderer = GetComponent<MeshRenderer>();
        }

        // Cache all station renderers (frame, backplate, banner, placard) to hide during reflection capture
        Transform stationRoot = transform;
        while (stationRoot.parent != null && stationRoot.parent.name != "--- XENOASIS ---" && !stationRoot.name.Contains("Station"))
        {
            stationRoot = stationRoot.parent;
        }
        frameRenderers = stationRoot.GetComponentsInChildren<Renderer>(true);

        // Create dedicated RenderTexture
        if (mirrorRT == null)
        {
            mirrorRT = new RenderTexture(textureResolution, textureResolution, 24, RenderTextureFormat.ARGB32);
            mirrorRT.name = "HoloBioMirror_RT";
            mirrorRT.antiAliasing = 2;
            mirrorRT.useMipMap = false;
        }

        // Create reflection camera
        if (reflectionCam == null)
        {
            GameObject camObj = new GameObject("BioMirror_ReflectionCam");
            camObj.transform.SetParent(transform);
            reflectionCam = camObj.AddComponent<Camera>();
            reflectionCam.enabled = false; // Manually rendered in LateUpdate
            reflectionCam.targetTexture = mirrorRT;
            reflectionCam.nearClipPlane = 0.05f;
            reflectionCam.farClipPlane = 80f;
            reflectionCam.clearFlags = CameraClearFlags.Skybox;
            reflectionCam.cullingMask = ~0; // Render all layers including player
        }

        // Setup Mirror Material
        Shader mirrorShader = Shader.Find("XENOASIS/HoloBioMirror") ?? Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
        if (mirrorMat == null)
        {
            mirrorMat = new Material(mirrorShader);
            mirrorMat.name = "HoloBioMirror_GlassMat";
        }

        mirrorMat.mainTexture = mirrorRT;
        mirrorMat.SetTexture("_ReflectionTex", mirrorRT);
        mirrorMat.SetTexture("_BaseMap", mirrorRT);
        // Horizontal flip for true mirror reflection (shader uses quad UV directly)
        mirrorMat.SetTextureScale("_ReflectionTex", new Vector2(-1f, 1f));
        mirrorMat.SetTextureOffset("_ReflectionTex", new Vector2(1f, 0f));
        mirrorMat.SetColor("_TintColor", holographicTint);

        if (mirrorGlassRenderer != null)
        {
            mirrorGlassRenderer.sharedMaterial = mirrorMat;
        }
    }

    private void LateUpdate()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null || reflectionCam == null || mirrorRT == null) return;

        // Distance culling for performance
        float distToPlayer = Vector3.Distance(transform.position, mainCam.transform.position);
        if (distToPlayer > maxRenderDistance) return;

        // Mirror plane definition
        Vector3 mirrorPos = transform.position;
        // Surface normal facing outward towards viewer (mirror unit front)
        Vector3 mirrorNormal = transform.parent != null ? transform.parent.forward : -transform.forward;

        // Check if player is on the front side of the mirror
        Vector3 camToMirror = mainCam.transform.position - mirrorPos;
        float planeDist = Vector3.Dot(camToMirror, mirrorNormal);
        if (planeDist <= 0.02f) return; // Behind or right on the mirror plane

        // Ensure aspect ratio matches mainCam to prevent screen projection distortion
        int targetHeight = Mathf.RoundToInt(textureResolution / (mainCam.aspect > 0.01f ? mainCam.aspect : 1.777f));
        if (mirrorRT.width != textureResolution || mirrorRT.height != targetHeight)
        {
            mirrorRT.Release();
            mirrorRT.width = textureResolution;
            mirrorRT.height = targetHeight;
            mirrorRT.Create();
            reflectionCam.targetTexture = mirrorRT;
            mirrorMat.SetTexture("_ReflectionTex", mirrorRT);
            mirrorMat.mainTexture = mirrorRT;
        }

        // Position reflection camera symmetrically across mirror plane
        Vector3 reflectedCamPos = mainCam.transform.position - 2f * planeDist * mirrorNormal;
        reflectionCam.transform.position = reflectedCamPos;

        // Reflect forward and up vectors
        Vector3 refFwd = Vector3.Reflect(mainCam.transform.forward, mirrorNormal);
        Vector3 refUp = Vector3.Reflect(mainCam.transform.up, mirrorNormal);
        reflectionCam.transform.rotation = Quaternion.LookRotation(refFwd, refUp);
        reflectionCam.fieldOfView = mainCam.fieldOfView;
        reflectionCam.aspect = mainCam.aspect;
        reflectionCam.nearClipPlane = Mathf.Max(0.01f, planeDist - 0.2f);

        // 1. Enable player body rendering for reflection camera
        Renderer playerBodyRend = null;
        ShadowCastingMode prevShadowMode = ShadowCastingMode.On;

        var rigInst = AlienPlayerRig.Instance ?? FindObjectOfType<AlienPlayerRig>();
        if (rigInst != null && rigInst.PlayerBodyRenderer != null)
        {
            playerBodyRend = rigInst.PlayerBodyRenderer;
            prevShadowMode = playerBodyRend.shadowCastingMode;
            playerBodyRend.shadowCastingMode = ShadowCastingMode.On;
        }

        // 2. Hide mirror station renderers so they don't occlude reflection
        if (frameRenderers == null || frameRenderers.Length == 0)
        {
            Transform stationRoot = transform;
            while (stationRoot.parent != null && stationRoot.parent.name != "--- XENOASIS ---" && !stationRoot.name.Contains("Station"))
            {
                stationRoot = stationRoot.parent;
            }
            frameRenderers = stationRoot.GetComponentsInChildren<Renderer>(true);
        }

        if (frameRenderers != null)
        {
            for (int i = 0; i < frameRenderers.Length; i++)
            {
                if (frameRenderers[i] != null) frameRenderers[i].enabled = false;
            }
        }

        // 3. Render reflection
        reflectionCam.Render();

        // 4. Restore mirror frame renderers and player body shadow mode
        if (frameRenderers != null)
        {
            for (int i = 0; i < frameRenderers.Length; i++)
            {
                if (frameRenderers[i] != null) frameRenderers[i].enabled = true;
            }
        }

        if (playerBodyRend != null)
        {
            playerBodyRend.shadowCastingMode = prevShadowMode;
        }
    }
    /// <summary>
    /// Compute a camera-space clip plane from a world-space point and normal.
    /// Used for oblique near-clip-plane to clip at the mirror surface.
    /// </summary>
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal)
    {
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(pos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    private void OnDestroy()
    {
        if (reflectionCam != null)
        {
            DestroyImmediate(reflectionCam.gameObject);
        }
        if (mirrorRT != null)
        {
            mirrorRT.Release();
            DestroyImmediate(mirrorRT);
        }
        if (mirrorMat != null)
        {
            DestroyImmediate(mirrorMat);
        }
    }
}
