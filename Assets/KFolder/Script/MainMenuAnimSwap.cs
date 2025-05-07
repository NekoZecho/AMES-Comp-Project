using UnityEngine;

public class AnimationEventSwap : MonoBehaviour
{
    private Animator animator;

    // Set this in the Unity Inspector with the name of the animation to switch to
    public string newAnimationName;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    // Function to swap to the new animation
    public void SwapAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(newAnimationName))
        {
            animator.Play(newAnimationName);
            
        }
    }
}
