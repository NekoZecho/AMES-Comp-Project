using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Enemy_Sight : MonoBehaviour
{
    [Header("Enemy Sight")]
    public float detectionRange = 10f;
    public float susRange = 3f;
    public float fieldOfViewAngle = 110f;
    public Transform player;
    public Transform eyes;

    [Header("Detected")]
    public bool seen;
    public bool inSusRange;
    [SerializeField]
    float unToAwareT = 2f;
    [SerializeField]
    float awareToSusT = 3f;
    [SerializeField]
    float susToAngyT = 2f;
    GameObject Player;
    Player_Movement PlyrMvmnt;
    bool timerSwapped;
    bool dTimerSwapped;
    public bool fullyDecayed;

    [Header("Progress")]
    public float decayTimer = 0f;
    public float awareProgress;
    float awarePT;
    public float susProgress;
    float susPT;
    public float aggroProgress;
    float aggroPT;

    [Header("Enemy State")]
    public EnemyState eState;
    public enum EnemyState
    {
        unaware,
        aware,
        suspicious,
        aggressive
    }
    public int EnemyMode;

    void Start()
    {
        seen = false;
        inSusRange = false;

        eState = EnemyState.unaware;

        Player = GameObject.FindGameObjectWithTag("Player");
        PlyrMvmnt = Player.GetComponent<Player_Movement>();

        awareProgress = 0;
        susProgress = 0;
        aggroProgress = 0;

        awarePT = 0;
        susPT = 0;
        aggroPT = 0;

        timerSwapped = false;
        dTimerSwapped = false;

        EnemyMode = 0;
}

    void Update()
    {
        switch (seen)
        {
            case true:
                if (awarePT != unToAwareT)
                {
                    eState = EnemyState.unaware;

                    awarePT += Time.deltaTime;
                    awareProgress = awarePT / unToAwareT;

                    if (awarePT > unToAwareT)
                    {
                        awarePT = unToAwareT;
                        awareProgress = 1;
                    }
                }

                else if (awarePT == unToAwareT && susPT != awareToSusT)
                {
                    eState = EnemyState.aware;

                    if (inSusRange)
                    {
                    susPT += Time.deltaTime;
                    }

                    susProgress = susPT / awareToSusT;

                    if (susPT > awareToSusT)
                    {
                        susPT = awareToSusT;
                        susProgress = 1;
                    }
                }

                else if (awarePT == unToAwareT && susPT == awareToSusT && aggroPT != susToAngyT)
                {
                    eState = EnemyState.suspicious;

                    if (inSusRange)
                    {
                        aggroPT += Time.deltaTime;
                    }

                    aggroProgress = aggroPT / susToAngyT;

                    if (aggroPT > susToAngyT)
                    {
                        aggroPT = susToAngyT;
                        aggroProgress = 1;
                    }
                }

                else if (aggroPT == susToAngyT)
                {
                   eState &= ~EnemyState.aggressive;
                   aggroProgress = 1;
                }
                break;

            case false:
                if (aggroPT != 0)
                {
                    eState = EnemyState.aggressive;

                    aggroPT -= (Time.deltaTime / 1.5f);
                    aggroProgress = aggroPT / susToAngyT;

                    if (aggroPT < 0)
                    {
                        aggroPT = 0;
                        aggroProgress = 0;
                    }
                }

                else if (aggroPT == 0 && susPT != 0)
                {
                    eState = EnemyState.suspicious;

                    susPT -= (Time.deltaTime / 1.5f);
                    susProgress = susPT / awareToSusT;

                    if (susPT < 0)
                    {
                        susPT = 0;
                        susProgress = 0;
                    }

                }

                else if (aggroPT == 0 && susPT == 0 && awarePT != 0)
                {
                    eState |= EnemyState.aware;

                    awarePT -= (Time.deltaTime / 1.5f);
                    awareProgress = awarePT / unToAwareT;

                    if (awarePT < 0)
                    {
                        awarePT = 0;
                        awareProgress = 0;
                    }
                }

                else if (awarePT == 0)
                {
                    eState = EnemyState.unaware;
                    awareProgress = 0;
                }
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

                        if (distToP <= susRange)
                        {
                            inSusRange = true;
                        }

                        else
                            inSusRange = false;
                    }

                    else
                    {
                        NotSeen();
                    }
                }

                else
                {
                    NotSeen();
                }
            }

            else
            {
                NotSeen();
            }
        }

        else
        {
            NotSeen();
        }
    }

    private void NotSeen()
    {
        seen = false;
        inSusRange = false;
    }
    }

