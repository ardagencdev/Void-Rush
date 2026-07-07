using UnityEngine;

public static class SpawnValidator
{
    public static bool IsAreaClear(Vector2 pos, float radius, Transform player = null, float playerSafeDistance = 0f)
    {
        if (player != null && Vector2.Distance(pos, player.position) < playerSafeDistance)
            return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            if (hit.CompareTag("Player")) return false;
            if (hit.CompareTag("Enemy")) return false;
            if (hit.CompareTag("Coin")) return false;
            if (hit.CompareTag("PowerUp")) return false;
            if (hit.CompareTag("Bomb")) return false;
            if (hit.CompareTag("Laser")) return false;

            if (hit.gameObject.layer == LayerMask.NameToLayer("Obstacle")) return false;
            if (hit.gameObject.layer == LayerMask.NameToLayer("Wall")) return false;
            if (hit.gameObject.layer == LayerMask.NameToLayer("Trap")) return false;
        }

        return true;
    }
}