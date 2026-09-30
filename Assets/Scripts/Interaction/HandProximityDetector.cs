using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// XENOASIS — HandProximityDetector.cs
/// Attach to any interactive object. Detects PICO 4 Ultra hand-tracking proximity.
/// Uses PICO PXR_HandTracking API to get palm joint positions and calculates
/// distance/orientation relative to this object's collider.
///
/// NOTE: This script uses the PICO Unity Integration SDK.
/// Make sure com.unity.xr.picoxr or the PICO OpenXR plugin is installed.
/// If building without PICO SDK, use the USE_MOCK_HANDS define for testing.
/// </summary>
public class HandProximityDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Maximum detection radius in meters")]
    [SerializeField] private float detectionRadius = 0.5f;

    [Tooltip("Distance at which NormalizedProximity = 1.0 (touching)")]
    [SerializeField] private float touchDistance = 0.05f;

    [Tooltip("Smoothing factor for proximity value (lerp speed)")]
    [SerializeField] private float smoothingSpeed = 8f;

    [Header("Hand Selection")]
    [SerializeField] private bool detectLeftHand = true;
    [SerializeField] private bool detectRightHand = true;
    [SerializeField] private bool requireBothHands = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    [Header("Events")]
    public UnityEvent<float> OnProximityChanged;   // 0-1 normalized proximity
    public UnityEvent OnHandEnterRange;             // first frame hand enters radius
    public UnityEvent OnHandExitRange;              // first frame hand leaves radius
    public UnityEvent OnHandTouching;               // hand is within touchDistance

    // Public properties for other scripts to read
    /// <summary>0 = out of range, 1 = touching the object</summary>
    public float NormalizedProximity { get; private set; }

    /// <summary>True if the palm normal is facing toward this object</summary>
    public bool IsPalmFacingObject { get; private set; }

    /// <summary>True if both left and right hands are within detection radius</summary>
    public bool IsBothHandsPresent { get; private set; }

    /// <summary>True if at least one hand is within detection radius</summary>
    public bool IsHandInRange { get; private set; }

    /// <summary>World position of the closest detected palm</summary>
    public Vector3 ClosestPalmPosition { get; private set; }

    /// <summary>World position of the left palm (if tracked)</summary>
    public Vector3 LeftPalmPosition { get; private set; }

    /// <summary>World position of the right palm (if tracked)</summary>
    public Vector3 RightPalmPosition { get; private set; }

    // Internal state
    private bool wasInRange = false;
    private float rawProximity = 0f;
    private Vector3 objectCenter;

    private void Update()
    {
        objectCenter = transform.position;

        // Get hand positions from PICO SDK (or mock positions for editor testing)
        bool leftTracked = false, rightTracked = false;
        Vector3 leftPalm = Vector3.zero, rightPalm = Vector3.zero;
        Vector3 leftPalmNormal = Vector3.forward, rightPalmNormal = Vector3.forward;

        GetHandPositions(
            out leftTracked, out leftPalm, out leftPalmNormal,
            out rightTracked, out rightPalm, out rightPalmNormal
        );

        LeftPalmPosition = leftPalm;
        RightPalmPosition = rightPalm;

        // Calculate distances
        float leftDist = leftTracked && detectLeftHand
            ? Vector3.Distance(leftPalm, objectCenter)
            : float.MaxValue;
        float rightDist = rightTracked && detectRightHand
            ? Vector3.Distance(rightPalm, objectCenter)
            : float.MaxValue;

        // Determine closest hand
        float closestDist;
        Vector3 closestPalm;
        Vector3 closestNormal;

        if (leftDist <= rightDist)
        {
            closestDist = leftDist;
            closestPalm = leftPalm;
            closestNormal = leftPalmNormal;
        }
        else
        {
            closestDist = rightDist;
            closestPalm = rightPalm;
            closestNormal = rightPalmNormal;
        }

        ClosestPalmPosition = closestPalm;

        // Both hands present check
        bool leftInRange = leftDist <= detectionRadius;
        bool rightInRange = rightDist <= detectionRadius;
        IsBothHandsPresent = leftInRange && rightInRange;

        // Compute normalized proximity (0 = out of range, 1 = touching)
        if (closestDist >= detectionRadius)
        {
            rawProximity = 0f;
        }
        else if (closestDist <= touchDistance)
        {
            rawProximity = 1f;
        }
        else
        {
            rawProximity = 1f - Mathf.InverseLerp(touchDistance, detectionRadius, closestDist);
        }

        // Apply requireBothHands filter
        if (requireBothHands && !IsBothHandsPresent)
        {
            rawProximity = 0f;
        }

        // Smooth the value
        NormalizedProximity = Mathf.Lerp(NormalizedProximity, rawProximity, Time.deltaTime * smoothingSpeed);

        // Palm facing check (dot product of palm normal vs direction to object)
        Vector3 dirToObject = (objectCenter - closestPalm).normalized;
        float dot = Vector3.Dot(closestNormal, dirToObject);
        IsPalmFacingObject = dot > 0.3f; // palm is roughly facing the object

        // Determine range state
        bool inRange = rawProximity > 0.01f;
        IsHandInRange = inRange;

        // Fire events
        OnProximityChanged?.Invoke(NormalizedProximity);

        if (inRange && !wasInRange)
            OnHandEnterRange?.Invoke();

        if (!inRange && wasInRange)
            OnHandExitRange?.Invoke();

        if (rawProximity >= 0.95f)
            OnHandTouching?.Invoke();

        wasInRange = inRange;
    }

    /// <summary>
    /// Retrieves hand positions from PICO SDK or mock input for editor testing.
    /// </summary>
    private void GetHandPositions(
        out bool leftTracked, out Vector3 leftPalm, out Vector3 leftPalmNormal,
        out bool rightTracked, out Vector3 rightPalm, out Vector3 rightPalmNormal)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        // ===== PICO XR Hand Tracking (Runtime on Device) =====
        // Uses PXR_HandTracking from PICO Unity Integration SDK
        // Joint index for palm center is typically HandJoint.Palm (index 0) or Wrist
        
        leftTracked = false;
        leftPalm = Vector3.zero;
        leftPalmNormal = Vector3.forward;
        rightTracked = false;
        rightPalm = Vector3.zero;
        rightPalmNormal = Vector3.forward;

        try
        {
            // Try to get hand tracking data via PICO SDK
            // PXR_HandTracking.GetJointLocations() returns joint positions
            // Adapt these calls to your specific PICO SDK version

            // Left Hand
            var leftHandData = new Unity.XR.PXR.HandJointLocations();
            if (Unity.XR.PXR.PXR_HandTracking.GetJointLocations(
                Unity.XR.PXR.HandType.HandLeft, ref leftHandData))
            {
                if (leftHandData.isActive > 0)
                {
                    leftTracked = true;
                    // Palm joint is typically at index 0
                    var palmJoint = leftHandData.jointLocations[0];
                    leftPalm = new Vector3(
                        palmJoint.pose.Position.x,
                        palmJoint.pose.Position.y,
                        -palmJoint.pose.Position.z // flip Z for Unity coordinate system
                    );
                    Quaternion palmRot = new Quaternion(
                        palmJoint.pose.Orientation.x,
                        palmJoint.pose.Orientation.y,
                        -palmJoint.pose.Orientation.z,
                        -palmJoint.pose.Orientation.w
                    );
                    leftPalmNormal = palmRot * Vector3.forward;
                }
            }

            // Right Hand
            var rightHandData = new Unity.XR.PXR.HandJointLocations();
            if (Unity.XR.PXR.PXR_HandTracking.GetJointLocations(
                Unity.XR.PXR.HandType.HandRight, ref rightHandData))
            {
                if (rightHandData.isActive > 0)
                {
                    rightTracked = true;
                    var palmJoint = rightHandData.jointLocations[0];
                    rightPalm = new Vector3(
                        palmJoint.pose.Position.x,
                        palmJoint.pose.Position.y,
                        -palmJoint.pose.Position.z
                    );
                    Quaternion palmRot = new Quaternion(
                        palmJoint.pose.Orientation.x,
                        palmJoint.pose.Orientation.y,
                        -palmJoint.pose.Orientation.z,
                        -palmJoint.pose.Orientation.w
                    );
                    rightPalmNormal = palmRot * Vector3.forward;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[HandProximityDetector] PICO hand tracking error: {e.Message}");
        }
#else
        // ===== MOCK HANDS (Unity Editor Testing) =====
        // Uses mouse position projected into world space for quick iteration
        leftTracked = false;
        leftPalm = Vector3.zero;
        leftPalmNormal = Vector3.forward;
        rightTracked = true;
        rightPalmNormal = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;

        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            rightPalm = ray.origin + ray.direction * 0.5f;
            rightTracked = true;
        }
        else
        {
            rightPalm = Vector3.zero;
            rightTracked = false;
        }
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Detection radius
        Gizmos.color = new Color(0f, 1f, 0.82f, 0.15f); // Cyan transparent
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Touch radius
        Gizmos.color = new Color(1f, 0.82f, 0.4f, 0.3f); // Gold transparent
        Gizmos.DrawWireSphere(transform.position, touchDistance);
    }
}
