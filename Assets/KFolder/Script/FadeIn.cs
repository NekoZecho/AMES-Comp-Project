using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInImage : MonoBehaviour
{
    public Image image; // Reference to the Image
    public float fadeDuration = 2f; // Duration of the fade-in effect

    private void Start()
    {
        // Ensure the image starts fully black (visible)
        Color color = image.color;
        color.a = 1f; // Start with full opacity
        image.color = color;

        // Start the fade-in coroutine
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float timeElapsed = 0f;

        // Fade from black (1f alpha) to transparent (0f alpha)
        while (timeElapsed < fadeDuration)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration); // Lerp the alpha
            image.color = color;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends fully transparent
        Color finalColor = image.color;
        finalColor.a = 0f;
        image.color = finalColor;
    }
}
