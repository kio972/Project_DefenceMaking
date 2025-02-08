using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject uiGroup;

    [SerializeField]
    private List<CardSelectUI> cardUis;
    [SerializeField]
    private List<DissolveController> dissolves;
    [SerializeField]
    private GameObject cardSkip;

    private bool initState = false;

    private Dictionary<TileType, Dictionary<int, int>> _cardPoolDic = new Dictionary<TileType, Dictionary<int, int>>();
    private Dictionary<string, (int index, TileType tileType, int token)> excludedCards = new Dictionary<string, (int index, TileType tileType, int token)>();
    public Dictionary<TileType, Dictionary<int, int>> cardPoolDic
    {
        get
        {
            if (!initState)
                Init();
            return _cardPoolDic;
        }
    }

    bool isStarted = false;
    int[] curSelectIndex = new int[3];
    bool[] curSelectIsAdd = new bool[3];

    bool isSelectingNow = false;

    [SerializeField]
    AK.Wwise.Event selectSound;

    private void DeActive()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetPause(false);
    }

    public void SelectCard(int i)
    {
        if (!isSelectingNow)
            return;

        if (i < 0 || i >= curSelectIndex.Length)
            return;

        foreach (var item in dissolves)
        {
            if(item.gameObject.activeInHierarchy)
                item.isDisappare = true;
        }
        int index = curSelectIndex[i];
        if (index != -1)
        {
            bool isAdd = curSelectIsAdd[i];
            if (isAdd)
                GameManager.Instance.cardDeckController.AddCard(index);
            else
                GameManager.Instance.cardDeckController.RemoveCard(index);

            dissolves[i].isDisappare = false;
        }
        else //skip을선택함
        {
            dissolves[3].isDisappare = false;
        }

        isSelectingNow = false;
        //gameObject.SetActive(false);
        Animator animator = GetComponent<Animator>();
        animator?.SetTrigger("End");
        selectSound?.Post(gameObject);
    }

    private TileType GetRandomCardType()
    {
        int pathToken = 20;
        int singleRoomToken = 15 + pathToken;
        int partRoomToken = 20 + singleRoomToken;
        int doorToken = 10 + partRoomToken;
        int environmentToken = 15 + doorToken;
        int herbToken = 10 + environmentToken;
        int special = 10 + herbToken;

        
        int token = UnityEngine.Random.Range(0, special);
        if (token < pathToken)
            return TileType.Path;
        else if (token < singleRoomToken)
            return TileType.Room_Single;
        else if (token < partRoomToken)
            return TileType.Room;
        else if (token < doorToken)
            return TileType.Door;
        else if (token < environmentToken)
            return TileType.Environment;
        else if (token < herbToken)
            return TileType.Herb;
        else
            return TileType.Special;
    }

    private bool IsCardAlreadySelected(int curIndex, int cardIndex, bool isAdd)
    {
        if (curIndex == 0)
            return false;

        // curSelectIndex와 curSelectIsAdd 배열을 사용하여 중복 체크
        for (int i = 0; i < curIndex; i++)
        {
            if (curSelectIndex[i] == cardIndex && curSelectIsAdd[i] == isAdd)
            {
                return true; // 중복된 카드가 이미 선택됨
            }
        }
        return false; // 중복되지 않음
    }

    public int GetRandomTileIndex(TileType tileType)
    {
        int totalCount = cardPoolDic[tileType].Count;
        if (totalCount == 0)
            return -1;

        int totalToken = 0;
        foreach(var kvp in cardPoolDic[tileType])
        {
            totalToken += kvp.Value;
        }

        int token = UnityEngine.Random.Range(0, totalToken);
        int curVal = 0;
        foreach(var kvp in cardPoolDic[tileType])
        {
            curVal += kvp.Value;
            if (token < curVal)
                return kvp.Key;
        }

        return -1;
    }

    public Card GetRandomCard()
    {
        TileType tileType = GetRandomCardType();

        int index = GetRandomTileIndex(tileType);
        if(index == -1)
            return GetRandomCard();

        return new Card(DataManager.Instance.deck_Table[index], index);
    }

    public Card GetDeckRandomCard()
    {
        int randomDeckIndex = UnityEngine.Random.Range(0, GameManager.Instance.cardDeckController.cardDeckCount);

        int index = GameManager.Instance.cardDeckController.cardDeck[randomDeckIndex];
        return new Card(DataManager.Instance.deck_Table[index], index);
    }

    private void SetCardUI(int index)
    {
        Card card;
        bool isAdd;
        do
        {
            int token = UnityEngine.Random.Range(0, 2);
            isAdd = token == 1;

            card = isAdd ? GetRandomCard() : GetDeckRandomCard();

        } while (IsCardAlreadySelected(index, card.cardIndex, isAdd));

        cardUis[index].SetCardUI(card, isAdd);

        curSelectIndex[index] = card.cardIndex;
        curSelectIsAdd[index] = isAdd;
    }

    public void StartCardSelect()
    {
        if (!initState)
            Init();

        if (!isStarted)
            return;

        GameManager.Instance.SetPause(true);

        SetCardUI(0);
        SetCardUI(1);

        int token = UnityEngine.Random.Range(0, 2);
        cardSkip.SetActive(token == 0);
        cardUis[2].gameObject.SetActive(token == 1);
        if (token == 1)
        {
            SetCardUI(2);
        }
        else
            curSelectIndex[2] = -1;

        gameObject.SetActive(true);

        foreach (var item in dissolves)
        {
            if(item.gameObject.activeInHierarchy)
                item.isAppare = true;
        }
        isSelectingNow = true;
    }

    public void AddCardToPool(string id)
    {
        if(!excludedCards.ContainsKey(id))
        {
            Debug.Log($"Invalid ID for current stage : {id}");
            return;
        }

        (int index, TileType tileType, int token) data = excludedCards[id];
        _cardPoolDic[data.tileType][data.index] = data.token;
        isStarted = true;
    }

    private void Init()
    {
        _cardPoolDic[TileType.Path] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Room_Single] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Room] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Door] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Environment] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Herb] = new Dictionary<int, int>();
        _cardPoolDic[TileType.Special] = new Dictionary<int, int>();


        foreach (var data in DataManager.Instance.start_deckTable)
        {
            int index = DataManager.Instance.deckListIndex[data["id"].ToString()];
            bool include = data["include"].ToString() == "TRUE" ? true : false;
            int token = Convert.ToInt32(data["token"]);
            TileType tileType = UtilHelper.GetTileType(data["type"].ToString());

            if (include)
            {
                _cardPoolDic[tileType][index] = token;
                isStarted = true;
            }
            else
                excludedCards[data["id"].ToString()] = (index, tileType, token);
        }
    }
}
