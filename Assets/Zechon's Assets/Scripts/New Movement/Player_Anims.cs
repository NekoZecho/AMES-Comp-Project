using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Anims : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Animator anim;
    public GameObject Player_Obj;
    private Player_Movement plyr;

    [Header("State Data")]
    public string moveRefState;

    void Start()
    {
        plyr = Player_Obj.GetComponent<Player_Movement>();
    }

    void Update()
    {
        MoveAnimUpdate();
    }

    private void MoveAnimUpdate()
    {
        switch (moveRefState)
        {
            case "idle":
                anim.SetBool("Walking", false);
                //anim.SetBool("Sprinting", false);
                anim.SetBool("Crouching", false);
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", false);

                if (anim.GetBool("Idle") == false)
                {
                    anim.SetBool("Idle", true);
                }
                break;
            

            case "walking":
                anim.SetBool("Idle", false);
                //anim.SetBool("Sprinting", false);
                anim.SetBool("Crouching", false);
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", false);

                if (anim.GetBool("Walking") == false)
                {
                    anim.SetBool("Walking", true);
                }
                break;

            case "crouching":
                anim.SetBool("Idle", false);
                //anim.SetBool("Sprinting", false);
                anim.SetBool("Walking", false);
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", false);

                if (anim.GetBool("Crouching") == false)
                {
                    anim.SetBool("Crouching", true);
                }
                break;

            case "crouchWalking":
                anim.SetBool("Idle", false);
                //anim.SetBool("Sprinting", false);
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", false);

                if (anim.GetBool("Crouching") == false || anim.GetBool("Walking") == false)
                {
                    anim.SetBool("Crouching", true);
                    anim.SetBool("Walking", true);
                }
                break;
        }
    }
}
