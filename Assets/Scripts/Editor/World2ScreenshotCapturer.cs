#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace Xenoasis.EditorTools
{
    public static class World2ScreenshotCapturer
    {
        private const string ArtifactDir = "C:/Users/Irham/.gemini/antigravity-cli/brain/cc549632-6fd0-42f5-b035-0a72ce89b3af";
        private const string ScreenshotDir = "Assets/Screenshots";

        [MenuItem("XENOASIS/Capture World 2 Screenshots")]
        public static void CaptureAll()
        {
            Directory.CreateDirectory(ScreenshotDir);
            Directory.CreateDirectory(ArtifactDir);

            // 1. Station 2 in WelcomeChamber
            CaptureStation2Pedestal();

            // 2. World 2 Overview
            CaptureWorld2Overview();

            // 3. World 2 Return Monolith
            CaptureWorld2ReturnMonolith();

            Debug.Log("[World2ScreenshotCapturer] ✦ All 3 screenshots captured and copied to artifacts!");
        }

        private static void CaptureStation2Pedestal()
        {
            string scenePath = "Assets/Scenes/WelcomeChamber.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject st2 = GameObject.Find("TheLivingTerranSpheres/Station_02_SparkOfCivilization");
            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;
            cam.fieldOfView = 62f;

            if (st2 != null)
            {
                Vector3 camPos = st2.transform.TransformPoint(new Vector3(0f, 1.45f, 3.5f));
                Vector3 target = st2.transform.TransformPoint(new Vector3(0f, 1.25f, 0f));
                camGo.transform.position = camPos;
                camGo.transform.rotation = Quaternion.LookRotation(target - camPos);
            }
            else
            {
                Vector3 target = new Vector3(-2.91f, 1.25f, 7.99f);
                Vector3 camPos = new Vector3(-1.70f, 1.45f, 4.65f);
                camGo.transform.position = camPos;
                camGo.transform.rotation = Quaternion.LookRotation(target - camPos);
            }

            RenderAndSave(cam, "station2_dive_pedestal_view.png");
            Object.DestroyImmediate(camGo);
        }

        private static void CaptureWorld2Overview()
        {
            string scenePath = "Assets/Scenes/World2_ForgeOfCivilization.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.fieldOfView = 65f;

            // Looking north across the Prometheus Hearth, Anvil, Astrolabe, Press, and Silicon Monolith
            camGo.transform.position = new Vector3(0f, 1.35f, -0.4f);
            camGo.transform.rotation = Quaternion.Euler(6f, 0f, 0f);

            RenderAndSave(cam, "world2_forge_of_civilization_overview.png");
            Object.DestroyImmediate(camGo);
        }

        private static void CaptureWorld2ReturnMonolith()
        {
            string scenePath = "Assets/Scenes/World2_ForgeOfCivilization.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.fieldOfView = 60f;

            // Looking south at the Chrono-Return Monolith (z = -4.8m)
            camGo.transform.position = new Vector3(0f, 1.45f, -2.0f);
            camGo.transform.rotation = Quaternion.Euler(4f, 180f, 0f);

            RenderAndSave(cam, "world2_forge_return_monolith.png");
            Object.DestroyImmediate(camGo);
        }

        private static void RenderAndSave(Camera cam, string filename)
        {
            int width = 1920;
            int height = 1080;
            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            string assetPath = Path.Combine(ScreenshotDir, filename);
            File.WriteAllBytes(assetPath, bytes);

            string artifactPath = Path.Combine(ArtifactDir, filename);
            File.WriteAllBytes(artifactPath, bytes);

            Debug.Log($"[World2ScreenshotCapturer] Saved {filename} ({bytes.Length / 1024} KB)");
        }
    }
}
#endif
