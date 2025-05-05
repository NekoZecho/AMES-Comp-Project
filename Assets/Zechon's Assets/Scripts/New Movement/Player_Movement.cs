using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    [SerializeField]
    CapsuleCollider clldr;
    [SerializeField]
    GameObject plyr_Obj;
    Rigidbody rb;
    private Player_Anims anim; 

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Movement")]
    public bool sliding;
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float plyrHeight;
    public LayerMask thisIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Climbing")]
    public bool freeze;
    public bool unlimited;
    public bool restricted;

    float horizInput;
    float vertInput;

    Vector3 moveDirection;


    [Header("State")]
    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = clldr.height;

        anim = plyr_Obj.GetComponent<Player_Anims>();
    }

    void Update()
    {
        // is there ground below me???
        grounded = Physics.Raycast(transform.position, Vector3.down, plyrHeight * 0.5f + 0.2f, thisIsGround);

        InputTimeWahoo();
        DontSpeed();
        StateHandler();
        AnimStringHandler();

        //this ground be havin friction (drag)
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
        PlayerMove();
    }

    void InputTimeWahoo()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        //if (Input.GetKey(jumpKey) && readyToJump && grounded)
        //{
        //    readyToJump = false;
        //    Jump();
//
        //    Invoke(nameof(JumpReset), jumpCD);
        //}
    }

    void PlayerMove()
    {
        if (!restricted)
        {
            //move calc dir
            moveDirection = orientation.forward * vertInput + orientation.right * horizInput;

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 10f, ForceMode.Force);
                }
            }

            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }

            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

            rb.useGravity = !OnSlope();
        }

        else
        {
            return;
        }
    }

    void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
        }
        else if (unlimited)
        {
            state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        }
        else if (Input.GetKey(crouchKey) && grounded)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey) && !(Input.GetKey(crouchKey)))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            anim.moveRefState = "sprinting";
        }
        else if (grounded && !(Input.GetKey(crouchKey)))
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            if (vertInput != 0 || horizInput != 0)
            {
                anim.moveRefState = "walking";
            }
            else
            {
                anim.moveRefState = "idle";
            }
        }
        else
        {
            state = MovementState.air;
        }
    }

    void DontSpeed()
    {
        //speeding on slopes aint allowed, so we doing this bit too
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

            //if speeding (very scary) do this stuff
            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    void Jump()
    {
        exitingSlope = true;

        //reset yo verticality speed
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        anim.moveRefState = "Jump";

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void JumpReset()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, plyrHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void AnimStringHandler()
    {
        if (state == MovementState.crouching)
        {
            if (vertInput != 0 || horizInput != 0)
            {
                anim.moveRefState = "crouchWalking";
            }
            else
            {
                anim.moveRefState = "crouching";
            }
        }
    }
}
