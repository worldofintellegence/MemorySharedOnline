using UnityEngine;
using SurvivalEngine;
using NetcodePlus;

/// <summary>
/// Danger zone that spawns enemy waves around its position. Spawning only happens on the server.
/// </summary>
public class EnemyWaveZone : MonoBehaviour
{
    public string zone_name = "Zone";
    public CharacterData[] enemies;
    public float radius = 30f;          //Spawn circle radius
    public int base_count = 3;          //Enemies spawned on wave 1
    public float count_per_wave = 1f;   //Extra enemies added each wave
    public int max_alive = 15;          //Will not spawn above this amount alive in the zone
    public LayerMask floor_layer = (1 << 9);

    public int SpawnWave(int wave)
    {
        int target = base_count + Mathf.FloorToInt((wave - 1) * count_per_wave);
        int room = max_alive - CountAlive();
        int count = Mathf.Min(target, room);
        for (int i = 0; i < count; i++)
        {
            if (FindSpawnPos(transform.position, radius, out Vector3 pos))
                Spawn(GetRandomEnemy(), pos);
        }
        return Mathf.Max(count, 0);
    }

    public int CountAlive()
    {
        int count = 0;
        foreach (CharacterData data in enemies)
            count += Character.CountInRange(data, transform.position, radius * 1.5f);
        return count;
    }

    public CharacterData GetRandomEnemy()
    {
        if (enemies == null || enemies.Length == 0)
            return null;
        return enemies[Random.Range(0, enemies.Length)];
    }

    public bool FindSpawnPos(Vector3 center, float range, out Vector3 pos)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * range;
            Vector3 test = center + new Vector3(offset.x, 0f, offset.y);
            if (PhysicsTool.FindGroundPosition(test, 100f, floor_layer.value, out pos))
                return true;
        }
        pos = center;
        return false;
    }

    public static Character Spawn(CharacterData data, Vector3 pos)
    {
        if (data == null || !TheNetwork.Get().IsServer)
            return null;
        return Character.Create(data, pos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
