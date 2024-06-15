using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

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
    [SerializeField]
    private GameObject soldOut;

    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string buyScript;
    [SerializeField]
    private string sellScript;

    [SerializeField]
    private FluctType fluctType; 

    private Item item;
    private Item _Item
    {
        get
        {
            if (item == null)
                item = GetComponent<Item>();
            return item;
        }
    }

    Refreshable refresh;

    private ShopUI _ShopUI
    {
        get
        {
            if (shopUI == null)
                shopUI = GetComponentInParent<ShopUI>();
            return shopUI;
        }
    }

    private ReactiveProperty<bool> isSoldOut = new ReactiveProperty<bool>(false);
    public bool IsSoldOut { get => isSoldOut.Value; set => isSoldOut.Value = value; }

    [SerializeField]
    private bool isTileItem = false;
    [SerializeField]
    private bool isRefreshable = true;

    private int _saledPrice { get => Mathf.FloorToInt(curPrice.Value * (1 - PassiveManager.Instance._shopSaleAmount.Value / 100f)); }

    public void BuyItem()
    {
        if (GameManager.Instance.gold < _saledPrice)
        {
            _ShopUI?.PlayScript("Shop034");
            return;
        }

        if(isTileItem && GameManager.Instance.cardDeckController.hand_CardNumber >= 10)
        {
            _ShopUI?.PlayScript("Shop035");
            return;
        }

        GameManager.Instance.gold -= _saledPrice;

        _Item?.UseItem();

        if (!string.IsNullOrEmpty(buyScript))
            _ShopUI?.PlayScript(buyScript);

        isSoldOut.Value = true;
    }

    private int DecreasePrice()
    {
        float changeVal = Random.Range(decreaseMin, decreaseMax);
        return Mathf.RoundToInt(curPrice.Value * changeVal / 100);
    }

    private int IncreasePrice()
    {
        float changeVal = Random.Range(increaseMin, increaseMax);
        return Mathf.RoundToInt(curPrice.Value * changeVal / 100);
    }

    private void RandomFluct()
    {
        int token = Random.Range(0, 2);
        int fluctVal = 0;
        if (token == 0)
            fluctVal -= DecreasePrice();
        else
            fluctVal += IncreasePrice();

        curPrice.Value += fluctVal;
    }

    public override void FluctPrice()
    {
        switch(fluctType)
        {
            case FluctType.Both:
                RandomFluct();
                break;
            case FluctType.IncreaseOnly:
                curPrice.Value += IncreasePrice();
                break;
            case FluctType.DecreaseOnly:
                curPrice.Value -= DecreasePrice();
                break;
        }

        itemPrice.text = curPrice.ToString();
        refresh?.RefreshItem();

        if(isRefreshable)
            isSoldOut.Value = false;
    }

    public override void UpdateCoolTime()
    {
        
    }

    public void SetItem(Sprite icon, string targetName)
    {
        itemIcon.sprite = icon;
        itemName.text = targetName;
        itemPrice.text = curPrice.ToString();
    }

    public void Init()
    {
        refresh = GetComponent<Refreshable>();
        if (buyBtn != null)
            buyBtn.onClick.AddListener(BuyItem);

        curPrice.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        PassiveManager.Instance._shopSaleAmount.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        isSoldOut.Subscribe(_ =>
        {
            buyBtn.gameObject.SetActive(!_);
            soldOut?.SetActive(_);
        });

        curPrice.Value = originPrice;
        refresh?.RefreshItem();
    }
}
