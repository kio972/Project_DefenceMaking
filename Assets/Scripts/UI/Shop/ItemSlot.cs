using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : FluctItem
{
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemPrice;
    [SerializeField]
    private Button buyBtn;

    public override void FluctPrice()
    {
        curPrice = Mathf.RoundToInt((float)curPrice * 1.2f);
    }

    public override void UpdateCoolTime()
    {
        if (coolStartWave == -1)
            return;

        int curWave = GameManager.Instance.CurWave;
        if (curWave >= coolStartWave + coolTime)
            coolStartWave = GameManager.Instance.CurWave;
    }
}
