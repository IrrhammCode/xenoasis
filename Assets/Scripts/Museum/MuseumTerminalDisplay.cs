using UnityEngine;

/// <summary>
/// XENOASIS — MuseumTerminalDisplay.cs
/// Powers the Interactive Alien Curator Kiosk (museum_interactive_terminal.glb).
/// Displays holographic curatorial research pages, cycles with tactile click/touch,
/// and plays high-tech auditory feedback.
/// </summary>
[ExecuteAlways]
public class MuseumTerminalDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct TerminalPage
    {
        public string title;
        public string eraCode;
        [TextArea(3, 6)]
        public string bodyText;
    }

    [Header("Terminal Logs")]
    [SerializeField] private TerminalPage[] pages;
    [SerializeField] private int currentPageIndex = 0;

    [Header("Holographic Display Renderers")]
    [SerializeField] private TextMesh titleTextMesh;
    [SerializeField] private TextMesh bodyTextMesh;
    [SerializeField] private TextMesh footerTextMesh;
    [SerializeField] private Light terminalScreenLight;

    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchPageClip;

    public void SetupTerminal(AudioClip clip)
    {
        this.switchPageClip = clip;
        InitializeDisplay();
    }

    public void SetCustomPages(TerminalPage[] customPages)
    {
        this.pages = customPages;
        UpdateDisplay();
    }

    private void Awake()
    {
        InitializeDisplay();
    }

    public void InitializeDisplay()
    {
        if (pages == null || pages.Length == 0)
        {
            pages = new TerminalPage[]
            {
                new TerminalPage
                {
                    title = "I. THE COSMIC WATER DELIVERY",
                    eraCode = "HADEAN EON // 4.100.000.000 B.C.",
                    bodyText = "During the Late Heavy Bombardment, millions of carbonaceous\nchondrite asteroids delivered over 1.38 billion cubic kilometers\nof extraterrestrial water ice onto the magma crust of Earth."
                },
                new TerminalPage
                {
                    title = "II. HYDROTHERMAL GENESIS",
                    eraCode = "ARCHEAN ABYSS // 3.800.000.000 B.C.",
                    bodyText = "In deep-sea alkaline hydrothermal vents, inorganic iron-sulfur\nchimneys and electrochemical proton gradients catalyzed\nthe first prebiotic cellular membranes and Terran RNA."
                },
                new TerminalPage
                {
                    title = "III. THE INTERNAL OCEAN",
                    eraCode = "ANTHROPOCENE // HOMO SAPIENS",
                    bodyText = "Human beings never truly left the sea. The 0.9% salinity\nof blood plasma and amniotic fluid mirrors the exact mineral\nbalance of the Archean ocean where life began."
                }
            };
        }

        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0.85f;
            audioSource.playOnAwake = false;
        }

        BuildHoloTextComponents();
        UpdateDisplay();
    }

    private void BuildHoloTextComponents()
    {
        Quaternion screenRot = Quaternion.Euler(14f, 90f, 0f);

        Transform existingTitle = transform.Find("HoloText_Title");
        if (existingTitle != null)
        {
            existingTitle.localPosition = new Vector3(0.065f, 0.45f, 0f);
            existingTitle.localRotation = screenRot;
            titleTextMesh = existingTitle.GetComponent<TextMesh>();
            if (titleTextMesh != null)
            {
                titleTextMesh.fontSize = 24;
                titleTextMesh.characterSize = 0.0070f;
            }
        }
        else
        {
            GameObject titleObj = new GameObject("HoloText_Title");
            titleObj.transform.SetParent(transform, false);
            titleObj.transform.localPosition = new Vector3(0.065f, 0.45f, 0f);
            titleObj.transform.localRotation = screenRot;

            titleTextMesh = titleObj.AddComponent<TextMesh>();
            titleTextMesh.fontSize = 24;
            titleTextMesh.characterSize = 0.0070f;
            titleTextMesh.anchor = TextAnchor.MiddleCenter;
            titleTextMesh.alignment = TextAlignment.Center;
            titleTextMesh.color = new Color(0.15f, 0.95f, 1.0f);
            titleTextMesh.fontStyle = FontStyle.Bold;
        }

        Transform existingBody = transform.Find("HoloText_Body");
        if (existingBody != null)
        {
            existingBody.localPosition = new Vector3(0.068f, 0.38f, 0f);
            existingBody.localRotation = screenRot;
            bodyTextMesh = existingBody.GetComponent<TextMesh>();
            if (bodyTextMesh != null)
            {
                bodyTextMesh.fontSize = 20;
                bodyTextMesh.characterSize = 0.0055f;
            }
        }
        else
        {
            GameObject bodyObj = new GameObject("HoloText_Body");
            bodyObj.transform.SetParent(transform, false);
            bodyObj.transform.localPosition = new Vector3(0.068f, 0.38f, 0f);
            bodyObj.transform.localRotation = screenRot;

            bodyTextMesh = bodyObj.AddComponent<TextMesh>();
            bodyTextMesh.fontSize = 20;
            bodyTextMesh.characterSize = 0.0055f;
            bodyTextMesh.anchor = TextAnchor.UpperCenter;
            bodyTextMesh.alignment = TextAlignment.Center;
            bodyTextMesh.color = new Color(0.90f, 0.96f, 1.0f);
        }

        Transform existingFooter = transform.Find("HoloText_Footer");
        if (existingFooter != null)
        {
            existingFooter.localPosition = new Vector3(0.071f, 0.285f, 0f);
            existingFooter.localRotation = screenRot;
            footerTextMesh = existingFooter.GetComponent<TextMesh>();
            if (footerTextMesh != null)
            {
                footerTextMesh.fontSize = 18;
                footerTextMesh.characterSize = 0.0050f;
            }
        }
        else
        {
            GameObject footerObj = new GameObject("HoloText_Footer");
            footerObj.transform.SetParent(transform, false);
            footerObj.transform.localPosition = new Vector3(0.071f, 0.285f, 0f);
            footerObj.transform.localRotation = screenRot;

            footerTextMesh = footerObj.AddComponent<TextMesh>();
            footerTextMesh.fontSize = 18;
            footerTextMesh.characterSize = 0.0050f;
            footerTextMesh.anchor = TextAnchor.MiddleCenter;
            footerTextMesh.alignment = TextAlignment.Center;
            footerTextMesh.color = new Color(1.0f, 0.85f, 0.35f);
        }

        Transform existingLight = transform.Find("Terminal_ScreenGlow");
        if (existingLight != null)
        {
            existingLight.localPosition = new Vector3(0.10f, 0.38f, 0f);
            terminalScreenLight = existingLight.GetComponent<Light>();
        }
        else
        {
            GameObject lightObj = new GameObject("Terminal_ScreenGlow");
            lightObj.transform.SetParent(transform, false);
            lightObj.transform.localPosition = new Vector3(0.10f, 0.38f, 0f);

            terminalScreenLight = lightObj.AddComponent<Light>();
            terminalScreenLight.type = LightType.Point;
            terminalScreenLight.color = new Color(0.15f, 0.85f, 1.0f);
            terminalScreenLight.intensity = 1.0f;
            terminalScreenLight.range = 1.2f;
        }
    }

    public void AdvancePage()
    {
        if (pages == null || pages.Length == 0) return;

        currentPageIndex = (currentPageIndex + 1) % pages.Length;
        UpdateDisplay();

        if (audioSource != null && switchPageClip != null)
        {
            audioSource.PlayOneShot(switchPageClip, 0.7f);
        }

        DioramaImmersionTrigger.TriggerPicoHaptics(0.25f, 0.08f);
    }

    private void UpdateDisplay()
    {
        if (pages == null || pages.Length == 0) return;

        var page = pages[currentPageIndex];
        if (titleTextMesh != null)
        {
            titleTextMesh.text = $"{page.title}\n<size=16>{page.eraCode}</size>";
        }

        if (bodyTextMesh != null)
        {
            bodyTextMesh.text = page.bodyText;
        }

        if (footerTextMesh != null)
        {
            footerTextMesh.text = $"[LOG {currentPageIndex + 1} / {pages.Length}] >> TOUCH / CLICK TO ADVANCE";
        }
    }

    public void OnTerminalInteracted()
    {
        AdvancePage();
    }
}
