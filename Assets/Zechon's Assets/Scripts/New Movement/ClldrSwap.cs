using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClldrSwap : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    CapsuleCollider clldr1;
    [SerializeField]
    CapsuleCollider clldr2;
    void Start()
    {
        clldr1.enabled = true;
        clldr2.enabled = false;
    }

    void Update()
    {
        
    }
}
