using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Roam_Behavior : MonoBehaviour, IEnemyBehavior
{
    private Enemy_Behavior_Master enemy;

    public enum RoamAreaType
    {
        Rectangular,
        Circular
    }

    [Header("Roaming Settings")]
    [SerializeField] private RoamAreaType roamAreaType = RoamAreaType.Rectangular;
    [SerializeField] private float roamRadius = 10f; // For circular
    [SerializeField] private Transform[] roamBounds; // 2 transforms for rectangular, 1 center for circular
    [SerializeField] private float waitTime = 2f;

    private bool isWaiting;

    public void UpdateBehavior(Enemy_Behavior_Master enemy)
    {
        this.enemy = enemy;

        if (enemy.agent.pathPending || isWaiting) return;

        if (!enemy.agent.hasPath || enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                StartCoroutine(WaitAndRoam());
            }

            enemy.SetAnimatorBool("Walking", false);
            enemy.SetAnimatorBool("Idle", true);
        }
        else if (enemy.agent.velocity.sqrMagnitude > 0.1f)
        {
            enemy.SetAnimatorBool("Walking", true);
            enemy.SetAnimatorBool("Idle", false);
        }
    }

    private IEnumerator WaitAndRoam()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        Vector3 roamPosition;
        if (TryGetRoamPosition(out roamPosition))
        {
            enemy.agent.SetDestination(roamPosition);
        }

        isWaiting = false;
    }

    private bool TryGetRoamPosition(out Vector3 result)
    {
        for (int i = 0; i < 10; i++) // Try 10 times
        {
            Vector3 randomPoint = roamAreaType == RoamAreaType.Circular
                ? GetRandomPointWithinCircle()
                : GetRandomPointWithinRect();

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = enemy.transform.position;
        return false;
    }

    private Vector3 GetRandomPointWithinRect()
    {
        if (roamBounds.Length < 2) return enemy.transform.position;

        Vector3 min = roamBounds[0].position;
        Vector3 max = roamBounds[1].position;

        float x = Random.Range(min.x, max.x);
        float z = Random.Range(min.z, max.z);
        return new Vector3(x, enemy.transform.position.y, z);
    }

    private Vector3 GetRandomPointWithinCircle()
    {
        if (roamBounds.Length < 1) return enemy.transform.position;

        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        Vector3 center = roamBounds[0].position;

        return new Vector3(center.x + randomCircle.x, enemy.transform.position.y, center.z + randomCircle.y);
    }
}
