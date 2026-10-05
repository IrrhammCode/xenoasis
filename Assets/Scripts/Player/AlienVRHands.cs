using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// XENOASIS — AlienVRHands.cs
/// High-Fidelity First-Person Interactive Alien Hands ("VR-Style"):
/// - Uses the EXACT authentic 3D mesh geometry and PBR materials extracted directly from the Alien Diplomat character model.
/// - 100% visual consistency with the full-body character seen in the HoloBioMirror and Third-Person mode:
///   * Identical dark metallic/glossy blue armor finish.
///   * Identical ornate golden wrist gauntlet cuffs.
///   * Identical slender extraterrestrial alien hands and fingers.
/// - Dynamic First-Person Animations:
///   * Organic idle breathing sway.
///   * Mouse-look rotational inertia / heavy physical lag.
///   * Walking stride bobbing synchronized with WASD movement.
///   * Interactive Grab & Reach gesture on Click / E / F / VR Trigger / Grip.
/// - Integrates seamlessly with all museum interactive exhibits and HandProximityDetector.
/// - Automatically hides during Third-Person mode (V / C keys).
/// </summary>
public class AlienVRHands : MonoBehaviour
{
    private static AlienVRHands _instance;
    public static AlienVRHands Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AlienVRHands>();
            return _instance;
        }
    }

    [Header("Hand Rig Transforms")]
    [SerializeField] private GameObject handsRoot;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [Header("Extracted Authentic Meshes")]
    [SerializeField] private Mesh leftHandMesh;
    [SerializeField] private Mesh rightHandMesh;
    [SerializeField] private Material characterHandMaterial;

    [Header("Kinematics & Inertia")]
    [SerializeField] private float mouseLagSpeed = 12f;
    [SerializeField] private float idleSwaySpeed = 1.6f;
    [SerializeField] private float walkBobSpeed = 8.0f;
    [SerializeField] private float grabSpeed = 14f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 2.8f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    // Natural resting poses in camera space
    private Vector3 leftRestLocalPos = new Vector3(-0.28f, -0.22f, 0.46f);
    private Vector3 rightRestLocalPos = new Vector3(0.28f, -0.22f, 0.46f);
    private Quaternion leftRestRot = Quaternion.Euler(-10f, 15f, -10f);
    private Quaternion rightRestRot = Quaternion.Euler(-10f, -15f, 10f);

    // Grab poses (inward flex / grip around target)
    private Quaternion leftGrabRot = Quaternion.Euler(18f, 25f, -28f);
    private Quaternion rightGrabRot = Quaternion.Euler(18f, -25f, 28f);

    // Current animation states
    private float leftGrabStrength = 0f;
    private float rightGrabStrength = 0f;
    private float targetLeftGrab = 0f;
    private float targetRightGrab = 0f;

    // Inertia & bobbing
    private Vector3 inertiaOffset = Vector3.zero;
    private Vector3 inertiaVelocity = Vector3.zero;
    private float walkCycle = 0f;
    private Camera playerCam;
    private bool isHandsActive = true;

    // Interaction target
    private GameObject currentTargetObj = null;
    private string currentTargetPrompt = "";
    private float reachProgress = 0f;

    // Dynamic palm lights
    private Light leftPalmLight;
    private Light rightPalmLight;

    private void Awake()
    {
        _instance = this;
        playerCam = GetComponent<Camera>() ?? Camera.main;

        LoadAuthenticAssets();
        BuildVRHands();
    }

    private void Start()
    {
        var freeLook = GetComponent<EditorFreeLookCamera>() ?? FindObjectOfType<EditorFreeLookCamera>();
        if (freeLook != null)
        {
            SetHandsActive(!freeLook.IsThirdPerson);
        }
    }

    private void LoadAuthenticAssets()
    {
#if UNITY_EDITOR
        if (leftHandMesh == null)
        {
            leftHandMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/AlienHand_Left_Mesh.asset");
        }
        if (rightHandMesh == null)
        {
            rightHandMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/AlienHand_Right_Mesh.asset");
        }
        if (characterHandMaterial == null)
        {
            var charPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tripo/alien_diplomat_walk.glb");
            if (charPrefab != null)
            {
                var smr = charPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null) characterHandMaterial = smr.sharedMaterial;
            }
        }
#endif
        if (characterHandMaterial == null)
        {
            var alienRig = AlienPlayerRig.Instance ?? FindObjectOfType<AlienPlayerRig>();
            if (alienRig != null && alienRig.PlayerBodyRenderer != null)
            {
                characterHandMaterial = alienRig.PlayerBodyRenderer.sharedMaterial;
            }
        }
    }

    private void BuildVRHands()
    {
        if (handsRoot != null) DestroyImmediate(handsRoot);

        handsRoot = new GameObject("AlienVRHands_Rig");
        handsRoot.transform.SetParent(transform, false);
        handsRoot.transform.localPosition = Vector3.zero;
        handsRoot.transform.localRotation = Quaternion.identity;

        // 1. Build Authentic Left Hand
        GameObject leftObj = new GameObject("AlienHand_Left");
        leftObj.transform.SetParent(handsRoot.transform, false);
        leftObj.transform.localPosition = leftRestLocalPos;
        leftObj.transform.localRotation = leftRestRot;
        leftObj.transform.localScale = Vector3.one * 1.80f;

        var mfL = leftObj.AddComponent<MeshFilter>();
        mfL.sharedMesh = leftHandMesh;
        var mrL = leftObj.AddComponent<MeshRenderer>();
        mrL.sharedMaterial = characterHandMaterial;
        mrL.shadowCastingMode = ShadowCastingMode.Off; // Prevent shadow clutter in first-person

        var lightLObj = new GameObject("PalmGlow_Left");
        lightLObj.transform.SetParent(leftObj.transform, false);
        lightLObj.transform.localPosition = new Vector3(0f, 0.02f, 0.05f);
        leftPalmLight = lightLObj.AddComponent<Light>();
        leftPalmLight.type = LightType.Point;
        leftPalmLight.color = new Color(0.2f, 0.95f, 1.0f);
        leftPalmLight.range = 0.5f;
        leftPalmLight.intensity = 0.6f;

        leftHand = leftObj.transform;

        // 2. Build Authentic Right Hand
        GameObject rightObj = new GameObject("AlienHand_Right");
        rightObj.transform.SetParent(handsRoot.transform, false);
        rightObj.transform.localPosition = rightRestLocalPos;
        rightObj.transform.localRotation = rightRestRot;
        rightObj.transform.localScale = Vector3.one * 1.80f;

        var mfR = rightObj.AddComponent<MeshFilter>();
        mfR.sharedMesh = rightHandMesh;
        var mrR = rightObj.AddComponent<MeshRenderer>();
        mrR.sharedMaterial = characterHandMaterial;
        mrR.shadowCastingMode = ShadowCastingMode.Off;

        var lightRObj = new GameObject("PalmGlow_Right");
        lightRObj.transform.SetParent(rightObj.transform, false);
        lightRObj.transform.localPosition = new Vector3(0f, 0.02f, 0.05f);
        rightPalmLight = lightRObj.AddComponent<Light>();
        rightPalmLight.type = LightType.Point;
        rightPalmLight.color = new Color(0.2f, 0.95f, 1.0f);
        rightPalmLight.range = 0.5f;
        rightPalmLight.intensity = 0.6f;

        rightHand = rightObj.transform;
    }

    private void Update()
    {
        if (!isHandsActive || handsRoot == null) return;

        UpdateInteractionDetection();
        UpdateGrabInput();
        UpdateHandAnimation();
    }

    private void UpdateInteractionDetection()
    {
        if (playerCam == null) playerCam = Camera.main;
        if (playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        currentTargetObj = null;
        currentTargetPrompt = "";

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayers))
        {
            InspectCandidate(hit.collider.gameObject);
        }

        if (currentTargetObj == null && rightHand != null)
        {
            Collider[] hits = Physics.OverlapSphere(rightHand.position, 1.2f, interactableLayers);
            foreach (var h in hits)
            {
                if (InspectCandidate(h.gameObject)) break;
            }
        }

        reachProgress = Mathf.MoveTowards(reachProgress, currentTargetObj != null ? 1.0f : 0.0f, Time.deltaTime * 6f);
    }

    private bool InspectCandidate(GameObject obj)
    {
        if (obj == null) return false;

        var scanner = obj.GetComponentInParent<DiplomaticHandprintScanner>();
        if (scanner != null)
        {
            currentTargetObj = scanner.gameObject;
            currentTargetPrompt = "DIPLOMATIC HANDPRINT SCANNER — [KLIK / E UNTUK SCAN TANGAN]";
            return true;
        }

        var pod = obj.GetComponentInParent<SamplePodController>();
        if (pod != null)
        {
            currentTargetObj = pod.gameObject;
            currentTargetPrompt = "LIVING SAMPLE STASIS POD — [KLIK / E UNTUK RESONANSI]";
            return true;
        }

        var cradle = obj.GetComponentInParent<PrimordialCradleExhibit>();
        if (cradle != null)
        {
            currentTargetObj = obj;
            currentTargetPrompt = "THE PRIMORDIAL CRADLE — [KLIK / SENTUH UNTUK RITUAL AIR PURBA]";
            return true;
        }

        var sparkExhibit = obj.GetComponentInParent<CivilizationSparkExhibit>();
        if (sparkExhibit != null)
        {
            currentTargetObj = obj;
            currentTargetPrompt = "THE SPARK OF INGENUITY — [KLIK / SENTUH UNTUK PICU API PROMETHEUS]";
            return true;
        }

        var archiveExhibit = obj.GetComponentInParent<HumanArchiveExhibit>();
        if (archiveExhibit != null)
        {
            currentTargetObj = obj;
            currentTargetPrompt = "THE HUMAN ARCHIVE — [KLIK / SENTUH UNTUK RESONANSI BENIH & GENOM]";
            return true;
        }

        var horizonExhibit = obj.GetComponentInParent<CosmicHorizonExhibit>();
        if (horizonExhibit != null)
        {
            currentTargetObj = obj;
            currentTargetPrompt = "THE COSMIC HORIZON — [KLIK / SENTUH UNTUK PUTAR GOLDEN RECORD]";
            return true;
        }

        var terminal = obj.GetComponentInParent<MuseumTerminalDisplay>();
        if (terminal != null)
        {
            currentTargetObj = terminal.gameObject;
            currentTargetPrompt = "CURATORIAL ARCHIVE TERMINAL — [KLIK / SENTUH UNTUK BACA LOG]";
            return true;
        }

        var turntable = obj.GetComponentInParent<GoldenRecordTurntable>();
        if (turntable != null)
        {
            currentTargetObj = turntable.gameObject;
            currentTargetPrompt = "VOYAGER GOLDEN RECORD — [KLIK / E UNTUK PUTAR / DENGARKAN]";
            return true;
        }

        var prometheus = obj.GetComponentInParent<PrometheusToolEvolution>();
        if (prometheus != null)
        {
            currentTargetObj = prometheus.gameObject;
            currentTargetPrompt = "PROMETHEUS TOOL EVOLUTION — [KLIK / E UNTUK AKTIVASI]";
            return true;
        }

        var seed = obj.GetComponentInParent<SeedVaultSprouter>();
        if (seed != null)
        {
            currentTargetObj = seed.gameObject;
            currentTargetPrompt = "SVALBARD SEED VAULT — [KLIK / E UNTUK TUMBUHKAN]";
            return true;
        }

        var gallery = obj.GetComponentInParent<MuseumArtworkGallery>();
        if (gallery != null)
        {
            currentTargetObj = gallery.gameObject;
            currentTargetPrompt = "HUMAN ARTWORK ROTUNDA — [KLIK / E UNTUK GANTI KARYA]";
            return true;
        }

        var ambassador = obj.GetComponentInParent<HumanAmbassador>();
        if (ambassador != null)
        {
            currentTargetObj = ambassador.gameObject;
            currentTargetPrompt = "HUMAN AMBASSADOR STATUE — [KLIK / E UNTUK DENGAR PESAN]";
            return true;
        }

        var detector = obj.GetComponentInParent<HandProximityDetector>();
        if (detector != null)
        {
            currentTargetObj = detector.gameObject;
            currentTargetPrompt = "INTERAKTIF — [KLIK / E UNTUK SENTUH / GRAB]";
            return true;
        }

        return false;
    }

    private void UpdateGrabInput()
    {
        bool isGrabbingRight = Input.GetMouseButton(0) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.F);
        bool isGrabbingLeft = Input.GetMouseButton(1) || Input.GetKey(KeyCode.Space);

        targetRightGrab = isGrabbingRight ? 1f : 0f;
        targetLeftGrab = isGrabbingLeft ? 1f : 0f;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F)) && currentTargetObj != null)
        {
            ExecuteInteraction(currentTargetObj);
        }
    }

    private void ExecuteInteraction(GameObject target)
    {
        Debug.Log("[AlienVRHands] GRAB / INTERACTION triggered on " + target.name);

        if (rightPalmLight != null)
        {
            rightPalmLight.intensity = 2.5f;
        }

        var cradle = target.GetComponentInParent<PrimordialCradleExhibit>();
        if (cradle != null)
        {
            cradle.OnPlayerInteract(target);
            return;
        }

        var sparkExhibit = target.GetComponentInParent<CivilizationSparkExhibit>();
        if (sparkExhibit != null)
        {
            sparkExhibit.OnPlayerInteract(target);
            return;
        }

        var archiveExhibit = target.GetComponentInParent<HumanArchiveExhibit>();
        if (archiveExhibit != null)
        {
            archiveExhibit.OnPlayerInteract(target);
            return;
        }

        var horizonExhibit = target.GetComponentInParent<CosmicHorizonExhibit>();
        if (horizonExhibit != null)
        {
            horizonExhibit.OnPlayerInteract(target);
            return;
        }

        var terminal = target.GetComponentInParent<MuseumTerminalDisplay>();
        if (terminal != null)
        {
            terminal.AdvancePage();
            return;
        }

        var scanner = target.GetComponentInParent<DiplomaticHandprintScanner>();
        if (scanner != null)
        {
            scanner.SendMessage("OnTriggerEnter", GetComponent<Collider>() ?? gameObject.AddComponent<SphereCollider>(), SendMessageOptions.DontRequireReceiver);
        }

        var pod = target.GetComponentInParent<SamplePodController>();
        if (pod != null)
        {
            pod.SendMessage("OnInteractionTriggered", SendMessageOptions.DontRequireReceiver);
        }

        var turntable = target.GetComponentInParent<GoldenRecordTurntable>();
        if (turntable != null)
        {
            turntable.SendMessage("TogglePlay", SendMessageOptions.DontRequireReceiver);
        }

        var prometheus = target.GetComponentInParent<PrometheusToolEvolution>();
        if (prometheus != null)
        {
            prometheus.SendMessage("AdvanceEvolutionStage", SendMessageOptions.DontRequireReceiver);
        }

        var seed = target.GetComponentInParent<SeedVaultSprouter>();
        if (seed != null)
        {
            seed.SendMessage("ToggleSproutState", SendMessageOptions.DontRequireReceiver);
        }

        var gallery = target.GetComponentInParent<MuseumArtworkGallery>();
        if (gallery != null)
        {
            gallery.SendMessage("CycleArtwork", SendMessageOptions.DontRequireReceiver);
        }

        var ambassador = target.GetComponentInParent<HumanAmbassador>();
        if (ambassador != null)
        {
            ambassador.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
        }

        var detector = target.GetComponentInParent<HandProximityDetector>();
        if (detector != null)
        {
            detector.OnHandTouching?.Invoke();
            detector.OnHandEnterRange?.Invoke();
        }
    }

    private void UpdateHandAnimation()
    {
        rightGrabStrength = Mathf.MoveTowards(rightGrabStrength, targetRightGrab, Time.deltaTime * grabSpeed);
        leftGrabStrength = Mathf.MoveTowards(leftGrabStrength, targetLeftGrab, Time.deltaTime * grabSpeed);

        if (rightPalmLight != null)
        {
            rightPalmLight.intensity = Mathf.MoveTowards(rightPalmLight.intensity, 0.6f + rightGrabStrength * 1.0f, Time.deltaTime * 6f);
        }
        if (leftPalmLight != null)
        {
            leftPalmLight.intensity = Mathf.MoveTowards(leftPalmLight.intensity, 0.6f + leftGrabStrength * 1.0f, Time.deltaTime * 6f);
        }

        // Mouse look inertia/lag
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 targetInertia = new Vector3(-mouseX * 0.02f, -mouseY * 0.02f, 0f);
        inertiaOffset = Vector3.SmoothDamp(inertiaOffset, targetInertia, ref inertiaVelocity, 0.08f);

        // Stride bobbing and organic breathing
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if (isMoving) walkCycle += Time.deltaTime * walkBobSpeed;

        float breathSway = Mathf.Sin(Time.time * idleSwaySpeed) * 0.005f;
        float walkBob = isMoving ? Mathf.Sin(walkCycle * 2f) * 0.012f : 0f;
        float walkSway = isMoving ? Mathf.Sin(walkCycle) * 0.010f : 0f;

        Vector3 reachOffset = new Vector3(0f, 0.05f, 0.14f) * reachProgress;

        // Animate Left Hand
        if (leftHand != null)
        {
            Vector3 posL = leftRestLocalPos + inertiaOffset + new Vector3(-walkSway, breathSway - walkBob, 0f);
            Quaternion rotL = Quaternion.Slerp(leftRestRot, leftGrabRot, leftGrabStrength);

            // Subtle scale clench on grab
            Vector3 scaleL = Vector3.Lerp(Vector3.one * 1.80f, new Vector3(1.68f, 1.68f, 1.80f), leftGrabStrength);

            leftHand.localPosition = Vector3.Lerp(leftHand.localPosition, posL, Time.deltaTime * mouseLagSpeed);
            leftHand.localRotation = Quaternion.Slerp(leftHand.localRotation, rotL, Time.deltaTime * mouseLagSpeed);
            leftHand.localScale = scaleL;
        }

        // Animate Right Hand (reaches forward towards interactable)
        if (rightHand != null)
        {
            Vector3 grabThrust = new Vector3(0f, 0.02f, 0.08f) * rightGrabStrength;
            Vector3 posR = rightRestLocalPos + inertiaOffset + reachOffset + grabThrust + new Vector3(walkSway, breathSway + walkBob, 0f);

            Quaternion baseRotR = Quaternion.Slerp(rightRestRot, Quaternion.Euler(-5f, -8f, 8f), reachProgress);
            Quaternion rotR = Quaternion.Slerp(baseRotR, rightGrabRot, rightGrabStrength);

            Vector3 scaleR = Vector3.Lerp(Vector3.one * 1.80f, new Vector3(1.68f, 1.68f, 1.80f), rightGrabStrength);

            rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, posR, Time.deltaTime * mouseLagSpeed);
            rightHand.localRotation = Quaternion.Slerp(rightHand.localRotation, rotR, Time.deltaTime * mouseLagSpeed);
            rightHand.localScale = scaleR;
        }
    }

    public void SetHandsActive(bool active)
    {
        isHandsActive = active;
        if (handsRoot != null)
        {
            handsRoot.SetActive(active);
        }
    }

    private void OnGUI()
    {
        if (!isHandsActive || currentTargetObj == null) return;

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Color(0.25f, 0.95f, 1.0f, 0.95f);

        float screenW = Screen.width;
        float screenH = Screen.height;

        GUI.Label(new Rect(screenW * 0.5f - 100, screenH * 0.5f - 30, 200, 30), "⬡ [ GRAB / TOUCH ] ⬡", style);

        style.fontSize = 13;
        style.normal.textColor = new Color(0.95f, 0.85f, 0.40f, 0.95f);
        GUI.Label(new Rect(screenW * 0.5f - 300, screenH * 0.5f + 5, 600, 30), currentTargetPrompt, style);
    }
}
