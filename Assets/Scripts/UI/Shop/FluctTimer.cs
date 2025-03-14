using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FluctTimer : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private int _curTime = 0;
    [SerializeField]
    private int fluctTime = 10;
    [SerializeField]
    private int _curRefreashTime = 0;
    [SerializeField]
    private int refreashTime = 3;

    public int CurTime { get => _curTime; set => _curTime = value; }
    public int CurRefreshTime { get => _curRefreashTime; set => _curRefreashTime = value; }

    [SerializeField]
    private TextMeshProUGUI text;

    private FluctItem[] items;

    public string refreshMessage;
    public string fluctMessage;

    [SerializeField]
    private AK.Wwise.Event fluctSound;

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
                slot.RefreshStock();

            IRefreshableItem refreshableItem = item.GetComponent<IRefreshableItem>();
            if (refreshableItem != null)
                refreshableItem.RefreshItem();
        }

        if (!string.IsNullOrEmpty(refreshMessage))
        {
            GameManager.Instance.notificationBar?.SetMesseage(DataManager.Instance.GetDescription(refreshMessage), NotificationType.Shop);
            //AudioManager.Instance.Play2DSound("Complete_Tech", SettingManager.Instance._FxVolume);
            fluctSound?.Post(gameObject);
        }
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
            GameManager.Instance.notificationBar?.SetMesseage(DataManager.Instance.GetDescription(fluctMessage), NotificationType.Shop);
    }

    public void IncreaseTime()
    {
        _curTime++;
        _curRefreashTime++;

        if(_curRefreashTime >= refreashTime)
        {
            _curRefreashTime = 0;
            RefreshItem();
        }

        if (_curTime >= fluctTime)
        {
            _curTime = 0;
            FluctPrice();
        }

        UpdateCoolTime();
        UpdateText();
    }

    private void UpdateText()
    {
        if (text != null)
            text.text = (refreashTime - _curRefreashTime).ToString();
    }

    private void OnEnable()
    {
        UpdateText();
    }
}