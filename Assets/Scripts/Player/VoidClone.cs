using System.Collections;
using UnityEngine;

public class VoidClone : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public float blinkSpeed = 10f;
    public float minAlpha = 0.35f;
    public float maxAlpha = 0.75f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void StartClone(float duration)
    {
        StartCoroutine(CloneVisualRoutine(duration));
    }

    private IEnumerator CloneVisualRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * blinkSpeed, 1f));
                spriteRenderer.color = c;
            }

            yield return null;
        }
    }
}