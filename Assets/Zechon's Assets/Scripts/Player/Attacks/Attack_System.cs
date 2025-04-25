using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_System : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode attkEnter = KeyCode.Keypad2;
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
    [SerializeField]
    Animator anim;
    Collider RightHand;
    Collider LeftHand;

    [Header("Bools")]
    public bool rightHandActive;
    public bool stance;
    private bool attacking;
    private bool blocking;
    private bool dodging;
    private bool stunned;

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
        pAState = PlayerAttackState.passive;

        rightHandActive = true;

        lAtkD = lAttackDuration;
        hAtkD = hAttackDuration;
        nyaBR = blockRecup;

        timeSinceLastHit = 0;

        anim.SetBool("rightHandActive", true);
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
        if (Input.GetKeyDown(attkEnter))
        {
            if (!stance)
            {
                stance = true;
                anim.SetBool("Stance", true);
                anim.SetBool("Idle", false);
            }
            else
            {
                stance = false;
                anim.SetBool("Stance", false);
                anim.SetBool("Idle", true);
            }
        }

        //light attack, duh
        else if (Input.GetKeyDown(lightAttack))
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
                        anim.SetBool("LAttack", true);
                        break;

                    case false:
                        attacking = true;
                        lAttackDuration = lAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        lightHits++;
                        rightHandActive = true;
                        anim.SetBool("LAttack", true);
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
                        rightHandActive = false;
                        break;

                    case false:
                        attacking = true;
                        hAttackDuration = hAtkD;
                        timeSinceLastHit = 0;
                        consecutiveHits++;
                        heavyHits++;
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

    private void HandToggle()
    {
        unAttack();
        if (rightHandActive)
        {
            rightHandActive = false;
            anim.SetBool("rightHandActive", false);
        }
        else
        {
            rightHandActive = true;
            anim.SetBool("rightHandActive", true);
        }
    }

    private void unAttack()
    {
        anim.SetBool("LAttack", false);
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
