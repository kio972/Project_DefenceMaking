using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardShortCutUI : MonoBehaviour
{
    private async UniTaskVoid ShowCardShortCut()
    {
        CardController card = GetComponentInParent<CardController>();
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        while(!card.isRemoved)
        {
            text.text = card.handIndex >= 9 ? "0" : $"{card.handIndex + 1}";
            transform.rotation = Quaternion.identity;

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        gameObject.SetActive(false);
    }

    void Start()
    {
        ShowCardShortCut().Forget();
    }
}
