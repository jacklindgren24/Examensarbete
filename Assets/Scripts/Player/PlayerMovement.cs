using UnityEngine;
using FMODUnity;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement instance;

    public float airAcceleration = 6;
    public float baseSpeed = 5;
    public float sprintSpeed = 8;
    public float backstepModifier = 0.66f;
    public float jumpHeight = 5;
    public float stoppingFriction = 12;

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

    [Space(15)]

    [EventRef]
    public string playerFootsteps;
    [EventRef]
    public string playerJump;
    [EventRef]
    public string playerLand;

    FMOD.Studio.EventInstance playerFootstepEv;

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

        // Check if player is standing on something (exclude triggers).
        IsGrounded = Physics.Raycast(transform.position, -transform.up, height + shell, ~0, QueryTriggerInteraction.Ignore);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * z + transform.right * x;
        if (dir.magnitude > 1) dir /= dir.magnitude; // Limit diagonal movement.
        if (z < 0) dir.z *= backstepModifier; // Modify backwards movement.

        if (IsGrounded)
        { // Grounded movement.
            if (Input.GetButton("Sprint") && z > 0) isSprinting = true;
            else isSprinting = false;

            if (x == 0 && z == 0)
            { // No input.
                rb.drag = stoppingFriction;
                playerFootstepEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            { // Input.
                if (isSprinting) rb.velocity = new Vector3(dir.x * sprintSpeed, rb.velocity.y, dir.z * sprintSpeed);
                else rb.velocity = new Vector3(dir.x * baseSpeed, rb.velocity.y, dir.z * baseSpeed);
                PlayFootstep();
            }
        }
        else
        { // Air movement.
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
            rb.velocity += transform.up * jumpHeight;
            RuntimeManager.PlayOneShot(playerJump);
        }
    }
}