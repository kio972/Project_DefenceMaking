using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HerbCard : MonoBehaviour, Item, IRefreshableItem, ICardPackList
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

    private List<int> _targetCards;

    public List<int> targetCards
    {
        get
        {
            if (_targetCards == null)
                UpdateTargetCards();
            return _targetCards;
        }
    }

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

    private void UpdateTargetCards()
    {
        _targetCards = new List<int>();
        string targetGrade = isSuperial ? "rare" : "normal";
        foreach (int herb in DataManager.Instance.herbCard_Indexs)
        {
            if (DataManager.Instance.deck_Table[herb]["grade"].ToString() == targetGrade)
                _targetCards.Add(herb);
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
        //shopUI?.PlayScript("\'" + DataManager.Instance.GetDescription(targetName.ToString()) + conver.Replace('%', '\''), track0, track1);
    }

    public void UseItem()
    {
        foreach (Card curCard in curCards)
            GameManager.Instance.cardDeckController.AddCard(curCard.cardIndex);
        packEffect?.SetCardPackSprite(transform);
        packEffect?.ShowEffect(curCards);
    }

    public void RefreshItem()
    {
        curCards = new List<Card>();
        
        for(int i = 0; i < number; i++)
        {
            int cardIndex = GetRandomIndex(targetCards);
            curCards.Add(new Card(DataManager.Instance.deck_Table[cardIndex], cardIndex));
        }

        //_ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.Deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
    }
}