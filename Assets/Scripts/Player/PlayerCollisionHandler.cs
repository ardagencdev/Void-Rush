using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public PlayerArmor playerArmor;
    public PlayerCoinCollector coinCollector;
    public GameStateManager gameStateManager;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerArmor == null)
            playerArmor = GetComponent<PlayerArmor>();

        if (coinCollector == null)
            coinCollector = GetComponent<PlayerCoinCollector>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerMovement != null && playerMovement.IsGameOver)
            return;

        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        HandleHit(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerMovement != null && playerMovement.IsGameOver)
            return;

        if (other.CompareTag("Enemy"))
            HandleHit(other.gameObject);

        if (other.CompareTag("Laser"))
            HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject danger)
    {
        if (playerArmor != null && playerArmor.IsImmune)
            return;

        if (playerArmor != null && playerArmor.HasArmor)
        {
            playerArmor.BreakArmor();

            GameObject rootDanger = GetDangerRoot(danger);

            if (rootDanger != null)
            {
                DeathFadeEffect fade = rootDanger.GetComponent<DeathFadeEffect>();

                if (fade != null)
                    fade.Play();
                else
                    Destroy(rootDanger);
            }

            return;
        }

        int finalScore = 0;

        if (coinCollector != null)
            finalScore = coinCollector.Score;

        string deathCause = GetDeathCause(danger);

        if (playerMovement != null)
            playerMovement.GameOver(deathCause);
        else
        {
            LastDeathInfo.Cause = deathCause;

            if (gameStateManager != null)
                gameStateManager.GameOver(finalScore);
        }
    }

    private string GetDeathCause(GameObject danger)
    {
        if (danger == null) return "UNKNOWN";

        if (danger.CompareTag("Laser") || danger.GetComponentInParent<LaserWall>() != null)
            return "LASER WALL";

        if (danger.GetComponentInParent<BossEnemyFollow>() != null)
            return "BOSS";

        if (danger.GetComponentInParent<MiniBossFollow>() != null)
            return "MINI BOSS";

        if (danger.GetComponentInParent<HunterEnemyFollow>() != null)
            return "HUNTER";

        if (danger.GetComponentInParent<ProjectileEnemyFollow>() != null)
            return "BLASTER";

        if (danger.GetComponentInParent<EnemyProjectile>() != null)
            return "LASER BULLET";

        if (danger.GetComponentInParent<EnemyFollow>() != null)
            return "STALKER";

        return danger.name.Replace("(Clone)", "").Trim().ToUpper();
    }

    private GameObject GetDangerRoot(GameObject danger)
    {
        if (danger == null) return null;

        EnemyFollow enemy = danger.GetComponentInParent<EnemyFollow>();
        if (enemy != null) return enemy.gameObject;

        BossEnemyFollow boss = danger.GetComponentInParent<BossEnemyFollow>();
        if (boss != null) return boss.gameObject;

        MiniBossFollow miniBoss = danger.GetComponentInParent<MiniBossFollow>();
        if (miniBoss != null) return miniBoss.gameObject;

        HunterEnemyFollow hunter = danger.GetComponentInParent<HunterEnemyFollow>();
        if (hunter != null) return hunter.gameObject;

        ProjectileEnemyFollow projectileEnemy = danger.GetComponentInParent<ProjectileEnemyFollow>();
        if (projectileEnemy != null) return projectileEnemy.gameObject;

        EnemyProjectile projectile = danger.GetComponentInParent<EnemyProjectile>();
        if (projectile != null) return projectile.gameObject;

        LaserWall laserWall = danger.GetComponentInParent<LaserWall>();
        if (laserWall != null) return laserWall.gameObject;

        return danger;
    }
}
