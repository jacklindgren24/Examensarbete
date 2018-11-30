using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float baseAcceleration = 6;
    public float sprintAcceleration = 1.5f;
    public float maxBaseSpeed = 8;
    public float maxSprintSpeed = 12;
    public float stopping = 10;

    Rigidbody rb;

	void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.drag = 0;

        if (Input.GetButton("Sprint"))
        { // Sprinting.
            rb.AddForce(new Vector3(x, 0, z) * sprintAcceleration, ForceMode.Impulse);
            rb.velocity = new Vector3
             (Mathf.Clamp(rb.velocity.x, -maxSprintSpeed, maxSprintSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -maxSprintSpeed, maxSprintSpeed));
        }
        else
        { // Not sprinting.
            rb.AddForce(new Vector3(x, 0, z) * baseAcceleration, ForceMode.Impulse);
            rb.velocity = new Vector3
             (Mathf.Clamp(rb.velocity.x, -maxBaseSpeed, maxBaseSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -maxBaseSpeed, maxBaseSpeed));
        }

        

        if (x == 0 && z == 0)
        { // Increase drag when not moving.
            rb.drag = stopping;
        }
    }
}
