using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("Refs")]
    public Transform orientation;
    public CapsuleCollider plyrClldr;
    private Rigidbody rb;
    private Player_Movement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideT;

    public float slideScaleY;
    private float startScaleY;

    [Header("Keybinds")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float hInput;
    private float vInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Player_Movement>();

        startScaleY = plyrClldr.height;
    }

    private void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (hInput != 0 || vInput != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && pm.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * vInput + orientation.right * hInput;

        //that regular typa slidin
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideT -= Time.deltaTime;
        }

        //sliding down da slopes
        else
        {
            rb.AddForce(pm.GetSlopeMDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideT <= 0)
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true;

        plyrClldr.height = startScaleY * slideScaleY;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideT = maxSlideTime;
    }

    private void StopSlide()
    {
        pm.sliding = false;

        plyrClldr.height = startScaleY;
    }
}
