using UnityEngine;

public class Headbob : MonoBehaviour {

    public float bobbingSpeedBase = 0.18f;
    public float bobbingSpeedSprint = 0.288f;
    public float bobbingAmount = 0.2f;
    public float midpoint = 1;

    float timer = 0;

    PlayerMovement pm;

    void Start()
    {
        pm = PlayerMovement.instance;
    }

    void Update()
    {
        if (pm.IsGrounded)
        {
            float waveslice = 0;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 cSharpConversion = transform.localPosition;

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                timer = 0;
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer = pm.isSprinting ? timer + bobbingSpeedSprint : timer + bobbingSpeedBase;

                if (timer > Mathf.PI * 2)
                {
                    timer = timer - (Mathf.PI * 2);
                }
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                cSharpConversion.y = midpoint + translateChange;
            }
            else
            {
                cSharpConversion.y = midpoint;
            }

            transform.localPosition = cSharpConversion;
        }
    }
}