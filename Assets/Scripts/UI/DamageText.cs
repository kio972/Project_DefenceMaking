using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI damageText;

    public void StartEffect(int value, float fontSize, Color color, bool isBold, bool isHeal)
    {
        string sign = "";
        if (isHeal)
            sign = "+";
        else
            sign = "-";
        damageText.fontSize = fontSize;
        damageText.color = color;
        if(isBold)
            damageText.text = "<b>" + sign + value.ToString() + "</b>";
        else
            damageText.text = sign + value.ToString();
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
