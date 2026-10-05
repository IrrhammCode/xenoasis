using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class CameraRigWatcher : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogWarning("[CameraRigWatcher] Awake on " + gameObject.name);
    }

    private void OnEnable()
    {
        Debug.LogWarning("[CameraRigWatcher] OnEnable on " + gameObject.name);
    }

    private void OnDisable()
    {
        Debug.LogWarning("[CameraRigWatcher] OnDisable on " + gameObject.name + "! StackTrace:\n" + System.Environment.StackTrace);
    }

    private void OnDestroy()
    {
        Debug.LogWarning("[CameraRigWatcher] OnDestroy on " + gameObject.name + "! StackTrace:\n" + System.Environment.StackTrace);
    }
}
