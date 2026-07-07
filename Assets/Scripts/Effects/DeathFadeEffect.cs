using UnityEngine;
using System.Collections;

public class DeathFadeEffect : MonoBehaviour
{
    public float fadeDuration = 0.45f;
    public bool destroyAfterFade = true;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private Vector3 originalScale;
    private bool isPlaying;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
        originalScale = transform.localScale;
    }

    public void Play()
    {
        if (isPlaying) return;

        isPlaying = true;

        DisablePhysics();

        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    private void DisablePhysics()
    {
        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
    }

    private IEnumerator FadeRoutine()
    {
        if (sr == null) yield break;

        Color startColor = sr.color;
        Color targetColor = Color.black;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fadeDuration;

            sr.color = Color.Lerp(startColor, targetColor, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        sr.color = targetColor;
        transform.localScale = targetScale;

        if (destroyAfterFade)
            Destroy(gameObject);
    }
}