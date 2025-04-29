using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float blockDmgPercent;
    private float blockDmgTaken;

    [Header("Test Bools")]
    [SerializeField] bool blocking;

    [SerializeField] Enemy_Sight enemySight;

    void Start()
    {
        currentHealth = maxHealth;

        blockDmgTaken = 1f - blockDmgPercent;
    }

    public void TakeDamage(float damage)
    {
        if (blocking)
            damage *= blockDmgTaken;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining: " +  currentHealth + ".");

        if (enemySight != null)
        {
            enemySight.ForceAggroState();
        }

        if (currentHealth < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
    }
}
