using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class MagicPowerUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    private int curUsed;
    private int curMax;
    
    StringBuilder targetString = new StringBuilder();

    public void UpdateUI()
    {
        targetString.Clear();
        targetString.Append(GameManager.Instance._CurMana.ToString());
        targetString.Append(" / ");
        targetString.Append(GameManager.Instance._TotalMana.ToString());
        text.text = targetString.ToString();

        curUsed = GameManager.Instance._CurMana;
        curMax = GameManager.Instance._TotalMana;
    }

    private void Update()
    {
        if(curUsed != GameManager.Instance._CurMana || curMax != GameManager.Instance._TotalMana)
            UpdateUI();
    }
}
