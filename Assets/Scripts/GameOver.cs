using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameOver : MonoBehaviour {

    public static string deathSnapshot = "snapshot:/Death";
    public static FMOD.Studio.EventInstance deathSnapEv;

    public void Retry()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathSnapEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathSnapEv.release();

        SceneManager.LoadScene("Arena", LoadSceneMode.Single);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
