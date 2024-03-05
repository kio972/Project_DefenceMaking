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

    private bool isSoldOut = false;
    public bool IsSoldOut { get => isSoldOut; }

    [SerializeField]
    private bool isTileItem = false;

    public void BuyItem()
    {
        if (GameManager.Instance.gold < curPrice)
        {
            _ShopUI?.PlayScript("Shop034");
            return;
        }

        if(isTileItem && GameManager.Instance.cardDeckController.hand_CardNumber >= 10)
        {
            _ShopUI?.PlayScript("Shop035");
            return;
        }

        GameManager.Instance.gold -= curPrice;

        _Item?.UseItem();

        if (!string.IsNullOrEmpty(buyScript))
            _ShopUI?.PlayScript(buyScript);

        isSoldOut = true;
        buyBtn.gameObject.SetActive(false);
        soldOut?.SetActive(true);
    }

    private int DecreasePrice()
    {
        float changeVal = Random.Range(decreaseMin, decreaseMax);
        return Mathf.RoundToInt(curPrice * changeVal / 100);
    }

    private int IncreasePrice()
    {
        float changeVal = Random.Range(increaseMin, increaseMax);
        return Mathf.RoundToInt(curPrice * changeVal / 100);
    }

    private void RandomFluct()
    {
        int token = Random.Range(0, 2);
        int fluctVal = 0;
        if (token == 0)
            fluctVal -= DecreasePrice();
        else
            fluctVal += IncreasePrice();

        curPrice += fluctVal;
    }

    public override void FluctPrice()
    {
        switch(fluctType)
        {
            case FluctType.Both:
                RandomFluct();
                break;
            case FluctType.IncreaseOnly:
                curPrice += IncreasePrice();
                break;
            case FluctType.DecreaseOnly:
                curPrice -= DecreasePrice();
                break;
        }

        itemPrice.text = curPrice.ToString();
        refresh?.RefreshItem();

        isSoldOut = false;
        buyBtn.gameObject.SetActive(true);
        soldOut?.SetActive(false);
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

    private void Awake()
    {
        refresh = GetComponent<Refreshable>();
        if(buyBtn != null)
            buyBtn.onClick.AddListener(BuyItem);
        
        curPrice = originPrice;
        refresh?.RefreshItem();
    }
}
