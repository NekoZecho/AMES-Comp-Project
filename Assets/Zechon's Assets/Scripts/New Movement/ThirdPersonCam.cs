using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Refs")]
    public Transform orientation;
    public Transform player;
    public Transform plyrObj;
    public Rigidbody rb;

    [Header("Floats")]
    public float rotateSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //You spin me right round, baby right round
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            plyrObj.forward = Vector3.Slerp(plyrObj.forward, inputDir.normalized, Time.deltaTime * rotateSpeed);
        }
    }
}
