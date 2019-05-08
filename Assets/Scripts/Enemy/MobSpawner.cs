using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public static bool isPaused;
    public static float minSpawnTime;
    public static float maxSpawnTime;
    public static int maxActive;
    public static int activeMobs;
    public static float spawnTime;
    public static float timer = 0;

    static Transform player;
    static GameObject[] spawners;

    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        spawners = GameObject.FindGameObjectsWithTag("MobSpawner");
        NewSpawnTime();
    }

    void Update()
    {
        if (!isPaused && activeMobs < maxActive && player != null)
        {
            timer += Time.deltaTime;

            if (timer >= spawnTime)
            { // Spawn.
                Spawn();
                NewSpawnTime();
                timer = 0;
            }
        }
    }

    void Spawn()
    {
        Vector3 extentsEnemy = Vector3.one * 2;
        Vector3 extentsPlayer = Vector3.one * 20;

        Transform spawner;
        List<int> rolledSpawners = new List<int>();
        while (true)
        { // Reroll spawn position while spawner is blocked.
            if (rolledSpawners.Count == spawners.Length)
                break;

            int roll = Random.Range(0, spawners.Length);
            while (rolledSpawners.Contains(roll))
                roll = Random.Range(0, spawners.Length);
            rolledSpawners.Add(roll);

            spawner = spawners[roll].transform;
            if (!Physics.CheckBox(spawner.position, extentsEnemy, spawner.rotation, LayerMask.GetMask("Enemy")) &&
                !Physics.CheckBox(spawner.position, extentsPlayer, spawner.rotation, LayerMask.GetMask("Player")))
            { // Not blocked; break from loop.
                Instantiate(enemyPrefab, spawner.position, spawner.rotation);
                break;
            }
        }
    }

    void NewSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
