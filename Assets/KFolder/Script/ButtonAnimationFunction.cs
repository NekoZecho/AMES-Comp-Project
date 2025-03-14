using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnableAnimationOnClick : MonoBehaviour
{
    // ========================================
    // PUBLIC VARIABLES (To be set in Inspector)
    // ========================================

    // UI Buttons
    public Button playButton;   // Play button reference
    public Button optionsButton; // Options button reference
    public Button quitButton;    // Quit button reference

    // Animator reference
    public Animator animator;

    // Boolean parameters for controlling transitions and animations
    public string playBool = "PlayTransition";    // Bool parameter name for the Play button transition
    public string optionsBool = "OptionsTransition";  // Bool parameter name for the Options button transition
    public string quitBool = "QuitTransition";    // Bool parameter name for the Quit button transition
    public string playDiffBool = "PlayDiff";    // Bool parameter for second animation of Play
    public string optionsDiffBool = "OptionsDiff"; // Bool parameter for second animation of Options
    public string quitDiffBool = "QuitDiff";    // Bool parameter for second animation of Quit

    // ========================
    // UNITY START METHOD
    // ========================

    void Start()
    {
        // Adding listeners to each button
        playButton.onClick.AddListener(() => StartCoroutine(ButtonClicked(playBool, playDiffBool)));
        optionsButton.onClick.AddListener(() => StartCoroutine(ButtonClicked(optionsBool, optionsDiffBool)));
        quitButton.onClick.AddListener(() => StartCoroutine(QuitButtonClicked()));
    }

    // ==============================
    // BUTTON CLICKED METHODS
    // ==============================

    // Coroutine for Play/Options button clicked
    private IEnumerator ButtonClicked(string initialBool, string diffBool)
    {
        // Set the initial bool to true (start the transition)
        animator.SetBool(initialBool, true);

        // Wait for the initial animation to finish
        float transitionDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(transitionDuration);

        // Now trigger the second animation by setting the second bool to true
        animator.SetBool(diffBool, true);
    }

    // Coroutine for Quit button clicked
    private IEnumerator QuitButtonClicked()
    {
        // Set the QuitTransition bool to true
        animator.SetBool(quitBool, true);

        // Wait for the QuitTransition to finish
        float transitionDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(transitionDuration);

        // Quit the game after the QuitTransition animation
        QuitGame();
    }

    // ==============================
    // GAME QUIT METHOD
    // ==============================

    // Method to quit the game (for Quit button)
    private void QuitGame()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }

    // ==============================
    // ANIMATION EVENT FUNCTION
    // ==============================

    // Animation Event function to reset the initial bool to false
    public void ResetTransitionBool(string transitionBool)
    {
        animator.SetBool(transitionBool, false);
    }
}
