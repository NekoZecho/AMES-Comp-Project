using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_System : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode lightAttack = KeyCode.Mouse0;
    public KeyCode heavyAttack = KeyCode.Mouse1;
    public KeyCode block = KeyCode.F;
    public KeyCode dodge = KeyCode.R;

    [Header("Cooldowns / Debounces")]
    [SerializeField]
    float lAttackDuration;
    float lAtkD;
    [SerializeField]
    float hAttackDuration;
    float hAtkD;
    [SerializeField]
    float blockRecup;
    float nyaBR;
    [SerializeField]
    float decisionDuration;
    float decdur;
    [SerializeField]
    float comboResiliance;

    [Header("Hits Info")]
    public float timeSinceLastHit;
    public int consecutiveHits;
    public int lightHits;
    public int heavyHits;
    public int blockedHits;

    [Header("Components")]
    Animator animator;
    Collider RightHand;
    Collider LeftHand;

    [Header("Bools")]
    private bool attacking;
    private bool blocking;
    private bool dodging;
    private bool stunned;
    public bool rightHandActive;

    [Header("Player Attack")]
    public PlayerAttackState pAState;
    public enum PlayerAttackState
    {
        passive,
        attacking,
        blocking,
        dodging,
        stunned
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        pAState = PlayerAttackState.passive;

        rightHandActive = true;

        lAtkD = lAttackDuration;
        hAtkD = hAttackDuration;
        nyaBR = blockRecup;

        timeSinceLastHit = 0;
    }

    void Update()
    {
        Timers();
        Attack();
        StateHandler();
    }

    private void StateHandler()
    {
        //For Enemy Attack / Detection Scripts
        if (attacking)
        {
            pAState = PlayerAttackState.attacking;
        }
        else if (blocking)
        {
            pAState = PlayerAttackState.blocking;
        }
        else if (dodging)
        {
            pAState = PlayerAttackState.dodging;
        }
        else if (stunned)
        {
            pAState= PlayerAttackState.stunned;
        }
        else
        {
            pAState = PlayerAttackState.passive;
        }
    }

    private void Attack()
    {
        //light attack, duh
        if (Input.GetKeyDown(lightAttack))
        {
            if (!stunned && !blocking && !attacking && lAttackDuration == 0 && hAttackDuration == 0)
            {
                switch (rightHandActive)
                {
                    case true:
                        attacking = true;
                        lAttackDuration = lAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        lightHits++;
                        Debug.Log("Light Attack Right!");
                        rightHandActive = false;
                        break;

                    case false:
                        attacking = true;
                        lAttackDuration = lAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        lightHits++;
                        Debug.Log("Light Attack Left!");
                        rightHandActive = true;
                        break;
                }
                
            }
        }
        //heavy attack, (heavy tf2 reference!!!)
        if (Input.GetKeyDown(heavyAttack))
        {
            if (!stunned && !blocking && !attacking && lAttackDuration == 0 && hAttackDuration == 0)
            {
                switch (rightHandActive)
                {
                    case true:
                        attacking = true;
                        hAttackDuration = hAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        heavyHits++;
                        Debug.Log("Heavy Attack Right!");
                        rightHandActive = false;
                        break;

                    case false:
                        attacking = true;
                        hAttackDuration = hAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        heavyHits++;
                        Debug.Log("Heavy Attack Left!");
                        rightHandActive = true;
                        break;
                }
            }
        }
        //blocking
        if (Input.GetKeyDown(block))
        {
            if (!stunned && !attacking && blockRecup == 0)
            {
                blocking = true;
            }
        }

    }

    private void Timers()
    {
        //Cooldown Timers and stuff
        if (lAttackDuration != 0)
        {
            lAttackDuration -= Time.deltaTime;
            if (lAttackDuration < 0)
            {
                lAttackDuration = 0;
            }
        }
        if (hAttackDuration != 0)
        {
            hAttackDuration -= Time.deltaTime;
            if (hAttackDuration < 0)
            {
                hAttackDuration = 0;
            }
        }
        if (blockRecup != 0)
        {
            blockRecup -= Time.deltaTime;
            if (blockRecup < 0)
            {
                blockRecup = 0;
            }
        }
        if (lAttackDuration == 0 && hAttackDuration == 0)
        {
            attacking = false;
        }

        //Combo tracking timers
        timeSinceLastHit += Time.deltaTime;
        if (timeSinceLastHit > comboResiliance)
        {
            consecutiveHits = 0;
        }
    }

    private void unAttack()
    {
        attacking = false;
        lAttackDuration = lAtkD;
    }

    private void lHit()
    {
        lAttackDuration = lAtkD;
        timeSinceLastHit = 0;
    }
    private void hHit()
    {
        hAttackDuration = hAtkD;
        timeSinceLastHit = 0;
    }

    private void unBlock()
    {
        blocking = false;
    }

    private void defaultStance()
    {
        unAttack();
        unBlock();
    }

    private void colliderToggle()
    {
        if (rightHandActive)
        {
            if (RightHand.enabled == true)
            {
                RightHand.enabled = false;
            }
            else
            {
                RightHand.enabled = true;
            }
        }
        else
        {
            if (LeftHand.enabled == true)
            {
                LeftHand.enabled = false;
            }
            else
            {
                LeftHand.enabled = true;
            }
        }
    }
}
