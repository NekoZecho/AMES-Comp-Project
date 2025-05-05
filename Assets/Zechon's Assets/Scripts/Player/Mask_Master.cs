using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mask_Master : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private GameObject StealthMask;
    [SerializeField] private MaskWheelUI maskWheelScript;

    [Header("UI Setup")]
    [SerializeField] private GameObject maskWheelUI;
    [SerializeField] private float timeSlowFactor = 0.1f;
    private bool isMaskWheelActive = false;

    [Header("Keybinds")]
    public KeyCode maskSwap = KeyCode.Q;

    [Header("Stealth Mask Settings")]
    [SerializeField] private float stealthPercent;
    [SerializeField] private float maskDonDoffTime;

    void Start()
    {
        StealthMask.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown(maskSwap))
        {
            ActivateMaskWheel();
        }
        else if (Input.GetKeyUp(maskSwap))
        {
            DeactivateMaskWheel();
        }
    }

    void ActivateMaskWheel()
    {
        Time.timeScale = timeSlowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        maskWheelUI.SetActive(true);
        maskWheelScript.ShowWheel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isMaskWheelActive = true;
    }

    void DeactivateMaskWheel()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        maskWheelScript.HideWheel(); // fades it out
        StartCoroutine(HideUIAfterFade());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMaskWheelActive = false;

        string selected = maskWheelScript.GetSelectedMaskID();
        //EquipMask(selected);
    }

    IEnumerator HideUIAfterFade()
    {
        yield return new WaitForSecondsRealtime(maskWheelScript.fadeDuration);
        maskWheelUI.SetActive(false);
    }
}
