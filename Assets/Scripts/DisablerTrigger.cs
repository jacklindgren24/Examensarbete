using UnityEngine;

public class DisablerTrigger : MonoBehaviour {

    public float timeToDisable = 4;
    public float timeToEnable = 5;

    float timer = 0;

    MeshRenderer mr;
    BoxCollider[] colliders;

	void Start()
    {
        mr = GetComponent<MeshRenderer>();
        colliders = GetComponents<BoxCollider>();
	}
	
    void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;

        if (timer >= timeToDisable)
        {
            timer = 0;

            mr.enabled = false;
            foreach (BoxCollider coll in colliders)
            {
                coll.enabled = false;
            }

            Invoke("Enable", timeToEnable);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        timer = 0;
    }

    void Enable()
    {
        mr.enabled = true;
        foreach (BoxCollider coll in colliders)
        {
            coll.enabled = true;
        }
    }
}
