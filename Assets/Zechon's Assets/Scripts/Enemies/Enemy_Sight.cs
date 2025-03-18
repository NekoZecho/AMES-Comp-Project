using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sight : MonoBehaviour
{
    GameObject plyr;
    Transform target;

    void Start()
    {
        plyr = GameObject.FindGameObjectWithTag("Player");
        target = plyr.transform;
    }

    void Update()
    {
        Vector3 targetDir = target.position - transform.position;
        Debug.DrawRay(transform.position, targetDir, Color.red);
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (angle < 5f)
            Debug.Log("Close");
    }
}
