using UnityEngine;

/// <summary>
/// Smooth gentle floating and spinning script for museum exhibition masterpieces
/// </summary>
public class ExhibitRotator : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public float bobAmplitude = 0.05f;
    public float bobSpeed = 1.2f;

    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = initialPos + new Vector3(0f, bob, 0f);
    }
}
