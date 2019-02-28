using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WaveCube : MonoBehaviour
{
    [EventRef]
    public string waveCubeSpawn;
    FMOD.Studio.EventInstance waveCubeEv;
    [EventRef]
    public string waveCubePickup;

    void Start()
    {
            waveCubeEv = RuntimeManager.CreateInstance(waveCubeSpawn);
            RuntimeManager.AttachInstanceToGameObject(waveCubeEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
            CubeSpawner();
    }

    public void CubeSpawner()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        waveCubeEv.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) waveCubeEv.start();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            waveCubeEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Destroy(gameObject);
            RuntimeManager.PlayOneShot(waveCubePickup, transform.position);
        }
    }

    void Update()
    {
        transform.Rotate(0.0f, 0.8f, 0.0f);
    }
}