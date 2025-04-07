using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [Header("Components")]
    Animator anim;

    [Header("Attack Check")]
    public bool AttackFrames;

    [Header("Keybinds")]
    public KeyCode Attack = KeyCode.Mouse0;

    void Start()
    {
        AttackFrames = false;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(Attack))
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }

    }

    public void CheckAttackToggle()
    {
        switch (AttackFrames)
        {
            case false:
                AttackFrames = true;
                break;

            case true:
                AttackFrames = false;
                break;
        }
    }
}
