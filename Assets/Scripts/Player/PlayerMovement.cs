using UnityEngine;
using FMODUnity;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement instance;

    public float airAcceleration = 1;
    public float baseSpeed = 11;
    public float backstepModifier = 0.66f;
    public float jumpHeight = 10;
    public float doubleJump = 9;
    public float stoppingFriction = 12;
    public float maxSlope = 35;
    bool canDoublejump = true;

    Rigidbody rb;
    Vector3 lastGroundedVelocity;
    float x, z;
    bool isGrounded = false;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (value != isGrounded)
            {  // Value changed.
                if (value == true)
                {
                    RuntimeManager.PlayOneShot(playerLand, transform.position);
                    canDoublejump = true;
                } 
            }

            isGrounded = value;
        }
    }

    [Space(15)]

    [EventRef]
    public string playerFootsteps;
    [EventRef]
    public string playerDoublejump;
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

        playerFootstepEv.setParameterValue("Sprinting", 0);
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
                rb.drag = stoppingFriction;
                playerFootstepEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            { // Input.
                rb.velocity = new Vector3(dir.x * baseSpeed, rb.velocity.y, dir.z * baseSpeed);
                PlayFootstep();
            }
        }
        else
        { // Air movement.
            Vector3 planarVelocity = rb.velocity + dir * airAcceleration;
            planarVelocity.y = 0;
            Vector3 newVelocity = Vector3.ClampMagnitude(planarVelocity, baseSpeed);
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;

            playerFootstepEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void Update()
    {
        if (IsGrounded)
        { // Grounded movement.
            if (Input.GetButtonDown("Jump"))
            { // Jump.
                rb.velocity += transform.up * jumpHeight;
                RuntimeManager.PlayOneShot(playerJump);
            }
        }
        else
        { // Aerial movement.
            if (Input.GetButtonDown("Jump") && canDoublejump)
            { // Double jump.
                Vector3 dir = rb.velocity;
                dir.y = doubleJump;
                rb.velocity = dir;
                RuntimeManager.PlayOneShot(playerDoublejump);
                canDoublejump = false;
            }
        }
    }
}