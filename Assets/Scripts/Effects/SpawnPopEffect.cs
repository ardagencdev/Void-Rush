using UnityEngine;
using System.Collections;

public class SpawnPopEffect : MonoBehaviour
{
    [Header("Spawn Effect")]
    public float spawnDuration = 0.25f;
    public float overshoot = 1.12f;

    [Header("Behaviour")]
    public bool playOnStart = false;

    private Vector3 targetScale;
    private Coroutine routine;

    private void Awake()
    {
        targetScale = transform.localScale;
        HideInstant();
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void HideInstant()
    {
        transform.localScale = Vector3.zero;
    }

    public void ShowInstant()
    {
        transform.localScale = targetScale;
    }

    public void Play()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(SpawnRoutine());
    }

    public IEnumerator PlayAndWait()
    {
        if (routine != null)
            StopCoroutine(routine);

        yield return SpawnRoutine();
        routine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        float time = 0f;
        transform.localScale = Vector3.zero;

        while (time < spawnDuration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(time / spawnDuration);
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * overshoot;

            transform.localScale = targetScale * scale;

            yield return null;
        }

        transform.localScale = targetScale;
    }
}
