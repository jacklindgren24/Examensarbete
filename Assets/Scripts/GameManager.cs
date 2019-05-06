using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using FMODUnity;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject winCanvas;
    public UnityEngine.UI.Text waveCounter;
    public GameObject countdown;
    public Transform goalSpawnerParent;
    public GameObject waveGoalPrefab;
    public float minimumGoalDistance = 40;

    [EventRef]
    public string pauseGame;
    [EventRef]
    public string unpauseGame;

    [Space(15)]

    public Wave[] waves;

    [Space(15)]

    [EventRef]
    public string waveCountdownEventRef;
    [EventRef]
    public string waveClearEventRef;

    int currentWave = -1;
    public int CurrentWave
    {
        get { return currentWave; }
        set { currentWave = value; }
    }

    float timer;
    public static int totalMobKills;
    public static int totalEliteKills;
    public static int totalHitscanShots;
    public static int totalHitscanHits;
    public static int totalProjectilesShot;
    public static int totalProjectilesHit;

    [HideInInspector]
    public bool isPaused = false;
    bool hasWon = false;
    List<Transform> goalSpawners = new List<Transform>();

    void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);

        for (int i = 0; i < goalSpawnerParent.childCount; i++)
        {
            goalSpawners.Add(goalSpawnerParent.GetChild(i));
        }

        SetPaused(false);
        hasWon = false;

        waveCounter.CrossFadeAlpha(0, 0, true); // Make wave counter transparent on awake.
	}

    void Start()
    {
        CurrentWave = -1;
        NextWave();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!isPaused && !hasWon)
            timer += Time.deltaTime;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L)) ToggleCursorState();
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
        if (Input.GetKeyDown(KeyCode.P)) ToggleSpawners();
        if (Input.GetKeyDown(KeyCode.Tab)) WinGame();
#endif
        if (Input.GetButtonDown("Pause") && !hasWon) SetPaused(!isPaused);
        if (Input.GetKeyDown(KeyCode.Backspace)) WriteData();
    }


    /// <summary> End the current wave. </summary>
    /// <param name="immediate"> End wave immediately? Otherwise, wait until all enemies are killed. </param>
    public void EndWave(bool immediate)
    {
        SetSpawnersPaused(true);

        if (immediate || EnemyController.enemies.Count == 0) NextWave();
        else StartCoroutine(WaitForEnemies());
    }

    IEnumerator WaitForEnemies()
    {
        while (EnemyController.enemies.Count > 0)
        { // Yield while there are still enemies present in scene.
            yield return null;
        }

        NextWave();
    }

    void NextWave()
    {
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (player.Health < 100)
            player.Health = 100;

        if (CurrentWave == waves.Length - 1)
        {
            WinGame();
        }
        else
        {
            foreach (GameObject enemy in EnemyController.enemies)
            { // Kill any remaining enemies.
                enemy.GetComponent<EnemyController>().Die();
            }

            CurrentWave++;
            Wave w = waves[CurrentWave];

            // Prepare spawners.
            MobSpawner.timer = 0;
            MobSpawner.maxActive = w.maxMobAmount;
            MobSpawner.minSpawnTime = w.mobMinSpawnTime;
            MobSpawner.maxSpawnTime = w.mobMaxSpawnTime;
            EliteSpawner.timer = 0;
            EliteSpawner.maxActive = w.maxEliteAmount;
            EliteSpawner.minSpawnTime = w.eliteMinSpawnTime;
            EliteSpawner.maxSpawnTime = w.eliteMaxSpawnTime;

            SetSpawnersPaused(true); // Pause spawners.

            PlayerController.SetIntensities(w.projectileIntensity, w.hitscanIntensity, w.meleeIntensity);

            waveCounter.CrossFadeAlpha(0, 2, false);

            if (CurrentWave > 0) RuntimeManager.PlayOneShot(waveClearEventRef);

            print("Wave " + (CurrentWave + 1));

            StartCoroutine(StartWave(w.startDelay));
        }
    }

    IEnumerator StartWave(float delay)
    {
        yield return new WaitForSeconds(delay - 3);
        
        RuntimeManager.PlayOneShot(waveCountdownEventRef);
        GameObject cd = Instantiate(countdown, gameCanvas.transform);
        Destroy(cd, cd.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length + 0.1f);

        yield return new WaitForSeconds(3);
        
        waveCounter.CrossFadeAlpha(1, 2, false);
        waveCounter.text = "Wave " + (currentWave + 1);

        // Spawn wave goal cube.
        List<Transform> potentials = new List<Transform>();
        foreach (Transform t in goalSpawners)
        { // Get distance to each spawner.
            if (Vector3.Distance(PlayerController.position, t.position) > minimumGoalDistance)
            { // Spawner is beyond minimum distance.
                potentials.Add(t);
            }
        }
        Transform goalSpawn = potentials[UnityEngine.Random.Range(0, potentials.Count)];
        Instantiate(waveGoalPrefab, goalSpawn.position, goalSpawn.rotation);

        SetSpawnersPaused(false); // Unpause spawners.
    }

    void WinGame()
    {
#if !UNITY_EDITOR
        WriteData();
#endif
        RuntimeManager.PlayOneShot(waveClearEventRef);
        RuntimeManager.GetBus("bus:/").setPaused(true);

        gameCanvas.SetActive(false);
        winCanvas.SetActive(true);

        hasWon = true;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void Restart()
    {
        ScoreCounter.Score = 0;

        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        GameOver.deathSnapEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        GameOver.deathSnapEv.release();

        foreach (GameObject enemy in EnemyController.enemies)
        {
            enemy.GetComponent<EnemyController>().enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            enemy.GetComponent<EnemyController>().enemyFootstepsEv.release();
        }

        EnemyController.enemies.Clear();

        SceneManager.LoadScene("Arena", LoadSceneMode.Single);
    }

    void ToggleCursorState()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;

        print("Cursor state toggled.");
    }

    void ToggleMute()
    {
        // Get master channel group.
        FMOD.ChannelGroup master;
        RuntimeManager.LowlevelSystem.getMasterChannelGroup(out master);

        // Get mute status.
        bool isMuted = false;
        master.getMute(out isMuted);

        master.setMute(!isMuted); // Invert mute status.

        print("Mute toggled.");
    }

    void ToggleSpawners()
    {
        MobSpawner.isPaused = !MobSpawner.isPaused;
        EliteSpawner.isPaused = !EliteSpawner.isPaused;

        print("Toggled spawners.");
    }

    public void SetSpawnersPaused(bool isPaused)
    {
        MobSpawner.isPaused = isPaused;
        EliteSpawner.isPaused = isPaused;
    }

    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;

        RuntimeManager.GetBus("bus:/").setPaused(isPaused);
        Cursor.visible = isPaused ? true : false;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = isPaused ? 0 : 1;
        pauseCanvas.SetActive(isPaused);
        gameCanvas.SetActive(!isPaused);
    }

    void WriteData()
    {
        string[] data =
        {
            "Time: " + timer + '\n',

            "WEAPONS (SHOTS / HITS)",
            "Hitscan: " + totalHitscanShots + " / " + totalHitscanHits,
            "Projectile: " + totalProjectilesShot + " / " + totalProjectilesHit,

            "KILLS",
            "Mob: " + totalMobKills,
            "Elite: " + totalEliteKills,
        };

        // Write to text file on desktop.
        int i = 1;
        while (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Test Data " + i + ".txt"))
        {
            i++;
        }
        File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Test Data " + i + ".txt", data);

        print("Wrote data to " + Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }

    public void Quit()
    {
        Application.Quit();
    }
}

[Serializable]
public struct Wave
{
    [Range(3, 10)]
    public float startDelay;

    [Header("Intensities")]
    public int hitscanIntensity;
    public int projectileIntensity;
    public int meleeIntensity;

    [Header("Mob")]
    public int maxMobAmount;
    public float mobMinSpawnTime;
    public float mobMaxSpawnTime;

    [Header("Elite")]
    public int maxEliteAmount;
    public float eliteMinSpawnTime;
    public float eliteMaxSpawnTime;
}
