using UnityEngine;
using FMODUnity;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement instance;

    public float baseAcceleration = 30;
    public float airAcceleration = 6;
    public float baseMaxSpeed = 5;
    public float sprintMaxSpeed = 8;
    public float jumpHeight = 5;
    public float stoppingFriction = 12;

    [EventRef]
    public string playerFootsteps;
    FMOD.Studio.EventInstance playerFootstepEv;
    [EventRef]
    public string playerJump;
    [EventRef]
    public string playerLand;

    Rigidbody rb;
    float height;
    Vector3 lastGroundedVelocity;
    [HideInInspector]
    public bool isSprinting = false;
    bool isGrounded = false;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (value != isGrounded)
            { 
                if (value == false)
                { // Record velocity at the point where player is no longer grounded.
                    lastGroundedVelocity = rb.velocity;
                    lastGroundedVelocity.x = Mathf.Abs(lastGroundedVelocity.x);
                    lastGroundedVelocity.z = Mathf.Abs(lastGroundedVelocity.z);
                }
                else
                { // Play landing sound on landing.
                    RuntimeManager.PlayOneShot(playerLand, transform.position);
                }
            }

            isGrounded = value;
        }
    }

    const float shell = 0.02f;

    void Awake()
    {
        if (instance == null) instance = this; else Destroy(gameObject);
    }

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        height = transform.localScale.y;

        playerFootstepEv = RuntimeManager.CreateInstance(playerFootsteps);
    }

    public void PlayFootstep()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        playerFootstepEv.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) playerFootstepEv.start();

        if (isSprinting) playerFootstepEv.setParameterValue("Sprinting", 1);
        else playerFootstepEv.setParameterValue("Sprinting", 0);
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
            if (Input.GetButton("Sprint")) isSprinting = true; else isSprinting = false;

            if (x == 0 && z == 0)
            { // No input.
                rb.drag = stoppingFriction;
                playerFootstepEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            { // Input.
                rb.AddForce(dir * baseAcceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
                PlayFootstep();
            }

            // Limit speed to max speed.
            float ms = isSprinting ? sprintMaxSpeed : baseMaxSpeed;
            if (rb.velocity.magnitude > ms) rb.velocity = rb.velocity.normalized * ms;
        }
        else
        {
            rb.AddForce(dir * airAcceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -lastGroundedVelocity.x, lastGroundedVelocity.x), 
              rb.velocity.y, Mathf.Clamp(rb.velocity.z, -lastGroundedVelocity.z, lastGroundedVelocity.z));

            playerFootstepEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void Update()
    {
        if (IsGrounded && Input.GetButtonDown("Jump"))
        { // Jump.
            rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
            RuntimeManager.PlayOneShot(playerJump, transform.position);
        }
    }
}