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

    private int curRefreashTime = 0;
    [SerializeField]
    private int refreashTime = 3;

    public int _CurTime { get => curTime; set => curTime = value; }

    [SerializeField]
    private TextMeshProUGUI text;

    private FluctItem[] items;

    public string refreshMessage;
    public string fluctMessage;

    private void UpdateCoolTime()
    {
        if (target == null)
            return;

        if (items == null)
            items = target.GetComponentsInChildren<FluctItem>(true);

        foreach (FluctItem item in items)
            item.UpdateCoolTime();
    }

    private void RefreshItem()
    {
        if (target == null)
            return;

        if (items == null)
            items = target.GetComponentsInChildren<FluctItem>(true);

        foreach (FluctItem item in items)
        {
            ItemSlot slot = item.GetComponent<ItemSlot>();
            if (slot != null && slot.IsRefreshable)
                slot.IsSoldOut = false;

            IRefreshableItem refreshableItem = item.GetComponent<IRefreshableItem>();
            if (refreshableItem != null)
                refreshableItem.RefreshItem();
        }

        if (!string.IsNullOrEmpty(fluctMessage))
            GameManager.Instance.notificationBar?.SetMesseage(refreshMessage, NotificationType.Shop);
    }

    private void FluctPrice()
    {
        if (target == null)
            return;

        if (items == null)
            items = target.GetComponentsInChildren<FluctItem>(true);

        foreach (FluctItem item in items)
            item.FluctPrice();

        if (!string.IsNullOrEmpty(fluctMessage))
            GameManager.Instance.notificationBar?.SetMesseage(fluctMessage, NotificationType.Shop);
    }

    public void IncreaseTime()
    {
        curTime++;
        curRefreashTime++;

        if(curRefreashTime >= refreashTime)
        {
            curRefreashTime = 0;
            RefreshItem();
        }

        if (curTime >= fluctTime)
        {
            curTime = 0;
            FluctPrice();
        }

        UpdateCoolTime();
        UpdateText();
    }

    private void UpdateText()
    {
        if (text != null)
            text.text = (refreashTime - curRefreashTime).ToString();
    }

    private void OnEnable()
    {
        UpdateText();
    }
}