using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float activationRange = 30;
    public bool isPaused;

    protected float spawnTime;
    protected float timer = 0;

    protected static Transform player;

    protected virtual void Start()
    {
        if (player == null) player = GameObject.FindWithTag("Player").transform;
    }

    public void ResetTimer()
    {
        timer = 0;
    }
}
