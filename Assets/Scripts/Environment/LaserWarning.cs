using System.Collections;
using UnityEngine;

public class LaserWarning : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public float blinkDuration = 2f;
    public int blinkCount = 2;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator PlayWarning()
    {
        if (spriteRenderer == null) yield break;

        int safeBlinkCount = Mathf.Max(1, blinkCount);
        float singleBlinkTime = blinkDuration / (safeBlinkCount * 2f);

        for (int i = 0; i < safeBlinkCount; i++)
        {
            SetVisible(true);
            yield return new WaitForSeconds(singleBlinkTime);

            SetVisible(false);
            yield return new WaitForSeconds(singleBlinkTime);
        }
    }

    private void SetVisible(bool visible)
    {
        spriteRenderer.enabled = visible;
    }
}