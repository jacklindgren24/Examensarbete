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
            ScoreCounter.Score += 100;
            RuntimeManager.PlayOneShot(waveCubePickup, transform.position);
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        if (!RuntimeManager.IsQuitting())
        {
            waveCubeEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            waveCubeEv.release();
        }
    }
}