using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Enemy Base Config")]
    public BehaviorType behavior;

    [Header("Nav Points (if in patrol)")]
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Movement Settings")]
    public float speed;
    public float angSpeed;

    [Header("Component References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Sight sight;

    [Header("Stare Reaction Settings")]
    [SerializeField] private float stareDuration = 2f;
    [SerializeField] private float suspiciousStareDuration = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    private bool hasEverReactedToAwareness = false;
    private Coroutine currentSuspiciousRoutine;

    private NavMeshAgent agent;
    private bool isWaiting = false;
    private bool awareReacting = false;
    [HideInInspector] public bool hasReactedToAwareness = false;

    public enum BehaviorType
    {
        Patrol,
        Stationary,
        Roaming
    }

    void Start()
    {
        NavAgentSetup();
        ConfigureSight();
    }

    private void NavAgentSetup()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.angularSpeed = angSpeed;
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    private void ConfigureSight()
    {
        if (sight == null) return;

        switch (behavior)
        {
            case BehaviorType.Patrol:
                sight.susRange = 4f;
                sight.detectionRange = 10f;
                sight.fieldOfViewAngle = 120f;
                break;

            case BehaviorType.Stationary:
                sight.susRange = 3f;
                sight.detectionRange = 15f;
                sight.fieldOfViewAngle = 90f;
                break;

            case BehaviorType.Roaming:
                sight.susRange = 5f;
                sight.detectionRange = 8f;
                sight.fieldOfViewAngle = 140f;
                break;
        }
    }

    void Update()
    {
        if (sight)
        {
            switch (sight.currentState)
            {
                case Enemy_Sight.EnemyState.Aware:
                    if (!hasEverReactedToAwareness)
                    {
                        StartCoroutine(AwareReaction());
                        hasEverReactedToAwareness = true;
                    }
                    break;

                case Enemy_Sight.EnemyState.Suspicious:
                    if (currentSuspiciousRoutine == null)
                    {
                        currentSuspiciousRoutine = StartCoroutine(SuspiciousWatch());
                    }
                    break;

                case Enemy_Sight.EnemyState.Unaware:
                    if (!isWaiting && !agent.hasPath)
                    {
                        ResumeBehavior();
                    }
                    break;
            }

            // If Suspicious just ended
            if (sight.currentState != Enemy_Sight.EnemyState.Suspicious && currentSuspiciousRoutine != null)
            {
                StopCoroutine(currentSuspiciousRoutine);
                currentSuspiciousRoutine = null;
                ResumeBehavior(); // Return to patrol, roam, or stationary
            }
        }


        if (isWaiting || awareReacting || agent.isStopped)
            return;

        switch (behavior)
        {
            case BehaviorType.Patrol:
                Patrol();
                break;
            case BehaviorType.Stationary:
                Guard();
                break;
            case BehaviorType.Roaming:
                Roam();
                break;
        }
    }


    //PATROL
    private void Patrol()
    {
        if (agent.pathPending || isWaiting) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
            StartCoroutine(WaitThenMoveToNextWaypoint());
        else if (anim)
        {
            anim.SetBool("Walking", true);
            anim.SetBool("Idle", false);
        }
    }

    private IEnumerator WaitThenMoveToNextWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true;
        if (anim) anim.SetBool("Walking", false);
        if (anim) anim.SetBool("Idle", true);

        float waitTime = Random.Range(1f, 3f);
        yield return new WaitForSeconds(waitTime);

        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypoint].position);
        agent.isStopped = false;
        if (anim) anim.SetBool("Walking", true);
        if (anim) anim.SetBool("Idle", false);

        isWaiting = false;
    }

    private IEnumerator AwareReaction()
    {
        awareReacting = true;
        agent.isStopped = true;
        if (anim) anim.SetBool("Walking", false);
        if (anim) anim.SetBool("Idle", true);

        float timer = 0f;
        while (timer < stareDuration)
        {
            Vector3 direction = (sight.player.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        if (anim) anim.SetBool("Walking", true);
        if (anim) anim.SetBool("Idle", false);

        awareReacting = false;
    }

    private IEnumerator SuspiciousWatch()
    {
        isWaiting = true;
        agent.isStopped = true;
        if (anim) anim.SetBool("Walking", false);
        if (anim) anim.SetBool("Idle", true);

        float timer = 0f;

        while (timer < suspiciousStareDuration)
        {
            Vector3 direction = (sight.player.position - transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            timer += Time.deltaTime;
            yield return null;
        }

        // After fixed duration, resume behavior
        isWaiting = false;
        agent.isStopped = false;

        if (behavior == BehaviorType.Patrol && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
            if (anim) anim.SetBool("Walking", true);
            if (anim) anim.SetBool("Idle", false);
        }

        currentSuspiciousRoutine = null;
    }



    private void ResumeBehavior()
    {
        isWaiting = false;
        agent.isStopped = false;

        if (behavior == BehaviorType.Patrol && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
            if (anim) anim.SetBool("Walking", true);
            if (anim) anim.SetBool("Idle", false);
        }
        else if (behavior == BehaviorType.Roaming)
        {
            // Roam logic here
        }
    }

    private void Guard()
    {
        agent.isStopped = true;
        if (anim) anim.SetBool("Walking", false);
        if (anim) anim.SetBool("Idle", true);
    }

    private void Roam()
    {
        // Implement random roaming logic if needed
    }
}