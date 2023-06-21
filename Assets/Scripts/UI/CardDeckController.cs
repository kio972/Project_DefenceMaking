using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDeckController : MonoBehaviour
{
    [SerializeField]
    private Button deckDrawBtn;

    [SerializeField]
    private List<GameObject> deckPrefab;

    [SerializeField]
    private Transform cardZone;

    private List<int> cardDeck;

    private bool initState = false;

    //초기 카드 숫자
    public int hand_CardNumber = 0;
    //최대 카드 숫자
    public int maxCardNumber = 10;

    private string ReturnDeck()
    {
        int random = cardDeck[UnityEngine.Random.Range(0, cardDeck.Count)];
        cardDeck.Remove(random);
        string cardPrefabPath = DataManager.Instance.Deck_Table[random]["prefab"].ToString();
        return "Prefab/Card/" + cardPrefabPath;
    }

    public void DrawDeck()
    {
        if (GameManager.Instance.gold < 200 || hand_CardNumber >= maxCardNumber)
            return;

        GameManager.Instance.gold -= 200;
        DrawCard();
    }

    public void DrawCard()
    {
        if(cardDeck.Count < 1) return;

        string targtPrerfab = ReturnDeck();
        hand_CardNumber++;
        GameObject targetPrefab = Resources.Load<GameObject>(targtPrerfab);
        GameObject temp = Instantiate(targetPrefab, cardZone);
        temp.transform.position = cardZone.transform.position;
    }

    private void SetDeck()
    {
        cardDeck = new List<int>();
        for(int i = 0; i < DataManager.Instance.Deck_Table.Count; i++)
        {
            int cardNumber = Convert.ToInt32(DataManager.Instance.Deck_Table[i]["num"]);
            for (int j = 0; j < cardNumber; j++)
                cardDeck.Add(Convert.ToInt32(DataManager.Instance.Deck_Table[i]["index"]));
        }
    }

    private void Init()
    {
        if (initState) return;

        deckDrawBtn.onClick.AddListener(DrawDeck);
        SetDeck();
        initState = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }
}
