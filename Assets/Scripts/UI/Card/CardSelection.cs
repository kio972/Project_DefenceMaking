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
    private GameObject cardSkip;

    private bool initState = false;

    private Dictionary<TileType, Dictionary<int, int>> cardPoolDic = new Dictionary<TileType, Dictionary<int, int>>();
    private Dictionary<string, (int index, TileType tileType, int token)> excludedCards = new Dictionary<string, (int index, TileType tileType, int token)>();

    bool isStarted = false;
    int[] curSelectIndex = new int[3];
    bool[] curSelectIsAdd = new bool[3];

    public void SelectCard(int i)
    {
        if (i < 0 || i >= curSelectIndex.Length)
            return;

        int index = curSelectIndex[i];
        if(index != -1)
        {
            bool isAdd = curSelectIsAdd[i];
            if (isAdd)
                GameManager.Instance.cardDeckController.AddCard(index);
            else
                GameManager.Instance.cardDeckController.RemoveCard(index);
        }

        GameManager.Instance.SetPause(false);
        gameObject.SetActive(false);
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

    private int GetRandomTileIndex(TileType tileType)
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

    private Card GetRandomCard()
    {
        TileType tileType = GetRandomCardType();

        int index = GetRandomTileIndex(tileType);
        if(index == -1)
            return GetRandomCard();

        return new Card(DataManager.Instance.deck_Table[index], index);
    }

    public void StartCardSelect()
    {
        if (!initState)
            Init();

        if (!isStarted)
            return;

        GameManager.Instance.SetPause(true);

        int token = UnityEngine.Random.Range(0, 2);
        bool isAdd = token == 1 ? true : false;
        Card firstCard = GetRandomCard();
        cardUis[0].SetCardUI(firstCard, isAdd);
        curSelectIndex[0] = firstCard.cardIndex;
        curSelectIsAdd[0] = isAdd;

        token = UnityEngine.Random.Range(0, 2);
        isAdd = token == 1 ? true : false;
        Card secondCard = GetRandomCard();
        cardUis[1].SetCardUI(secondCard, isAdd);
        curSelectIndex[1] = secondCard.cardIndex;
        curSelectIsAdd[1] = isAdd;

        token = UnityEngine.Random.Range(0, 2);
        cardSkip.SetActive(token == 0);
        cardUis[2].gameObject.SetActive(token == 1);
        if (token == 1)
        {
            token = UnityEngine.Random.Range(0, 2);
            isAdd = token == 1 ? true : false;
            Card thirdCard = GetRandomCard();
            cardUis[2].SetCardUI(thirdCard, isAdd);
            curSelectIndex[2] = thirdCard.cardIndex;
            curSelectIsAdd[2] = isAdd;
        }
        else
            curSelectIndex[2] = -1;

        gameObject.SetActive(true);
    }

    public void AddCardToPool(string id)
    {
        if(!excludedCards.ContainsKey(id))
        {
            Debug.Log($"Invalid ID for current stage : {id}");
            return;
        }

        (int index, TileType tileType, int token) data = excludedCards[id];
        cardPoolDic[data.tileType][data.index] = data.token;
        isStarted = true;
    }

    private void Init()
    {
        cardPoolDic[TileType.Path] = new Dictionary<int, int>();
        cardPoolDic[TileType.Room_Single] = new Dictionary<int, int>();
        cardPoolDic[TileType.Room] = new Dictionary<int, int>();
        cardPoolDic[TileType.Door] = new Dictionary<int, int>();
        cardPoolDic[TileType.Environment] = new Dictionary<int, int>();
        cardPoolDic[TileType.Herb] = new Dictionary<int, int>();
        cardPoolDic[TileType.Special] = new Dictionary<int, int>();


        foreach (var data in DataManager.Instance.start_deckTable)
        {
            int index = DataManager.Instance.deckListIndex[data["id"].ToString()];
            bool include = data["include"].ToString() == "TRUE" ? true : false;
            int token = Convert.ToInt32(data["token"]);
            TileType tileType = UtilHelper.GetTileType(data["type"].ToString());

            if (include)
            {
                cardPoolDic[tileType][index] = token;
                isStarted = true;
            }
            else
                excludedCards[data["id"].ToString()] = (index, tileType, token);
        }
    }
}
