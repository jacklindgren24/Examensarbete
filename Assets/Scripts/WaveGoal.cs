using UnityEngine;
using FMODUnity;

public class WaveGoal : MonoBehaviour
{
    public float rotationSpeed = 8;

    [EventRef]
    public string waveCubeSpawn;
    FMOD.Studio.EventInstance waveCubeEv;
    [EventRef]
    public string waveCubePickup;

    void OnEnable()
    {
        waveCubeEv = RuntimeManager.CreateInstance(waveCubeSpawn);
        RuntimeManager.AttachInstanceToGameObject(waveCubeEv, transform, GetComponent<Rigidbody>());
        waveCubeEv.start();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.EndWave(false);
            Destroy(gameObject);
            ScoreScript.scoreValue += 100;

        }
    }

    void Update()
    {
        transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
    }

    void OnDisable()
    {
        if (!RuntimeManager.IsQuitting())
        {
            waveCubeEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            RuntimeManager.PlayOneShot(waveCubePickup, transform.position);
        }
    }
}