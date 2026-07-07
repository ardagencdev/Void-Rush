using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameplayMusicFade : MonoBehaviour
{
    [Range(0f, 1f)]
    public float gameplayMusicBaseVolume = 0.2f;

    public float fadeInDuration = 0.6f;
    public float fadeOutDuration = 0.4f;

    private AudioSource source;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        source.volume = 0f;
        source.Play();

        FadeIn();
    }

    public void FadeIn()
    {
        FadeTo(GetTargetVolume(), fadeInDuration);
    }

    public void FadeOut()
    {
        FadeTo(0f, fadeOutDuration, true);
    }

    public void RefreshVolume()
    {
        FadeTo(GetTargetVolume(), 0.2f);
    }

    private float GetTargetVolume()
    {
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        bool gameplayMusicOn = PlayerPrefs.GetInt("GameplayMusicOn", 1) == 1;

        if (!soundOn || !gameplayMusicOn)
            return 0f;

        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        return volume * gameplayMusicBaseVolume;
    }

    private void FadeTo(float target, float duration, bool stopAfter = false)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(target, duration, stopAfter));
    }

    IEnumerator FadeRoutine(float target, float duration, bool stopAfter)
    {
        float start = source.volume;

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p);

            source.volume = Mathf.Lerp(start, target, p);

            yield return null;
        }

        source.volume = target;

        if (stopAfter)
            source.Stop();

        fadeRoutine = null;
    }
}