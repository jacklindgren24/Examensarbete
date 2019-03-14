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

    void OnEnable()
    {
        healthEv = RuntimeManager.CreateInstance(healthSpawn);
        RuntimeManager.AttachInstanceToGameObject(healthEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        healthEv.start();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            RuntimeManager.PlayOneShot(healthPickup, transform.position);

            other.GetComponent<PlayerController>().Health += heal;
            ScoreCounter.Score += 10;
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        if (!RuntimeManager.IsQuitting())
        {
            healthEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            healthEv.release();
        }
    }
}
