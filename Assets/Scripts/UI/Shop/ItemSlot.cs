using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEditor.Rendering.LookDev;

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
    public int stockCount;

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
        int.TryParse(data["Stock"].ToString(), out stockCount);
    }
}

public class ItemSlot : FluctItem, ISlot
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
    private ShopUI _shopUI;

    [SerializeField]
    private string buyScript;
    [SerializeField]
    private string soldOutScript;
    [SerializeField]
    private string sellScript;

    [SerializeField]
    private FluctType fluctType;

    private Item _item;

    public Image ItemIcon { get => itemIcon; }

    public ItemInfo itemInfo { get; private set; }
    public Item item
    {
        get
        {
            if (_item == null)
                _item = GetComponent<Item>();
            return _item;
        }
    }

    private ShopUI shopUI
    {
        get
        {
            if (_shopUI == null)
                _shopUI = GetComponentInParent<ShopUI>();
            return _shopUI;
        }
    }

    private ReactiveProperty<bool> isSoldOut = new ReactiveProperty<bool>(false);
    public bool IsSoldOut { get => isSoldOut.Value; set => isSoldOut.Value = value; }

    [SerializeField]
    private bool isTileItem = false;
    [SerializeField]
    private bool isRefreshable = true;
    public bool IsRefreshable { get => isRefreshable; }

    private int _saledPrice { get => Mathf.FloorToInt(curPrice.Value * (1 - PassiveManager.Instance._shopSaleAmount.Value / 100f)); }

    [SerializeField]
    private ShopSlotInfo slotInfo;

    [SerializeField]
    private int _stockCount = 1;
    private int _curStockCount;
    public int curStockCount { get => _curStockCount; }

    private void Update()
    {
        Color targetColor = GameManager.Instance.gold >= curPrice.Value ? Color.white : Color.red;
        itemPrice.color = targetColor;
    }

    public void RefreshStock()
    {
        _curStockCount = _stockCount;
        isSoldOut.Value = false;
    }

    public void OnClick()
    {
        if (_item is IMalPoongSunOnClick script)
            script.PlayOnClickScript();

        slotInfo?.UpdateInfo(this);
    }

    public void SendInfo()
    {
        OnClick();
    }

    public void BuyItem()
    {
        if (GameManager.Instance.gold < _saledPrice)
        {
            shopUI?.PlayScript("Shop034");
            AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            return;
        }

        if(isTileItem && GameManager.Instance.cardDeckController.hand_CardNumber >= 10)
        {
            shopUI?.PlayScript("Shop035");
            AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            return;
        }

        GameManager.Instance.gold -= _saledPrice;

        _curStockCount--;
        item?.UseItem();
        shopUI?.InvokeBuyItemEvents(item);

        if (!string.IsNullOrEmpty(buyScript))
            shopUI?.PlayScript(buyScript);

        if(_curStockCount <= 0)
            isSoldOut.Value = true;
        slotInfo?.UpdateInfo(this);
        AudioManager.Instance.Play2DSound("UI_Shop_Buy", SettingManager.Instance._UIVolume);
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
        _stockCount = itemInfo.stockCount;
    }

    public void Init()
    {
        SetItemInfo();

        
        if (btn != null)
            btn.onClick.AddListener(OnClick);

        curPrice.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        PassiveManager.Instance._shopSaleAmount.Subscribe(_ => itemPrice.text = _saledPrice.ToString());
        isSoldOut.Subscribe(_ => soldOut?.SetActive(_));

        curPrice.Value = originPrice;
        _curStockCount = _stockCount;
        IRefreshableItem refresh = GetComponent<IRefreshableItem>();
        refresh?.RefreshItem();
    }
}
