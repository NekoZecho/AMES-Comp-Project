using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    [SerializeField]
    Animator anim;
    [SerializeField]
    CapsuleCollider clldr;

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

    float horizInput;
    float vertInput;

    Vector3 moveDirection;

    Rigidbody rb;
    
    public MovementState state;
    public enum MovementState
    {
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
    }

    void Update()
    {
        Debug.Log(moveSpeed);
        Debug.Log(grounded);
        // is there ground below me???
        grounded = Physics.Raycast(transform.position, Vector3.down, plyrHeight * 0.5f + 0.2f, thisIsGround);

        InputTimeWahoo();
        DontSpeed();
        StateHandler();

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

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(JumpReset), jumpCD);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            clldr.height = startYScale * crouchYScale;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            clldr.height = startYScale;
        }
    }

    void PlayerMove()
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

        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    void StateHandler()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            anim.SetBool("Walking", false);
        }
        else if(grounded && !(Input.GetKey(crouchKey)))
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
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
}
