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
            if (health <= 0) Die();
        }
    }

    public bool godMode = false;

    void Die()
    {
        Vector3 camPos = Camera.main.transform.position;
        Quaternion camRot = Camera.main.transform.rotation;

        FMODUnity.RuntimeManager.PauseAllEvents(true);
        StartCoroutine(GameOver(camPos, camRot));
        Destroy(gameObject);

    }

    IEnumerator GameOver(Vector3 camPos, Quaternion camRot)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);

        yield return new WaitUntil(() => loading.isDone);

        print(camPos);
        print(camRot);
        Camera.main.transform.position = camPos;
        Camera.main.transform.rotation = camRot;
    }
}
