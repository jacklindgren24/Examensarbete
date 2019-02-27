using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public UnityEngine.UI.Text waveCounter;

    [Space(15)]

    public Wave[] waves;
    int currentWave = -1;
    public int CurrentWave
    {
        get { return currentWave; }
        set { currentWave = value; }
    }
    int waveKills = 0;
    public int WaveKills
    {
        get { return waveKills; }
        set
        {
            waveKills = value;

            if (waveKills >= waves[CurrentWave].mobAmount + waves[CurrentWave].eliteAmount)
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

    List<Spawner> spawners = new List<Spawner>();

    [FMODUnity.EventRef]
    public string waveCountdownEventRef;

    void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);

        waveCounter.CrossFadeAlpha(0, 0, true); // Make wave counter transparent on awake.
	}

    void Start()
    {
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawners.Add(spawner.GetComponent<Spawner>());
        }

        CurrentWave = -1;
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

        if (CurrentWave > waves.Length) WinGame();

        waveKills = 0;
        CurrentWave++;
        Wave w = waves[CurrentWave];
 
        MobSpawner.spawnsLeft = w.mobAmount;
        MobSpawner.minSpawnTime = w.mobMinSpawnTime;
        MobSpawner.maxSpawnTime = w.mobMaxSpawnTime;
        EliteSpawner.spawnsLeft = w.eliteAmount;
        EliteSpawner.minSpawnTime = w.eliteMinSpawnTime;
        EliteSpawner.maxSpawnTime = w.eliteMaxSpawnTime;

        foreach (Spawner s in spawners) s.ResetTimer();

        PlayerController.SetIntensities(w.projectileIntensity, w.hitscanIntensity, w.meleeIntensity);

        if (waveCounter.color.a < 1) waveCounter.CrossFadeAlpha(0, 2, false);

        print("Wave " + (CurrentWave + 1));

        StartCoroutine(StartWave(w.startDelay));
    }

    IEnumerator StartWave(float delay)
    {
        yield return new WaitForSeconds(delay - 3);

        FMODUnity.RuntimeManager.PlayOneShot(waveCountdownEventRef);

        yield return new WaitForSeconds(3);

        if (waveCounter.color.a > 0) waveCounter.CrossFadeAlpha(1, 2, false);
        waveCounter.text = "Wave " + (currentWave + 1);

        foreach (Spawner spawner in spawners) spawner.isPaused = false;
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

    void Quit()
    {
        Application.Quit();
    }
}

[System.Serializable]
public struct Wave
{
    [Range(3, 10)]
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
