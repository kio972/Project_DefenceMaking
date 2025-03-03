using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPoolExpand2 : MonoBehaviour, IResearch
{
    [SerializeField]
    private string targetCardId;

    public void ActiveResearch()
    {
        if (string.IsNullOrEmpty(targetCardId))
            return;

        int index = DataManager.Instance.deckListIndex[targetCardId];
        GameManager.Instance.cardDeckController.AddCard(index);
        if (GameManager.Instance.cardDeckController.hand_CardNumber >= GameManager.Instance.cardDeckController.maxCardNumber)
            GameManager.Instance.cardDeckController.EnqueueCard(index);
        else
            GameManager.Instance.cardDeckController.DrawCard(index);
    }
}
