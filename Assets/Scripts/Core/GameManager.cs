using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// XENOASIS — GameManager.cs
/// Controls the overall game flow: Inception → Exploration → Climax → End.
/// Manages sequence timing, fade transitions, and end screen.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GamePhase
    {
        Inception,      // Fade in, establishing presence and awe
        Exploration,    // Player interacts with the 3 offerings
        Climax,         // All offerings sealed, beacon fires
        End             // "The gift was received." end screen
    }

    [Header("Current State")]
    [SerializeField] private GamePhase currentPhase = GamePhase.Inception;

    [Header("Inception Settings")]
    [Tooltip("Duration of the initial fade-in from black (seconds)")]
    [SerializeField] private float inceptionFadeInDuration = 3.0f;
    [Tooltip("Delay before fade-in starts (seconds)")]
    [SerializeField] private float inceptionDelay = 1.0f;

    [Header("Climax Settings")]
    [Tooltip("Delay after all offerings complete before climax starts (seconds)")]
    [SerializeField] private float climaxDelay = 2.0f;
    [Tooltip("Duration of the climax beacon sequence (seconds)")]
    [SerializeField] private float climaxDuration = 45.0f;

    [Header("End Screen Settings")]
    [Tooltip("Duration of the white fade-out at the end (seconds)")]
    [SerializeField] private float endFadeOutDuration = 3.0f;

    [Header("References")]
    [SerializeField] private FadeController fadeController;
    [SerializeField] private BeaconClimaxVFX beaconClimaxVFX;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private StardustController stardustController;
    [SerializeField] private PassthroughTransition passthroughTransition;
    [SerializeField] private GameObject endScreenCanvas;
    [SerializeField] private BiomeTransitionController biomeTransitionController;
    [SerializeField] private UFOController ufoController;
    [SerializeField] private UFODescentSequence ufoDescentSequence;

    [Header("Events")]
    public UnityEvent OnInceptionStart;
    public UnityEvent OnExplorationStart;
    public UnityEvent OnClimaxStart;
    public UnityEvent OnEndStart;

    public GamePhase CurrentPhase => currentPhase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Hide end screen
        if (endScreenCanvas != null)
            endScreenCanvas.SetActive(false);

        // Subscribe to OfferingManager
        if (OfferingManager.Instance != null)
        {
            OfferingManager.Instance.AllOfferingsCompleted += OnAllOfferingsSealed;
        }

        // Begin the experience
        StartCoroutine(InceptionSequence());
    }

    private void OnDestroy()
    {
        if (OfferingManager.Instance != null)
        {
            OfferingManager.Instance.AllOfferingsCompleted -= OnAllOfferingsSealed;
        }
    }

    /// <summary>
    /// Phase 1: The Inception — fade from black, establish awe.
    /// </summary>
    private IEnumerator InceptionSequence()
    {
        currentPhase = GamePhase.Inception;
        OnInceptionStart?.Invoke();

        Debug.Log("[GameManager] Phase: INCEPTION — Starting fade in...");

        // Start with black screen
        if (fadeController != null)
            fadeController.SetFadeImmediate(1f); // fully black

        // Optional: MR Passthrough opening (PICO 4 Ultra)
        if (passthroughTransition != null)
            yield return passthroughTransition.PassthroughSequence();

        // Wait before starting fade
        yield return new WaitForSeconds(inceptionDelay);

        // Start ambient audio
        if (audioManager != null)
            audioManager.StartAmbient();

        // Fade in from black
        if (fadeController != null)
            fadeController.FadeTo(0f, inceptionFadeInDuration); // fade to clear

        yield return new WaitForSeconds(inceptionFadeInDuration);

        // Transition to Exploration phase
        currentPhase = GamePhase.Exploration;
        OnExplorationStart?.Invoke();
        Debug.Log("[GameManager] Phase: EXPLORATION — Player can now interact with offerings.");
    }

    /// <summary>
    /// Called when all 3 offerings are sealed.
    /// </summary>
    private void OnAllOfferingsSealed()
    {
        if (currentPhase == GamePhase.Climax || currentPhase == GamePhase.End)
            return;

        StartCoroutine(ClimaxSequence());
    }

    /// <summary>
    /// Phase 3: The Climax — beacon fires, cosmos responds.
    /// </summary>
    private IEnumerator ClimaxSequence()
    {
        currentPhase = GamePhase.Climax;
        OnClimaxStart?.Invoke();

        Debug.Log("[GameManager] Phase: CLIMAX — All offerings sealed. Beacon igniting...");

        // Brief pause for dramatic effect
        yield return new WaitForSeconds(climaxDelay);

        // Fire the beacon VFX
        if (beaconClimaxVFX != null)
            beaconClimaxVFX.IgniteBeacon();

        // Accelerate cosmic stardust toward the beacon
        if (stardustController != null)
            stardustController.SetClimaxMode(true);

        // Trigger climax audio
        if (audioManager != null)
            audioManager.PlayClimaxCrescendo();

        // Trigger biome climax transition (all biomes merge into golden burst)
        if (biomeTransitionController != null)
            biomeTransitionController.TriggerClimax();

        // UFO responds to beacon with golden light
        if (ufoController != null)
            ufoController.RespondToBeacon();

        // Wait for the climax sequence to play out
        yield return new WaitForSeconds(climaxDuration);

        // Fade to white
        if (fadeController != null)
            fadeController.FadeToWhite(endFadeOutDuration);

        yield return new WaitForSeconds(endFadeOutDuration + 1f);

        // Show end screen
        StartCoroutine(EndSequence());
    }

    /// <summary>
    /// Phase 4: End — "The gift was received."
    /// </summary>
    private IEnumerator EndSequence()
    {
        currentPhase = GamePhase.End;
        OnEndStart?.Invoke();

        Debug.Log("[GameManager] Phase: END — The gift was received.");

        // Show end screen UI
        if (endScreenCanvas != null)
            endScreenCanvas.SetActive(true);

        // Stop all audio gradually
        if (audioManager != null)
            audioManager.FadeOutAll(3f);

        yield return null;
    }
}
