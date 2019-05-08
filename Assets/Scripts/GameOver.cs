using UnityEngine;

public class GameOver : MonoBehaviour {

    public static string deathSnapshot = "snapshot:/Death";
    public static FMOD.Studio.EventInstance deathSnapEv;

    public void RetryFromWaveStart()
    {
        GameManager.instance.Restart(false);
    }

    public void RetryFromGameStart()
    {
        GameManager.instance.Restart(true);
    }
}
