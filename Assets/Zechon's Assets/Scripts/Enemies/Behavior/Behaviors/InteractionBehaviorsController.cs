using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBehaviorsController : MonoBehaviour
{
    private Enemy_Behavior_Master enemyMaster;

    [Header("Suspicious Behavior Settings")]
    [SerializeField] private SuspiciousBehavior suspiciousBehavior;

    void Start()
    {
        // Get reference to the EnemyBehaviorMaster component
        enemyMaster = GetComponent<Enemy_Behavior_Master>();

        // Ensure suspicious behavior is properly assigned
        if (suspiciousBehavior == null)
        {
            suspiciousBehavior = gameObject.AddComponent<SuspiciousBehavior>();
        }
    }

    void Update()
    {
        // Depending on the behavior, manage suspicious or other behaviors
        if (enemyMaster.currentBehavior is SuspiciousBehavior)
        {
            suspiciousBehavior.UpdateBehavior(enemyMaster);
        }
    }
}
