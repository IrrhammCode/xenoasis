using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// XENOASIS — EndScreenUI.cs
/// Displays the final end card: "The gift was received." → "XENOASIS" → credits.
/// Animated text fade-in sequence.
/// </summary>
public class EndScreenUI : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI creditsText;

    [Header("Strings")]
    [SerializeField] private string endMessage = "The gift was received.";
    [SerializeField] private string titleString = "XENOASIS";
    [SerializeField] private string creditString = "Tripothon S1 · Built with Tripo AI + World Labs + PICO 4 Ultra";

    [Header("Timing")]
    [SerializeField] private float messageDelay = 1f;
    [SerializeField] private float messageFadeDuration = 2f;
    [SerializeField] private float titleDelay = 3f;
    [SerializeField] private float titleFadeDuration = 2f;
    [SerializeField] private float creditsDelay = 2f;
    [SerializeField] private float creditsFadeDuration = 2f;

    private void OnEnable()
    {
        // Auto-find children if not assigned in Inspector
        if (messageText == null || titleText == null || creditsText == null)
        {
            var tmps = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in tmps)
            {
                if (messageText == null && t.name.IndexOf("Message", System.StringComparison.OrdinalIgnoreCase) >= 0) messageText = t;
                else if (titleText == null && t.name.IndexOf("Title", System.StringComparison.OrdinalIgnoreCase) >= 0) titleText = t;
                else if (creditsText == null && t.name.IndexOf("Credit", System.StringComparison.OrdinalIgnoreCase) >= 0) creditsText = t;
            }
        }

        // Initialize all text invisible
        SetAlpha(messageText, 0f);
        SetAlpha(titleText, 0f);
        SetAlpha(creditsText, 0f);

        if (messageText != null) messageText.text = endMessage;
        if (titleText != null) titleText.text = titleString;
        if (creditsText != null) creditsText.text = creditString;

        StartCoroutine(EndCardSequence());
    }

    private IEnumerator EndCardSequence()
    {
        // "The gift was received."
        yield return new WaitForSeconds(messageDelay);
        yield return FadeText(messageText, messageFadeDuration);

        // "XENOASIS"
        yield return new WaitForSeconds(titleDelay);
        yield return FadeText(titleText, titleFadeDuration);

        // Credits
        yield return new WaitForSeconds(creditsDelay);
        yield return FadeText(creditsText, creditsFadeDuration);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float duration)
    {
        if (text == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(text, Mathf.Lerp(0f, 1f, elapsed / duration));
            yield return null;
        }
        SetAlpha(text, 1f);
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
