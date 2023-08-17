using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI damageText;

    public void StartEffect(int value, bool isHeal = false)
    {
        string sign = "";
        if (isHeal)
            sign = "+";
        else
            sign = "-";
        damageText.text = sign + value.ToString();
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
