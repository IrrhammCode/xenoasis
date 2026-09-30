using UnityEngine;
using System.Collections;

/// <summary>
/// XENOASIS — GuidingTrail.cs
/// Soft particle trail on the floor that guides the player's gaze
/// from spawn point toward the central water basin.
/// Subtle wayfinding without UI.
/// </summary>
public class GuidingTrail : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem trailParticles;
    [SerializeField] private Transform targetDestination;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 8f;
    [SerializeField] private float delayBeforeStart = 3f;

    [Header("Auto-Disable")]
    [Tooltip("Disable trail once player reaches this distance to the target")]
    [SerializeField] private float disableAtDistance = 2f;
    [SerializeField] private Transform playerTransform;

    private bool isActive = false;

    private IEnumerator Start()
    {
        if (trailParticles != null)
            trailParticles.Stop();

        yield return new WaitForSeconds(delayBeforeStart);

        if (trailParticles != null)
        {
            trailParticles.Play();
            isActive = true;
        }
    }

    private void Update()
    {
        if (!isActive || playerTransform == null || targetDestination == null) return;

        float dist = Vector3.Distance(
            new Vector3(playerTransform.position.x, 0, playerTransform.position.z),
            new Vector3(targetDestination.position.x, 0, targetDestination.position.z)
        );

        if (dist <= disableAtDistance)
        {
            StartCoroutine(FadeOutTrail());
            isActive = false;
        }
    }

    private IEnumerator FadeOutTrail()
    {
        if (trailParticles == null) yield break;

        var emission = trailParticles.emission;
        float startRate = emission.rateOverTime.constant;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            emission.rateOverTime = Mathf.Lerp(startRate, 0f, t);
            yield return null;
        }

        trailParticles.Stop();
    }
}
