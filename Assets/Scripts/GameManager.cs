using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Wave[] waves;

    List<Spawner> spawners = new List<Spawner>();
    public static int currentWave = -1;
    int waveKills = 0;
    public int WaveKills
    {
        get { return waveKills; }
        set
        {
            waveKills = value;

            if (waveKills >= waves[currentWave].mobAmount + waves[currentWave].eliteAmount)
            {
                NextWave();
            }
        }
    }

    public static int totalMobKills;
    public static int totalEliteKills;
    public static int totalHitscanShots;
    public static int totalHitscanHits;
    public static int totalProjectilesShot;
    public static int totalProjectilesHit;
    public static int totalMeleeAttacks;
    public static int totalMeleeHits;

    void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);
	}

    void Start()
    {
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawners.Add(spawner.GetComponent<Spawner>());
        }

        currentWave = -1;
        NextWave();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) ToggleCursorState();
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
        if (Input.GetKeyDown(KeyCode.P)) ToggleSpawners();
        if (Input.GetKeyDown(KeyCode.Escape)) Quit();
    }

    void NextWave()
    {
        SetSpawnersPaused(true);

        if (currentWave > waves.Length) WinGame();

        currentWave++;
        waveKills = 0;
        MobSpawner.spawnsLeft = waves[currentWave].mobAmount;
        MobSpawner.minSpawnTime = waves[currentWave].mobMinSpawnTime;
        MobSpawner.maxSpawnTime = waves[currentWave].mobMaxSpawnTime;
        EliteSpawner.spawnsLeft = waves[currentWave].eliteAmount;
        EliteSpawner.minSpawnTime = waves[currentWave].eliteMinSpawnTime;
        EliteSpawner.maxSpawnTime = waves[currentWave].eliteMaxSpawnTime;

        foreach (Spawner s in spawners) s.ResetTimer();

        print("Wave: " + (currentWave + 1));

        Invoke("EnableSpawners", waves[currentWave].startDelay);
    }

    void WinGame()
    {

    }

    void ToggleCursorState()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void ToggleMute()
    {
        // Get master channel group.
        FMOD.ChannelGroup master;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out master);

        // Get mute status.
        bool isMuted = false;
        master.getMute(out isMuted);

        master.setMute(!isMuted); // Invert mute status.
    }

    void ToggleSpawners()
    {
        foreach (Spawner spawner in spawners)
        { // Invert paused status on every spawner.
            spawner.isPaused = !spawner.isPaused;
        }
    }

    void SetSpawnersPaused(bool isPaused)
    {
        foreach (Spawner spawner in spawners)
        { // Invert paused status on every spawner.
            spawner.isPaused = isPaused;
        }
    }

    void EnableSpawners()
    {
        foreach (Spawner spawner in spawners)
        { // Invert paused status on every spawner.
            spawner.isPaused = false;
        }

        print("Enabled spawners.");
    }

    void Quit()
    {
        Application.Quit();
    }
}

[System.Serializable]
public struct Wave
{
    public float startDelay;

    [Header("Mob")]
    public int mobAmount;
    public float mobMinSpawnTime;
    public float mobMaxSpawnTime;

    [Header("Elite")]
    public int eliteAmount;
    public float eliteMinSpawnTime;
    public float eliteMaxSpawnTime;
}
