using UnityEngine;
using System.IO;
using System.Collections;

/// <summary>
/// XENOASIS — LiveDescentRecorder.cs
/// Records high-res screenshots during the live sequence in Play Mode to verify all user requirements:
/// - Orbital target sector selection in deep space
/// - Progressive multi-stage atmospheric entry
/// - Vertical tractor beam descent from UFO
/// - Post-touchdown UFO ascent & motorized roof closing
/// - First-person alien hands & 3D alien character reflection
/// </summary>
public class LiveDescentRecorder : MonoBehaviour
{
    private static readonly string OutputDir = @"C:\Users\Irham\.gemini\antigravity-cli\brain\0a29a8e0-a0ce-4546-a7c8-d32da40355b5";

    private bool c0, c1, c2, c3, c4, c5, c6, c7, c8;
    private float timer = 0f;

    private void Start()
    {
        Debug.Log("[LiveDescentRecorder] ✦ Live sequence recorder active.");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // T = 2.5s: Orbital Sector Selection & First-Person Alien Hands in Space
        if (timer >= 2.5f && !c0)
        {
            c0 = true;
            CaptureFrame("stage_0_orbit_sector_selection.png", "Stage 0: Orbital Target Sector Selection with Alien Hands");
            // Automatically confirm sector selection after capturing frame so descent starts smoothly
            var sel = PlanetarySectorSelector.Instance;
            if (sel != null) sel.ConfirmSelection();
        }
        // T = 7.5s: Exosphere Approach
        else if (timer >= 7.5f && !c1)
        {
            c1 = true;
            CaptureFrame("stage_1_exosphere_orbit.png", "Stage 1: Exosphere Orbital Approach to Earth");
        }
        // T = 13.5s: Mesosphere Re-Entry Plasma Shockwave
        else if (timer >= 13.5f && !c2)
        {
            c2 = true;
            CaptureFrame("stage_2_mesosphere_plasma.png", "Stage 2: Mesosphere Re-entry Plasma Shockwave Burn");
        }
        // T = 19.5s: Tropospheric Cloud Canopy Penetration
        else if (timer >= 19.5f && !c3)
        {
            c3 = true;
            CaptureFrame("stage_3_troposphere_clouds.png", "Stage 3: Troposphere Volumetric Cloud Penetration");
        }
        // T = 24.5s: Biosphere Reveal & Hover Under UFO Mothership
        else if (timer >= 24.5f && !c4)
        {
            c4 = true;
            CaptureFrame("stage_4_biosphere_ufo_hover.png", "Stage 4: Biosphere Reveal & Rendezvous Under UFO");
        }
        // T = 28.5s: Ventral Tractor Beam Descent ("Diturunin dari UFO")
        else if (timer >= 28.5f && !c5)
        {
            c5 = true;
            CaptureFrame("stage_5_tractor_beam_descent.png", "Stage 5: Ventral Tractor Beam Descent Through Dome");
        }
        // T = 33.5s: Touchdown on Dais, UFO Ascends, Roof Shutter Closes
        else if (timer >= 33.5f && !c6)
        {
            c6 = true;
            CaptureFrame("stage_6_touchdown_roof_closing.png", "Stage 6: Touchdown on Dais & Roof Iris Sealing");
            StartCoroutine(CaptureBioMirrorAndOverhead());
        }
        else if (timer >= 41.0f)
        {
#if UNITY_EDITOR
            Debug.Log("[LiveDescentRecorder] ✦ All stages verified! Stopping Play Mode...");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private IEnumerator CaptureBioMirrorAndOverhead()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        yield return new WaitForSeconds(2.0f);

        // Capture looking up at sealed roof & ascended UFO
        Quaternion origRot = cam.transform.localRotation;
        cam.transform.localRotation = Quaternion.Euler(-75f, 0f, 0f);
        yield return new WaitForEndOfFrame();
        CaptureFrame("stage_7_sealed_roof_ufo_ascended.png", "Stage 7: Sealed Roof Shutter & Ascended UFO Overhead");

        // Turn to look at Alien Bio-Mirror Station (-2.8, 0, 0.8) to see 3D Alien Character & First-Person Alien Hands
        // Hide HUD elements that are children of camera rig for clean framing
        var visorHud = cam.transform.parent?.Find("FlightVisorHUD");
        if (visorHud != null) visorHud.gameObject.SetActive(false);
        var mfdCenter = cam.transform.parent?.Find("MFD_Center_UI");
        if (mfdCenter != null) mfdCenter.gameObject.SetActive(false);
        var mfdLeft = cam.transform.parent?.Find("MFD_Left_UI");
        if (mfdLeft != null) mfdLeft.gameObject.SetActive(false);
        var mfdRight = cam.transform.parent?.Find("MFD_Right_UI");
        if (mfdRight != null) mfdRight.gameObject.SetActive(false);

        cam.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        yield return new WaitForEndOfFrame();
        CaptureFrame("stage_8_alien_character_biomirror.png", "Stage 8: Alien Character Reflection in Bio-Mirror");

        // Restore HUD
        if (visorHud != null) visorHud.gameObject.SetActive(true);
        if (mfdCenter != null) mfdCenter.gameObject.SetActive(true);
        if (mfdLeft != null) mfdLeft.gameObject.SetActive(true);
        if (mfdRight != null) mfdRight.gameObject.SetActive(true);

        cam.transform.localRotation = origRot;
    }

    private void CaptureFrame(string filename, string description)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture prevRT = cam.targetTexture;

        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = prevRT;

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string fullPath = Path.Combine(OutputDir, filename);
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log($"[LiveDescentRecorder] Saved [{description}] ({bytes.Length / 1024} KB) -> {filename}");
    }
}
