using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Patrol_Behavior : MonoBehaviour, IEnemyBehavior
{
    private Enemy_Behavior_Master enemy;

    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypoint = 0;
    private bool waiting = false;

    public void UpdateBehavior(Enemy_Behavior_Master enemy)
    {
        this.enemy = enemy;

        if (waiting || enemy.agent.pathPending)
            return;

        if (!enemy.agent.hasPath || enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
        {
            enemy.SetAnimatorBool("Walking", false);
            enemy.SetAnimatorBool("Idle", true);

            if (!waiting)
                enemy.StartCoroutine(WaitAndMove());
        }
        else
        {
            enemy.SetAnimatorBool("Walking", true);
            enemy.SetAnimatorBool("Idle", false);
        }
    }

    private IEnumerator WaitAndMove()
    {
        waiting = true;

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        enemy.agent.SetDestination(waypoints[currentWaypoint].position);
        enemy.SetAgentSpeed(enemy.MoveSpeed);

        enemy.SetAnimatorBool("Walking", true);
        enemy.SetAnimatorBool("Idle", false);

        waiting = false;
    }
}


