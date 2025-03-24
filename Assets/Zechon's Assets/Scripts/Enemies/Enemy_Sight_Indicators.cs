using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Sight_Indicators : MonoBehaviour
{
    [Header("Enemy Detection")]
    Enemy_Sight Sight;
    Image unaware;
    Image sus;
    Image aware;
    Image aggro;

    void Start()
    {
        Sight = GetComponentInParent<Enemy_Sight>();
        unaware = transform.GetChild(0).GetComponent<Image>();
        sus = transform.GetChild(1).GetComponent<Image>();
        aware = transform.GetChild(2).GetComponent<Image>();
        aggro = transform.GetChild(3).GetComponent<Image>();

        sus.fillAmount = 0;
    }

    void Update()
    {
        switch (Sight.eState)
        {
            case Enemy_Sight.EnemyState.unaware:
                    unaware.enabled = true;
                    sus.enabled = true;
                    aware.enabled = false;
                    aggro.enabled = false;
                    sus.fillAmount = Sight.progress;
                
                break;

            case Enemy_Sight.EnemyState.suspicious:
                unaware.enabled = false;
                sus.enabled = true;
                aware.enabled = false;
                aggro.enabled = false;
                break;
        }
    }
}
