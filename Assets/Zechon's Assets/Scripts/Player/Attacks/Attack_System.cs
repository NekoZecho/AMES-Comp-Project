using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Attack_System : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode lightAttack = KeyCode.Mouse0;
    public KeyCode heavyAttack = KeyCode.Mouse1;
    public KeyCode block = KeyCode.F;
    public KeyCode dodge = KeyCode.R;

    [Header("Cooldowns / Debounces")]
    [SerializeField] float lAttackCooldown;
    float lAtkCD;
    [SerializeField] float hAttackCooldown;
    float hAtkCD;
    [SerializeField] float blockRecup;
    float nyaBR;
    [SerializeField] float decisionDuration;
    float decdur;
    [SerializeField] float comboResiliance;
    [SerializeField] private float stanceDuration = 3f;
    private Coroutine stanceTimeoutRoutine;
    private bool canAttackAfterStance = true;
    [SerializeField] float stanceEntryDelay = 0.2f; // How long before attacks are allowed after stance enters

    [Header("Hits Info")]
    public float timeSinceLastHit;
    public float timeSinceLastAttack;
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

    private Coroutine stanceRoutine;
    public enum PlayerAttackState
    {
        passive,
        attacking,
        blocking,
        dodging,
        stunned
    }

    [Header("Debug")]
    [SerializeField] private float ROOTTIME;
    

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

        StartCoroutine(ROOTTOGGLE());
    }

    IEnumerator ROOTTOGGLE()
    {
        yield return new WaitForSeconds(ROOTTIME);
        anim.applyRootMotion = true;
        Debug.Log("Run");
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

    public void OnHandTrigger(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Enemy_Health enemy = other.GetComponent<Enemy_Health>();

        float damage = 0f;

        if (anim.GetBool("LAttack"))
        {
            timeSinceLastHit = 0;
            consecutiveHits++;
            lightHits++;

            damage = 10f;
        }
        else if (anim.GetBool("HAttack"))
        {
            timeSinceLastHit = 0;
            consecutiveHits++;
            heavyHits++;

            damage = 25f;
        }

        enemy.TakeDamage(damage);
    }

    private void Attack()
    {
        // Enter stance with heavy attack key if not already in stance
        if (Input.GetKeyDown(heavyAttack) && !stance)
        {
            EnterStance();
            Debug.Log("Entering The Stance");
        }

        if (stance)
        {
            if (!canAttackAfterStance)
                return; // prevent attacking immediately after stance activation

            // Light Attack
            if (Input.GetKeyDown(lightAttack))
            {
                if (!stunned && !blocking && !attacking && lAttackCooldown == 0 && hAttackCooldown == 0 && !anim.GetBool("HAttack"))
                {
                    lAttack();
                }
            }

            // Heavy Attack
            if (Input.GetKeyDown(heavyAttack))
            {
                if (!stunned && !blocking && !attacking && lAttackCooldown == 0 && hAttackCooldown == 0 && !anim.GetBool("LAttack"))
                {
                    hAttack();
                }
            }

            // Blocking
            if (Input.GetKeyDown(block))
            {
                if (!stunned && !attacking && blockRecup == 0)
                {
                    //Block();
                }
            }
        }

    }

    private void Timers()
    {
        timeSinceLastAttack += Time.deltaTime;

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
            lAttackCooldown = lAtkCD;
        }
        else if (anim.GetBool("HAttack"))
        {
            anim.SetBool("HAttack", false);
            hAttackCooldown = hAtkCD;
        }

        attacking = false;
        anim.SetBool("Stance", true);

        stanceTimeoutRoutine = StartCoroutine(StanceTimeout());
    }

    private void lAttack()
    {
        attacking = true;
        anim.SetBool("Stance", false);
        anim.SetBool("LAttack", true);

        timeSinceLastAttack = 0f;
    }

    private void hAttack()
    {
        attacking = true;
        anim.SetBool("Stance", false);
        anim.SetBool("HAttack", true);

        timeSinceLastAttack = 0f;
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

    IEnumerator SmoothLayerTransition(bool enableStance)
    {
        float duration = 0.5f; // duration of the transition
        float time = 0f;

        float startLayer0 = anim.GetLayerWeight(0);
        float startLayer1 = anim.GetLayerWeight(1);

        float targetLayer0 = enableStance ? 0f : 1f;
        float targetLayer1 = enableStance ? 1f : 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            anim.SetLayerWeight(0, Mathf.Lerp(startLayer0, targetLayer0, t));
            anim.SetLayerWeight(1, Mathf.Lerp(startLayer1, targetLayer1, t));

            yield return null;
        }

        // Ensure final values are set
        anim.SetLayerWeight(0, targetLayer0);
        anim.SetLayerWeight(1, targetLayer1);
    }
    private void EnterStance()
    {
        Debug.Log("Entered Stance");
        if (stance)
            return; // already in stance — don't restart everything

        stance = true;
        canAttackAfterStance = false;

        // Stop any ongoing stance transition or timeout
        if (stanceRoutine != null)
            StopCoroutine(stanceRoutine);
        if (stanceTimeoutRoutine != null)
            StopCoroutine(stanceTimeoutRoutine);

        stanceRoutine = StartCoroutine(SmoothLayerTransition(true));
        stanceTimeoutRoutine = StartCoroutine(StanceTimeout());

        anim.SetBool("Stance", true);

        StartCoroutine(AllowAttacksAfterDelay());
    }

    private void ExitStance()
    {
        stance = false;
        anim.SetBool("Stance", false);
        if (stanceRoutine != null)
            StopCoroutine(stanceRoutine);
        stanceRoutine = StartCoroutine(SmoothLayerTransition(false));
    }

    private IEnumerator StanceTimeout()
    {
        float inactivityTimer = 0f;
        float timeSinceEnteredStance = 0f;

        while (inactivityTimer < stanceDuration)
        {
            bool isBusy = attacking || anim.GetBool("LAttack") || anim.GetBool("HAttack");

            // Increment time since entering stance
            timeSinceEnteredStance += Time.deltaTime;

            // Log for debugging
            //Debug.Log($"Busy: {isBusy}, Time Since Last Attack: {timeSinceLastAttack:F2}, Time Since Entered Stance: {timeSinceEnteredStance:F2}, Inactivity: {inactivityTimer:F2}, StanceDUR: {stanceDuration}, Attacking: {attacking}, Anims(L & H): {anim.GetBool("LAttack")} & {anim.GetBool("HAttack")}");

            // If not busy, allow the inactivity timer to count
            if (!isBusy)
            {
                inactivityTimer += Time.deltaTime;
            }
            else
            {
                inactivityTimer = 0f; // Reset inactivity timer if the player is busy (attacking)
            }

            // Wait for the next frame
            yield return null;
        }

        // Exit stance after timer reaches the limit
        ExitStance();
    }

    private IEnumerator AllowAttacksAfterDelay()
    {
        yield return new WaitForSeconds(stanceEntryDelay);
        canAttackAfterStance = true;
    }

}
