using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMENU : MonoBehaviour
{
    public float rotationX = 0f;           
    public float rotationY = 0f;           

    public float sensitivity = 15f;        
    public float smoothSpeed = 5f;        

    private float targetRotationX = 0f;    
    private float targetRotationY = 0f;    

    private void Update()
    {
       
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        
        targetRotationX += mouseY * sensitivity;
        targetRotationY += mouseX * 1 * sensitivity;

       
        rotationX = Mathf.Lerp(rotationX, targetRotationX, smoothSpeed * Time.deltaTime);
        rotationY = Mathf.Lerp(rotationY, targetRotationY, smoothSpeed * Time.deltaTime);

       
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
