using System.Collections;
using UnityEngine;

/// <summary>
/// XENOASIS — DiplomaticHandprintScanner.cs
/// Powers Pavilion 5: Apollo 11 Lunar Bootprint (1969) & Alien Diplomatic Sensor.
/// Displays Armstrong's historic lunar footprint in regolith. An interactive diplomatic
/// scanner allows the extraterrestrial traveler to place their hand, illuminating an alien
/// glowing handprint alongside humanity's footprint as a treaty of galactic peace.
/// </summary>
public class DiplomaticHandprintScanner : MuseumExhibitController
{
    [Header("Lunar Plaque Components")]
    [SerializeField] private Transform moonRegolithPlaque;
    [SerializeField] private GameObject humanBootprintVisual;
    [SerializeField] private GameObject alienHandprintVisual;
    [SerializeField] private Transform scannerBeam;
    [SerializeField] private ParticleSystem peaceHarmonicsPS;
    [SerializeField] private Light diplomaticHaloLight;
    [SerializeField] private TMPro.TextMeshPro treatyStatusTMP;

    [Header("Diplomatic Audio")]
    [SerializeField] private AudioSource diplomaticAudioSource;
    [SerializeField] private AudioClip treatyChimeClip;

    private bool isTreatySigned = false;
    private Coroutine scanCoroutine;

    protected override void Start()
    {
        base.Start();
        if (alienHandprintVisual != null) alienHandprintVisual.SetActive(false);
        if (scannerBeam != null) scannerBeam.gameObject.SetActive(false);
        if (peaceHarmonicsPS != null) peaceHarmonicsPS.Stop();
        UpdateTreatyStatus(false);
    }

    protected override void Update()
    {
        base.Update();

        if (scannerBeam != null && scannerBeam.gameObject.activeSelf)
        {
            float pingPongZ = Mathf.PingPong(Time.time * 0.8f, 0.4f) - 0.2f;
            scannerBeam.localPosition = new Vector3(0.35f, 0.05f, pingPongZ);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        if (scanCoroutine != null) StopCoroutine(scanCoroutine);
        scanCoroutine = StartCoroutine(PerformDiplomaticHandScan());
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }

    private IEnumerator PerformDiplomaticHandScan()
    {
        if (scannerBeam != null) scannerBeam.gameObject.SetActive(true);
        if (treatyStatusTMP != null)
        {
            treatyStatusTMP.text = "<color=#FFD700><b>MEMINDAI TELAPAK TANGAN ALIEN...</b></color>\n<size=80%>KALIBRASI BIOMETRIK EKSTRATERESTRIAL</size>";
        }

        yield return new WaitForSeconds(1.6f);

        if (scannerBeam != null) scannerBeam.gameObject.SetActive(false);
        isTreatySigned = true;

        if (alienHandprintVisual != null) alienHandprintVisual.SetActive(true);
        if (peaceHarmonicsPS != null) peaceHarmonicsPS.Play();
        if (diplomaticHaloLight != null) diplomaticHaloLight.intensity = 4.0f;

        if (diplomaticAudioSource != null && treatyChimeClip != null)
        {
            diplomaticAudioSource.PlayOneShot(treatyChimeClip, 1.0f);
        }

        UpdateTreatyStatus(true);
    }

    private void UpdateTreatyStatus(bool signed)
    {
        if (treatyStatusTMP != null)
        {
            if (signed)
            {
                treatyStatusTMP.text =
                    "<b><color=#00FF88>PAKTA PERDAMAIAN BUMI-KOSMOS TERVERIFIKASI</color></b>\n" +
                    "<size=75%><color=#80D8FF>\"KAMI DATANG DENGAN DAMAI UNTUK SELURUH ALAM SEMESTA\"</color>\n" +
                    "<color=#FFD700>STATUS: KONTAK PERTAMA DIABADIKAN DALAM SEJARAH</color></size>";
            }
            else
            {
                treatyStatusTMP.text =
                    "<b><color=#00E5FF>SENSOR KONTAK DIPLOMATIK BUMI</color></b>\n" +
                    "<size=75%><color=#E0F7FA>SENTUH SENSOR UNTUK MENGABADIKAN JEJAK TANGAN ANDA</color></size>";
            }
        }
    }
}
