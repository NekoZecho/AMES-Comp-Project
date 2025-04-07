using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [Header("Components")]
    Player_Attack_Animation AtkAnimRef;

    void Start()
    {
        AtkAnimRef = GetComponent<Player_Attack_Animation>();
    }

    void Update()
    {
        
    }
}
