using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float baseSpeed = 6;
    public float sprintSpeed = 10;
    public float jumpHeight = 2;

    const float shell = 0.02f;

    Rigidbody rb;
    float height;

	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        height = transform.localScale.y;
	}
	
	void FixedUpdate ()
    {
        bool isGrounded = Physics.Raycast(transform.position, -transform.up, height + shell);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetButton("Sprint"))
        { // Sprinting.
            rb.MovePosition(rb.position + transform.localRotation * new Vector3(x, 0, z) * sprintSpeed * Time.fixedDeltaTime);
        }
        else
        { // Normal.
            rb.MovePosition(rb.position + transform.localRotation * new Vector3(x, 0, z) * baseSpeed * Time.fixedDeltaTime);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        { // Jump.
            rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
        }
    }
}
