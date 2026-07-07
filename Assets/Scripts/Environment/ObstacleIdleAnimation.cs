using UnityEngine;

public class ObstacleIdleAnimation : MonoBehaviour
{
    public bool rotate = true;
    public float rotateSpeed = 30f;

    public bool pulseScale = false;
    public float pulseAmount = 0.08f;
    public float pulseSpeed = 2f;

    public bool pulseColor = false;
    public Color color1 = Color.white;
    public Color color2 = Color.cyan;

    public bool shake = false;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 20f;

    private SpriteRenderer sr;
    private Vector3 startScale;
    private Vector3 startPos;

    private void Start()
    {
        startScale = transform.localScale;
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Rotate
        if (rotate)
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        // Scale Pulse
        if (pulseScale)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = startScale * scale;
        }

        // Color Pulse
        if (pulseColor && sr != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            sr.color = Color.Lerp(color1, color2, t);
        }

        // Shake
        if (shake)
        {
            float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f;
            float y = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f;
            transform.position = startPos + new Vector3(x, y, 0f) * shakeAmount;
        }
    }
}