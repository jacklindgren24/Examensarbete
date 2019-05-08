using UnityEngine;

public class DisablerTrigger : MonoBehaviour {

    public float timeToDisable = 4;
    public float timeToEnable = 5;
    public float resetRate = 1;

    float timer = 0;
    bool active = false;

    MeshRenderer mr;
    BoxCollider[] colliders;

	void Start()
    {
        mr = GetComponent<MeshRenderer>();
        colliders = GetComponents<BoxCollider>();
	}

    void Update()
    {
        if (!active)
            timer -= resetRate * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        active = true;
    }

    void OnTriggerExit(Collider other)
    {
        active = false;
    }

    void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;

        if (timer >= timeToDisable)
        {
            timer = 0;
            Disable();
            Invoke("Enable", timeToEnable);
        }
    }

    void Enable()
    {
        mr.enabled = true;
        foreach (BoxCollider coll in colliders)
        {
            coll.enabled = true;
        }
    }

    void Disable()
    {
        active = false;
        mr.enabled = false;
        foreach (BoxCollider coll in colliders)
        {
            coll.enabled = false;
        }
    }
}
