using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HealthPickup : MonoBehaviour
{
    public int heal = 50;

    [EventRef]
    public string healthSpawn;
    FMOD.Studio.EventInstance healthEv;
    [EventRef]
    public string healthPickup;

    void Start()
    {
        healthEv = RuntimeManager.CreateInstance(healthSpawn);
        RuntimeManager.AttachInstanceToGameObject(healthEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        HealthSpawner();
    }

    public void HealthSpawner()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        healthEv.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) healthEv.start();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().Health += heal;
            healthEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Destroy(gameObject);
            RuntimeManager.PlayOneShot(healthPickup, transform.position);
        }
    }
}
