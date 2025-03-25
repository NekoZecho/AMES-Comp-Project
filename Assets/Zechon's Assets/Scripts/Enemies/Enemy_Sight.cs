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
    float unToSusT = 1;
    [SerializeField]
    float susToAwareT = 3f;
    [SerializeField]
    float awareToAngyT = 2f;
    [SerializeField]
    float undetectTimerSus = 4f;
    [SerializeField]
    float undetectTimerAwa = 10f;
    [SerializeField]
    float undetectTimerAgg = 20f;
    float decayTimer = 0f;
    float progressionTimer = 0f;
    public float progress;
    GameObject Player;
    Player_Movement PlyrMvmnt;
    bool timerSwapped;

    [Header("Enemy State")]
    public EnemyState eState;
    public enum EnemyState
    {
        unaware,
        aware,
        suspicious,
        aggressive
    }

    void Start()
    {
        seen = false;

        eState = EnemyState.unaware;

        Player = GameObject.FindGameObjectWithTag("Player");
        PlyrMvmnt = Player.GetComponent<Player_Movement>();

        progress = 0;

        timerSwapped = false;
    }

    void Update()
    {
        switch (seen)
        {
            case true:
                AgressionStateHandler();
                break;

            case false:
                StateDecayHandler();
                break;
        }
    }

    void FixedUpdate()
    {
        Vector3 dirToPlayer = player.position - eyes.position;
        float angle = Vector3.Angle(dirToPlayer, eyes.forward);
        Debug.DrawRay(eyes.position, dirToPlayer);

        if (angle < fieldOfViewAngle / 2)
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
                    seen = false;
                }
            }
        }
    }

    private void AgressionStateHandler()
    {
        switch (eState)
        {
            case EnemyState.unaware:

                break;

            case EnemyState.aware:

                break;
        }
    }

    private void StateDecayHandler()
    {

    }
}
