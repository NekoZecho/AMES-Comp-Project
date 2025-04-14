using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Third_Person_Movement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController cont;
    public Transform cam;

    [Header("Floats")]
    public float speed = 6f;
    public float smoothTurnT = 0.1f;
    float smoothTurnV;

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetedAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetedAngle, ref smoothTurnV, smoothTurnT);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetedAngle, 0f) * Vector3.forward;
            cont.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
