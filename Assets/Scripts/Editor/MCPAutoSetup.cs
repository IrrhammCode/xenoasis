using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Xenoasis.Editor
{
    [InitializeOnLoad]
    public static class MCPAutoSetup
    {
        static MCPAutoSetup()
        {
            ApplyMCPPreferences();
            EditorApplication.delayCall += () =>
            {
                ConnectBridgeQuiet();
            };
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
                {
                    EditorApplication.delayCall += () => ConnectBridgeQuiet();
                }
            };
        }

        [MenuItem("XENOASIS/MCP/Configure & Connect Session", false, 30)]
        public static void ConfigureAndConnect()
        {
            ApplyMCPPreferences();
            ConnectBridge(showDialog: false);
        }

        [MenuItem("XENOASIS/MCP/Apply Settings Only", false, 31)]
        public static void ApplySettingsOnly()
        {
            ApplyMCPPreferences();
            Debug.Log("[XENOASIS MCP] MCP EditorPrefs configured: UVX, HTTP (Local), http://127.0.0.1:8080");
        }

        public static void ApplyMCPPreferences()
        {
            try
            {
                string uvxPath = @"C:\Users\Irham\AppData\Local\Programs\uv\uvx.exe";
                if (!File.Exists(uvxPath))
                {
                    uvxPath = @"C:\Users\Irham\AppData\Local\Microsoft\WindowsApps\uvx.exe";
                }

                EditorPrefs.SetString("MCPForUnity.UvxPath", uvxPath);
                EditorPrefs.SetBool("MCPForUnity.UseHttpTransport", false);
                EditorPrefs.SetString("MCPForUnity.HttpUrl", "http://127.0.0.1:8080");
                EditorPrefs.SetString("MCPForUnity.HttpTransportScope", "local");
                EditorPrefs.SetBool("MCPForUnity.HttpServerLaunchConfirmed", true);
                Application.runInBackground = true;

                Debug.Log($"[XENOASIS MCP] Preferences set: uvx={uvxPath}, UseHttpTransport=false");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[XENOASIS MCP] Failed to apply MCP preferences: {ex.Message}");
            }
        }

        public static void ConnectBridgeQuiet()
        {
            ConnectBridge(showDialog: false);
        }

        public static async void ConnectBridge(bool showDialog = false)
        {
            try
            {
                Type serviceLocatorType = null;
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var t = asm.GetType("MCPForUnity.Editor.Services.MCPServiceLocator");
                    if (t != null)
                    {
                        serviceLocatorType = t;
                        break;
                    }
                }

                if (serviceLocatorType == null)
                {
                    Debug.LogWarning("[XENOASIS MCP] MCPForUnity package is not loaded yet.");
                    return;
                }

                var bridgeProp = serviceLocatorType.GetProperty("Bridge", BindingFlags.Public | BindingFlags.Static);
                if (bridgeProp == null) return;

                var bridge = bridgeProp.GetValue(null);
                if (bridge == null) return;

                var isRunningProp = bridge.GetType().GetProperty("IsRunning", BindingFlags.Public | BindingFlags.Instance);
                if (isRunningProp != null && (bool)isRunningProp.GetValue(bridge))
                {
                    Debug.Log("[XENOASIS MCP] Session is already active.");
                    return;
                }

                var startMethod = bridge.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance);
                if (startMethod != null)
                {
                    var task = (System.Threading.Tasks.Task<bool>)startMethod.Invoke(bridge, null);
                    bool result = await task;
                    Debug.Log($"[XENOASIS MCP] Bridge StartAsync result: {result}");
                }

                // Also ensure StdioBridgeHost is active on port 6400
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var stdioType = asm.GetType("MCPForUnity.Editor.Services.Transport.Transports.StdioBridgeHost");
                    if (stdioType != null)
                    {
                        var startAutoConnect = stdioType.GetMethod("StartAutoConnect", BindingFlags.Public | BindingFlags.Static);
                        if (startAutoConnect != null)
                        {
                            startAutoConnect.Invoke(null, null);
                            Debug.Log("[XENOASIS MCP] StdioBridgeHost.StartAutoConnect() invoked.");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[XENOASIS MCP] Error connecting bridge: {ex.Message}");
            }
        }
    }
}
