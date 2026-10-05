using UnityEngine;

/// <summary>
/// XENOASIS — MuseumArtworkGallery.cs
/// Powers Pavilion 2: The Rosetta Stone & Floating Masterpiece Art Gallery.
/// Displays the Rosetta Stele with ancient scripts and 3 floating holographic art frames
/// (Prehistoric Cave Art, Borobudur Relief, Da Vinci's Mona Lisa, Van Gogh's Starry Night).
/// </summary>
public class MuseumArtworkGallery : MuseumExhibitController
{
    [Header("Art Gallery Frames")]
    [SerializeField] private Transform[] artFrames;
    [SerializeField] private Transform rosettaStone;
    [SerializeField] private GameObject translationBeam;
    [SerializeField] private TMPro.TextMeshPro artTitleTMP;

    private readonly string[] artworkTitles = new string[]
    {
        "LUKISAN GUA PRESEJARAH (40.000 SM) — MAROS & LASCAUX",
        "RELIEF CANDI BOROBUDUR (ABAD KE-8 M) — MAHARAKYAT NUSANTARA",
        "MONA LISA (1503 M) — LEONARDO DA VINCI",
        "THE STARRY NIGHT (1889 M) — VINCENT VAN GOGH"
    };

    private Vector3[] basePositions;
    private int currentArtworkIndex = 0;

    protected override void Start()
    {
        base.Start();

        if (artFrames != null && artFrames.Length > 0)
        {
            basePositions = new Vector3[artFrames.Length];
            for (int i = 0; i < artFrames.Length; i++)
            {
                if (artFrames[i] != null) basePositions[i] = artFrames[i].localPosition;
            }
        }

        if (translationBeam != null) translationBeam.SetActive(false);
        UpdateArtTitle();
    }

    protected override void Update()
    {
        base.Update();

        // Subtle gentle floating hover animation for art frames
        float time = Time.time;
        if (artFrames != null)
        {
            for (int i = 0; i < artFrames.Length; i++)
            {
                if (artFrames[i] != null)
                {
                    float offset = Mathf.Sin(time * 1.8f + i * 1.2f) * 0.04f;
                    artFrames[i].localPosition = basePositions[i] + Vector3.up * offset;
                    artFrames[i].localRotation = Quaternion.Euler(0, Mathf.Sin(time * 0.8f + i) * 3f, 0);
                }
            }
        }

        // Rosetta subtle translation pulse
        if (translationBeam != null && translationBeam.activeSelf)
        {
            translationBeam.transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.Self);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        currentArtworkIndex = (currentArtworkIndex + 1) % artworkTitles.Length;
        UpdateArtTitle();

        if (translationBeam != null)
        {
            translationBeam.SetActive(true);
        }
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
        if (translationBeam != null)
        {
            translationBeam.SetActive(false);
        }
    }

    private void UpdateArtTitle()
    {
        if (artTitleTMP != null)
        {
            artTitleTMP.text = $"<color=#00E5FF><b>KARYA SENI AKTIF:</b></color>\n<size=80%><color=#FFD700>{artworkTitles[currentArtworkIndex]}</color></size>";
        }
    }
}
