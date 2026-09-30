using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// XENOASIS — OfferingManager.cs
/// Central singleton that tracks the completion state of all 3 sacred offerings.
/// When all offerings are sealed, triggers the Climax Beacon sequence.
/// </summary>
public class OfferingManager : MonoBehaviour
{
    public static OfferingManager Instance { get; private set; }

    public enum OfferingState { Locked, Active, Complete }

    [Header("Offering States")]
    [SerializeField] private OfferingState[] offeringStates = new OfferingState[3];

    [Header("Offering Labels (for debugging)")]
    [SerializeField] private string[] offeringNames = { "Water", "Crystal", "Flora" };

    [Header("Pedestal References")]
    [Tooltip("Assign the 3 offering pedestal GameObjects here")]
    [SerializeField] private GameObject[] pedestals = new GameObject[3];

    [Header("Pedestal Materials")]
    [Tooltip("Material to apply when an offering is completed (gold glow)")]
    [SerializeField] private Material completedPedestalMaterial;

    [Header("Events")]
    public UnityEvent<int> OnOfferingComplete;
    public UnityEvent OnAllOfferingsComplete;

    // C# events for script-to-script communication
    public event Action<int> OfferingCompleted;
    public event Action AllOfferingsCompleted;

    private int completedCount = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize all offerings as Active (ready to interact)
        for (int i = 0; i < offeringStates.Length; i++)
        {
            offeringStates[i] = OfferingState.Active;
        }
    }

    /// <summary>
    /// Called by individual offering controllers when their interaction is complete.
    /// </summary>
    /// <param name="offeringIndex">0 = Water, 1 = Crystal, 2 = Flora</param>
    public void CompleteOffering(int offeringIndex)
    {
        if (offeringIndex < 0 || offeringIndex >= offeringStates.Length)
        {
            Debug.LogError($"[OfferingManager] Invalid offering index: {offeringIndex}");
            return;
        }

        if (offeringStates[offeringIndex] == OfferingState.Complete)
        {
            Debug.LogWarning($"[OfferingManager] Offering '{offeringNames[offeringIndex]}' is already complete.");
            return;
        }

        // Mark as complete
        offeringStates[offeringIndex] = OfferingState.Complete;
        completedCount++;

        Debug.Log($"[OfferingManager] Offering '{offeringNames[offeringIndex]}' sealed! ({completedCount}/3)");

        // Visual feedback: change pedestal material to gold glow
        if (pedestals[offeringIndex] != null && completedPedestalMaterial != null)
        {
            Renderer renderer = pedestals[offeringIndex].GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = completedPedestalMaterial;
            }
        }

        // Fire events
        OnOfferingComplete?.Invoke(offeringIndex);
        OfferingCompleted?.Invoke(offeringIndex);

        // Check if all offerings are complete
        if (completedCount >= 3)
        {
            Debug.Log("[OfferingManager] ALL OFFERINGS SEALED — Initiating Climax Beacon!");
            OnAllOfferingsComplete?.Invoke();
            AllOfferingsCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Query the state of a specific offering.
    /// </summary>
    public OfferingState GetOfferingState(int index)
    {
        if (index < 0 || index >= offeringStates.Length)
            return OfferingState.Locked;
        return offeringStates[index];
    }

    /// <summary>
    /// Returns true if all 3 offerings are complete.
    /// </summary>
    public bool AreAllComplete()
    {
        return completedCount >= 3;
    }

    /// <summary>
    /// Returns the number of completed offerings (0-3).
    /// </summary>
    public int GetCompletedCount()
    {
        return completedCount;
    }
}
