using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousBehavior : MonoBehaviour, IEnemyBehavior
{
    private Enemy_Behavior_Master enemy;
    private Transform player;

    [Header("Suspicious Settings")]
    [SerializeField] private float suspiciousRange = 15f;
    [SerializeField] private float stareDuration = 3f;
    [SerializeField] private float returnDelay = 2f;

    private Vector3 lastKnownPosition;
    private bool isStaring;

    public void UpdateBehavior(Enemy_Behavior_Master enemy)
    {
        this.enemy = enemy;

        player = enemy.Sight.player;

        if (player != null)
        {
            // Approach the player if they are visible and within suspicious range
            if (Vector3.Distance(enemy.transform.position, player.position) <= suspiciousRange && enemy.Sight.isPlayerVisible)
            {
                ApproachPlayer();
            }
            else
            {
                // If the player goes beyond the suspicious range
                if (!isStaring)
                {
                    StartCoroutine(StareAtLastPosition());
                }
            }
        }
    }

    private void ApproachPlayer()
    {
        // Move towards the player but only within the suspicious range
        Vector3 targetPosition = player.position;

        // Prevent going beyond the range of the suspicious behavior
        if (Vector3.Distance(enemy.transform.position, enemy.transform.position) > suspiciousRange)
        {
            targetPosition = enemy.transform.position + (targetPosition - enemy.transform.position).normalized * suspiciousRange;
        }

        // Move towards the target position
        enemy.agent.SetDestination(targetPosition);
        enemy.SetAnimatorBool("Walking", true);
    }

    private IEnumerator StareAtLastPosition()
    {
        isStaring = true;

        // Store the last known position
        lastKnownPosition = player.position;

        // Wait for the stare duration
        yield return new WaitForSeconds(stareDuration);

        // After the stare duration, return to the original behavior
        enemy.SwitchBehavior(Enemy_Behavior_Master.EnemyBehavior.Patrol);

        isStaring = false;
    }
}