using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 0.12f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float hideDelay = 0.45f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (valueText != null)
        {
            canvasGroup = valueText.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = valueText.gameObject.AddComponent<CanvasGroup>();
        }

        if (slider != null)
            slider.onValueChanged.AddListener(UpdateText);

        HideInstant();
    }

    private void OnEnable()
    {
        if (slider != null)
            UpdateText(slider.value);

        HideInstant();
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(UpdateText);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Show();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HideAfterDelay();
    }

    private void UpdateText(float value)
    {
        if (valueText == null) return;

        valueText.text = Mathf.RoundToInt(value * 100f) + "%";

        if (gameObject.activeInHierarchy)
            Show();
    }

    private void Show()
    {
        if (valueText == null || canvasGroup == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        valueText.gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(FadeRoutine(1f, fadeInDuration));
    }

    private void HideAfterDelay()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(HideDelayRoutine());
    }

    private IEnumerator HideDelayRoutine()
    {
        yield return new WaitForSecondsRealtime(hideDelay);
        yield return FadeRoutine(0f, fadeOutDuration);

        if (valueText != null)
            valueText.gameObject.SetActive(false);
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private void HideInstant()
    {
        if (valueText == null) return;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        valueText.gameObject.SetActive(false);
    }
}