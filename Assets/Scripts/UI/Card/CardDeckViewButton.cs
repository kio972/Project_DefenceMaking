using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckViewButton : MonoBehaviour
{
    [SerializeField]
    private CardDeckController _cardDeckController;
    private CardDeckController cardDeckController
    {
        get
        {
            if(_cardDeckController == null)
                _cardDeckController = GameManager.Instance.cardDeckController;
            return _cardDeckController;
        }
    }

    [SerializeField]
    private CardPackView _cardPackView;

    public void ShowCardDeck(bool controlSpeed)
    {
        _cardPackView.SetCardList(cardDeckController.cardDeck);
        if(controlSpeed)
        {
            UIManager.Instance.SetTab(_cardPackView.gameObject, true, () => { GameManager.Instance.SetPause(false); });
            GameManager.Instance.SetPause(true);
        }
        else
            UIManager.Instance.SetTab(_cardPackView.gameObject, true);
    }

    public void CloseCardDeck()
    {
        UIManager.Instance.SetTab(_cardPackView.gameObject, false);
    }
}
