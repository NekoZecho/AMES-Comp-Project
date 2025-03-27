using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Sight_Indicators : MonoBehaviour
{
    [Header("Enemy Detection")]
    Enemy_Sight Sight;
    Image sus;
    Image aware;
    Image aggro;
    bool swapped;

    void Start()
    {
        Sight = GetComponentInParent<Enemy_Sight>();
        aware = transform.GetChild(0).GetComponent<Image>();
        sus = transform.GetChild(1).GetComponent<Image>();
        aggro = transform.GetChild(2).GetComponent<Image>();

        sus.fillAmount = 0;
        aware.fillAmount = 0;

        swapped = false;
    }

    void Update()
    {
        switch (Sight.eState)
        {
            case Enemy_Sight.EnemyState.unaware:
                    aware.enabled = true;
                    sus.enabled = false;
                    aggro.enabled = false;
                aware.fillAmount = Sight.progress;
                break;
        }
    }
}
