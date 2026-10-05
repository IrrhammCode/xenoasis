using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// XENOASIS — AlienPlayerRig.cs
/// Embodies the player as an extraterrestrial alien diplomat / traveler:
/// - Connects the high-fidelity 3D rigged Alien Diplomat character model (alien_diplomat_walk.glb) directly to the player rig.
/// - Controls skeletal animation playback: natural walking animation with swinging arms and stepping legs when moving, subtle idle breathing/posture when standing.
/// - In First-Person (Cockpit & exploration): Casts shadows cleanly without head-mesh clipping into the camera view.
/// - In Third-Person (Chase Camera - toggle with 'V' / 'C'): Displays the full animated 3D Alien Diplomat body walking and exploring.
/// - Synchronizes smooth turning physics towards camera view and movement direction.
/// </summary>
public class AlienPlayerRig : MonoBehaviour
{
    private static AlienPlayerRig _instance;
    public static AlienPlayerRig Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AlienPlayerRig>();
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("Player 3D Alien Avatar Body")]
    [SerializeField] private GameObject alienCharacterPrefab;
    [SerializeField] private Transform playerAlienBody;
    [SerializeField] private Renderer playerBodyRenderer;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private RuntimeAnimatorController alienAnimatorController;

    public Renderer PlayerBodyRenderer => playerBodyRenderer;
    public Animator BodyAnimator => bodyAnimator;

    [Header("Bio-Mirror / Avatar Station")]
    [SerializeField] private Transform bioMirrorStation;

    private float bodyYaw = 0f;
    private float bodyYawVelocity = 0f;
    private bool isThirdPersonMode = false;

    private void Awake()
    {
        Debug.Log("[AlienPlayerRig] Awake running on " + gameObject.name);
        _instance = this;

        // Delete any old primitive hands/arms from earlier iterations
        CleanOldAlienObjects();

        BuildPlayerAlienBody();
        BuildBioMirrorAvatar();

        // Default to First-Person view (shadow only to prevent head clipping)
        SetPerspectiveMode(false);
    }

    private void CleanOldAlienObjects()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Transform oldHands = cam.transform.Find("AlienHands_FirstPerson");
            if (oldHands != null) DestroyImmediate(oldHands.gameObject);
        }

        Transform handsInRig = transform.Find("AlienHands_FirstPerson");
        if (handsInRig != null) DestroyImmediate(handsInRig.gameObject);
    }

    private void BuildPlayerAlienBody()
    {
        Transform existing = transform.Find("PlayerAlienBody");
        GameObject bodyObj;
        if (existing != null)
        {
            bodyObj = existing.gameObject;
        }
        else
        {
            bodyObj = new GameObject("PlayerAlienBody");
            bodyObj.transform.SetParent(transform, false);
        }

        bodyObj.transform.localPosition = Vector3.zero;
        bodyObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        bodyObj.transform.localScale = Vector3.one * 1.80f;

        // Check if rigged skeleton already present
        var existingSmr = bodyObj.GetComponentInChildren<SkinnedMeshRenderer>();
        var existingAnim = bodyObj.GetComponent<Animator>() ?? bodyObj.GetComponentInChildren<Animator>();

        if (existingSmr == null)
        {
            // Remove any legacy static mesh filter/renderer
            var oldMf = bodyObj.GetComponent<MeshFilter>();
            if (oldMf != null) DestroyImmediate(oldMf);
            var oldMr = bodyObj.GetComponent<MeshRenderer>();
            if (oldMr != null) DestroyImmediate(oldMr);

            GameObject riggedPrefab = null;
#if UNITY_EDITOR
            riggedPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/alien_diplomat_walk.glb");
#endif
            if (riggedPrefab == null) riggedPrefab = alienCharacterPrefab;

            if (riggedPrefab != null)
            {
                var instantiated = Instantiate(riggedPrefab, bodyObj.transform, false);
                instantiated.name = "AlienDiplomat_SkeletalMesh";
                instantiated.transform.localPosition = Vector3.zero;
                instantiated.transform.localRotation = Quaternion.identity;
                instantiated.transform.localScale = Vector3.one;

                existingSmr = instantiated.GetComponentInChildren<SkinnedMeshRenderer>();
                existingAnim = instantiated.GetComponent<Animator>();
            }
        }

        if (existingAnim == null)
        {
            existingAnim = bodyObj.AddComponent<Animator>();
        }

        bodyAnimator = existingAnim;
        bodyAnimator.applyRootMotion = false;

#if UNITY_EDITOR
        if (alienAnimatorController == null)
        {
            alienAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/AlienDiplomatAnimator.controller");
        }
#endif
        if (alienAnimatorController != null && (bodyAnimator.runtimeAnimatorController == null || bodyAnimator.runtimeAnimatorController != alienAnimatorController))
        {
            bodyAnimator.runtimeAnimatorController = alienAnimatorController;
        }

        if (existingSmr != null)
        {
            playerBodyRenderer = existingSmr;
            existingSmr.receiveShadows = true;
        }
        else
        {
            playerBodyRenderer = bodyObj.GetComponent<MeshRenderer>();
        }

        playerAlienBody = bodyObj.transform;
    }

    private void Update()
    {
        // Check movement input
        bool canMove = (UFODescentSequence.Instance == null || UFODescentSequence.Instance.CurrentState == UFODescentSequence.SequenceState.ChamberExploration);
        bool isMoving = canMove && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

        // Animate skeletal limbs (arms swing, legs stride) when walking; standing posture when idle
        if (bodyAnimator != null)
        {
            bodyAnimator.SetBool("IsWalking", isMoving);
            bodyAnimator.SetFloat("Speed", isMoving ? 1f : 0f);
        }

        // Update 3D Alien Character Body Movement & Orientation
        if (playerAlienBody != null)
        {
            float targetBodyYaw = 0f;
            Camera cam = Camera.main;
            float camYaw = cam != null ? cam.transform.localEulerAngles.y : 0f;

            if (isThirdPersonMode)
            {
                // In third person: body turns smoothly towards movement direction if moving
                float h = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
                float v = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);

                if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f)
                {
                    float moveAngle = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
                    targetBodyYaw = camYaw + moveAngle;
                }
                else
                {
                    targetBodyYaw = bodyYaw;
                }
            }
            else
            {
                targetBodyYaw = camYaw;
            }

            bodyYaw = Mathf.SmoothDampAngle(bodyYaw, targetBodyYaw, ref bodyYawVelocity, 0.08f);

            // Ground position is maintained strictly at rig base (Y=0 locally)
            playerAlienBody.localPosition = Vector3.zero;
            playerAlienBody.localRotation = Quaternion.Euler(0f, bodyYaw + 90f, 0f);
        }
    }

    public void SetPerspectiveMode(bool thirdPerson)
    {
        isThirdPersonMode = thirdPerson;

        if (playerBodyRenderer == null && playerAlienBody != null)
        {
            playerBodyRenderer = (Renderer)playerAlienBody.GetComponentInChildren<SkinnedMeshRenderer>()
                ?? (Renderer)playerAlienBody.GetComponent<MeshRenderer>();
        }

        if (playerBodyRenderer != null)
        {
            // In Third Person: render full 3D alien character model walking and moving!
            // In First Person: cast shadows only so the head/collar never clips the camera view!
            playerBodyRenderer.shadowCastingMode = thirdPerson
                ? ShadowCastingMode.On
                : ShadowCastingMode.ShadowsOnly;
        }
    }

    private void BuildBioMirrorAvatar()
    {
        GameObject mirrorStationObj = GameObject.Find("AlienBioMirrorStation");
        if (mirrorStationObj == null)
        {
            mirrorStationObj = new GameObject("AlienBioMirrorStation");
            mirrorStationObj.transform.position = new Vector3(-3.20f, 0.08f, 1.20f);
            mirrorStationObj.transform.rotation = Quaternion.Euler(0, 110.56f, 0);
        }

        bioMirrorStation = mirrorStationObj.transform;

        // The player now embodies the Alien Diplomat directly!
        // Deactivate duplicate static clone at the station so the pedestal is open for the player.
        Transform staticModel = mirrorStationObj.transform.Find("AlienDiplomat_Tripo3D");
        if (staticModel != null)
        {
            staticModel.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Debug.LogWarning("[AlienPlayerRig] OnDestroy CALLED on " + gameObject.name + "! StackTrace:\n" + System.Environment.StackTrace);
    }
}
