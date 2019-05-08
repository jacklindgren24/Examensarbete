using UnityEngine;

public class DisablerTrigger : MonoBehaviour {

    public float timeToDisable = 4;
    public float timeToEnable = 5;

    float timer = 0;
    bool active = false;

    MeshRenderer mr;
    BoxCollider[] colliders;

    const float resetRate = 0.25f;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        colliders = GetComponents<BoxCollider>();
	}

    void Update()
    {
        if (!active)
            timer = Mathf.Clamp(timer - resetRate * Time.deltaTime, 0, float.PositiveInfinity);

        Color materialColor = mr.material.color;
        materialColor.a = 1 - timer / timeToDisable;
        mr.material.color = materialColor;
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
        timer = 0;
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
