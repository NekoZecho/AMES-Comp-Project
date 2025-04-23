using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Nav Points")]
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Speed")]
    public float speed;
    public float angSpeed;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}
