using UnityEngine;
using UnityEditor;

/// <summary>
/// XENOASIS — PICOBuildSettingsEditor.cs
/// 1-Click Unity Editor configuration for PICO 4 Ultra standalone VR/MR build.
/// Run from Unity menu: XENOASIS > Configure PICO 4 Ultra Build Settings.
/// </summary>
public class PICOBuildSettingsEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("XENOASIS/Configure PICO 4 Ultra Build Settings")]
    public static void ConfigureForPICO4Ultra()
    {
        Debug.Log("[XENOASIS] Configuring PlayerSettings for PICO 4 Ultra (Standalone APK)...");

        // 1. Switch to Android platform
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            Debug.Log("[XENOASIS] Switched build target to Android.");
        }

        // 2. Identity & Versioning
        PlayerSettings.companyName = "XenoasisTeam";
        PlayerSettings.productName = "XENOASIS - The Memory of Water";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.xenoasis.memoryofwater");
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        // 3. Architecture & Scripting (IL2CPP + ARM64 required for PICO 4 Ultra)
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        // 4. Android SDK Levels (PICO OS is Android 10+ / API 29+)
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34; // Android 14 / latest

        // 5. Rendering Pipeline & Color Space
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.gpuSkinning = true;

        // 6. XR Settings
        // PICO 4 Ultra uses Single Pass Instanced rendering for 90 FPS efficiency
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

        // 7. Fullscreen & Orientation
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        Debug.Log("[XENOASIS] PICO 4 Ultra PlayerSettings successfully configured!");

        EditorUtility.DisplayDialog(
            "XENOASIS — PICO 4 Ultra Setup",
            "PICO 4 Ultra Build Settings Configured!\n\n" +
            "✔ Target Platform: Android (ARM64)\n" +
            "✔ Scripting Backend: IL2CPP\n" +
            "✔ Min SDK: Android API 29 (Android 10)\n" +
            "✔ Color Space: Linear\n" +
            "✔ Stereo Rendering: Single Pass Instanced\n\n" +
            "Next: Ensure 'PICO XR' is checked under Project Settings > XR Plug-in Management.",
            "Great!"
        );
    }
#endif
}
