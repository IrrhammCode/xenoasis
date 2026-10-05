#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace Xenoasis.EditorTools
{
    public static class World3ScreenshotCapturer
    {
        private const string ArtifactDir = "C:/Users/Irham/.gemini/antigravity-cli/brain/cc549632-6fd0-42f5-b035-0a72ce89b3af";
        private const string ScreenshotDir = "Assets/Screenshots";

        [MenuItem("XENOASIS/Capture World 3 Screenshots")]
        public static void CaptureAll()
        {
            Directory.CreateDirectory(ScreenshotDir);
            Directory.CreateDirectory(ArtifactDir);

            // 1. Station 3 in WelcomeChamber
            CaptureStation3Pedestal();

            // 2. World 3 Overview
            CaptureWorld3Overview();

            // 3. World 3 Return Monolith
            CaptureWorld3ReturnMonolith();

            // Restore WelcomeChamber as active scene
            EditorSceneManager.OpenScene("Assets/Scenes/WelcomeChamber.unity", OpenSceneMode.Single);

            Debug.Log("[World3ScreenshotCapturer] ✦ All 3 screenshots captured and copied to artifacts!");
        }

        private static void CaptureStation3Pedestal()
        {
            string scenePath = "Assets/Scenes/WelcomeChamber.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject st3 = GameObject.Find("TheLivingTerranSpheres/Station_03_HeartOfHumanity");
            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;
            cam.fieldOfView = 62f;

            if (st3 != null)
            {
                Vector3 camPos = st3.transform.TransformPoint(new Vector3(0f, 1.45f, 3.5f));
                Vector3 target = st3.transform.TransformPoint(new Vector3(0f, 1.25f, 0f));
                camGo.transform.position = camPos;
                camGo.transform.rotation = Quaternion.LookRotation(target - camPos);
            }
            else
            {
                Vector3 target = new Vector3(2.91f, 1.25f, 7.99f);
                Vector3 camPos = new Vector3(1.70f, 1.45f, 4.65f);
                camGo.transform.position = camPos;
                camGo.transform.rotation = Quaternion.LookRotation(target - camPos);
            }

            RenderAndSave(cam, "station3_dive_pedestal_view.png");
            Object.DestroyImmediate(camGo);
        }

        private static void CaptureWorld3Overview()
        {
            string scenePath = "Assets/Scenes/World3_HeartOfHumanity.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.fieldOfView = 65f;

            // Looking north across Svalbard Vault, Rosetta Stele, Alexandria, DNA Spire, Violoncello
            camGo.transform.position = new Vector3(0f, 1.35f, -0.4f);
            camGo.transform.rotation = Quaternion.Euler(6f, 0f, 0f);

            RenderAndSave(cam, "world3_heart_of_humanity_overview.png");
            Object.DestroyImmediate(camGo);
        }

        private static void CaptureWorld3ReturnMonolith()
        {
            string scenePath = "Assets/Scenes/World3_HeartOfHumanity.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject camGo = new GameObject("ScreenshotCam");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.fieldOfView = 60f;

            // Looking south at the Chronos Return Monolith (z = -4.8m)
            camGo.transform.position = new Vector3(0f, 1.45f, -2.0f);
            camGo.transform.rotation = Quaternion.Euler(4f, 180f, 0f);

            RenderAndSave(cam, "world3_humanity_return_monolith.png");
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

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            string projPath = Path.Combine(ScreenshotDir, filename);
            File.WriteAllBytes(projPath, bytes);

            string artifactPath = Path.Combine(ArtifactDir, filename);
            File.WriteAllBytes(artifactPath, bytes);

            Debug.Log($"[World3ScreenshotCapturer] Saved screenshot: {projPath} and copied to artifact: {artifactPath}");
        }
    }
}
#endif
