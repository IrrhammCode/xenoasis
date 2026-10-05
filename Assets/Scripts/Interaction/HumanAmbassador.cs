using UnityEngine;

/// <summary>
/// XENOASIS — HumanAmbassador.cs
/// Displays a holographic representation of the Terran Diplomatic Envoy
/// who welcomes the alien travelers at the airlock entrance.
/// </summary>
public class HumanAmbassador : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private TMPro.TextMeshPro subtitleTMP;
    [SerializeField] private Light holoProjectorLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip welcomeSpeechClip;

    [Header("Animation")]
    [SerializeField] private float bobbingSpeed = 1.2f;
    [SerializeField] private float bobbingAmount = 0.03f;

    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.localPosition;
        if (subtitleTMP != null)
        {
            subtitleTMP.text = "<b>TERRAN DIPLOMATIC ENVOY</b>\n<i>\"Greetings, travelers of the stars. Welcome to Earth.\nWe present our Living Archive to introduce our home to you.\"</i>";
        }

        if (audioSource != null && welcomeSpeechClip != null)
        {
            audioSource.PlayOneShot(welcomeSpeechClip);
        }
    }

    private void Update()
    {
        // Gentle holographic floating animation
        float y = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.localPosition = initialPos + new Vector3(0, y, 0);

        if (holoProjectorLight != null)
        {
            // Subtle holographic flicker
            holoProjectorLight.intensity = 1.2f + Mathf.Sin(Time.time * 12f) * 0.15f;
        }
    }
}
