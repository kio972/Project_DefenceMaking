using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCardEffect : MonoBehaviour
{
    CardFramework targetCard;

    public void SetCard(CardFramework card) => targetCard = card;

    private async UniTaskVoid ExcuteRecycle()
    {
        CardDeckController cardDeck = GameManager.Instance.cardDeckController;
        var cards = cardDeck.CardZone.GetComponentsInChildren<CardController>();
        int count = cards.Length - 1;
        foreach (var card in cards)
        {
            if (card == targetCard)
                continue;
            card.RemoveCard(false).Forget();
        }

        for (int i = 0; i < count; i++)
        {
            cardDeck.DrawCard();
            await UniTask.Delay(100, cancellationToken: cardDeck.GetCancellationTokenOnDestroy());
        }
    }

    public void Recyclebin()
    {
        ExcuteRecycle().Forget();
    }
}
