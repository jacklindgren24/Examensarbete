using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public int health = 100;
    public int Health
    {
        get { return health; }
        set
        {
            health = godMode ? 100 : value;
            if (health <= 0) StartCoroutine(GameOver(Camera.main.transform.position, Camera.main.transform.rotation));
        }
    }

    public bool godMode = false;

    IEnumerator GameOver(Vector3 camPos, Quaternion camRot)
    {
        Destroy(transform.GetChild(0).gameObject);

        FMODUnity.RuntimeManager.PauseAllEvents(true);

        AsyncOperation loading = SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loading.isDone);

        Camera.main.transform.position = camPos;
        Camera.main.transform.rotation = camRot;

        Destroy(gameObject);
    }
}
