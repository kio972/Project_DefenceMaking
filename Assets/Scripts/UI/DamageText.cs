using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI damageText;

    public void StartEffect(int value, Color color, bool isHeal)
    {
        string sign = "";
        if (isHeal)
            sign = "+";
        else
            sign = "-";
        damageText.color = color;
        damageText.text = sign + value.ToString();
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
