using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Ground Check")]
    public float plyrHeight;
    public LayerMask thisIsGround;
    bool grounded;

    float horizInput;
    float vertInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // is there ground below me???
        grounded = Physics.Raycast(transform.position, Vector3.down, plyrHeight * 0.5f + 0.2f, thisIsGround);

        InputTimeWahoo();

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
    }

    void PlayerMove()
    {
        //move calc dir
        moveDirection = orientation.forward * vertInput + orientation.right * horizInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    void DontSpeed()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
