using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public struct ItemInfo
{
    public string itemId;
    public string itemName;
    public string itemDesc;
    public int originPrice;
    public int curPrice;
    public float increaseMin;
    public float increaseMax;
    public float decreaseMin;
    public float decreaseMax;

    public ItemInfo(Dictionary<string, object> data)
    {
        itemId = data["id"].ToString();
        itemName = data["Name"].ToString();
        itemDesc = data["Desc"].ToString();
        originPrice = System.Convert.ToInt32(data["Price"]);
        curPrice = originPrice;
        float.TryParse(data["IncreaseMin"].ToString(), out increaseMin);
        float.TryParse(data["IncreaseMax"].ToString(), out increaseMax);
        float.TryParse(data["DecreaseMin"].ToString(), out decreaseMin);
        float.TryParse(data["DecreaseMax"].ToString(), out decreaseMax);
    }
}

public class ItemSlot : FluctItem
{
    [SerializeField]
    private string itemId;
    public string ItemId { get => itemId; }

    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemPrice;
    [SerializeField]
    private Button btn;
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

    public Image ItemIcon { get => itemIcon; }

    public ItemInfo itemInfo { get; private set; }
    private Item _Item
    {
        get
        {
            if (item == null)
                item = GetComponent<Item>();
            return item;
        }
    }

    IRefreshableItem refresh;

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

    [SerializeField]
    private ShopSlotInfo slotInfo;

    public void OnClick()
    {
        if (item is IMalPoongSunOnClick script)
            script.PlayOnClickScript();

        slotInfo?.UpdateInfo(this);
    }

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
        slotInfo?.UpdateInfo(this);
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

    private void SetItemInfo()
    {
        if (string.IsNullOrEmpty(itemId))
            return;

        itemInfo = new ItemInfo(DataManager.Instance.shopListDic[itemId]);
        increaseMin = itemInfo.increaseMin;
        increaseMax = itemInfo.increaseMax;
        decreaseMin = itemInfo.decreaseMin;
        decreaseMax = itemInfo.decreaseMax;
        originPrice = itemInfo.originPrice;
        curPrice.Value = itemInfo.curPrice;
    }

    public void Init()
    {
        SetItemInfo();

        refresh = GetComponent<IRefreshableItem>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);

        curPrice.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        PassiveManager.Instance._shopSaleAmount.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        isSoldOut.Subscribe(_ => soldOut?.SetActive(_));

        curPrice.Value = originPrice;
        refresh?.RefreshItem();
    }
}
