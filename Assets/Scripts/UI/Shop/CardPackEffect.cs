using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

public class CardPackEffect : MonoBehaviour
{
    [SerializeField]
    private Transform deckPos;

    [SerializeField]
    private Image packImg;

    [SerializeField]
    private List<Sprite> packSprites;

    [SerializeField]
    private CardUI cardEx;

    private List<CardUI> cardObjects = new List<CardUI>();
    private List<Vector3> targetPositions = new List<Vector3>();

    private CancellationTokenSource cancellationToken = new CancellationTokenSource();

    [SerializeField]
    private float lerpTime = 0.1f;

    private void InitCards(List<Card> cards)
    {
        packImg.gameObject.SetActive(true);
        //if (cardObjects == null)
        //    cardObjects = GetComponentsInChildren<CardUI>(true).ToList();

        foreach (CardUI card in cardObjects)
        {
            card.gameObject.SetActive(false);
            card.transform.localScale = Vector3.one;
        }

        Vector3 originPos = packImg.transform.position;
        float xSpacing = cardEx.transform.position.x - originPos.x;
        targetPositions.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            if (i >= cardObjects.Count)
                cardObjects.Add(Instantiate(cardEx, transform));
            cardObjects[i].SetCardUI(cards[i]);
            cardObjects[i].transform.SetAsFirstSibling();
            cardObjects[i].gameObject.SetActive(true);
        }

        float xOffset = xSpacing * cards.Count;

        int floorCount = Mathf.CeilToInt((float)cards.Count / 5);
        float ySpacing = packImg.rectTransform.sizeDelta.y + 20;
        float yOrigin = (floorCount - 1) * 0.5f * ySpacing;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            Vector3 cardPos = originPos;
            cardPos.x += xSpacing * (i + 1);
            cardObjects[i].transform.position = cardPos;
            targetPositions.Add(originPos + new Vector3(xSpacing * 30 * (i % 5 + 1) + xOffset, yOrigin - (i / 5 * ySpacing)));
        }
    }

    private async UniTaskVoid SetCards(List<Card> cards)
    {
        InitCards(cards);
        gameObject.SetActive(true);
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        for (int i = cards.Count - 1; i >= 0; i--)
            await UtilHelper.MoveEffect(cardObjects[i].transform, targetPositions[i], lerpTime, cancellationToken);

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
        packImg.gameObject.SetActive(false);

        float toDeckLerpTime = 0.2f;
        for (int i = 0; i < cards.Count; i++)
        {
            UtilHelper.ScaleEffect(cardObjects[i].transform, Vector3.one * 0.3f, toDeckLerpTime, cancellationToken).Forget();
            if (i == cards.Count - 1)
                await UtilHelper.MoveEffect(cardObjects[i].transform, deckPos.position, toDeckLerpTime, cancellationToken);
            else
                UtilHelper.MoveEffect(cardObjects[i].transform, deckPos.position, toDeckLerpTime, cancellationToken).Forget();
        }
        gameObject.SetActive(false);
    }

    public void ShowEffect(List<Card> cards)
    {
        cancellationToken.Cancel();
        cancellationToken.Dispose();
        cancellationToken = new CancellationTokenSource();

        SetCards(cards).Forget();
    }
}
