using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sight : MonoBehaviour
{
    [Header("Enemy Sight")]
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 110f;
    public Transform player;
    public Transform eyes;

    [Header("Detected")]
    public bool seen;
    [SerializeField]

    [Header("Enemy State")]
    public EnemyState eState;
    public enum EnemyState
    {
        
    }

    void Start()
    {
        seen = false;
    }

    void FixedUpdate()
    {
        Vector3 dirToPlayer = player.position -  eyes.position;
        float angle = Vector3.Angle(dirToPlayer, eyes.forward);
        Debug.DrawRay(eyes.position, dirToPlayer);

        if (angle < fieldOfViewAngle)
        {
            if (Vector3.Distance(eyes.position, player.position) < detectionRange)
            {
                RaycastHit hit;
                if (Physics.Raycast(eyes.position, dirToPlayer.normalized, out hit,detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        seen = true;
                    }
                }
            }
        }
    }

    void Update()
    {
        switch (seen)
        {
            case true:

                break;

            case false:
                break;
        }
    }
}
