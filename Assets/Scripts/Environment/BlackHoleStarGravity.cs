using UnityEngine;

public class BlackHoleStarGravity : MonoBehaviour
{
    [Header("Gravity")]
    public float gravityStrength = 35f;
    public float influenceRadius = 6f;
    public float minDistance = 0.5f;

    [Header("Swirl")]
    public float swirlStrength = 6f;

    [Header("Consume")]
    public bool consumeStars = true;
    public float consumeRadius = 0.45f;

    private void OnEnable()
    {
        NearStarsBlackHoleController.Register(this);
    }

    private void OnDisable()
    {
        NearStarsBlackHoleController.Unregister(this);
    }
}