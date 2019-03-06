using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameOver : MonoBehaviour {

    public static string deathSnapshot = "snapshot:/Death";
    public static FMOD.Studio.EventInstance deathSnapEv;

    public void Retry()
    {
        ScoreCounter.Score = 0;

        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathSnapEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathSnapEv.release();

        foreach (GameObject enemy in EnemyController.enemies)
        {
            enemy.GetComponent<EnemyController>().enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            enemy.GetComponent<EnemyController>().enemyFootstepsEv.release();
        }

        EnemyController.enemies.Clear();

        SceneManager.LoadScene("Arena", LoadSceneMode.Single);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
