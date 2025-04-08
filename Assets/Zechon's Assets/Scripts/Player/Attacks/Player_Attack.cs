using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [Header("Components")]
    Player_Attack_Animation AtkAnimRef;
    GameObject HitboxObj;
    BoxCollider clldr;

    void Start()
    {
        AtkAnimRef = GetComponent<Player_Attack_Animation>();
        HitboxObj = GameObject.Find("R_Arm_Hitbox");
        clldr = HitboxObj.GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (AtkAnimRef.AttackFrames)
        {
            clldr.enabled = true;
        }
        else
        {
            clldr.enabled = false;
        }
    }

    private void
}
