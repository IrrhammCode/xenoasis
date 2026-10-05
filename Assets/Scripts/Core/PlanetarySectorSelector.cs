using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// XENOASIS — PlanetarySectorSelector.cs
/// Orbital Planetary Map / Sector Target Selection in deep space before descent:
/// Player selects their destination sector on Earth.
/// Once confirmed, the multi-stage atmospheric descent process begins!
/// </summary>
public class PlanetarySectorSelector : MonoBehaviour
{
    public static PlanetarySectorSelector Instance { get; private set; }

    [Header("Sector Selection State")]
    [SerializeField] private int selectedSector = 1; // 1 = Terran Embassy, 2 = Pacific, 3 = Sahara
    [SerializeField] private bool hasConfirmed = false;

    [Header("Holographic UI Elements")]
    [SerializeField] private GameObject selectorCanvas;
    [SerializeField] private TMPro.TextMeshPro sectorTitleTMP;
    [SerializeField] private TMPro.TextMeshPro sectorDetailsTMP;
    [SerializeField] private TMPro.TextMeshPro promptInstructionTMP;
    [SerializeField] private Transform targetingReticle;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchClip;
    [SerializeField] private AudioClip lockConfirmedClip;

    [Header("Events")]
    public UnityEvent<int> OnSectorSelected = new UnityEvent<int>();
    public UnityEvent<int> OnSectorConfirmed = new UnityEvent<int>();

    private float reticlePulseTimer = 0f;

    private readonly string[] sectorTitles = {
        "SEKTOR 01: [ TERRAN EMBASSY & ALPINE SANCTUARY ]",
        "SEKTOR 02: [ PACIFIC HYDROSPHERE ]",
        "SEKTOR 03: [ SAHARA GEOTHERMAL EXPANSE ]"
    };

    private readonly string[] sectorDetails = {
        "<b>KOORDINAT:</b> 46°12'N, 08°31'E · VALLE DI SANCTUARY\n<b>BIOSFER:</b> Danau Gletser Alpen, Hutan Konifer, Kubah Arsip Bumi\n<b>ATMOSFER:</b> N2 78% · O2 21% (100% Layak Huni)\n<b>STATUS DIPLOMATIK:</b> Duta Besar Manusia Menunggu di Kubah\n<b>VEKTOR PENETRASI:</b> Eksosfer → Mesosfer → Troposfer Terkunci",

        "<b>KOORDINAT:</b> 14°30'S, 142°15'W · PALUNG KEDALAMAN\n<b>BIOSFER:</b> Samudra Terbuka, Arus Termal Lautan Pasifik\n<b>ATMOSFER:</b> N2 78% · O2 21% · Salinitas Tinggi\n<b>STATUS DIPLOMATIK:</b> Wilayah Oseanografi Terbuka\n<b>VEKTOR PENETRASI:</b> Trajektori Alternatif",

        "<b>KOORDINAT:</b> 22°45'N, 14°20'E · GURUN SILIKA\n<b>BIOSFER:</b> Cekungan Panas Bumi & Formasi Batuan Kristal\n<b>ATMOSFER:</b> N2 78% · O2 21% · Kelembapan 12%\n<b>STATUS DIPLOMATIK:</b> Wilayah Arsip Mineral\n<b>VEKTOR PENETRASI:</b> Trajektori Alternatif"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        if (hasConfirmed) return;

        // Number keys 1, 2, 3 to switch sectors
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectSector(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectSector(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectSector(3);
        }

        // Left Click or Space / Enter to confirm selection and initiate descent
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ConfirmSelection();
        }

        // Pulse Reticle
        if (targetingReticle != null)
        {
            reticlePulseTimer += Time.deltaTime * 3.5f;
            float s = 1.0f + Mathf.Sin(reticlePulseTimer) * 0.08f;
            targetingReticle.localScale = new Vector3(s, s, 1f);
            targetingReticle.Rotate(0, 0, 15f * Time.deltaTime);
        }
    }

    public void SelectSector(int index)
    {
        if (hasConfirmed) return;
        selectedSector = Mathf.Clamp(index, 1, 3);
        UpdateDisplay();
        if (audioSource != null && switchClip != null)
        {
            audioSource.PlayOneShot(switchClip, 0.7f);
        }
        OnSectorSelected?.Invoke(selectedSector);
    }

    public void ConfirmSelection()
    {
        if (hasConfirmed) return;
        hasConfirmed = true;

        if (audioSource != null && lockConfirmedClip != null)
        {
            audioSource.PlayOneShot(lockConfirmedClip, 1.0f);
        }

        if (promptInstructionTMP != null)
        {
            promptInstructionTMP.text = "<b><color=#00FF88>✓ KOORDINAT TERKUNCI // MEMULAI TRAJEKTORI DESCENT...</color></b>";
        }

        OnSectorConfirmed?.Invoke(selectedSector);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(1.8f);
        if (selectorCanvas != null)
        {
            selectorCanvas.SetActive(false);
        }
    }

    private void UpdateDisplay()
    {
        int idx = selectedSector - 1;
        if (sectorTitleTMP != null)
        {
            sectorTitleTMP.text = $"<b><color=#00FFFF>{sectorTitles[idx]}</color></b>";
        }

        if (sectorDetailsTMP != null)
        {
            sectorDetailsTMP.text = sectorDetails[idx];
        }

        if (promptInstructionTMP != null)
        {
            promptInstructionTMP.text = "<b>[ KLIK / TEKAN SPASI / ENTER UNTUK MENGUNCI KOORDINAT & TURUN ]</b>\n<size=75%>Gunakan Tombol [1], [2], [3] untuk Memilih Sektor Alternatif</size>";
        }
    }
}
