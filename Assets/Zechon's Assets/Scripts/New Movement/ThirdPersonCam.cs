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
    public Transform combatLookAt;
    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject climbingCam;
    private LedgeGrabbing ledge;
    public GameObject plyr;

    [Header("Floats and Quaternions")]
    public float rotateSpeed;
    public float rotationSpeed;
    public float rotationDur;
    float horizontalInput;
    float verticalInput;

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Climbing
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraStyle(CameraStyle.Basic);
        ledge = plyr.GetComponent<LedgeGrabbing>();
    }

    void Update()
    {
        if (ledge.holding)
        {
            //SwitchCameraStyle(CameraStyle.Climbing);
        }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                SwitchCameraStyle(CameraStyle.Basic);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SwitchCameraStyle(CameraStyle.Combat);
            }

        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 camForward = transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            if (camForward.sqrMagnitude > 0.001f)
            {
                // Create target rotation and smoothly rotate toward it
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Debug.Log($"Rotating to: {targetRotation}");
            }
        }

        if (currentStyle == CameraStyle.Basic)
        {
            //You spin me right round, baby right round
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            
            if (inputDir != Vector3.zero)
            {
                plyrObj.forward = Vector3.Slerp(plyrObj.forward, inputDir.normalized, Time.deltaTime * rotateSpeed);
            }
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

           plyrObj.forward = dirToCombatLookAt.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        climbingCam.SetActive(false);

        if (newStyle == CameraStyle.Basic)
        {
            thirdPersonCam.SetActive(true);
        }
        if (newStyle == CameraStyle.Combat)
        {
            combatCam.SetActive(true);
        }
        if (newStyle == CameraStyle.Climbing)
        {
            climbingCam.SetActive(true);
        }

        currentStyle = newStyle;
    }
}
