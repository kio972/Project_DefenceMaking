using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotInfo : MonoBehaviour, ISlotInformer
{
    [SerializeField]
    private Image card_illust;
    [SerializeField]
    private LanguageText card_Name;
    [SerializeField]
    private LanguageText card_Description;

    [SerializeField]
    private Image buyBtn;
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private TextMeshProUGUI buyText;
    [SerializeField]
    private TextMeshProUGUI card_Cost;

    private ItemSlot _curslot;
    public ISlot curSlot { get => _curslot; }

    private bool _isSoldOut;
    [SerializeField]
    private Sprite[] btnSprites;
    [SerializeField]
    private Sprite[] bgSprites;
    [SerializeField]
    private GameObject soldOutImage;

    //[SerializeField]
    //private ItemSlot initSlot;

    [SerializeField]
    private GameObject cardPackViewBtn;
    [SerializeField]
    private CardPackView cardPackView;

    [SerializeField]
    private TextMeshProUGUI stockText;

    private void UpdateText()
    {
        buyText.text = DataManager.Instance.GetDescription(_isSoldOut ? "ui_SoldOut" : "ui_Purchase");
        if(curSlot != null)
            stockText.text = $"{DataManager.Instance.GetDescription("ui_Stock")} : {_curslot.curStockCount}";
    }

    public void ViewCardPack()
    {
        if(_curslot.item is ICardPackList cardPack)
        {
            cardPackView?.SetCardList(cardPack.targetCards);
        }
    }

    private void SetSoldOutImg(bool isSoldOut)
    {
        int index = isSoldOut ? 1 : 0;
        buyBtn.sprite = btnSprites[index];
        //bgImg.sprite = bgSprites[index];
        buyText.text = DataManager.Instance.GetDescription(isSoldOut ? "ui_SoldOut" : "ui_Purchase");
        soldOutImage.SetActive(isSoldOut);
    }

    public void UpdateInfo(ItemSlot data, object additional = null)
    {
        _curslot?.SetClicked(false);
        _curslot = data;
        _curslot?.SetClicked(true);

        if(additional != null)
        {
            card_Name.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemName, additional);
            card_Description.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemDesc, additional);
        }
        else
        {
            card_Name.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemName);
            card_Description.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemDesc);
        }

        card_illust.sprite = data.ItemIcon.sprite;

        card_Cost.text = data._CurPrice.ToString();

        _isSoldOut = data.IsSoldOut;
        SetSoldOutImg(_isSoldOut);
        cardPackViewBtn.SetActive(_curslot.item is ICardPackList);
        stockText.text = $"{DataManager.Instance.GetDescription("ui_Stock")} : {data.curStockCount}";
    }
    public void ExcuteAction()
    {
        Buy();
    }

    public void Buy()
    {
        if (_isSoldOut)
            return;

        _curslot.BuyItem();
        if (_curslot.IsSoldOut)
        {
            _isSoldOut = true;
            SetSoldOutImg(true);
        }
    }

    private void OnEnable()
    {
        ItemSlot targetSlot = null;
        foreach(var slot in GameManager.Instance.shop.itemSlots)
        {
            if(slot.gameObject.activeSelf)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot != null)
            UpdateInfo(targetSlot);
    }

    private void Start()
    {
        LanguageManager.Instance.AddLanguageAction(() => UpdateText());
    }
}
