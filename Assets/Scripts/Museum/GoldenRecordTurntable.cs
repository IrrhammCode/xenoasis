using System.Collections;
using UnityEngine;

/// <summary>
/// XENOASIS — GoldenRecordTurntable.cs
/// Powers Pavilion 1: The Voyager Golden Record (1977).
/// Spins the gold phonograph disc, swings the tone arm, plays Earth greetings & music,
/// and projects the floating holographic pulsar map and hydrogen molecule diagram.
/// </summary>
public class GoldenRecordTurntable : MuseumExhibitController
{
    [Header("Golden Record Components")]
    [SerializeField] private Transform recordDisc;
    [SerializeField] private Transform toneArm;
    [SerializeField] private GameObject pulsarHoloProjector;
    [SerializeField] private ParticleSystem goldDustVFX;
    [SerializeField] private Light goldenHaloLight;

    [Header("Playback Dynamics")]
    [SerializeField] private float spinSpeed = 100f; // degrees/sec
    [SerializeField] private AudioSource recordAudioSource;
    [SerializeField] private AudioClip[] earthGreetingsClips;

    private bool isPlaying = false;
    private Quaternion armRestRot = Quaternion.Euler(0, -20f, 0);
    private Quaternion armPlayRot = Quaternion.Euler(0, -48f, 0);
    private int currentGreetingIndex = 0;

    protected override void Start()
    {
        base.Start();
        if (toneArm != null) toneArm.localRotation = armRestRot;
        if (pulsarHoloProjector != null) pulsarHoloProjector.SetActive(false);
        if (goldDustVFX != null) goldDustVFX.Stop();
    }

    protected override void Update()
    {
        base.Update();

        if (isPlaying && recordDisc != null)
        {
            recordDisc.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
        }

        if (pulsarHoloProjector != null && pulsarHoloProjector.activeSelf)
        {
            pulsarHoloProjector.transform.Rotate(Vector3.up, 18f * Time.deltaTime, Space.World);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        isPlaying = true;
        StopAllCoroutines();
        StartCoroutine(AnimateArm(armPlayRot, true));
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
        isPlaying = false;
        StopAllCoroutines();
        StartCoroutine(AnimateArm(armRestRot, false));
    }

    private IEnumerator AnimateArm(Quaternion targetRot, bool startingPlayback)
    {
        if (toneArm != null)
        {
            Quaternion startRot = toneArm.localRotation;
            float elapsed = 0f;
            float dur = 1.0f;
            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                toneArm.localRotation = Quaternion.Slerp(startRot, targetRot, elapsed / dur);
                yield return null;
            }
            toneArm.localRotation = targetRot;
        }

        if (startingPlayback)
        {
            if (pulsarHoloProjector != null) pulsarHoloProjector.SetActive(true);
            if (goldDustVFX != null) goldDustVFX.Play();
            if (goldenHaloLight != null) goldenHaloLight.intensity = 3.5f;

            if (recordAudioSource != null && earthGreetingsClips != null && earthGreetingsClips.Length > 0)
            {
                var clip = earthGreetingsClips[currentGreetingIndex % earthGreetingsClips.Length];
                if (clip != null)
                {
                    recordAudioSource.clip = clip;
                    recordAudioSource.loop = true;
                    recordAudioSource.Play();
                }
                currentGreetingIndex++;
            }
        }
        else
        {
            if (pulsarHoloProjector != null) pulsarHoloProjector.SetActive(false);
            if (goldDustVFX != null) goldDustVFX.Stop();
            if (goldenHaloLight != null) goldenHaloLight.intensity = 1.5f;
            if (recordAudioSource != null) recordAudioSource.Stop();
        }
    }
}
