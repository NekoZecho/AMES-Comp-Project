using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Billboarding : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + camTransform.forward);
    }
}
