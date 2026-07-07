using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossScreenEffect : MonoBehaviour
{
    [Header("References")]
    public Image warningImage;

    [Header("Pulse Settings")]
    public float fadeDuration = 0.5f;
    public float maxAlpha = 0.18f;
    public float waitBetweenPulses = 1.2f;

    private Coroutine pulseCoroutine;
    private bool isActive;

    private void Start()
    {
        SetAlpha(0f);
    }

    public void StartEffect()
    {
        if (warningImage == null) return;

        StopEffect();

        isActive = true;
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    public void StopEffect()
    {
        isActive = false;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        SetAlpha(0f);
    }

    private IEnumerator PulseRoutine()
    {
        while (isActive)
        {
            yield return Fade(0f, maxAlpha);
            yield return Fade(maxAlpha, 0f);
            yield return new WaitForSeconds(waitBetweenPulses);
        }

        SetAlpha(0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration && isActive)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, time / fadeDuration));
            yield return null;
        }

        if (isActive)
            SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (warningImage == null) return;

        Color color = warningImage.color;
        color.a = alpha;
        warningImage.color = color;
    }
}