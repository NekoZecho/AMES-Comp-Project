using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Sight_Indicators : MonoBehaviour
{
    [Header("Enemy Detection")]
    Enemy_Sight Sight;
    Image sus;
    Image aware;
    Image aggro;

    void Start()
    {
        Sight = GetComponentInParent<Enemy_Sight>();
        aware = transform.GetChild(0).GetComponent<Image>();
        sus = transform.GetChild(1).GetComponent<Image>();
        aggro = transform.GetChild(2).GetComponent<Image>();

        sus.fillAmount = 0;
        aware.fillAmount = 0;
        aggro.fillAmount = 0;
    }

    void Update()
    {
        aware.fillAmount = Sight.awareProgress;
        sus.fillAmount = Sight.susProgress;
        aggro.fillAmount = Sight.aggroProgress;

        if (aggro.fillAmount > 0)
        {
            aware.enabled = false;
            sus.enabled = false;
        }

        else
        {
            aware.enabled = true;
            sus.enabled = true;
        }
    }
}
