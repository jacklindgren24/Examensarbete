﻿using UnityEngine;

public class EliteSpawner : Spawner
{
    public static int spawnsLeft;
    public static float minSpawnTime;
    public static float maxSpawnTime;

    protected override void Start()
    {
        base.Start();
        NewSpawnTime();
    }

    void Update()
    {
        if (!isPaused && spawnsLeft > 0 && player != null)
        {
            if (Vector3.Distance(player.position, transform.position) <= activationRange)
            { // Only update timer if player is within range.
                timer += Time.deltaTime;

                if (timer >= spawnTime)
                { // Spawn.
                    Spawn();
                    NewSpawnTime();
                    timer = 0;
                }
            }
        }
    }

    void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
        spawnsLeft--;
    }

    void NewSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
