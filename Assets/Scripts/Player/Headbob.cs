﻿using UnityEngine;

public class Headbob : MonoBehaviour {

    public float bobbingSpeedBase = 0.145f;
    public float bobbingAmountBase = 0.08f;
    public float midpoint = 1;

    float timer = 0;

    PlayerMovement pm;

    void Start()
    {
        pm = PlayerMovement.instance;
    }

    void Update()
    {
        if (pm.IsGrounded && !GameManager.instance.isPaused)
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
                timer = timer + bobbingSpeedBase * Time.deltaTime;

                if (timer > Mathf.PI * 2)
                {
                    timer = timer - (Mathf.PI * 2);
                }
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmountBase;

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