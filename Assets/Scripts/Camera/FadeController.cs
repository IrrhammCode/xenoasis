using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// XENOASIS — FadeController.cs
/// Handles smooth screen fading (fade-to-black, fade-to-white, fade-in).
/// Works with a CanvasGroup or UI Image positioned directly in front of the VR camera.
/// </summary>
public class FadeController : MonoBehaviour
{
    [Header("Fade UI Elements")]
    [Tooltip("CanvasGroup containing the fade image")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Tooltip("UI Image used for fading (can change color to black or white)")]
    [SerializeField] private Image fadeImage;

    [Header("Defaults")]
    [SerializeField] private Color defaultFadeColor = Color.black;
    [SerializeField] private Color whiteFadeColor = Color.white;

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = GetComponent<CanvasGroup>();

        if (fadeImage == null && fadeCanvasGroup != null)
            fadeImage = fadeCanvasGroup.GetComponentInChildren<Image>();

        if (fadeImage != null)
            fadeImage.color = defaultFadeColor;
    }

    /// <summary>
    /// Instantly set the fade alpha without interpolation.
    /// </summary>
    /// <param name="alpha">0 = completely transparent, 1 = completely opaque</param>
    public void SetFadeImmediate(float alpha)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = Mathf.Clamp01(alpha);
            fadeCanvasGroup.blocksRaycasts = alpha > 0.01f;
        }
    }

    /// <summary>
    /// Smoothly fade to target alpha using the current fade color.
    /// </summary>
    public void FadeTo(float targetAlpha, float duration)
    {
        if (fadeImage != null)
            fadeImage.color = defaultFadeColor;

        StartFade(targetAlpha, duration);
    }

    /// <summary>
    /// Smoothly fade to white (used for the divine climax transition).
    /// </summary>
    public void FadeToWhite(float duration)
    {
        if (fadeImage != null)
            fadeImage.color = whiteFadeColor;

        StartFade(1f, duration);
    }

    /// <summary>
    /// Smoothly fade to black.
    /// </summary>
    public void FadeToBlack(float duration)
    {
        if (fadeImage != null)
            fadeImage.color = defaultFadeColor;

        StartFade(1f, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha = targetAlpha;
            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.01f;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.01f;
        currentFadeCoroutine = null;
    }
}
