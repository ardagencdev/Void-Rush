using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlowScreenEffect : MonoBehaviour
{
    [Header("References")]
    public RawImage vignetteImage;
    public Camera mainCamera;

    [Header("Vignette")]
    public float vignetteAlpha = 0.35f;
    public float vignetteFadeIn = 0.15f;
    public float vignetteFadeOut = 0.25f;

    [Header("Camera Pulse")]
    public float zoomAmount = 0.35f;
    public float zoomInDuration = 0.08f;
    public float zoomOutDuration = 0.18f;

    private Coroutine effectRoutine;
    private Coroutine zoomRoutine;
    private float originalCameraSize;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            originalCameraSize = mainCamera.orthographicSize;

        if (vignetteImage != null)
        {
            vignetteImage.texture = CreateVignetteTexture(512, 512);
            vignetteImage.color = new Color(0f, 0f, 0f, 0f);
            vignetteImage.raycastTarget = false;
        }
    }

    public void PlayEffect(float duration)
    {
        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        effectRoutine = StartCoroutine(EffectRoutine(duration));
        zoomRoutine = StartCoroutine(CameraPulseRoutine());
    }

    private IEnumerator EffectRoutine(float duration)
    {
        yield return FadeVignette(0f, vignetteAlpha, vignetteFadeIn);

        yield return new WaitForSecondsRealtime(duration);

        yield return FadeVignette(vignetteAlpha, 0f, vignetteFadeOut);

        effectRoutine = null;
    }

    private IEnumerator CameraPulseRoutine()
    {
        if (mainCamera == null) yield break;

        originalCameraSize = mainCamera.orthographicSize;
        float targetSize = originalCameraSize - zoomAmount;

        float time = 0f;

        while (time < zoomInDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / zoomInDuration;

            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, targetSize, t);

            yield return null;
        }

        time = 0f;

        while (time < zoomOutDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / zoomOutDuration;

            mainCamera.orthographicSize = Mathf.Lerp(targetSize, originalCameraSize, t);

            yield return null;
        }

        mainCamera.orthographicSize = originalCameraSize;
        zoomRoutine = null;
    }

    private IEnumerator FadeVignette(float from, float to, float duration)
    {
        if (vignetteImage == null) yield break;

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float alpha = Mathf.Lerp(from, to, t);
            vignetteImage.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        vignetteImage.color = new Color(0f, 0f, 0f, to);
    }

    private Texture2D CreateVignetteTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Vector2 center = new Vector2(width / 2f, height / 2f);
        float maxDistance = Vector2.Distance(Vector2.zero, center);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float t = Mathf.InverseLerp(maxDistance * 0.35f, maxDistance, distance);
                t = Mathf.SmoothStep(0f, 1f, t);

                texture.SetPixel(x, y, new Color(0f, 0f, 0f, t));
            }
        }

        texture.Apply();
        return texture;
    }
}