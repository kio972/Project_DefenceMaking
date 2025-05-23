using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public enum CardPackType
{
    None,
    Path,
    Room,
    Environment,
}

public interface ICardPackList
{
    List<int> targetCards { get; }
}

public class CardPack : MonoBehaviour, Item, IRefreshableItem, INeedUnlockItem, ICardPackList
{
    public CardPackType cardType;

    [SerializeField]
    private int pathNumber;
    [SerializeField]
    private int roomNumber;
    [SerializeField]
    private int roomPartNumber;
    [SerializeField]
    private int environmentNumber;

    [SerializeField]
    private List<string> _targetPathIds;
    [SerializeField]
    private List<string> _targetRoomIds;
    [SerializeField]
    private List<string> _targetRoomPartIds;
    [SerializeField]
    private List<string> _targetEnvironmentIds;

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
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;

    //[SerializeField]
    //private string targetName;
    List<Card> curCards = new List<Card>();

    [SerializeField]
    CardPackEffect packEffect;

    ItemSlot _itemSlot;
    public ItemSlot itemSlot
    {
        get
        {
            if (_itemSlot == null)
                _itemSlot = GetComponent<ItemSlot>();
            return _itemSlot;
        }
    }

    [SerializeField]
    private string targetQuestId;

    public bool IsUnlock
    {
        get
        {
            if (string.IsNullOrEmpty(targetQuestId))
                return true;

            if (QuestManager.Instance.IsQuestEnded(targetQuestId))
                return true;

            if (QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == targetQuestId).Count() >= 1)
                return true;

            return false;
        }
    }

    private void UpdateTargetCards()
    {
        _targetCards = new List<int>();
        List<List<string>> targets = null;
        if (cardType == CardPackType.Room)
            targets = new List<List<string>>() { _targetRoomIds, _targetRoomPartIds, _targetPathIds, _targetEnvironmentIds };
        else if (cardType == CardPackType.Environment)
            targets = new List<List<string>>() { _targetEnvironmentIds, _targetPathIds, _targetRoomIds, _targetRoomPartIds };
        else
            targets = new List<List<string>>() { _targetPathIds, _targetRoomIds, _targetRoomPartIds, _targetEnvironmentIds };

        foreach (var item in targets)
        {
            foreach (var id in item)
                _targetCards.Add(DataManager.Instance.deckListIndex[id]);
        }
    }

    public void AddCardPool(CardType cardType, string targetId)
    {
        List<string> targetPool = null;
        if (cardType == CardType.PathTile)
            targetPool = _targetPathIds;
        if (cardType == CardType.RoomTile)
            targetPool = _targetRoomPartIds;
        if (cardType == CardType.Environment)
            targetPool = _targetEnvironmentIds;

        targetPool?.Add(targetId);
        RefreshItem();
        UpdateTargetCards();
    }

    private int GetRandomIndex(List<string> target)
    {
        if (target == null || target.Count == 0)
            return 0;

        int randomIndex = Random.Range(0, target.Count);

        if(DataManager.Instance.deckListIndex.ContainsKey(target[randomIndex]))
            return DataManager.Instance.deckListIndex[target[randomIndex]];

        return 0;
    }

    private int GetRandomIndex(List<int> target)
    {
        int randomIndex = Random.Range(0, target.Count);

        return target[randomIndex];
    }

    public void OnClick()
    {
        if (itemSlot.IsSoldOut)
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
        curCards.Clear();
        SetCards(pathNumber, roomNumber, roomPartNumber, environmentNumber);

        //_ItemSlot?.SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.Deck_Table[targetIndex]["prefab"].ToString()), DataManager.Instance.GetDescription(targetName.ToString()));
    }

    private void SetCards(int path, int singleRoom, int partRoom, int environment)
    {
        List<int> cardIndexs = new List<int>();
        for (int i = 0; i < path; i++)
            cardIndexs.Add(GetRandomIndex(_targetPathIds));
        for (int i = 0; i < singleRoom; i++)
            cardIndexs.Add(GetRandomIndex(_targetRoomIds));
        for (int i = 0; i < partRoom; i++)
            cardIndexs.Add(GetRandomIndex(_targetRoomPartIds));
        for (int i = 0; i < environment; i++)
            cardIndexs.Add(GetRandomIndex(_targetEnvironmentIds));

        foreach (int cardIndex in cardIndexs)
            curCards.Add(new Card(DataManager.Instance.deck_Table[cardIndex], cardIndex));

        curCards = UtilHelper.ShuffleList(curCards);
    }
}