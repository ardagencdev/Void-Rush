using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BeaconPulseWave : MonoBehaviour
{
    public float duration = 1f;
    public float startScale = 0.1f;
    public float endScale = 6f;

    [Header("Buff Check")]
    public LayerMask enemyLayers;

    private SpriteRenderer sr;
    private BeaconEnemy source;
    private bool canBuff;
    private float timer;

    private readonly HashSet<EnemyBuffTarget> hitTargets = new HashSet<EnemyBuffTarget>();

    public void Initialize(BeaconEnemy beacon, bool buffEnabled)
    {
        source = beacon;
        canBuff = buffEnabled;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);
        float currentScale = Mathf.Lerp(startScale, endScale, t);

        transform.localScale = Vector3.one * currentScale;

        if (canBuff)
        {
            float radius = sr != null ? sr.bounds.extents.x + 0.4f : currentScale;
            CheckBuffTargets(radius);
        }

        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
        }

        if (timer >= duration)
            Destroy(gameObject);
    }

    private void CheckBuffTargets(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            EnemyBuffTarget target = hit.GetComponentInParent<EnemyBuffTarget>();

            if (target == null) continue;
            if (target.IsBuffed) continue;
            if (hitTargets.Contains(target)) continue;
            if (target.GetComponent<BeaconEnemy>() != null) continue;

            hitTargets.Add(target);

            if (source != null)
                source.ApplyBuffToTarget(target.gameObject);
        }
    }
}