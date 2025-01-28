using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPackView : MonoBehaviour
{
    [SerializeField]
    List<CardUI> cardUIs;

    Dictionary<int, Card> cardDic = new Dictionary<int, Card>();

    public void SetCardList(List<int> targetCards)
    {
        foreach (var card in cardUIs)
            card.gameObject.SetActive(false);

        for(int i = 0; i < targetCards.Count; i++)
        {
            if (!cardDic.ContainsKey(targetCards[i]))
            {
                Card card = new Card(DataManager.Instance.deck_Table[targetCards[i]], targetCards[i]);
                cardDic.Add(targetCards[i], card);
            }

            if(i >= cardUIs.Count)
            {
                CardUI newCardUI = Instantiate(cardUIs[0], cardUIs[0].transform.parent);
                cardUIs.Add(newCardUI);
            }

            cardUIs[i].SetCardUI(cardDic[targetCards[i]]);
            cardUIs[i].gameObject.SetActive(true);
        }
    }
}
