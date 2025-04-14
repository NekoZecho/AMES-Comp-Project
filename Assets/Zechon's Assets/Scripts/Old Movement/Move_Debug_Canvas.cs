using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Move_Debug_Canvas : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    GameObject plyr;
    Rigidbody rb;
    Player_Movement plyr_mvmnt;

    [Header("Data")]
    [SerializeField]
    TMP_Text Speed;
    [SerializeField]
    TMP_Text Slope;
    [SerializeField]
    TMP_Text MState;

    void Start()
    {
        rb = plyr.GetComponent<Rigidbody>();
        plyr_mvmnt = plyr.GetComponent<Player_Movement>();
    }

    
    void Update()
    {
        Speed.text = "Speed: " + rb.velocity.magnitude.ToString("F2");
        Slope.text = "Slope: " + plyr_mvmnt.dAngle.ToString("F1");
        switch (plyr_mvmnt.mState)
        {
            case Player_Movement.MovementState.walking:
                MState.text = "Walking";
                break;

            case Player_Movement.MovementState.sprinting:
                MState.text = "Sprinting";
                break;

            case Player_Movement.MovementState.crouching:
                MState.text = "Crouching";
                break;

            case Player_Movement.MovementState.air:
                MState.text = "Jumping";
                break;
        }
    }
}
