using System.Collections;
using UnityEngine;

public class SpawnScaleEffect : MonoBehaviour
{
    [Header("Collect Particle")]
    public GameObject collectParticlePrefab;

    [Header("Spawn Effect")]
    public float spawnDuration = 0.2f;

    [Header("Collect Effect")]
    public float collectDuration = 0.12f;

    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        StartCoroutine(SpawnEffect());
    }

    private IEnumerator SpawnEffect()
    {
        float time = 0f;

        while (time < spawnDuration)
        {
            time += Time.deltaTime;

            float t = SmoothStep(time / spawnDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            yield return null;
        }

        transform.localScale = targetScale;
    }

    public void Collect()
    {
        StopAllCoroutines();
        StartCoroutine(CollectEffect());
    }

    private IEnumerator CollectEffect()
    {
        Vector3 startScale = transform.localScale;

        float time = 0f;

        while (time < collectDuration)
        {
            time += Time.deltaTime;

            float t = time / collectDuration;
            t *= t;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        if (collectParticlePrefab != null)
            Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);

        Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
    }

    private float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
}


