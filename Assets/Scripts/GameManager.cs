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

        waveKills = 0;
        currentWave++;
        Wave w = waves[currentWave];
 
        MobSpawner.spawnsLeft = w.mobAmount;
        MobSpawner.minSpawnTime = w.mobMinSpawnTime;
        MobSpawner.maxSpawnTime = w.mobMaxSpawnTime;
        EliteSpawner.spawnsLeft = w.eliteAmount;
        EliteSpawner.minSpawnTime = w.eliteMinSpawnTime;
        EliteSpawner.maxSpawnTime = w.eliteMaxSpawnTime;

        foreach (Spawner s in spawners) s.ResetTimer();

        PlayerController.SetIntensities(w.projectileIntensity, w.hitscanIntensity, w.meleeIntensity);

        print("Wave: " + (currentWave + 1));

        Invoke("EnableSpawners", w.startDelay);
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

    [Header("Intensities")]
    public int hitscanIntensity;
    public int projectileIntensity;
    public int meleeIntensity;

    [Header("Mob")]
    public int mobAmount;
    public float mobMinSpawnTime;
    public float mobMaxSpawnTime;

    [Header("Elite")]
    public int eliteAmount;
    public float eliteMinSpawnTime;
    public float eliteMaxSpawnTime;
}
