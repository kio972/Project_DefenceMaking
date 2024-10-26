using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class RandomCard : MonoBehaviour, Item, IRefreshableItem
{
    [SerializeField]
    private TileType targetType;

    private int targetIndex;
    public int _TargetIndex { get => targetIndex; }
    StringBuilder targetName = new StringBuilder();

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

        string conver = script["script"].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();
        shopUI?.PlayScript("\'" + DataManager.Instance.GetDescription(targetName.ToString()) + conver.Replace('%', '\''), track0, track1);
    }

    public void UseItem()
    {
        GameManager.Instance.cardDeckController.AddCard(targetIndex);
        GameManager.Instance.cardDeckController.DrawCard(targetIndex);
    }

    public void RefreshItem()
    {
        switch (targetType)
        {
            case TileType.Path:
                targetIndex = GetRandomIndex(DataManager.Instance.pathCard_Indexs);
                break;
            case TileType.Room:
                targetIndex = GetRandomIndex(DataManager.Instance.RoomTypeCard_Indexs);
                break;
            case TileType.Environment:
                targetIndex = GetRandomIndex(DataManager.Instance.environmentCard_Indexs);
                break;
        }

        object target = DataManager.Instance.deck_Table[targetIndex]["text_name"];
        targetName.Clear();
        targetName.Append(target.ToString());
        _ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
    }
}
