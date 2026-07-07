using UnityEngine;

public class MainMenuStarColorRandomizer : MonoBehaviour
{
    [Header("Reference")]
    public ParticleSystem nearStars;

    [Header("Random Colors")]
    public Color[] possibleColors =
    {
        new Color(0.2f, 0.6f, 1f, 0.9f), // Blue
        new Color(1f, 0.2f, 0.2f, 0.9f), // Red
        new Color(0.6f, 0.2f, 1f, 0.9f), // Purple
        new Color(0.2f, 1f, 0.6f, 0.9f), // Green
        new Color(1f, 0.8f, 0.2f, 0.9f)  // Yellow
    };

    private void Start()
    {
        ApplyRandomColor();
    }

    private void ApplyRandomColor()
    {
        if (nearStars == null) return;
        if (possibleColors == null || possibleColors.Length == 0) return;

        Color randomColor = possibleColors[Random.Range(0, possibleColors.Length)];

        var main = nearStars.main;
        main.startColor = new ParticleSystem.MinMaxGradient(randomColor);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[main.maxParticles];
        int count = nearStars.GetParticles(particles);

        for (int i = 0; i < count; i++)
            particles[i].startColor = randomColor;

        nearStars.SetParticles(particles, count);
    }
}