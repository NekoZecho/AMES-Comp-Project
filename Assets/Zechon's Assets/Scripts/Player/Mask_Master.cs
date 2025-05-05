using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask_Master : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private GameObject StealthMask;

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
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // keep physics in sync
        maskWheelUI.SetActive(true);
        isMaskWheelActive = true;

        // Optionally: Lock camera and player movement here
    }

    void DeactivateMaskWheel()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        maskWheelUI.SetActive(false);
        isMaskWheelActive = false;

    }
}
