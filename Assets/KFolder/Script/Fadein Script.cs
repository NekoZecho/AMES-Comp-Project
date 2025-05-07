using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FadeinScript : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public float fadeInSpeed = 1f;
    public float delayBeforeStart = 2f;

    private Image button1Image, button2Image, button3Image;
    private Text button1Text, button2Text, button3Text;
    private TextMeshProUGUI button1TextTMP, button2TextTMP, button3TextTMP;

    void Start()
    {
        button1Image = button1.GetComponent<Image>();
        button2Image = button2.GetComponent<Image>();
        button3Image = button3.GetComponent<Image>();

        button1Text = button1.GetComponentInChildren<Text>();
        button2Text = button2.GetComponentInChildren<Text>();
        button3Text = button3.GetComponentInChildren<Text>();

        button1TextTMP = button1.GetComponentInChildren<TextMeshProUGUI>();
        button2TextTMP = button2.GetComponentInChildren<TextMeshProUGUI>();
        button3TextTMP = button3.GetComponentInChildren<TextMeshProUGUI>();

        button1Image.color = new Color(button1Image.color.r, button1Image.color.g, button1Image.color.b, 0f);
        button2Image.color = new Color(button2Image.color.r, button2Image.color.g, button2Image.color.b, 0f);
        button3Image.color = new Color(button3Image.color.r, button3Image.color.g, button3Image.color.b, 0f);

        if (button1Text != null)
            button1Text.color = new Color(button1Text.color.r, button1Text.color.g, button1Text.color.b, 0f);
        if (button2Text != null)
            button2Text.color = new Color(button2Text.color.r, button2Text.color.g, button2Text.color.b, 0f);
        if (button3Text != null)
            button3Text.color = new Color(button3Text.color.r, button3Text.color.g, button3Text.color.b, 0f);

        if (button1TextTMP != null)
            button1TextTMP.color = new Color(button1TextTMP.color.r, button1TextTMP.color.g, button1TextTMP.color.b, 0f);
        if (button2TextTMP != null)
            button2TextTMP.color = new Color(button2TextTMP.color.r, button2TextTMP.color.g, button2TextTMP.color.b, 0f);
        if (button3TextTMP != null)
            button3TextTMP.color = new Color(button3TextTMP.color.r, button3TextTMP.color.g, button3TextTMP.color.b, 0f);

        StartCoroutine(FadeInButtonsCoroutine());
    }

    IEnumerator FadeInButtonsCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        StartCoroutine(FadeInButton(button1Image, button1Text, button1TextTMP));
        yield return new WaitForSeconds(fadeInSpeed);

        StartCoroutine(FadeInButton(button2Image, button2Text, button2TextTMP));
        yield return new WaitForSeconds(fadeInSpeed);

        StartCoroutine(FadeInButton(button3Image, button3Text, button3TextTMP));
    }

    IEnumerator FadeInButton(Image buttonImage, Text buttonText, TextMeshProUGUI buttonTextTMP)
    {
        float timeElapsed = 0f;
        Color startColor = buttonImage.color;
        Color startTextColor = buttonText != null ? buttonText.color : buttonTextTMP.color;

        while (timeElapsed < fadeInSpeed)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeInSpeed);
            buttonImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            if (buttonText != null)
                buttonText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, alpha);
            if (buttonTextTMP != null)
                buttonTextTMP.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, alpha);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        buttonImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        if (buttonText != null)
            buttonText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 1f);
        if (buttonTextTMP != null)
            buttonTextTMP.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 1f);
    }
}
