using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public class CardPack : MonoBehaviour, Item, Refreshable
{
    [SerializeField]
    private int pathNumber;
    [SerializeField]
    private int roomNumber;
    [SerializeField]
    private int roomPartNumber;
    [SerializeField]
    private int environmentNumber;

    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;

    //[SerializeField]
    //private string targetName;
    List<Card> curCards = new List<Card>();

    [SerializeField]
    CardPackEffect packEffect;

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
        //shopUI?.PlayScript("\'" + DataManager.Instance.GetDescription(targetName.ToString()) + conver.Replace('%', '\''), track0, track1);
    }

    public void UseItem()
    {
        foreach (Card curCard in curCards)
            GameManager.Instance.cardDeckController.AddCard(curCard.cardIndex);
        packEffect?.ShowEffect(curCards);
    }

    public void RefreshItem()
    {
        curCards.Clear();
        SetCards(pathNumber, roomNumber, roomPartNumber, environmentNumber);

        //_ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.Deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
    }

    private void SetCards(int path, int singleRoom, int partRoom, int environment)
    {
        List<int> cardIndexs = new List<int>();
        for (int i = 0; i < path; i++)
            cardIndexs.Add(GetRandomIndex(DataManager.Instance.PathCard_Indexs));
        for (int i = 0; i < singleRoom; i++)
            cardIndexs.Add(GetRandomIndex(DataManager.Instance.RoomCard_Indexs));
        for (int i = 0; i < partRoom; i++)
            cardIndexs.Add(GetRandomIndex(DataManager.Instance.RoomPartCard_Indexs));
        for (int i = 0; i < environment; i++)
            cardIndexs.Add(GetRandomIndex(DataManager.Instance.EnvironmentCard_Indexs));

        foreach (int cardIndex in cardIndexs)
            curCards.Add(new Card(DataManager.Instance.Deck_Table[cardIndex], cardIndex));

        curCards = UtilHelper.ShuffleList(curCards);
    }
}