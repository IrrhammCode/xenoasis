using UnityEngine;
using TMPro;

/// <summary>
/// XENOASIS — CockpitInfogramDisplay.cs
/// Powers the dynamic sci-fi Earth infographics and animated telemetry on the
/// UFO cockpit's left and right MFD screens and side holographic consoles.
/// Displays real-time atmospheric data, biome breakdowns, Earth vital signs,
/// radio spectrum analysis, and rotates the 3D holographic mini-globe.
/// </summary>
[ExecuteAlways]
public class CockpitInfogramDisplay : MonoBehaviour
{
    [Header("Holographic 3D Consoles")]
    [SerializeField] private Transform holoEarthMini;
    [SerializeField] private Transform holoRadarRing;

    private void Update()
    {
        // Smoothly Rotate Holographic Mini-Globe on Console Desk
        if (holoEarthMini != null)
        {
            holoEarthMini.Rotate(Vector3.up, 24f * Time.deltaTime, Space.Self);
        }

        // Smoothly Spin Holographic Radar Ring on Console Desk
        if (holoRadarRing != null)
        {
            holoRadarRing.Rotate(Vector3.forward, -42f * Time.deltaTime, Space.Self);
        }
    }
}

