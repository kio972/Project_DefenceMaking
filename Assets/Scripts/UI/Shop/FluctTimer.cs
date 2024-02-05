using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FluctTimer : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private int curTime = 0;
    [SerializeField]
    private int fluctTime = 10;

    [SerializeField]
    private TextMeshProUGUI text;

    private FluctItem[] items;

    public void ResetTime()
    {
        curTime = 0;
    }

    private void FluctPrice()
    {
        if (target == null)
            return;

        if (items == null)
            items = target.GetComponentsInChildren<FluctItem>(true);

        foreach (FluctItem item in items)
            item.FluctPrice();
    }

    public void IncreaseTime()
    {
        curTime++;
        if(curTime >= fluctTime)
        {
            ResetTime();
            FluctPrice();
        }

        UpdateText();
    }

    private void UpdateText()
    {
        if (text != null)
            text.text = (fluctTime - curTime).ToString();
    }

    private void OnEnable()
    {
        UpdateText();
    }
}