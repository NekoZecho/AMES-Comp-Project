using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_System : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode lightAttack = KeyCode.Mouse0;
    public KeyCode heavyAttack = KeyCode.Mouse1;
    public KeyCode block = KeyCode.F;
    public KeyCode rdodge = KeyCode.R;

    [Header("Cooldowns / Debounces")]
    [SerializeField]
    float LAttackDuration;
    [SerializeField]
    float HAttackDuration;
    [SerializeField]
    float blockRecup;
    [SerializeField]
    float decisionDuration;

    [Header("Hits Info")]
    public int consecutiveHits;
    public int lightHits;
    public int heavyHits;
    public int blockedHits;

    [Header("Components")]
    Animator animator;
    Collider RightHand;
    Collider LeftHand;

    [Header("Bools")]
    public bool attacking;
    public bool blocking;
    public bool staggered;
    public bool dodging;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
}
