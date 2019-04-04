using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ProximityTracker : MonoBehaviour
{
    public float angle = 180;
    public float range = 20;

    [EventRef]
    public string eventPath;

    int layerMask;
    Dictionary<int, EventInstance> events = new Dictionary<int, EventInstance>();

    void Start()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        List<int> presentIDs = new List<int>();
        Collider[] results = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach (Collider result in results)
        {
            Vector3 dir = result.transform.position - transform.position;
            if (Vector3.Angle(dir, -transform.forward) < angle)
            { // Check if enemy is within specified angle.
                if (events.Count < results.Length)
                { // Add event instance if there are not enough instances for every enemy.
                    EventInstance evInstance = RuntimeManager.CreateInstance(eventPath);
                    evInstance.start();
                    events.Add(result.GetInstanceID(), evInstance);
                }

                presentIDs.Add(result.GetInstanceID());
            }
        }

        foreach (int key in events.Keys)
        {
            if (!presentIDs.Contains(key))
            { // This event is connected to an enemy which is not present in proximity check; release event.
                events[key].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                events[key].release();
                events.Remove(key);
            }
        }
	}
}
