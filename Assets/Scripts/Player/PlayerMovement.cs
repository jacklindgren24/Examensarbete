using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float baseAcceleration = 30;
    public float airAcceleration = 6;
    public float baseMaxSpeed = 5;
    public float sprintMaxSpeed = 8;
    public float jumpHeight = 5;
    public float stoppingFriction = 12;

    [FMODUnity.EventRef]
    public string playerJump; //player jump sound

    [FMODUnity.EventRef]
    public string playerFootstepsEv;
    FMOD.Studio.EventInstance playerFootstep;

    const float shell = 0.02f;

    Rigidbody rb;
    float height;
    Vector3 lastGroundedVelocity;
    bool isGrounded = false;
    bool IsGrounded
    {
        get { return isGrounded; }
        set
        { // Record velocity at the point where player is no longer grounded.
            if (value != isGrounded)
            {
                lastGroundedVelocity = rb.velocity;
                lastGroundedVelocity.x = Mathf.Abs(lastGroundedVelocity.x);
                lastGroundedVelocity.z = Mathf.Abs(lastGroundedVelocity.z);
            }

            isGrounded = value;
        }
    }

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        height = transform.localScale.y;
        playerFootstep = FMODUnity.RuntimeManager.CreateInstance(playerFootstepsEv);
    }

    public void PlayFootstep()
    {
        playerFootstep.start();
    }

        void FixedUpdate ()
    {
        rb.drag = 0;
        IsGrounded = Physics.Raycast(transform.position, -transform.up, height + shell); // Check if player is standing on something.

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * z + transform.right * x;

        if (IsGrounded)
        {
            rb.AddForce(dir * baseAcceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);

            float ms = Input.GetButton("Sprint") ? sprintMaxSpeed : baseMaxSpeed;
            if (rb.velocity.magnitude > ms) rb.velocity = rb.velocity.normalized * ms;

            if (x == 0 && z == 0) rb.drag = stoppingFriction;
        }
        else
        {
            rb.AddForce(dir * airAcceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.velocity = new Vector3
              (Mathf.Clamp(rb.velocity.x, -lastGroundedVelocity.x, lastGroundedVelocity.x), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -lastGroundedVelocity.z, lastGroundedVelocity.z));
        }
    }

    void Update()
    {
        if (IsGrounded && Input.GetButtonDown("Jump"))
        { // Jump.
            rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
            FMODUnity.RuntimeManager.PlayOneShot(playerJump, transform.position);
        }
    }
}