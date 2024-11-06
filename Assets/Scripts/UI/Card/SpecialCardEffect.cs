using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCardEffect : MonoBehaviour
{
    CardFramework targetCard;

    public void SetCard(CardFramework card) => targetCard = card;

    public void TrashCan()
    {
        CardDeckController cardDeck = GameManager.Instance.cardDeckController;
        var cards = cardDeck.CardZone.GetComponentsInChildren<CardController>();
        foreach (var card in cards)
        {
            if (card == targetCard)
                continue;
            card.RemoveCard(false).Forget();
        }
    }
}
