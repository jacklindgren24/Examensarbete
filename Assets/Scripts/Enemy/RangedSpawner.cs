using UnityEngine;

public class RangedSpawner : Spawner
{
    public static float minSpawnTime;
    public static float maxSpawnTime;
    public static int maxActive;
    public static int activeRangedEnemies;

    protected override void Start()
    {
        base.Start();
        NewSpawnTime();
    }

    void Update()
    {
        if (!isPaused && activeRangedEnemies < maxActive && player != null)
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
    }

    void NewSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
