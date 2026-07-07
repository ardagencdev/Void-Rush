using UnityEngine;
using System.Collections;

public class CameraZoomPulse : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    [Header("Zoom Settings")]
    public float zoomAmount = 0.4f;
    public float zoomDuration = 0.08f;
    public float returnDuration = 0.12f;

    private float originalSize;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    private void Start()
    {
        if (cam != null)
            originalSize = cam.orthographicSize;
    }

    public void Pulse()
    {
        if (cam == null) return;

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float targetSize = originalSize - zoomAmount;

        yield return Zoom(originalSize, targetSize, zoomDuration);
        yield return Zoom(targetSize, originalSize, returnDuration);

        cam.orthographicSize = originalSize;
        pulseRoutine = null;
    }

    private IEnumerator Zoom(float from, float to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            cam.orthographicSize = Mathf.Lerp(from, to, time / duration);

            yield return null;
        }

        cam.orthographicSize = to;
    }
}