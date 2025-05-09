using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;

public class Old_Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool jumpReady;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchScaleY;
    private float startScaleY;
    private bool canUncrouch;
    private bool crouching;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground & Wall Check")]
    public float playerHeight;
    public LayerMask theEnvironment;
    bool grounded;

    [Header("Collide And Slide")]
    [SerializeField]
    int maxBounces = 5;
    [SerializeField]
    float skinThickness = .015f;
    Bounds bounds;
    [SerializeField]
    GameObject Player_Collider;

    [Header("Slope Handling")]
    public float slopeAngleMax;
    private RaycastHit slopehit;
    private bool exitingSlope;
    [HideInInspector]
    public float dAngle;


    [Header("Orientation")]
    public Transform orientation;

    float horizInput;
    float vertInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Movement State")]
    public MovementState mState;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        jumpReady = true;

        startScaleY = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, theEnvironment);

        bounds = Player_Collider.GetComponent<Collider>().bounds;
        bounds.Expand(-2 * skinThickness);

        MyInput();
        ControlSpeed();
        MStateHandler();
        uncrouchCheck();


        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && jumpReady && grounded)
        {
            jumpReady = false;

            Jump();

            Invoke(nameof(JumpReset), jumpCD);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchScaleY, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouching = true;
        }

        if (!Input.GetKey(crouchKey) && canUncrouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, startScaleY, transform.localScale.z);
            crouching = false;
        }
    }

    private void MStateHandler()
    {
        if (Input.GetKey(crouchKey) || crouching)
        {
            mState = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        else if (grounded && Input.GetKey(sprintKey))
        {
            mState = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        else if (grounded && !crouching)
        {
            mState = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
        {
            mState = MovementState.air;        }
        
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * vertInput + orientation.right * horizInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
            rb.AddForce(moveDirection.normalized *  moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();

        Debug.DrawRay(transform.position, moveDirection, Color.yellow);
    }

    private void ControlSpeed()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }           
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        } 
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        jumpReady = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopehit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            dAngle = angle;
            return angle < slopeAngleMax && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopehit.normal).normalized;
    }

    private void uncrouchCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.3f))
            canUncrouch = false;
        
        else
            canUncrouch = true;
    }

    private Vector3 CollideSlide(Vector3 vel, Vector3 pos, int depth)
    {
        if (depth >= maxBounces)
            return Vector3.zero;

        float dist = vel.magnitude + skinThickness;

        RaycastHit hit;
        if (Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, theEnvironment))
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinThickness);
            Vector3 leftover = vel - snapToSurface;

            if (snapToSurface.magnitude <= skinThickness)
            {
                snapToSurface = Vector3.zero;
            }

            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= mag;

            return snapToSurface + CollideSlide(leftover, pos + snapToSurface, depth + 1);
        }

        return vel;
    }
}
