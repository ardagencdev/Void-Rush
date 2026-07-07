using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.15f;
    public float strength = 0.10f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    public void Shake()
    {
        originalPosition = transform.position;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            transform.position = originalPosition + new Vector3(x, y, 0f);

            yield return null;
        }

        transform.position = originalPosition;
        shakeCoroutine = null;
    }
}