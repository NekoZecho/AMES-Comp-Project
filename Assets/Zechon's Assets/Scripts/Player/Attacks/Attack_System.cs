using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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
    float lAttackCooldown;
    float lAtkCD;
    [SerializeField]
    float hAttackCooldown;
    float hAtkCD;
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
    int swapNum;

    [Header("Components")]
    [SerializeField]
    Animator anim;
    [SerializeField]
    Collider RightHand;
    [SerializeField]
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

        lAtkCD = lAttackCooldown;
        hAtkCD = hAttackCooldown;
        nyaBR = blockRecup;

        timeSinceLastHit = 0;

        anim.SetBool("rightHandActive", true);

        RightHand.enabled = false;
        LeftHand.enabled = false;
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
                anim.SetLayerWeight(1, 1f);
                anim.SetLayerWeight(0, 0f);
                anim.SetBool("Stance", true);
            }
            else
            {
                stance = false;
                anim.SetLayerWeight(1, 0f);
                anim.SetLayerWeight(0, 1f);
                anim.SetBool("Stance", false);
            }
        }

        if (stance)
        {
            //light attack, duh
            if (Input.GetKeyDown(lightAttack))
            {
                if (!stunned && !blocking && !attacking && lAttackCooldown == 0 && hAttackCooldown == 0 && !(anim.GetBool("HAttack")))
                {
                    lAttack();
                }
            }
            //heavy attack, (heavy tf2 reference!!!)
            if (Input.GetKeyDown(heavyAttack))
            {
                if (!stunned && !blocking && !attacking && lAttackCooldown == 0 && hAttackCooldown == 0 && !(anim.GetBool("LAttack")))
                {
                    hAttack();
                }
            }
            //blocking
            if (Input.GetKeyDown(block))
            {
                if (!stunned && !attacking && blockRecup == 0)
                {
                    //Block();
                }
            }
        }
    }

    public void OnHandTrigger(Collider other)
    {
        timeSinceLastHit = 0;
        Debug.Log(other.gameObject.name);
    }


    private void Timers()
    {
        //Cooldown Timers and stuff
        if (lAttackCooldown != 0)
        {
            lAttackCooldown -= Time.deltaTime;
            if (lAttackCooldown < 0)
            {
                lAttackCooldown = 0;
            }
        }
        if (hAttackCooldown != 0)
        {
            hAttackCooldown -= Time.deltaTime;
            if (hAttackCooldown < 0)
            {
                hAttackCooldown = 0;
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
        if (lAttackCooldown == 0 && hAttackCooldown == 0)
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
        
        if (anim.GetBool("HAttack"))
        {
            if (rightHandActive)
            {
                swapNum = 0;
                rightHandActive = false;
                anim.SetBool("rightHandActive", false);
            }
            else
            {
                swapNum = 0;
                rightHandActive = true;
                anim.SetBool("rightHandActive", true);
            }
        }
        else
        {
            swapNum ++;
            switch (swapNum)
            {
                case 2:
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
                    break;

                case 3:
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
                    break;

                case 4:
                    if (rightHandActive)
                    {
                        swapNum = 0;
                        rightHandActive = false;
                        anim.SetBool("rightHandActive", false);
                    }
                    else
                    {
                        swapNum = 0;
                        rightHandActive = true;
                        anim.SetBool("rightHandActive", true);
                    }
                    break;
            }
        }
        unAttack();
    }
    private void unAttack()
    {
        if (anim.GetBool("LAttack"))
        {
            anim.SetBool("LAttack", false);
            anim.SetBool("Stance", true);
            lAttackCooldown = lAtkCD;
            Debug.Log("L");
        }
        else if (anim.GetBool("HAttack"))
        {
            anim.SetBool("HAttack", false);
            anim.SetBool("Stance", true);
            hAttackCooldown = hAtkCD;
            Debug.Log("H");
        }
    }

    private void lAttack()
    {
        attacking = true;
        anim.SetBool("Stance", false);
        anim.SetBool("LAttack", true);
    }

    private void hAttack()
    {
        attacking = true;
        anim.SetBool("Stance", false);
        anim.SetBool("HAttack", true);
    }

    private void lHit()
    {
        timeSinceLastHit = 0;
        consecutiveHits++;
        lightHits++;
    }

    private void hHit()
    {
        timeSinceLastHit = 0;
        consecutiveHits++;
        heavyHits++;
    }

    private void Block()
    {
        //blocking = true;
        stance = false;
        anim.SetBool("Stance", false);
        //anim.SetBool("Block", true);
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

    private void colliderEnable()
    {
        if (rightHandActive)
        {
                RightHand.enabled = true;
        }
        else
        {
                LeftHand.enabled = true;
        }
    }

    private void colliderDisable()
    {
        if (rightHandActive)
        {
            RightHand.enabled = false;
        }
        else
        {
            LeftHand.enabled = false;
        }
    }
}
