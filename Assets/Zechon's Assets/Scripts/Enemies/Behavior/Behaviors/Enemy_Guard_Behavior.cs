using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Guard_Behavior : MonoBehaviour, IEnemyBehavior
{
    private Enemy_Behavior_Master enemy;

    [Header("Guard Scan Settings")]
    [SerializeField] private float guardScanInterval = 4f;
    [SerializeField] private float guardScanDuration = 2f;
    [SerializeField] private float guardScanAngle = 60f;
    [SerializeField] private float guardScanSpeed = 1.5f;

    private float scanTimer = 0f;
    private bool isScanning = false;

    public void UpdateBehavior(Enemy_Behavior_Master enemy)
    {
        this.enemy = enemy;

        enemy.SetAnimatorBool("Walking", false);
        enemy.SetAnimatorBool("Idle", true);
        enemy.agent.isStopped = true;

        // Handle periodic scanning
        if (!isScanning)
        {
            scanTimer += Time.deltaTime;
            if (scanTimer >= guardScanInterval)
            {
                scanTimer = 0f;
                enemy.StartCoroutine(ScanArea());
            }
        }
    }

    private IEnumerator ScanArea()
    {
        isScanning = true;
        float elapsed = 0f;

        Quaternion startRotation = enemy.transform.rotation;
        Quaternion leftRotation = Quaternion.Euler(0f, -guardScanAngle, 0f) * startRotation;
        Quaternion rightRotation = Quaternion.Euler(0f, guardScanAngle, 0f) * startRotation;

        // Look left
        while (elapsed < guardScanDuration / 2f)
        {
            enemy.transform.rotation = Quaternion.Slerp(startRotation, leftRotation, (elapsed / (guardScanDuration / 2f)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        // Look right
        while (elapsed < guardScanDuration / 2f)
        {
            enemy.transform.rotation = Quaternion.Slerp(leftRotation, rightRotation, (elapsed / (guardScanDuration / 2f)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to center
        elapsed = 0f;
        while (elapsed < guardScanDuration / 2f)
        {
            enemy.transform.rotation = Quaternion.Slerp(rightRotation, startRotation, (elapsed / (guardScanDuration / 2f)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        isScanning = false;
    }
}

