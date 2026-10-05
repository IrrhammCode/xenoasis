using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xenoasis.Core
{
    [System.Serializable]
    public class HumanityRelicInfo
    {
        public string relicName;
        public string eraLabel;
        [TextArea(3, 6)] public string description;
        public Color glowColor = new Color(0.2f, 0.9f, 0.8f);
        public AudioClip proximitySound;
    }

    /// <summary>
    /// World 3 manager script for XENOASIS — The Heart of Humanity
    /// (Sanctuary of Human Soul, Language & Sacred Life // Ptolemaic to Deep Time).
    /// Controls polar midnight moonlight, aurora borealis color cycling,
    /// cryogenic seed vault glows, 6 interactive cultural & biological relics,
    /// and return portal back to Station III in the Welcome Chamber.
    /// </summary>
    public class HumanityWorldManager : MonoBehaviour
    {
        public static HumanityWorldManager Instance { get; private set; }

        #region Lighting & Atmosphere
        [Header("Lighting & Polar Atmosphere")]
        [SerializeField] private Light moonLight;
        [SerializeField] private Color midnightSkyColor = new Color(0.02f, 0.05f, 0.12f);
        [SerializeField] private Color emeraldAuroraColor = new Color(0.15f, 0.95f, 0.65f);
        [SerializeField] private Color violetAuroraColor = new Color(0.65f, 0.25f, 0.95f);
        [SerializeField] private float auroraCycleSpeed = 0.35f;

        [SerializeField] private Color ambientSkyColor = new Color(0.08f, 0.15f, 0.22f);
        [SerializeField] private Color ambientEquatorColor = new Color(0.05f, 0.12f, 0.18f);
        [SerializeField] private Color ambientGroundColor = new Color(0.02f, 0.04f, 0.08f);

        [Header("Cryogenic & Aurora Glow Lights")]
        [SerializeField] private Light[] cryogenicGlowLights;
        [SerializeField] private float cryoPulseSpeed = 2.5f;
        [SerializeField] private float cryoPulseAmplitude = 0.35f;
        private float[] baseCryoIntensities;
        #endregion

        #region Particle Systems
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem auroraRibbons;
        [SerializeField] private ParticleSystem seedSproutSpores;
        [SerializeField] private ParticleSystem whisperingGlyphs;
        [SerializeField] private ParticleSystem glacialDiamondDust;
        #endregion

        #region Audio Systems
        [Header("Audio Systems")]
        [SerializeField] private AudioSource ambientPolarAudio;
        [SerializeField] private AudioSource whisperedLanguagesAudio;
        [SerializeField] private AudioSource celloMelodyAudio;
        [SerializeField] private AudioSource relicInteractionAudio;
        #endregion

        #region Interactive Relics
        [Header("Interactive Relics (6 Items)")]
        [SerializeField] private Transform[] interactiveRelics;
        [SerializeField] private HumanityRelicInfo[] relicInfos;
        [SerializeField] private float interactionRadius = 2.2f;
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
            InitializeCryoIntensities();
        }

        private void Start()
        {
            InitializeWorld();
            FindPlayer();
            OnPlayerSpawn();
        }

        private void Update()
        {
            UpdateAuroraAtmosphere();
            UpdateCryogenicPulsing();
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
        private void InitializeCryoIntensities()
        {
            if (cryogenicGlowLights != null && cryogenicGlowLights.Length > 0)
            {
                baseCryoIntensities = new float[cryogenicGlowLights.Length];
                for (int i = 0; i < cryogenicGlowLights.Length; i++)
                {
                    if (cryogenicGlowLights[i] != null)
                    {
                        baseCryoIntensities[i] = cryogenicGlowLights[i].intensity;
                    }
                }
            }
        }

        public void InitializeWorld()
        {
            // Ambient Lighting setup
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;

            // Fog: Ethereal arctic glacial mist
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.04f, 0.10f, 0.16f);
            RenderSettings.fogDensity = 0.009f;

            // Particle scaling
            SetParticleIntensity(1.0f);

            // Audio startup
            if (ambientPolarAudio != null && !ambientPolarAudio.isPlaying)
                ambientPolarAudio.Play();
            if (whisperedLanguagesAudio != null && !whisperedLanguagesAudio.isPlaying)
                whisperedLanguagesAudio.Play();
            if (celloMelodyAudio != null && !celloMelodyAudio.isPlaying)
                celloMelodyAudio.Play();

            // Cache relic materials for proximity highlighting
            CacheRelicMaterials();

            Debug.Log("[HumanityWorldManager] ✦ World 3: The Heart of Humanity initialized.");
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
                GameObject rig = GameObject.Find("[CameraRig]") ?? GameObject.Find("XRRig") ?? GameObject.Find("Player");
                if (rig != null) playerTransform = rig.transform;
            }
        }

        public void OnPlayerSpawn()
        {
            if (playerSpawnPoint != null && playerTransform != null)
            {
                Transform rigRoot = playerTransform.root;
                rigRoot.position = playerSpawnPoint.position;
                rigRoot.rotation = playerSpawnPoint.rotation;
            }

            StartCoroutine(FadeInSequence());
        }

        private IEnumerator FadeInSequence()
        {
            yield return new WaitForSeconds(0.2f);
            float elapsed = 0f;
            while (elapsed < spawnFadeInDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        #endregion

        #region Atmosphere & Lighting Updates
        private void UpdateAuroraAtmosphere()
        {
            float t = (Mathf.Sin(Time.time * auroraCycleSpeed) + 1.0f) * 0.5f;
            Color currentAurora = Color.Lerp(emeraldAuroraColor, violetAuroraColor, t);

            if (moonLight != null)
            {
                // Dynamic subtle color temperature cycling following aurora ribbons
                moonLight.color = Color.Lerp(new Color(0.65f, 0.85f, 1.0f), currentAurora, 0.18f);
            }

            RenderSettings.ambientSkyColor = Color.Lerp(ambientSkyColor, currentAurora * 0.3f, 0.25f);
        }

        private void UpdateCryogenicPulsing()
        {
            if (cryogenicGlowLights == null || baseCryoIntensities == null) return;

            for (int i = 0; i < cryogenicGlowLights.Length; i++)
            {
                if (cryogenicGlowLights[i] != null && i < baseCryoIntensities.Length)
                {
                    float offset = i * 1.35f;
                    float wave = Mathf.Sin(Time.time * cryoPulseSpeed + offset) * cryoPulseAmplitude;
                    cryogenicGlowLights[i].intensity = baseCryoIntensities[i] + wave;
                }
            }
        }
        #endregion

        #region Interactive Relics System
        private void CacheRelicMaterials()
        {
            if (interactiveRelics == null) return;

            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] != null)
                {
                    var rend = interactiveRelics[i].GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        originalRelicMaterials[i] = rend.sharedMaterials;
                    }
                }
            }
        }

        private void CheckPlayerRelicProximity()
        {
            if (playerTransform == null || interactiveRelics == null) return;

            Vector3 pPos = playerTransform.position;
            int closestIndex = -1;
            float closestDist = interactionRadius;

            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] == null) continue;

                float dist = Vector3.Distance(pPos, interactiveRelics[i].position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }

            // Return Portal Proximity Check
            if (returnSphere != null)
            {
                float returnDist = Vector3.Distance(pPos, returnSphere.position);
                if (returnDist < 1.8f)
                {
                    // In range of return sphere
                }
            }

            if (closestIndex != currentActiveRelicIndex)
            {
                if (currentActiveRelicIndex != -1)
                {
                    DeactivateRelicHighlight(currentActiveRelicIndex);
                }

                currentActiveRelicIndex = closestIndex;

                if (currentActiveRelicIndex != -1)
                {
                    ActivateRelicHighlight(currentActiveRelicIndex);
                }
            }
        }

        public void ActivateRelicHighlight(int index)
        {
            if (index < 0 || index >= interactiveRelics.Length || interactiveRelics[index] == null) return;

            // Trigger audio
            if (relicInfos != null && index < relicInfos.Length && relicInfos[index].proximitySound != null)
            {
                if (relicInteractionAudio != null)
                {
                    relicInteractionAudio.PlayOneShot(relicInfos[index].proximitySound);
                }
            }

            // Haptic cue for PICO 4 Ultra
            TriggerPicoHaptics(0.4f, 0.15f);

            Debug.Log($"[HumanityWorldManager] Player inspected Relic {index + 1}: {relicInfos[index].relicName} ({relicInfos[index].eraLabel})");
        }

        public void DeactivateRelicHighlight(int index)
        {
            if (index < 0 || index >= interactiveRelics.Length || interactiveRelics[index] == null) return;
        }

        public HumanityRelicInfo GetNearestRelicInfo(Vector3 pos)
        {
            if (interactiveRelics == null || relicInfos == null) return null;

            int best = -1;
            float minD = 999f;
            for (int i = 0; i < interactiveRelics.Length; i++)
            {
                if (interactiveRelics[i] == null) continue;
                float d = Vector3.Distance(pos, interactiveRelics[i].position);
                if (d < minD)
                {
                    minD = d;
                    best = i;
                }
            }

            if (best >= 0 && best < relicInfos.Length) return relicInfos[best];
            return null;
        }
        #endregion

        #region Return Portal & Transition
        /// <summary>
        /// Initiates the return dive transition back to Station III in Welcome Chamber.
        /// </summary>
        public void TriggerReturnTransition()
        {
            if (returnSphere == null) return;

            var divingTransition = FindObjectOfType<Xenoasis.Player.SphereDivingTransition>();
            if (divingTransition != null && !divingTransition.IsTransitioning)
            {
                Debug.Log("[HumanityWorldManager] ✦ Returning to Welcome Chamber Station III...");
                TriggerPicoHaptics(0.9f, 0.2f);
                divingTransition.BeginReturnTransition(returnSphere);
            }
        }

        public static void TriggerPicoHaptics(float strength, float duration)
        {
            #if !UNITY_EDITOR
            try
            {
                var leftHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
                var rightHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
                if (leftHand.isValid) leftHand.SendHapticImpulse(0, strength, duration);
                if (rightHand.isValid) rightHand.SendHapticImpulse(0, strength, duration);
            }
            catch {}
            #endif
        }
        #endregion

        #region Particle System Controls
        public void SetParticleIntensity(float multiplier)
        {
            if (auroraRibbons != null)
            {
                var em = auroraRibbons.emission;
                em.rateOverTimeMultiplier = 25f * multiplier;
            }
            if (seedSproutSpores != null)
            {
                var em = seedSproutSpores.emission;
                em.rateOverTimeMultiplier = 35f * multiplier;
            }
            if (whisperingGlyphs != null)
            {
                var em = whisperingGlyphs.emission;
                em.rateOverTimeMultiplier = 20f * multiplier;
            }
            if (glacialDiamondDust != null)
            {
                var em = glacialDiamondDust.emission;
                em.rateOverTimeMultiplier = 40f * multiplier;
            }
        }
        #endregion
    }
}
