using UnityEngine;

public class GameOver : MonoBehaviour {

    public static string deathSnapshot = "snapshot:/Death";
    public static FMOD.Studio.EventInstance deathSnapEv;

    public void Retry()
    {
        GameManager.instance.Restart();
    }
}
