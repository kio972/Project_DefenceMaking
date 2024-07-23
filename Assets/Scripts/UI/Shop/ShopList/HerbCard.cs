using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HerbCard : MonoBehaviour, Item, Refreshable
{
    [SerializeField]
    private bool isSuperial;
    [SerializeField]
    private int number = 3;

    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;

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
        List<int> targets = new List<int>();
        string targetGrade = isSuperial ? "rare" : "normal";
        foreach(int herb in DataManager.Instance.HerbCard_Indexs)
        {
            if (DataManager.Instance.Deck_Table[herb]["grade"].ToString() == targetGrade)
                targets.Add(herb);
        }

        for(int i = 0; i < number; i++)
        {
            int cardIndex = GetRandomIndex(targets);
            curCards.Add(new Card(DataManager.Instance.Deck_Table[cardIndex], cardIndex));
        }

        //_ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.Deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
    }
}