using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// XENOASIS — SoundWaveVisualizer.cs
/// Emits concentric expanding soundwave ripples (rings) synchronized with crystal harmonic resonance.
/// </summary>
public class SoundWaveVisualizer : MonoBehaviour
{
    [Header("Wave Ring Prefab / Mesh")]
    [SerializeField] private GameObject waveRingPrefab;
    [SerializeField] private Transform waveSpawnOrigin;

    [Header("Wave Settings")]
    [SerializeField] private float maxRingScale = 3.5f;
    [SerializeField] private float expansionSpeed = 2.0f;
    [SerializeField] private Color waveColor = new Color(0f, 1f, 0.82f, 0.6f); // Cyan
    [SerializeField] private Color goldenSweetSpotColor = new Color(1f, 0.82f, 0.4f, 0.8f);

    private class ActiveWave
    {
        public GameObject Instance;
        public float CurrentScale;
        public Material MaterialInstance;
    }

    private readonly List<ActiveWave> activeWaves = new List<ActiveWave>();

    private void Update()
    {
        for (int i = activeWaves.Count - 1; i >= 0; i--)
        {
            var wave = activeWaves[i];
            wave.CurrentScale += expansionSpeed * Time.deltaTime;
            float progress = wave.CurrentScale / maxRingScale;

            if (wave.Instance != null)
            {
                wave.Instance.transform.localScale = Vector3.one * wave.CurrentScale;

                if (wave.MaterialInstance != null)
                {
                    Color c = wave.MaterialInstance.color;
                    c.a = Mathf.Lerp(0.8f, 0f, progress);
                    wave.MaterialInstance.color = c;
                }
            }

            if (progress >= 1.0f)
            {
                if (wave.Instance != null)
                    Destroy(wave.Instance);
                activeWaves.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Spawns an expanding soundwave ring.
    /// </summary>
    public void EmitPulse(bool isHarmonicSweetSpot = false)
    {
        Vector3 spawnPos = waveSpawnOrigin != null ? waveSpawnOrigin.position : transform.position;
        GameObject ringObj;

        if (waveRingPrefab != null)
        {
            ringObj = Instantiate(waveRingPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // Fallback: Primitive Cylinder or Quad scaled flat
            ringObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ringObj.transform.position = spawnPos;
            ringObj.transform.localScale = new Vector3(0.1f, 0.001f, 0.1f);
            var col = ringObj.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }

        var renderer = ringObj.GetComponent<Renderer>();
        Material mat = renderer != null ? renderer.material : null;
        if (mat != null)
        {
            mat.color = isHarmonicSweetSpot ? goldenSweetSpotColor : waveColor;
        }

        activeWaves.Add(new ActiveWave
        {
            Instance = ringObj,
            CurrentScale = 0.2f,
            MaterialInstance = mat
        });
    }
}
