using UnityEngine;

/// <summary>
/// XENOASIS — PlanetEarthGlobe.cs
/// Controls the photorealistic 3D celestial Planet Earth in deep space.
/// Provides axial tilt (23.5 degrees), realistic diurnal rotation,
/// and smooth approach scaling during orbital descent.
/// </summary>
public class PlanetEarthGlobe : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float axialTilt = 23.44f;
    [SerializeField] private float rotationSpeed = 0.5f; // degrees per second
    [SerializeField] private Transform cloudLayer;
    [SerializeField] private float cloudRotationSpeed = 0.8f; // clouds drift slightly faster than surface

    [Header("Atmosphere")]
    [SerializeField] private Transform atmosphereHaze;

    private void Start()
    {
        // Apply Earth's real axial tilt
        transform.localRotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    private void Update()
    {
        // Rotate Earth around its tilted Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // Clouds rotate slightly faster to simulate atmospheric circulation
        if (cloudLayer != null)
        {
            cloudLayer.Rotate(Vector3.up, cloudRotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
