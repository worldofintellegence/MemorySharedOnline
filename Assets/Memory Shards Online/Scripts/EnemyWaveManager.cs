using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SurvivalEngine;
using NetcodePlus;

/// <summary>
/// Starts enemy waves in every danger zone at an interval, sends raids toward the town and spawns a boss.
/// Server only. The wave number is stored in WorldData so a restarted server continues where it was.
/// </summary>
public class EnemyWaveManager : MonoBehaviour
{
    public EnemyWaveZone[] zones;
    public float first_wave_delay = 20f; //Seconds before wave 1
    public float wave_interval = 90f;    //Seconds between waves

    [Header("Town Raids")]
    public int raid_every = 3;           //Every X waves, a raid marches on the town (0 = off)
    public CharacterData[] raid_enemies;
    public Transform[] raid_gates;       //Town gates, raids spawn outside one of them
    public Transform raid_target;        //Where raids march to (town center)
    public float raid_spawn_distance = 35f;
    public int raid_base_count = 4;
    public float raid_count_per_wave = 0.5f;
    public LayerMask floor_layer = (1 << 9);

    [Header("Boss")]
    public int boss_every = 5;           //Every X waves, the boss spawns if not already alive (0 = off)
    public CharacterData boss;
    public Transform boss_spawn;

    public UnityAction<int> onWaveStart;
    public UnityAction<int> onRaidStart;
    public UnityAction<int> onBossSpawn;

    private int wave_index = 0;
    private float timer = 0f;
    private float raid_timer = 0f;
    private List<AnimalWild> raiders = new List<AnimalWild>();

    private const string wave_uid = "enemy_wave_index";
    private const string timer_uid = "enemy_wave_timer";

    private static EnemyWaveManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        WorldData wdata = WorldData.Get();
        if (wdata.HasCustomInt(wave_uid))
            wave_index = wdata.GetCustomInt(wave_uid);
        if (wdata.HasCustomFloat(timer_uid))
            timer = wdata.GetCustomFloat(timer_uid);
    }

    void Update()
    {
        if (!TheNetwork.Get().IsServer)
            return;
        if (TheGame.Get().IsPaused())
            return;

        timer += Time.deltaTime;
        WorldData.Get().SetCustomFloat(timer_uid, timer);

        float interval = wave_index == 0 ? first_wave_delay : wave_interval;
        if (timer >= interval)
        {
            timer = 0f;
            StartNextWave();
        }

        raid_timer += Time.deltaTime;
        if (raid_timer > 5f)
        {
            raid_timer = 0f;
            UpdateRaiders();
        }
    }

    public void StartNextWave()
    {
        wave_index++;
        WorldData.Get().SetCustomInt(wave_uid, wave_index);

        int total = 0;
        foreach (EnemyWaveZone zone in zones)
        {
            if (zone != null)
                total += zone.SpawnWave(wave_index);
        }
        Debug.Log("Wave " + wave_index + " started, " + total + " enemies spawned");
        onWaveStart?.Invoke(wave_index);

        if (raid_every > 0 && wave_index % raid_every == 0)
            StartRaid();

        if (boss_every > 0 && wave_index % boss_every == 0)
            SpawnBoss();
    }

    public void StartRaid()
    {
        if (raid_target == null || raid_gates == null || raid_gates.Length == 0 || raid_enemies == null || raid_enemies.Length == 0)
            return;

        Transform gate = raid_gates[Random.Range(0, raid_gates.Length)];
        Vector3 dir = gate.position - raid_target.position;
        dir.y = 0f;
        Vector3 center = gate.position + dir.normalized * raid_spawn_distance;

        int count = raid_base_count + Mathf.FloorToInt(wave_index * raid_count_per_wave);
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 6f;
            Vector3 test = center + new Vector3(offset.x, 0f, offset.y);
            if (!PhysicsTool.FindGroundPosition(test, 100f, floor_layer.value, out Vector3 pos))
                continue;

            CharacterData data = raid_enemies[Random.Range(0, raid_enemies.Length)];
            Character character = EnemyWaveZone.Spawn(data, pos);
            AnimalWild wild = character != null ? character.GetComponent<AnimalWild>() : null;
            if (wild != null)
            {
                wild.MoveToTarget(raid_target.position, true);
                raiders.Add(wild);
            }
        }
        Debug.Log("Raid on wave " + wave_index + " from gate " + gate.name);
        onRaidStart?.Invoke(wave_index);
    }

    public void SpawnBoss()
    {
        if (boss == null || boss_spawn == null)
            return;
        if (Character.CountInRange(boss, boss_spawn.position, 60f) > 0)
            return; //Previous boss still alive

        EnemyWaveZone.Spawn(boss, boss_spawn.position);
        Debug.Log("Boss spawned on wave " + wave_index);
        onBossSpawn?.Invoke(wave_index);
    }

    //Raiders go back to wandering when they reach a target or lose a player, keep pushing them to the town
    private void UpdateRaiders()
    {
        for (int i = raiders.Count - 1; i >= 0; i--)
        {
            AnimalWild wild = raiders[i];
            if (wild == null || wild.IsDead())
            {
                raiders.RemoveAt(i);
                continue;
            }

            float dist = (wild.transform.position - raid_target.position).magnitude;
            if (!wild.IsMoving() && dist > 12f)
                wild.MoveToTarget(raid_target.position, true);
        }
    }

    public int GetWave()
    {
        return wave_index;
    }

    public static EnemyWaveManager Get()
    {
        return instance;
    }
}
