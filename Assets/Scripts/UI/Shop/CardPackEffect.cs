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
    private Image fadeImg;

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

    [SerializeField]
    private GameObject confirmBtn;

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
                cardObjects.Add(Instantiate(cardEx, cardEx.transform.parent));
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

        cardEx.gameObject.SetActive(false);
    }
    readonly Color fadeColor = new Color(0, 0, 0, 0.8f);
    private async UniTaskVoid SetCards(List<Card> cards)
    {
        InitCards(cards);
        gameObject.SetActive(true);
        await UtilHelper.IColorEffect(fadeImg.transform, Color.clear, fadeColor, 0.5f);
        //await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        foreach(CardUI card in cardObjects)
            card.GetComponent<CardUIEffect>()?.SetDrawState(false);

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            AudioManager.Instance.Play2DSound("Click_card_01", SettingManager.Instance._FxVolume);
            await UtilHelper.MoveEffect(cardObjects[i].transform, targetPositions[i], lerpTime, cancellationToken);
        }

        foreach (CardUI card in cardObjects)
            card.GetComponent<CardUIEffect>()?.SetDrawState(true);

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
        confirmBtn.SetActive(true);
        await UniTask.WaitUntil(() => !confirmBtn.activeSelf || Input.GetKeyDown(KeyCode.Return));
        confirmBtn.SetActive(false);

        foreach (CardUI card in cardObjects)
        {
            card.GetComponent<CardUIEffect>()?.SetDrawState(false);
            card.GetComponent<CardUIEffect>()?.ResetEffect();
        }

        packImg.gameObject.SetActive(false);
        UtilHelper.IColorEffect(fadeImg.transform, fadeColor, Color.clear, 0.3f).Forget();
        float toDeckLerpTime = 0.5f;
        AudioManager.Instance.Play2DSound("Click_card_01", SettingManager.Instance._FxVolume);
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

    public void SetCardPackSprite(Transform packTransform)
    {
        ItemSlot itemSlot = packTransform.GetComponent<ItemSlot>();
        if (itemSlot == null)
            return;

        packImg.sprite = itemSlot.ItemIcon.sprite;
    }

    public void ShowEffect(List<Card> cards)
    {
        cancellationToken.Cancel();
        cancellationToken.Dispose();
        cancellationToken = new CancellationTokenSource();

        SetCards(cards).Forget();
    }
}
