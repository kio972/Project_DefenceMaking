using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardController : CardFramework
{
    private void RemoveCard(bool isRecycle)
    {
        GameManager.Instance.cardDeckController.hand_CardNumber--;

        GameManager.Instance.cardDeckController.DiscardCard(this.transform, cardIndex);
        //GameManager.Instance.cardDeckController.cards.Remove(this.transform);
        GameManager.Instance.cardDeckController.SetCardPosition();

        CardUI cardUI = GetComponent<CardUI>();
        cardUI?.DiscardEffect(isRecycle);
    }

    protected override void SetObjectOnMap(bool cancel = false)
    {
        bool recycle = GameManager.Instance.cardDeckController.IsRecycle;
        if (!cancel && recycle)
        {
            Destroy(instancedObject);
            instancedObject = null;
            GameManager.Instance.cardDeckController.AddCard(cardIndex);
        }
        else
            base.SetObjectOnMap();

        if (instancedObject == null)
            RemoveCard(recycle);
    }
}
