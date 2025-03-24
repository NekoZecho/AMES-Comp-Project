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
        suspicious,
        aware,
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
        Debug.Log(eState.ToString());
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
                        detectionHandler();
                    }
                    else
                        decayHandler();
                }
            }
            else
                decayHandler();
            
        }
    }

    private void decayHandler()
    {
        if (seen && eState == EnemyState.unaware)
        {
            seen = false;
            eState = EnemyState.unaware;
            progressionTimer = 0;
            if (decayTimer !<= 0)
            {
            decayTimer -= Time.deltaTime;
            progress = decayTimer / undetectTimerSus;
            }
            else
            {
                timerSwapped = false;
            }
        }
        else if (seen && eState == EnemyState.suspicious)
        {
            DecayTimerSwitch();
            if (progress >= 0)
            {
                decayTimer -= Time.deltaTime;
                progress = decayTimer / undetectTimerSus;
            }
            if (decayTimer <= 0)
            {
                seen = false;
                decayTimer = 0;
                eState = EnemyState.unaware;
                progressionTimer = 0;
            }
        }
    }

    private void detectionHandler()
    {
        switch (eState)
        {
            case EnemyState.unaware:
                if (PlyrMvmnt.mState == Player_Movement.MovementState.sprinting && seen)
                {
                    progressionTimer += Time.deltaTime;
                    progress = progressionTimer / unToSusT;
                    Debug.Log(progress);
                    if (progressionTimer >= unToSusT)
                    {
                        eState = EnemyState.suspicious;
                        progressionTimer = 0;
                        progress = 0;
                    }
                }
                break;
        }
    }

    private void DecayTimerSwitch()
    {
        switch (timerSwapped)
        {
            case true:
                break;

            case false:
                if (eState == EnemyState.unaware)
                {
                    decayTimer = undetectTimerSus;
                    timerSwapped = true;
                }
                break;
        }
    }
}
