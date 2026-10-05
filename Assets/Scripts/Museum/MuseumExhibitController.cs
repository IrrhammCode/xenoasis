using UnityEngine;
using TMPro;

/// <summary>
/// XENOASIS — MuseumExhibitController.cs
/// Base controller for interactive pavilions in the Museum of Humanity.
/// Manages player proximity detection, highlight illumination, and exhibit interaction.
/// </summary>
public class MuseumExhibitController : MonoBehaviour
{
    [Header("Exhibit Info")]
    [SerializeField] protected string exhibitTitle = "Artefak Peradaban";
    [SerializeField] protected string exhibitEra = "Era Kemanusiaan";
    [TextArea(2, 4)]
    [SerializeField] protected string exhibitDescription = "Deskripsi artefak peradaban manusia.";

    [Header("Proximity & Interaction")]
    [SerializeField] protected float interactionRange = 3.2f;
    [SerializeField] protected KeyCode interactKey = KeyCode.E;
    [SerializeField] protected TextMeshPro placardText;
    [SerializeField] protected GameObject interactionPrompt;
    [SerializeField] protected Light exhibitSpotlight;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip activationClip;

    protected bool isPlayerNearby = false;
    protected bool isInteracting = false;
    protected Transform playerCameraTransform;
    protected float baseSpotlightIntensity = 1.8f;

    protected virtual void Start()
    {
        if (Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }

        if (exhibitSpotlight != null)
        {
            baseSpotlightIntensity = exhibitSpotlight.intensity;
        }

        UpdatePlacard();
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (playerCameraTransform == null && Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }

        if (playerCameraTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerCameraTransform.position);
        bool nearby = dist <= interactionRange;

        if (nearby != isPlayerNearby)
        {
            isPlayerNearby = nearby;
            OnPlayerProximityChanged(isPlayerNearby);
        }

        if (isPlayerNearby)
        {
            if (Input.GetKeyDown(interactKey))
            {
                ToggleInteraction();
            }
        }
    }

    protected virtual void OnPlayerProximityChanged(bool nearby)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(nearby);
        }

        if (exhibitSpotlight != null)
        {
            exhibitSpotlight.intensity = nearby ? baseSpotlightIntensity * 1.5f : baseSpotlightIntensity;
        }
    }

    public virtual void ToggleInteraction()
    {
        if (isInteracting)
        {
            StopInteraction();
        }
        else
        {
            StartInteraction();
        }
    }

    public virtual void StartInteraction()
    {
        isInteracting = true;
        if (audioSource != null && activationClip != null)
        {
            audioSource.PlayOneShot(activationClip, 0.85f);
        }
        Debug.Log($"[Museum of Humanity] Interacting with: {exhibitTitle}");
    }

    public virtual void StopInteraction()
    {
        isInteracting = false;
    }

    protected virtual void UpdatePlacard()
    {
        if (placardText != null)
        {
            placardText.text =
                $"<b><color=#00E5FF>{exhibitTitle.ToUpper()}</color></b>\n" +
                $"<size=75%><color=#FFD700>[ {exhibitEra} ]</color>\n" +
                $"<color=#E0F7FA>{exhibitDescription}</color></size>";
        }
    }

    public void SetupPlacard(string title, string era, string desc, TextMeshPro textComponent, GameObject promptObj, Light spot)
    {
        exhibitTitle = title;
        exhibitEra = era;
        exhibitDescription = desc;
        placardText = textComponent;
        interactionPrompt = promptObj;
        exhibitSpotlight = spot;
        UpdatePlacard();
    }
}
