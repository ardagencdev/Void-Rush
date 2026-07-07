using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class NearStarsBlackHoleController : MonoBehaviour
{
    private static readonly List<BlackHoleStarGravity> blackHoles = new List<BlackHoleStarGravity>();

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    [Header("Effect")]
    public float globalMultiplier = 1f;
    public float maxParticleSpeed = 8f;

    private float maxParticleSpeedSqr;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        particles = new ParticleSystem.Particle[main.maxParticles];

        maxParticleSpeedSqr = maxParticleSpeed * maxParticleSpeed;
    }

    private void OnValidate()
    {
        maxParticleSpeedSqr = maxParticleSpeed * maxParticleSpeed;
    }

    private void LateUpdate()
    {
        int blackHoleCount = blackHoles.Count;
        if (blackHoleCount == 0) return;

        int count = ps.GetParticles(particles);
        float delta = Time.deltaTime * globalMultiplier;

        for (int i = 0; i < count; i++)
        {
            Vector3 velocity = particles[i].velocity;
            Vector3 particlePos = particles[i].position;

            for (int b = blackHoleCount - 1; b >= 0; b--)
            {
                BlackHoleStarGravity blackHole = blackHoles[b];

                if (blackHole == null)
                {
                    blackHoles.RemoveAt(b);
                    blackHoleCount--;
                    continue;
                }

                Vector3 blackHolePos = blackHole.transform.position;
                Vector3 toBlackHole = blackHolePos - particlePos;

                float distanceSqr = toBlackHole.sqrMagnitude;
                float influenceSqr = blackHole.influenceRadius * blackHole.influenceRadius;

                if (distanceSqr > influenceSqr)
                    continue;

                float realDistance = Mathf.Sqrt(distanceSqr);

                if (blackHole.consumeStars)
                {
                    float consumeSqr = blackHole.consumeRadius * blackHole.consumeRadius;

                    if (distanceSqr <= consumeSqr)
                    {
                        particles[i].remainingLifetime = 0f;
                        break;
                    }
                }

                if (realDistance <= 0.001f)
                    continue;

                float distance = Mathf.Max(realDistance, blackHole.minDistance);
                Vector3 direction = toBlackHole / realDistance;

                float gravityForce = blackHole.gravityStrength / (distance * distance);
                velocity += direction * gravityForce * delta;

                Vector3 swirlDirection = new Vector3(-direction.y, direction.x, 0f);
                float swirlPower = 1f - Mathf.Clamp01(realDistance / blackHole.influenceRadius);

                velocity += swirlDirection * blackHole.swirlStrength * swirlPower * delta;
            }

            if (velocity.sqrMagnitude > maxParticleSpeedSqr)
                velocity = velocity.normalized * maxParticleSpeed;

            particles[i].velocity = velocity;
        }

        ps.SetParticles(particles, count);
    }

    public static void Register(BlackHoleStarGravity blackHole)
    {
        if (blackHole != null && !blackHoles.Contains(blackHole))
            blackHoles.Add(blackHole);
    }

    public static void Unregister(BlackHoleStarGravity blackHole)
    {
        if (blackHole != null)
            blackHoles.Remove(blackHole);
    }
}