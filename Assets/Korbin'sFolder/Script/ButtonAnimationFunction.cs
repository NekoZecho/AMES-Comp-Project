using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnableAnimationOnClick : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Animator animator;

    void Start()
    {
        // Button1 will trigger both animations in sequence.
        button1.onClick.AddListener(() => StartCoroutine(PlayAnimationsSequentially()));
        // Other buttons, if needed, can trigger just the first animation.
        button2.onClick.AddListener(() => TriggerAnimation());
        button3.onClick.AddListener(() => TriggerAnimation());
    }

    // This coroutine will play the animations one after another.
    private IEnumerator PlayAnimationsSequentially()
    {
        // Play the first animation (Transition)
        TriggerAnimation();
        // Wait for the duration of the Transition animation (or any time you want before triggering the next)
        yield return new WaitForSeconds(1f); // Adjust this value to match the duration of your Transition animation

        // Play the second animation (DiffTransition)
        DiffTriggerAnimation();
    }

    public void TriggerAnimation()
    {
        animator.SetTrigger("Transition");
    }

    public void DiffTriggerAnimation()
    {
        animator.SetTrigger("DiffTransition");
    }
}
