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

    [Header("Aggro Settings")]
    [SerializeField] private float aggroLingerTime = 5f; // Time to stay aggressive before decay
    private float aggroLingerTimer = 0f;

    [Header("Detected")]
    public bool seen;
    public bool inSusRange;
    [SerializeField] float timeToAware = 2f;
    [SerializeField] float timeToSuspicious = 3f;
    [SerializeField] float timeToAggressive = 2f;
    public bool fullyDecayed;
    public bool hasStared = false;

    [Header("Progress")]
    public float awareProgress;
    public float suspiciousProgress;
    public float aggressiveProgress;

    private float awareTimer;
    private float suspiciousTimer;
    private float aggressiveTimer;

    [Header("Enemy State")]
    public EnemyState currentState = EnemyState.Unaware;
    public int enemyMode;

    public enum EnemyState
    {
        Unaware,
        Aware,
        Suspicious,
        Aggressive
    }

    void Start()
    {
        ResetState();
    }

    void Update()
    {
        UpdateState();
    }

    void FixedUpdate()
    {
        CheckLineOfSight();
    }

    void ResetState()
    {
        seen = false;
        inSusRange = false;
        awareProgress = 0f;
        suspiciousProgress = 0f;
        aggressiveProgress = 0f;
        awareTimer = 0f;
        suspiciousTimer = 0f;
        aggressiveTimer = 0f;
        enemyMode = 0;
        currentState = EnemyState.Unaware;
    }

    void UpdateState()
    {
        if (seen)
        {
            if (awareTimer < timeToAware)
            {
                currentState = EnemyState.Unaware;
                awareTimer += Time.deltaTime;
                awareProgress = Mathf.Clamp01(awareTimer / timeToAware);

                if (awareTimer >= timeToAware)
                {
                    enemyMode = 1;
                }
            }
            else if (suspiciousTimer < timeToSuspicious)
            {
                currentState = EnemyState.Aware;
                if (inSusRange)
                    suspiciousTimer += Time.deltaTime;

                suspiciousProgress = Mathf.Clamp01(suspiciousTimer / timeToSuspicious);
            }
            else if (aggressiveTimer < timeToAggressive)
            {
                currentState = EnemyState.Suspicious;
                if (inSusRange)
                {
                    aggressiveTimer += Time.deltaTime;
                    aggroLingerTimer = 0f; // reset linger timer
                }
                aggressiveProgress = Mathf.Clamp01(aggressiveTimer / timeToAggressive);
            }
            else
            {
                currentState = EnemyState.Aggressive;
                aggressiveProgress = 1f;
            }
        }
        else
        {
            if (aggressiveTimer > 0f)
            {
                if (aggroLingerTimer < aggroLingerTime)
                {
                    currentState = EnemyState.Aggressive;
                    aggroLingerTimer += Time.deltaTime;
                }
                else
                {
                    currentState = EnemyState.Aggressive;
                    aggressiveTimer -= Time.deltaTime / 1.5f;
                    aggressiveProgress = Mathf.Clamp01(aggressiveTimer / timeToAggressive);
                }
            }
            else if (suspiciousTimer > 0f)
            {
                currentState = EnemyState.Suspicious;
                suspiciousTimer -= Time.deltaTime / 1.5f;
                suspiciousProgress = Mathf.Clamp01(suspiciousTimer / timeToSuspicious);
            }
            else if (awareTimer > 0f)
            {
                currentState = EnemyState.Aware;
                awareTimer -= Time.deltaTime / 1.5f;
                awareProgress = Mathf.Clamp01(awareTimer / timeToAware);
            }
            else
            {
                ResetState();
            }
        }
    }

    void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - eyes.position;
        float angle = Vector3.Angle(directionToPlayer, eyes.forward);

        if (angle < fieldOfViewAngle / 2 && Vector3.Distance(eyes.position, player.position) < detectionRange)
        {
            if (Physics.Raycast(eyes.position, directionToPlayer.normalized, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player_Body"))
                {
                    seen = true;
                    float horizontalDistance = Vector3.Distance(
                        new Vector3(hit.point.x, 0, hit.point.z),
                        new Vector3(eyes.position.x, 0, eyes.position.z)
                    );

                    inSusRange = horizontalDistance <= susRange;
                    return;
                }
            }
        }

        NotSeen();
    }

    void NotSeen()
    {
        seen = false;
        inSusRange = false;
    }

    public void ForceAggroState()
    {
        seen = true;
        inSusRange = true;
        aggroLingerTimer = 0f;

        // Skip all progress and force to full aggressive state
        awareProgress = 1f;
        suspiciousProgress = 1f;
        aggressiveProgress = 1f;

        awareTimer = timeToAware;
        suspiciousTimer = timeToSuspicious;
        aggressiveTimer = timeToAggressive;

        currentState = EnemyState.Aggressive;
        enemyMode = 1;
    }
}

