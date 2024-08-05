using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotInfo : MonoBehaviour
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

    private ItemSlot curslot;

    private bool _isSoldOut;
    [SerializeField]
    private Sprite[] btnSprites;
    [SerializeField]
    private Sprite[] bgSprites;

    [SerializeField]
    private ItemSlot initSlot;

    private void SetSoldOutImg(bool isSoldOut)
    {
        int index = isSoldOut ? 1 : 0;
        buyBtn.sprite = btnSprites[index];
        bgImg.sprite = bgSprites[index];
        buyText.text = isSoldOut ? "품절" : "구매";
    }

    public void UpdateInfo(ItemSlot data)
    {
        curslot = data;

        card_Name.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemName);
        card_Description.ChangeLangauge(SettingManager.Instance.language, data.itemInfo.itemDesc);

        card_illust.sprite = data.ItemIcon.sprite;

        card_Cost.text = data._CurPrice.ToString();

        _isSoldOut = data.IsSoldOut;
        SetSoldOutImg(_isSoldOut);
    }

    public void Buy()
    {
        if (_isSoldOut)
            return;

        curslot.BuyItem();
        if (curslot.IsSoldOut)
        {
            _isSoldOut = true;
            SetSoldOutImg(true);
        }
    }

    private void OnEnable()
    {
        if (initSlot != null)
            UpdateInfo(initSlot);
    }
}
