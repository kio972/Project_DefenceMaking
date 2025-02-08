using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class RandomCard : MonoBehaviour, Item, IRefreshableItem, IInfoChangeableItem
{
    //[SerializeField]
    //private TileType targetType;
    [SerializeField]
    private Image card_frame;

    [SerializeField]
    private Sprite[] frameSprites;

    private int GetFrameIndex(CardType tileType, bool isAdd)
    {
        if (!isAdd)
            return 0;

        if (tileType is CardType.PathTile)
            return 1;
        if (tileType is CardType.RoomTile)
            return 2;
        if (tileType is CardType.Environment)
            return 3;

        return 4;

    }

    private int targetIndex;
    public int _TargetIndex { get => targetIndex; }
    StringBuilder targetName = new StringBuilder();

    private bool isAdd;

    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;

    ItemSlot itemSlot;
    ItemSlot _ItemSlot
    {
        get
        {
            if (itemSlot == null)
                itemSlot = GetComponent<ItemSlot>();
            return itemSlot;
        }
    }

    Card _curCard;

    public ItemInfo Info
    {
        get
        {
            string itemId = isAdd ? "shop_item0033" : "shop_item0034";
            return new ItemInfo(DataManager.Instance.shopListDic[itemId]);
        }
    }

    public object additional
    {
        get
        {
            return DataManager.Instance.GetDescription(_curCard.cardName);
        }
    }

    private int GetRandomIndex(List<int> target)
    {
        int randomIndex = Random.Range(0, target.Count);

        return target[randomIndex];
    }

    public void OnClick()
    {
        if (_ItemSlot.IsSoldOut)
            return;

        Dictionary<string, object> script = DataManager.Instance.GetMalpoongsunScript(descScript);
        if (script == null)
            return;

        string conver = script[SettingManager.Instance.language.ToString()].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();
        shopUI?.PlayScript("\'" + DataManager.Instance.GetDescription(targetName.ToString()) + conver.Replace('%', '\''), track0, track1);
    }

    public void UseItem()
    {
        if(isAdd)
            GameManager.Instance.cardDeckController.AddCard(targetIndex);
        else
            GameManager.Instance.cardDeckController.RemoveCard(targetIndex);
    }

    private void OnEnable()
    {
        if (itemSlot.IsSoldOut)
            return;

        if(!isAdd && !GameManager.Instance.cardDeckController.cardDeck.Contains(_curCard.cardIndex))
        {
            _curCard = GameManager.Instance.cardSelector.GetDeckRandomCard();
            RefreshItem();
        }
    }

    private void UpdateCard()
    {
        targetIndex = _curCard.cardIndex;
        card_frame.sprite = frameSprites[GetFrameIndex(_curCard.cardType, isAdd)];
        object target = DataManager.Instance.deck_Table[targetIndex]["text_name"];
        targetName.Clear();
        targetName.Append(target.ToString());
        _ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
        _ItemSlot?.ForceUpdateItemInfo(Info);
    }

    public void RefreshItem()
    {
        //switch (targetType)
        //{
        //    case TileType.Path:
        //        targetIndex = GetRandomIndex(DataManager.Instance.pathCard_Indexs);
        //        break;
        //    case TileType.Room:
        //        targetIndex = GetRandomIndex(DataManager.Instance.RoomTypeCard_Indexs);
        //        break;
        //    case TileType.Environment:
        //        targetIndex = GetRandomIndex(DataManager.Instance.environmentCard_Indexs);
        //        break;
        //}

        isAdd = UnityEngine.Random.Range(0, 2) == 1;
        _curCard = isAdd ? GameManager.Instance.cardSelector.GetRandomCard() : GameManager.Instance.cardSelector.GetDeckRandomCard();
        UpdateCard();
    }
}
