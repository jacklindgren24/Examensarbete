﻿using UnityEngine;

public class EliteSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public static bool isPaused;
    public static float minSpawnTime;
    public static float maxSpawnTime;
    public static int maxActive;
    public static int activeElites;
    public static float spawnTime;
    public static float timer = 0;

    static Transform player;
    static GameObject[] spawners;

    void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        spawners = GameObject.FindGameObjectsWithTag("EliteSpawner");
        NewSpawnTime();
    }

    void Update()
    {
        if (!isPaused && activeElites < maxActive && player != null)
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
        Transform spawner = spawners[Random.Range(0, spawners.Length)].transform;
        Instantiate(enemyPrefab, spawner.position, spawner.rotation);
    }

    void NewSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
