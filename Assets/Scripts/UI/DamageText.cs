using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI damageText;

    public void StartEffect(string value, float fontSize, Color color, bool isBold)
    {
        damageText.fontSize = fontSize;
        damageText.color = color;
        if(isBold)
            damageText.text = "<b>" + value + "</b>";
        else
            damageText.text = value;
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
