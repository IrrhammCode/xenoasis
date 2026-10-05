using UnityEngine;

/// <summary>
/// XENOASIS — EditorFreeLookCamera.cs
/// Enables smooth WASD movement and Right-Click mouselook in Unity Editor Play Mode.
/// Respects parent transform orientation and prevents falling out of moving vehicles.
/// Automatically disabled when running on PICO / Android VR device so XR tracking takes over.
/// </summary>
public class EditorFreeLookCamera : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.5f;
    [SerializeField] private float smoothTime = 0.05f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Perspective Mode (Toggle with 'V' or 'C')")]
    [SerializeField] private bool isThirdPerson = false;
    [SerializeField] private float thirdPersonDistance = 2.4f;
    [SerializeField] private float thirdPersonHeight = 1.35f;
    [SerializeField] private float thirdPersonShoulderOffset = 0.30f;
    [SerializeField] private Vector3 firstPersonLocalPos = new Vector3(0f, 1.55f, 0.08f); // Alien diplomat eye level

    private float pitch = 0f;
    private float yaw = 0f;
    private float targetPitch = 0f;
    private float targetYaw = 0f;
    private float pitchVelocity;
    private float yawVelocity;

    public bool IsThirdPerson => isThirdPerson;

    public void SetLookOrientation(float newPitch, float newYaw)
    {
        pitch = newPitch;
        yaw = newYaw;
        targetPitch = newPitch;
        targetYaw = newYaw;
    }

    public void LookAt(Vector3 worldTarget)
    {
        Vector3 dir = (worldTarget - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            Vector3 euler = rot.eulerAngles;
            float p = euler.x > 180f ? euler.x - 360f : euler.x;
            SetLookOrientation(p, euler.y);
        }
    }

    private void Start()
    {
        Vector3 rot = transform.localEulerAngles;
        pitch = rot.x;
        yaw = rot.y;
        targetPitch = pitch;
        targetYaw = yaw;
    }

    private void Update()
    {
        // Smooth Mouselook enabled at all times (Space Orbit, UFO Cockpit, Atmospheric Descent, Chamber)
        // Hold Right Mouse Button (or Left Mouse Button) to look around 360 degrees
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            targetYaw += Input.GetAxis("Mouse X") * mouseSensitivity * 10f;
            targetPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * 10f;
            targetPitch = Mathf.Clamp(targetPitch, -82f, 82f);
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVelocity, smoothTime);
        pitch = Mathf.SmoothDampAngle(pitch, targetPitch, ref pitchVelocity, smoothTime);

        // WASD Movement (enabled once traveler lands on dais for Chamber Exploration)
        bool canMove = (UFODescentSequence.Instance == null || UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.ChamberExploration);

        // Perspective Mode Toggle (V / C keys or mouse scrollwheel) in Chamber Exploration
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C))
            {
                SetThirdPersonMode(!isThirdPerson);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.05f && isThirdPerson)
            {
                SetThirdPersonMode(false);
            }
            else if (scroll < -0.05f && !isThirdPerson)
            {
                SetThirdPersonMode(true);
            }
        }

        // Apply Camera Perspective Positioning
        if (isThirdPerson && canMove)
        {
            Vector3 focusPoint = new Vector3(0f, thirdPersonHeight, 0f);
            Quaternion orbitRot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 orbitDir = orbitRot * -Vector3.forward;
            Vector3 sideOffset = Quaternion.Euler(0f, yaw, 0f) * Vector3.right * thirdPersonShoulderOffset;
            Vector3 targetPos = focusPoint + orbitDir * thirdPersonDistance + sideOffset;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 12f);
            Vector3 lookTarget = focusPoint + sideOffset * 0.4f;
            transform.localRotation = Quaternion.LookRotation(lookTarget - transform.localPosition, Vector3.up);
        }
        else
        {
            // In cockpit: local position is Vector3.zero (aligned with pilot seat)
            // Once egressing from UFO hatch, descending in tractor beam, touching down, or exploring:
            // ALWAYS maintain standing eye level (1.55m above rig ground) so camera never sinks into floor!
            bool inUfoCockpit = UFODescentSequence.Instance != null &&
                (UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.SpaceFlight ||
                 UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.ReEntryBurn ||
                 UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.CloudPenetration ||
                 UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.SurfaceApproach);

            Vector3 targetFpPos = inUfoCockpit ? Vector3.zero : firstPersonLocalPos;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetFpPos, Time.deltaTime * 12f);
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        if (canMove)
        {
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1.0f);

            // Grounded horizontal movement relative to yaw orientation
            Vector3 forward = Vector3.ProjectOnPlane(Quaternion.Euler(0f, yaw, 0f) * Vector3.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(Quaternion.Euler(0f, yaw, 0f) * Vector3.right, Vector3.up).normalized;
            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move += forward;
            if (Input.GetKey(KeyCode.S)) move -= forward;
            if (Input.GetKey(KeyCode.D)) move += right;
            if (Input.GetKey(KeyCode.A)) move -= right;

            Transform moveTarget = transform.parent != null ? transform.parent : transform;

            if (move.sqrMagnitude > 0.001f)
            {
                Vector3 moveDelta = move.normalized * speed * Time.deltaTime;
                Vector3 checkStart = moveTarget.position + Vector3.up * 0.4f;
                Vector3 checkEnd = moveTarget.position + Vector3.up * 1.5f;

                // Sphere/Capsule check for physical obstacles (tables, consoles, walls)
                bool blocked = false;
                RaycastHit obstHit;
                if (Physics.CapsuleCast(checkStart, checkEnd, 0.30f, move.normalized, out obstHit, moveDelta.magnitude + 0.08f))
                {
                    if (!obstHit.transform.IsChildOf(moveTarget) && !obstHit.collider.isTrigger &&
                        obstHit.collider.name != "Landing_Dais_Plinth" && obstHit.collider.name != "Museum_Floor_Foundation")
                    {
                        blocked = true;
                    }
                }

                if (!blocked)
                {
                    Vector3 nextPos = moveTarget.position + moveDelta;

                    // Circular Chamber Room Boundary Clamping (Radius <= 16.5m)
                    Vector2 horiz = new Vector2(nextPos.x, nextPos.z);
                    const float maxRadius = 16.5f;
                    if (horiz.sqrMagnitude > maxRadius * maxRadius)
                    {
                        horiz = horiz.normalized * maxRadius;
                        nextPos.x = horiz.x;
                        nextPos.z = horiz.y;
                    }

                    moveTarget.position = new Vector3(nextPos.x, moveTarget.position.y, nextPos.z);
                }
            }

            // Grounded Walking Physics (Strictly clamped to Dais Y=0.24m or Museum Floor Y=0.03m)
            // Exhibits, consoles, and tables are OBSTACLES, NEVER ground to walk on!
            float horizontalDist = new Vector2(moveTarget.position.x, moveTarget.position.z).magnitude;
            float targetGroundY = (horizontalDist <= 5.25f) ? 0.24f : 0.03f;

            // Smoothly step between dais and floor (step height = 0.21m) without flying or snapping
            float newY = Mathf.MoveTowards(moveTarget.position.y, targetGroundY, 4.0f * Time.deltaTime);
            moveTarget.position = new Vector3(moveTarget.position.x, newY, moveTarget.position.z);
        }
    }

    public void SetThirdPersonMode(bool thirdPerson)
    {
        isThirdPerson = thirdPerson;
        if (AlienPlayerRig.Instance != null)
        {
            AlienPlayerRig.Instance.SetPerspectiveMode(isThirdPerson);
        }
        if (AlienVRHands.Instance != null)
        {
            AlienVRHands.Instance.SetHandsActive(!isThirdPerson);
        }
    }
#endif
}
