using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xenoasis.Core
{
    [System.Serializable]
    public class CivilizationRelicInfo
    {
        public string relicName;
        public string eraLabel;
        [TextArea(3, 6)] public string description;
        public Color glowColor = new Color(1f, 0.6f, 0.2f);
        public AudioClip proximitySound;
    }

    /// <summary>
    /// World 2 manager script for XENOASIS — The Forge of Civilization
    /// (Paleolithic to Silicon // 1.500.000 B.C. - 2026 A.D.).
    /// Controls sunset lighting, forge hearth flickering, floating embers,
    /// 6 interactive technological relics, and return portal to Museum Station II.
    /// </summary>
    public class CivilizationWorldManager : MonoBehaviour
    {
        public static CivilizationWorldManager Instance { get; private set; }

        #region Lighting & Atmosphere
        [Header("Lighting & Atmosphere")]
        [SerializeField] private Light sunLight;
        [SerializeField] private Color sunsetSkyColor = new Color(1.0f, 0.45f, 0.2f); // Deep amber sunset
        [SerializeField] private Color duskSkyColor = new Color(0.35f, 0.15f, 0.45f); // Dusky purple
        [SerializeField] private float duskCycleSpeed = 0.2f;

        [SerializeField] private Color ambientSkyColor = new Color(0.2f, 0.12f, 0.15f);
        [SerializeField] private Color ambientEquatorColor = new Color(0.3f, 0.18f, 0.1f);
        [SerializeField] private Color ambientGroundColor = new Color(0.08f, 0.05f, 0.04f);

        [Header("Flickering Firelights")]
        [SerializeField] private Light[] forgeFireLights;
        [SerializeField] private float fireFlickerSpeed = 8.0f;
        [SerializeField] private float fireFlickerIntensityRange = 0.4f;
        private float[] baseFireIntensities;
        #endregion

        #region Particle Systems
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem floatingEmbers;
        [SerializeField] private ParticleSystem forgeSmoke;
        [SerializeField] private ParticleSystem hearthFlames;
        [SerializeField] private ParticleSystem siliconSparks;
        #endregion

        #region Audio Systems
        [Header("Audio Systems")]
        [SerializeField] private AudioSource ambientForgeAudio;
        [SerializeField] private AudioSource ambientWindAudio;
        [SerializeField] private AudioSource relicInteractionAudio;
        #endregion

        #region Interactive Relics
        [Header("Interactive Relics (6 Items)")]
        [SerializeField] private Transform[] interactiveRelics;
        [SerializeField] private CivilizationRelicInfo[] relicInfos;
        [SerializeField] private float interactionRadius = 2.0f;
        [SerializeField] private Material glowHighlightMaterial;

        private Transform playerTransform;
        private Dictionary<int, Material[]> originalRelicMaterials = new Dictionary<int, Material[]>();
        private int currentActiveRelicIndex = -1;
        #endregion

        #region Return Portal
        [Header("Return Portal")]
        [SerializeField] private Transform returnMonolith;
        [SerializeField] private Transform returnSphere;
        [SerializeField] private ParticleSystem returnPortalParticles;
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
            InitializeFireIntensities();
        }

        private void Start()
        {
            InitializeWorld();
            FindPlayer();
            CacheRelicMaterials();
        }

        private void Update()
        {
            UpdateSunAtmosphere();
            UpdateFireFlicker();
            UpdateReturnSphereAnimation();
        }

        private void FixedUpdate()
        {
            CheckPlayerRelicProximity();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
        #endregion

        #region Initialization
        public void InitializeWorld()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.18f, 0.10f, 0.12f);
            RenderSettings.fogDensity = 0.015f;

            if (ambientForgeAudio != null && !ambientForgeAudio.isPlaying) ambientForgeAudio.Play();
            if (ambientWindAudio != null && !ambientWindAudio.isPlaying) ambientWindAudio.Play();

            if (floatingEmbers != null && !floatingEmbers.isPlaying) floatingEmbers.Play();
            if (forgeSmoke != null && !forgeSmoke.isPlaying) forgeSmoke.Play();
            if (hearthFlames != null && !hearthFlames.isPlaying) hearthFlames.Play();
            if (siliconSparks != null && !siliconSparks.isPlaying) siliconSparks.Play();

            StartCoroutine(FadeInRoutine());
        }

        private void InitializeFireIntensities()
        {
            if (forgeFireLights != null && forgeFireLights.Length > 0)
            {
                baseFireIntensities = new float[forgeFireLights.Length];
                for (int i = 0; i < forgeFireLights.Length; i++)
                {
                    if (forgeFireLights[i] != null)
                        baseFireIntensities[i] = forgeFireLights[i].intensity;
                }
            }
        }

        private void FindPlayer()
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                playerTransform = mainCam.transform;
            }
            else
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
        }

        private void CacheRelicMaterials()
        {
            if (interactiveRelics == null) return;
            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] != null)
                {
                    Renderer[] renderers = interactiveRelics[i].GetComponentsInChildren<Renderer>();
                    if (renderers.Length > 0)
                    {
                        List<Material> mats = new List<Material>();
                        foreach (var r in renderers)
                        {
                            mats.AddRange(r.sharedMaterials);
                        }
                        originalRelicMaterials[i] = mats.ToArray();
                    }
                }
            }
        }
        #endregion

        #region Atmosphere & Lighting
        private void UpdateSunAtmosphere()
        {
            if (sunLight == null) return;
            float t = (Mathf.Sin(Time.time * duskCycleSpeed) + 1f) * 0.5f;
            sunLight.color = Color.Lerp(sunsetSkyColor, duskSkyColor, t);
        }

        private void UpdateFireFlicker()
        {
            if (forgeFireLights == null || baseFireIntensities == null) return;
            for (int i = 0; i < forgeFireLights.Length; i++)
            {
                if (forgeFireLights[i] == null) continue;
                float noise = Mathf.PerlinNoise(Time.time * fireFlickerSpeed + i * 17.3f, 0f);
                float delta = (noise - 0.5f) * 2f * fireFlickerIntensityRange;
                forgeFireLights[i].intensity = Mathf.Max(0.1f, baseFireIntensities[i] + delta);
            }
        }

        private void UpdateReturnSphereAnimation()
        {
            if (returnSphere == null) return;
            returnSphere.Rotate(Vector3.up, 30f * Time.deltaTime, Space.World);
            float bobbing = Mathf.Sin(Time.time * 2.5f) * 0.05f;
            returnSphere.localPosition = new Vector3(returnSphere.localPosition.x, 1.6f + bobbing, returnSphere.localPosition.z);
        }
        #endregion

        #region Relic Interactions & Proximity
        private void CheckPlayerRelicProximity()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            Vector3 pPos = playerTransform.position;
            int closestIndex = -1;
            float closestDist = float.MaxValue;

            if (interactiveRelics != null)
            {
                for (int i = 0; i < interactiveRelics.Length; i++)
                {
                    if (interactiveRelics[i] == null) continue;
                    float dist = Vector3.Distance(pPos, interactiveRelics[i].position);
                    if (dist < interactionRadius && dist < closestDist)
                    {
                        closestDist = dist;
                        closestIndex = i;
                    }
                }
            }

            // Check return monolith proximity
            if (returnMonolith != null)
            {
                float returnDist = Vector3.Distance(pPos, returnMonolith.position);
                if (returnDist < interactionRadius * 1.2f)
                {
                    // In vicinity of return monolith
                    if (returnPortalParticles != null && !returnPortalParticles.isPlaying)
                        returnPortalParticles.Play();
                }
            }

            if (closestIndex != currentActiveRelicIndex)
            {
                if (currentActiveRelicIndex != -1) DeactivateRelicGlow(currentActiveRelicIndex);
                currentActiveRelicIndex = closestIndex;
                if (currentActiveRelicIndex != -1)
                {
                    ActivateRelicGlow(currentActiveRelicIndex);
                    TriggerPicoProximityHaptic();
                }
            }
        }

        public void ActivateRelicGlow(int relicIndex)
        {
            if (interactiveRelics == null || relicIndex < 0 || relicIndex >= interactiveRelics.Length) return;
            Transform relic = interactiveRelics[relicIndex];
            if (relic == null) return;

            Renderer[] renderers = relic.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                Material[] mats = r.materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m].HasProperty("_EmissionColor"))
                    {
                        mats[m].EnableKeyword("_EMISSION");
                        Color glow = (relicInfos != null && relicIndex < relicInfos.Length) ? relicInfos[relicIndex].glowColor : Color.yellow;
                        mats[m].SetColor("_EmissionColor", glow * 2.0f);
                    }
                }
            }

            if (relicInfos != null && relicIndex < relicInfos.Length && relicInfos[relicIndex].proximitySound != null)
            {
                if (relicInteractionAudio != null)
                {
                    relicInteractionAudio.PlayOneShot(relicInfos[relicIndex].proximitySound);
                }
            }
        }

        public void DeactivateRelicGlow(int relicIndex)
        {
            if (interactiveRelics == null || relicIndex < 0 || relicIndex >= interactiveRelics.Length) return;
            Transform relic = interactiveRelics[relicIndex];
            if (relic == null) return;

            Renderer[] renderers = relic.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                Material[] mats = r.materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m].HasProperty("_EmissionColor"))
                    {
                        mats[m].SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        private void TriggerPicoProximityHaptic()
        {
            // Send haptic feedback to both controllers on PICO 4 Ultra
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
                UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller, leftHandDevices);
            foreach (var device in leftHandDevices)
            {
                device.SendHapticImpulse(0, 0.35f, 0.08f);
            }

            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
                UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, rightHandDevices);
            foreach (var device in rightHandDevices)
            {
                device.SendHapticImpulse(0, 0.35f, 0.08f);
            }
        }
        #endregion

        #region Return Transition
        public void TriggerReturnTransition()
        {
            var transition = FindObjectOfType<Xenoasis.Player.SphereDivingTransition>();
            if (transition != null)
            {
                transition.BeginReturnTransition(returnSphere != null ? returnSphere : returnMonolith);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("WelcomeChamber");
            }
        }
        #endregion

        #region Fade Routine
        private IEnumerator FadeInRoutine()
        {
            if (playerSpawnPoint != null && playerTransform != null)
            {
                Transform root = playerTransform.root;
                root.position = playerSpawnPoint.position;
                root.rotation = playerSpawnPoint.rotation;
            }

            float timer = 0f;
            while (timer < spawnFadeInDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        #endregion
    }
}
