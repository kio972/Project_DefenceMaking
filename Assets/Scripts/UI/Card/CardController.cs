using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Cysharp.Threading.Tasks;

public class CardController : CardFramework
{
    private bool isRemoved = false;

    public async UniTaskVoid RemoveCard(bool isRecycle)
    {
        if (isRemoved)
            return;

        isRemoved = true;
        disposables.Dispose();
        GameManager.Instance.cardDeckController.hand_CardNumber--;

        GameManager.Instance.cardDeckController.DiscardCard(this.transform, cardIndex);
        //GameManager.Instance.cardDeckController.cards.Remove(this.transform);

        CardInputControl cardInputControl = GetComponent<CardInputControl>();
        cardInputControl?.StopAllInputCheck();

        CardUIEffect cardUI = GetComponent<CardUIEffect>();
        cardUI?.DiscardEffect(isRecycle);

        await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        GameManager.Instance.cardDeckController.SetCardPosition();
    }

    protected override void SetObjectOnMap(bool cancel = false)
    {
        //bool recycle = GameManager.Instance.cardDeckController.IsRecycle;
        base.SetObjectOnMap(cancel);
        if (cancel)
            return;

        if (_cardInfo.Value.cardType == CardType.Magic)
        {
            SpecialCardEffect effect = gameObject.GetComponent<SpecialCardEffect>();
            if (effect != null && !cancel)
                RemoveCard(false).Forget();
        }
        else if (instancedObject == null)
            RemoveCard(false).Forget();
    }
}
