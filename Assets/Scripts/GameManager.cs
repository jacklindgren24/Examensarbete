using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Canvas canvas;
    public UnityEngine.UI.Text waveCounter;
    public GameObject countdown;
    public Transform goalSpawnerParent;
    public GameObject waveGoalPrefab;

    [Space(15)]

    public Wave[] waves;

    [Space(15)]

    [FMODUnity.EventRef]
    public string waveCountdownEventRef;
    [FMODUnity.EventRef]
    public string waveClearEventRef;

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

            //if (waveKills >= waves[CurrentWave].mobAmount + waves[CurrentWave].eliteAmount)
            //{
            //    NextWave();
            //}
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

    List<Spawner> enemySpawners = new List<Spawner>();
    List<Transform> goalSpawners = new List<Transform>();

    void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);

        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            enemySpawners.Add(spawner.GetComponent<Spawner>());
        }
        SetSpawnersPaused(true);

        for (int i = 0; i < goalSpawnerParent.childCount; i++)
        {
            goalSpawners.Add(goalSpawnerParent.GetChild(i));
        }

        waveCounter.CrossFadeAlpha(0, 0, true); // Make wave counter transparent on awake.
	}

    void Start()
    {
        CurrentWave = -1;
        NextWave();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) ToggleCursorState();
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
        if (Input.GetKeyDown(KeyCode.P)) ToggleSpawners();
        if (Input.GetKeyDown(KeyCode.Escape)) Quit();
    }
#endif

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

            waveKills = 0;
            CurrentWave++;
            Wave w = waves[CurrentWave];

            MobSpawner.maxActive = w.maxMobAmount;
            MobSpawner.minSpawnTime = w.mobMinSpawnTime;
            MobSpawner.maxSpawnTime = w.mobMaxSpawnTime;
            EliteSpawner.maxActive = w.maxEliteAmount;
            EliteSpawner.minSpawnTime = w.eliteMinSpawnTime;
            EliteSpawner.maxSpawnTime = w.eliteMaxSpawnTime;

            foreach (Spawner s in enemySpawners) s.ResetTimer();

            PlayerController.SetIntensities(w.projectileIntensity, w.hitscanIntensity, w.meleeIntensity);

            if (waveCounter.color.a < 1) waveCounter.CrossFadeAlpha(0, 2, false);

            if (CurrentWave > 0) FMODUnity.RuntimeManager.PlayOneShot(waveClearEventRef);

            print("Wave " + (CurrentWave + 1));

            StartCoroutine(StartWave(w.startDelay));
        }
    }

    IEnumerator StartWave(float delay)
    {
        yield return new WaitForSeconds(delay - 3);

        FMODUnity.RuntimeManager.PlayOneShot(waveCountdownEventRef);
        GameObject cd = Instantiate(countdown, canvas.transform);
        Destroy(cd, cd.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length + 0.1f);

        yield return new WaitForSeconds(3);

        if (waveCounter.color.a > 0) waveCounter.CrossFadeAlpha(1, 2, false);
        waveCounter.text = "Wave " + (currentWave + 1);

        // Spawn wave goal cube.
        Dictionary<float, Transform> distances = new Dictionary<float, Transform>();
        foreach (Transform t in goalSpawners)
        { // Get distance to each spawner.
            distances.Add(Vector3.Distance(PlayerController.position, t.position), t);
        }
        Transform goalSpawn = distances[distances.Keys.Max()]; // Get farthest spawner.
        Instantiate(waveGoalPrefab, goalSpawn.position, goalSpawn.rotation);

        SetSpawnersPaused(false); // Unpause spawners.
    }

    void WinGame()
    {

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
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out master);

        // Get mute status.
        bool isMuted = false;
        master.getMute(out isMuted);

        master.setMute(!isMuted); // Invert mute status.

        print("Mute toggled.");
    }

    void ToggleSpawners()
    {
        foreach (Spawner spawner in enemySpawners)
        { // Invert paused status on every spawner.
            spawner.isPaused = !spawner.isPaused;
        }

        print("Toggled spawners.");
    }

    void SetSpawnersPaused(bool isPaused)
    {
        foreach (Spawner spawner in enemySpawners)
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
    public int maxMobAmount;
    public float mobMinSpawnTime;
    public float mobMaxSpawnTime;

    [Header("Elite")]
    public int maxEliteAmount;
    public float eliteMinSpawnTime;
    public float eliteMaxSpawnTime;
}
