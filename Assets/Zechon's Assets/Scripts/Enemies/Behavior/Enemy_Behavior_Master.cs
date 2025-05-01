using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Behavior_Master : MonoBehaviour
{
    [Header("Enemy Base Config")]
    public EnemyBehavior behavior;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float chaseSpeed = 5.5f;

    public float MoveSpeed => moveSpeed;
    public float TurnSpeed => turnSpeed;
    public float ChaseSpeed => chaseSpeed;


    public enum EnemyBehavior
    {
        Patrol,
        Guard,
        Roaming
    }

    private IEnemyBehavior currentBehavior;

    [Header("Component References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Sight sight;

    [HideInInspector] public NavMeshAgent agent;

    // References to attached behavior scripts
    private Enemy_Patrol_Behavior patrolBehavior;
    private Enemy_Guard_Behavior guardBehavior;
    private Enemy_Roam_Behavior roamBehavior;

    void Awake()
    {
        patrolBehavior = GetComponent<Enemy_Patrol_Behavior>();
        guardBehavior = GetComponent<Enemy_Guard_Behavior>();
        roamBehavior = GetComponent<Enemy_Roam_Behavior>();
    }

    void Start()
    {
        NavAgentSetup();
        ConfigureSight();
        SwitchBehavior(behavior);
    }

    private void NavAgentSetup()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
    }


    private void ConfigureSight()
    {
        if (sight == null) return;

        switch (behavior)
        {
            case EnemyBehavior.Patrol:
                sight.susRange = 4f;
                sight.detectionRange = 10f;
                sight.fieldOfViewAngle = 105f;
                break;
            case EnemyBehavior.Guard:
                sight.susRange = 3f;
                sight.detectionRange = 15f;
                sight.fieldOfViewAngle = 90f;
                break;
            case EnemyBehavior.Roaming:
                sight.susRange = 5f;
                sight.detectionRange = 8f;
                sight.fieldOfViewAngle = 120f;
                break;
        }
    }

    void Update()
    {
        currentBehavior?.UpdateBehavior(this);
    }

    public void SwitchBehavior(EnemyBehavior newBehavior)
    {
        behavior = newBehavior;

        // Disable all behaviors first
        if (patrolBehavior) patrolBehavior.enabled = false;
        if (guardBehavior) guardBehavior.enabled = false;
        if (roamBehavior) roamBehavior.enabled = false;

        // Enable the correct one
        switch (behavior)
        {
            case EnemyBehavior.Patrol:
                currentBehavior = patrolBehavior;
                if (patrolBehavior) patrolBehavior.enabled = true;
                break;
            case EnemyBehavior.Guard:
                currentBehavior = guardBehavior;
                if (guardBehavior) guardBehavior.enabled = true;
                break;
            case EnemyBehavior.Roaming:
                currentBehavior = roamBehavior;
                if (roamBehavior) roamBehavior.enabled = true;
                break;
        }
    }

    public void SetAnimatorBool(string name, bool value)
    {
        anim?.SetBool(name, value);
    }

    public void SetAgentSpeed(float speed)
{
    agent.speed = speed;
}

}
