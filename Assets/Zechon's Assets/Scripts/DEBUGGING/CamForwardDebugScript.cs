using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamForwardDebugScript : MonoBehaviour
{
    RaycastHit hit;

    void Update()
    {
        int plyrLyr = LayerMask.NameToLayer("Player");
        int layermask = ~(1 << plyrLyr);

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, layermask))
        {
            Debug.DrawLine(transform.position, hit.transform.position, Color.blue);
        }
    }
}
