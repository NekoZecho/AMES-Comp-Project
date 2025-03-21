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
    float susToAwareT = 2f;
    bool susToAwareB;
    [SerializeField]
    float awareToAngyT = 1f;
    [SerializeField]
    float undetectTimerSus = 4f;
    [SerializeField]
    float undetectTimerAwa = 10f;
    [SerializeField]
    float undetectTimerAgg = 20f;
    GameObject Player;
    Player_Movement PlyrMvmnt;

    [Header("Enemy State")]
    public EnemyState eState;
    public enum EnemyState
    {
        unaware,
        suspicious,
        aware,
        aggressive
    }

    void Start()
    {
        seen = false;
        susToAwareB = false;

        Player = GameObject.FindGameObjectWithTag("Player");
        PlyrMvmnt = Player.GetComponent<Player_Movement>();
    }

    void Update()
    {
        eStateHandler();
    }

    void FixedUpdate()
    {
        Vector3 dirToPlayer = player.position - eyes.position;
        float angle = Vector3.Angle(dirToPlayer, eyes.forward);
        Debug.DrawRay(eyes.position, dirToPlayer);

        if (angle < fieldOfViewAngle)
        {
            if (Vector3.Distance(eyes.position, player.position) < detectionRange)
            {
                RaycastHit hit;
                if (Physics.Raycast(eyes.position, dirToPlayer.normalized, out hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player_Body"))
                    {
                        seen = true;
                    }
                }
            }
        }
        else
            seen = false;
    }

    private void eStateHandler()
    {
        if (!seen)
            eState = EnemyState.unaware;
        else if (seen)
        {
            if (eState == EnemyState.unaware && PlyrMvmnt.mState == Player_Movement.MovementState.sprinting)
            {
                susToAwareB = true;
                Debug.Log("susToAwareB");
            }
        }
    }
}
