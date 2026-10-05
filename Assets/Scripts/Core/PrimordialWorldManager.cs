using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xenoasis.Core
{
    [System.Serializable]
    public class RelicInfo
    {
        public string relicName;
        [TextArea(3, 6)] public string description;
        public Color glowColor;
        public AudioClip proximitySound;
    }

    /// <summary>
    /// World 1 manager script for XENOASIS - The Primordial Cradle 
    /// (Archean Eon Earth, 3.8 billion years ago).
    /// </summary>
    public class PrimordialWorldManager : MonoBehaviour
    {
        public static PrimordialWorldManager Instance { get; private set; }

        #region Lighting Management
        [Header("Lighting & Atmosphere")]
        [SerializeField] private Light sunLight;
        [SerializeField] private Color sunStartColor = new Color(1f, 0.6f, 0.2f); // Warm amber
        [SerializeField] private Color sunEndColor = new Color(0.6f, 0.2f, 0.8f); // Violet
        [SerializeField] private float auroraCycleSpeed = 0.5f;

        [SerializeField] private Color ambientSkyColor = new Color(0.05f, 0.2f, 0.2f);
        [SerializeField] private Color ambientEquatorColor = new Color(0.1f, 0.25f, 0.2f);
        [SerializeField] private Color ambientGroundColor = new Color(0.02f, 0.1f, 0.05f);
        #endregion

        #region Particle Systems Management
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem geothermalSteam;
        [SerializeField] private ParticleSystem mineralSparkles;
        [SerializeField] private ParticleSystem auroraRibbons;
        [SerializeField] private ParticleSystem waterDroplets;
        
        private ParticleSystem[] allAtmosphericParticles;
        #endregion

        #region Water System
        [Header("Water System")]
        [SerializeField] private Transform waterSurface;
        [SerializeField] private Material waterMaterial;
        [SerializeField] private Color shallowWaterColor = new Color(0.4f, 0.8f, 0.7f); // Milky turquoise
        [SerializeField] private Color deepWaterColor = new Color(0.0f, 0.3f, 0.2f); // Dark emerald
        [SerializeField] private float waveAnimationSpeed = 1.0f;
        
        private static readonly int WaveSpeedProp = Shader.PropertyToID("_WaveSpeed");
        private static readonly int WaveScaleProp = Shader.PropertyToID("_WaveScale");
        private static readonly int TintColorProp = Shader.PropertyToID("_TintColor");
        private static readonly int ReflectionStrengthProp = Shader.PropertyToID("_ReflectionStrength");
        
        private float currentWaveTime = 0f;
        #endregion

        #region Audio System
        [Header("Audio System")]
        [SerializeField] private AudioSource ambientOceanAudio;
        [SerializeField] private AudioSource geothermalAudio;
        [SerializeField] private AudioSource windAudio;
        #endregion

        #region Interactive Relics
        [Header("Interactive Relics")]
        [SerializeField] private Transform[] interactiveRelics;
        [SerializeField] private RelicInfo[] relicInfos;
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private Material glowHighlightMaterial;
        
        private Transform playerTransform;
        private Dictionary<int, Material[]> originalRelicMaterials = new Dictionary<int, Material[]>();
        private int currentActiveRelicIndex = -1;
        #endregion

        #region Return Portal
        [Header("Return Portal")]
        [SerializeField] private Transform returnMonolith;
        [SerializeField] private Transform returnSphere;
        [SerializeField] private ParticleSystem portalParticles;
        #endregion

        #region Spawn System
        [Header("Spawn System")]
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private float spawnFadeInDuration = 1.5f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            allAtmosphericParticles = new ParticleSystem[] 
            { 
                geothermalSteam, mineralSparkles, auroraRibbons, waterDroplets 
            };
        }

        private void Start()
        {
            InitializeWorld();
            CacheRelicMaterials();
            
            // Try to find player if not explicitly set
            if (playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
        }

        private void Update()
        {
            UpdateLighting();
            UpdateWaterAnimation();
        }

        private void FixedUpdate()
        {
            CheckRelicProximity();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            // Restore any modified materials
            foreach (var kvp in originalRelicMaterials)
            {
                DeactivateRelicGlow(kvp.Key);
            }
        }
        #endregion

        #region Core Systems
        public void InitializeWorld()
        {
            // Setup atmospheric lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;

            // Setup Fog
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = ambientSkyColor;
            RenderSettings.fogDensity = 0.02f;

            // Initialize Water Material
            if (waterMaterial != null)
            {
                waterMaterial.SetColor(TintColorProp, Color.Lerp(shallowWaterColor, deepWaterColor, 0.5f));
            }

            // Start Audio
            if (ambientOceanAudio != null && !ambientOceanAudio.isPlaying) ambientOceanAudio.Play();
            if (geothermalAudio != null && !geothermalAudio.isPlaying) geothermalAudio.Play();
            if (windAudio != null && !windAudio.isPlaying) windAudio.Play();

            // Set default particle intensity
            SetParticleIntensity(1.0f);
        }

        public void OnPlayerSpawn()
        {
            if (playerTransform != null && playerSpawnPoint != null)
            {
                playerTransform.position = playerSpawnPoint.position;
                playerTransform.rotation = playerSpawnPoint.rotation;
            }
            
            StartCoroutine(FadeInRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            // Implementation of fade-in from white
            float elapsed = 0f;
            while (elapsed < spawnFadeInDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            Debug.Log("Player spawn fade-in complete.");
        }

        private void UpdateLighting()
        {
            if (sunLight != null)
            {
                float t = (Mathf.Sin(Time.time * auroraCycleSpeed) + 1f) / 2f;
                sunLight.color = Color.Lerp(sunStartColor, sunEndColor, t);
            }
        }

        private void UpdateWaterAnimation()
        {
            if (waterMaterial != null)
            {
                currentWaveTime += Time.deltaTime * waveAnimationSpeed;
                waterMaterial.SetFloat(WaveSpeedProp, currentWaveTime);
                
                float scale = 1.0f + Mathf.Sin(Time.time * 0.2f) * 0.1f;
                waterMaterial.SetFloat(WaveScaleProp, scale);
            }
        }

        public void SetParticleIntensity(float multiplier)
        {
            if (allAtmosphericParticles == null) return;
            
            foreach (var ps in allAtmosphericParticles)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTimeMultiplier *= multiplier;
                }
            }
        }
        #endregion

        #region Audio Systems
        public void CrossfadeAudio(AudioSource from, AudioSource to, float duration)
        {
            StartCoroutine(CrossfadeAudioRoutine(from, to, duration));
        }

        private IEnumerator CrossfadeAudioRoutine(AudioSource from, AudioSource to, float duration)
        {
            float elapsed = 0f;
            float fromStartVolume = from != null ? from.volume : 0f;
            float toStartVolume = to != null ? to.volume : 0f;
            float targetVolume = Mathf.Max(fromStartVolume, 1.0f); // default target volume

            if (to != null && !to.isPlaying) to.Play();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (from != null) from.volume = Mathf.Lerp(fromStartVolume, 0f, t);
                if (to != null) to.volume = Mathf.Lerp(toStartVolume, targetVolume, t);

                yield return null;
            }

            if (from != null)
            {
                from.volume = 0f;
                from.Stop();
            }
            if (to != null) to.volume = targetVolume;
        }
        #endregion

        #region Relic Interaction
        private void CacheRelicMaterials()
        {
            if (interactiveRelics == null) return;

            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] != null)
                {
                    Renderer rend = interactiveRelics[i].GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        originalRelicMaterials[i] = rend.materials;
                    }
                }
            }
        }

        private void CheckRelicProximity()
        {
            if (playerTransform == null || interactiveRelics == null) return;

            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] == null) continue;

                float dist = Vector3.Distance(playerTransform.position, interactiveRelics[i].position);
                if (dist <= interactionRadius && dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }

            if (closestIndex != currentActiveRelicIndex)
            {
                if (currentActiveRelicIndex != -1)
                {
                    DeactivateRelicGlow(currentActiveRelicIndex);
                }

                if (closestIndex != -1)
                {
                    ActivateRelicGlow(closestIndex);
                }

                currentActiveRelicIndex = closestIndex;
            }
        }

        public RelicInfo GetNearestRelicInfo(Vector3 playerPos)
        {
            if (interactiveRelics == null || relicInfos == null) return null;

            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] == null) continue;

                float dist = Vector3.Distance(playerPos, interactiveRelics[i].position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }

            if (closestIndex >= 0 && closestIndex < relicInfos.Length)
            {
                return relicInfos[closestIndex];
            }

            return null;
        }

        public void ActivateRelicGlow(int relicIndex)
        {
            if (glowHighlightMaterial == null || interactiveRelics == null) return;
            if (relicIndex < 0 || relicIndex >= interactiveRelics.Length) return;
            if (interactiveRelics[relicIndex] == null) return;

            Renderer rend = interactiveRelics[relicIndex].GetComponentInChildren<Renderer>();
            if (rend != null && originalRelicMaterials.ContainsKey(relicIndex))
            {
                Material[] originalMats = originalRelicMaterials[relicIndex];
                Material[] highlightedMats = new Material[originalMats.Length + 1];
                
                for (int i = 0; i < originalMats.Length; i++)
                {
                    highlightedMats[i] = originalMats[i];
                }
                
                highlightedMats[originalMats.Length] = glowHighlightMaterial;
                rend.materials = highlightedMats;
            }
        }

        public void DeactivateRelicGlow(int relicIndex)
        {
            if (interactiveRelics == null || relicIndex < 0 || relicIndex >= interactiveRelics.Length) return;
            if (interactiveRelics[relicIndex] == null) return;

            Renderer rend = interactiveRelics[relicIndex].GetComponentInChildren<Renderer>();
            if (rend != null && originalRelicMaterials.ContainsKey(relicIndex))
            {
                rend.materials = originalRelicMaterials[relicIndex];
            }
        }
        #endregion

        #region Return Portal
        public void TriggerReturnTransition()
        {
            Debug.Log("Triggering Return Transition...");
            
            if (portalParticles != null && !portalParticles.isPlaying)
            {
                portalParticles.Play();
            }

            // Calls to other managers like SphereDivingTransition.BeginReturnTransition() would go here
        }
        #endregion
    }
}
