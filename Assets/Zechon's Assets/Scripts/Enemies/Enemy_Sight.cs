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
    public bool inSusRange;
    float unToAwareT = 1;
    [SerializeField]
    float awareToSusT = 3f;
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
        inSusRange = false;

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

                        var hitPoint = hit.point;
                        hitPoint.y = 0;
                        var EnemyPos = eyes.position;
                        EnemyPos.y = 0;

                        float distToP = Vector3.Distance(hitPoint, EnemyPos);
                        Debug.Log(distToP);

                        if (distToP <= (detectionRange / 2f))
                        {
                            inSusRange = true;
                        }

                        else
                            inSusRange = false;
                    }

                    else
                    {
                        seen = false;
                        inSusRange = false;
                    }
                }
            }
        }
    }

    private void AgressionStateHandler()
    {
        switch (eState)
        {
            case EnemyState.unaware:
                if (!timerSwapped)
                {
                    progressionTimer = 0;
                    timerSwapped = true;
                }

                progressionTimer += Time.deltaTime;
                progress = progressionTimer / unToAwareT;

                if (progressionTimer >= unToAwareT)
                {
                    eState = EnemyState.aware;
                    timerSwapped = false;
                    progress = 0;
                }
                break;

            case EnemyState.aware:
                if (inSusRange || (seen && PlyrMvmnt.mState == Player_Movement.MovementState.sprinting))
                {
                    if (!timerSwapped)
                    {
                        progressionTimer = 0;
                        timerSwapped = true;
                    }

                    progressionTimer += Time.deltaTime;
                    progress = progressionTimer / awareToSusT;

                    if (progressionTimer >= awareToSusT)
                    {
                        eState = EnemyState.suspicious;
                        timerSwapped = false;
                    }
                }
                break;
        }
    }

    private void StateDecayHandler()
    {

    }
}
