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
    public float maxSlope = 35;

    Rigidbody rb;
    float height;
    Vector3 lastGroundedVelocity;
    float x, z;
    bool sprintToggle = false;
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

    float castRadius;
    float castDistance;

    const float shell = 0.02f;
    const float sphereCastRadiusShrink = 0.1f;

    void Awake()
    {
        if (instance == null) instance = this; else Destroy(gameObject);
    }

    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        castRadius = GetComponent<CapsuleCollider>().radius * transform.localScale.x - sphereCastRadiusShrink;
        castDistance = transform.localScale.y + shell - castRadius + sphereCastRadiusShrink;

        playerFootstepEv = RuntimeManager.CreateInstance(playerFootsteps);
    }

    void PlayFootstep()
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

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * z + transform.right * x;
        if (dir.magnitude > 1) dir /= dir.magnitude; // Limit diagonal movement.
        if (z < 0) dir.z *= backstepModifier; // Modify backwards movement.

        // Check if player is standing on something (exclude triggers).
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(transform.position, castRadius, -transform.up, out hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        { // Standing on surface.
            if (Vector3.Angle(transform.up, hit.normal) <= maxSlope)
            { // Standing on non-steep surface; account for surface normal.
                IsGrounded = true;
                dir = Vector3.ProjectOnPlane(dir, hit.normal);
            }
            else
            {  // Standing on steep surface; treat as not grounded.
                IsGrounded = false;
            }
        }
        else
        { // Not standing on surface.
            IsGrounded = false;
        }

        if (IsGrounded)
        { // Grounded movement.
            if (x == 0 && z == 0)
            { // No input.
                isSprinting = false;
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

    public void SetSprintToggle(bool state)
    {
        sprintToggle = state;
    }

    void Update()
    {
        if (IsGrounded)
        { // Grounded movement.
            if (sprintToggle)
            {
                if (Input.GetButtonDown("Sprint") && z > 0) isSprinting = !isSprinting;
            }
            else
            {
                if (Input.GetButton("Sprint") && z > 0) isSprinting = true;
                else isSprinting = false;
            }

            if (Input.GetButtonDown("Jump"))
            { // Jump.
                rb.velocity += transform.up * jumpHeight;
                RuntimeManager.PlayOneShot(playerJump);
            }
        }
    }
}