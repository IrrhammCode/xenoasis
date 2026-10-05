using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Xenoasis.Player
{
    /// <summary>
    /// Handles the VR transition animation for diving into the Living Terran Sphere.
    /// Controls hands, sphere movement, visual effects, audio, haptics, and scene loading.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SphereDivingTransition : MonoBehaviour
    {
        [Header("Transforms & Anchors")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform leftHandAnchor;
        [SerializeField] private Transform rightHandAnchor;

        [Header("Visual Effects")]
        [SerializeField] private Material bubbleRefractionMaterial;
        [SerializeField] private ParticleSystem bubbleBurstParticles;
        [SerializeField] private CanvasGroup fadeOverlay;

        [Header("Audio")]
        [SerializeField] private AudioClip waterWhooshClip;
        [SerializeField] private AudioClip membraneBurstClip;
        [SerializeField] private AudioClip subBassDropClip;

        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Scene Management")]
        [SerializeField] private string targetSceneName = "World1_PrimordialCradle";
        [SerializeField] private string returnSceneName = "WelcomeChamber";

        private AudioSource audioSource;
        private Transform activeSphere;

        public bool IsTransitioning { get; private set; }
        public static event Action<string> OnWorldTransition;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (fadeOverlay != null)
            {
                fadeOverlay.alpha = 0f;
                fadeOverlay.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            IsTransitioning = false;
        }

        /// <summary>
        /// Main entry point for diving into the world.
        /// </summary>
        /// <param name="sphereTransform">The transform of the Living Terran Sphere being dived into.</param>
        public void BeginDiveTransition(Transform sphereTransform)
        {
            if (IsTransitioning) return;
            IsTransitioning = true;
            activeSphere = sphereTransform;
            StartCoroutine(DiveCoroutine());
        }

        /// <summary>
        /// Reverse transition back to the museum.
        /// </summary>
        /// <param name="returnSphereTransform">The sphere representing the return portal.</param>
        public void BeginReturnTransition(Transform returnSphereTransform)
        {
            if (IsTransitioning) return;
            IsTransitioning = true;
            activeSphere = returnSphereTransform;
            StartCoroutine(ReturnCoroutine());
        }

        private IEnumerator DiveCoroutine()
        {
            if (cameraTransform == null) cameraTransform = Camera.main != null ? Camera.main.transform : transform;

            Vector3 startSpherePos = activeSphere.position;
            Vector3 waistHeightPos = cameraTransform.position + cameraTransform.forward * 0.4f + Vector3.down * 0.5f;

            // Phase 1 - Sphere Summon (0.0-0.5s)
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                activeSphere.position = Vector3.Lerp(startSpherePos, waistHeightPos, liftCurve.Evaluate(t));
                yield return null;
            }

            // Phase 2 - Dual Hand Cradle (0.5-1.5s)
            if (audioSource != null && waterWhooshClip != null)
            {
                audioSource.PlayOneShot(waterWhooshClip);
            }

            Vector3 sphereLeftPoint = activeSphere.position - activeSphere.right * 0.15f;
            Vector3 sphereRightPoint = activeSphere.position + activeSphere.right * 0.15f;

            Vector3 leftHandStart = leftHandAnchor != null ? leftHandAnchor.position : sphereLeftPoint + Vector3.left;
            Vector3 rightHandStart = rightHandAnchor != null ? rightHandAnchor.position : sphereRightPoint + Vector3.right;

            elapsed = 0f;
            duration = 1.0f;
            TriggerPicoHaptics(120f, 0.2f, duration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                if (leftHandAnchor != null) leftHandAnchor.position = Vector3.Lerp(leftHandStart, sphereLeftPoint, liftCurve.Evaluate(t));
                if (rightHandAnchor != null) rightHandAnchor.position = Vector3.Lerp(rightHandStart, sphereRightPoint, liftCurve.Evaluate(t));
                
                yield return null;
            }

            // Phase 3 - Lift to Head (1.5-2.2s)
            Vector3 finalHeadPos = cameraTransform.position + cameraTransform.forward * 0.15f; // close to face
            Vector3 initialWaistPos = activeSphere.position;
            Vector3 initialScale = activeSphere.localScale;
            Vector3 targetScale = initialScale * 15f; // envelop viewport

            elapsed = 0f;
            duration = 0.7f;
            TriggerPicoHaptics(120f, 0.2f, 0.1f); // initial haptic, will ramp conceptually

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curvedT = liftCurve.Evaluate(t);
                
                activeSphere.position = Vector3.Lerp(initialWaistPos, finalHeadPos, curvedT);
                
                float dist = Vector3.Distance(activeSphere.position, cameraTransform.position);
                if (dist < 0.35f)
                {
                    activeSphere.localScale = Vector3.Lerp(initialScale, targetScale, scaleCurve.Evaluate(t));
                }

                // Simulate haptic ramp
                float freq = Mathf.Lerp(120f, 380f, t);
                float amp = Mathf.Lerp(0.2f, 0.7f, t);
                TriggerPicoHaptics(freq, amp, 0.05f);

                yield return null;
            }

            // Phase 4 - Bubble Refraction (2.0-2.5s)
            if (audioSource != null && subBassDropClip != null)
            {
                audioSource.PlayOneShot(subBassDropClip);
            }

            if (bubbleRefractionMaterial != null)
            {
                bubbleRefractionMaterial.SetFloat("_Distortion", 1.0f);
                bubbleRefractionMaterial.SetFloat("_Opacity", 1.0f);
            }
            yield return new WaitForSeconds(0.5f);

            // Phase 5 - Membrane Pop & Scene Load (2.2-2.8s)
            TriggerPicoHaptics(320f, 0.9f, 0.05f); // Sharp snap

            if (audioSource != null && membraneBurstClip != null)
            {
                audioSource.PlayOneShot(membraneBurstClip);
            }

            if (bubbleBurstParticles != null)
            {
                bubbleBurstParticles.transform.position = cameraTransform.position + cameraTransform.forward * 0.2f;
                bubbleBurstParticles.Play();
            }

            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.alpha = 1f;
            }

            // Async load
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            while (!asyncLoad.isDone)
            {
                if (fadeOverlay != null && fadeOverlay.alpha > 0)
                {
                    fadeOverlay.alpha -= Time.deltaTime * 1.5f;
                }
                yield return null;
            }

            IsTransitioning = false;
            OnWorldTransition?.Invoke(targetSceneName);
        }

        private IEnumerator ReturnCoroutine()
        {
            // Simple flash return sequence
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.alpha = 1f;
            }
            
            TriggerPicoHaptics(200f, 0.5f, 0.1f);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(returnSceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            IsTransitioning = false;
            OnWorldTransition?.Invoke(returnSceneName);
        }

        /// <summary>
        /// Static helper for PICO 4 Ultra broadband linear actuator haptics.
        /// Falls back to standard OpenXR haptic impulse.
        /// </summary>
        public static void TriggerPicoHaptics(float frequency, float amplitude, float duration)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, devices);
            
            foreach (var device in devices)
            {
                HapticCapabilities capabilities;
                if (device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
                {
                    // Basic fallback to OpenXR SendHapticImpulse. 
                    // PICO SDK specific haptics would be routed here if available via an extension package.
                    uint channel = 0;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}
