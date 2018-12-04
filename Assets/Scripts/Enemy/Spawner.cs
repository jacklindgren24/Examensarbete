using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject enemyPrefab;
    public float minSpawnTime = 3;
    public float maxSpawnTime = 6;
    public float activationRange = 30;

    float spawnTime;
    float timer = 0;

    Transform player;

	void Start ()
    {
        spawnTime = minSpawnTime;
        player = GameObject.FindWithTag("Player").transform;
	}
	
	void Update ()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.position, transform.position) <= activationRange)
            { // Only update timer of player is within range.
                timer += Time.deltaTime;

                if (timer >= spawnTime)
                {
                    Spawn();
                    NewSpawnTime();
                    timer = 0;
                }
            }
        }
	}

    void NewSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
