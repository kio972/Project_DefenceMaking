using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    private Image card_Frame;
    [SerializeField]
    private Image card_Frame2;
    [SerializeField]
    private Image card_Frame_Mask;
    [SerializeField]
    private Image card_illust;
    [SerializeField]
    private Image card_Rank;
    [SerializeField]
    private LanguageText card_Name;
    [SerializeField]
    private LanguageText card_Description;

    [SerializeField]
    private float mouseOverTime = 0.2f;

    private bool drawEnd = false;

    public Vector3 originPos;
    public Quaternion originRot;

    public int originSiblingIndex = -1;

    private CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource _scaleToken = new();
    private CancellationTokenSource _moveToken = new();

    private string GetFrameName(string cardFrame)
    {
        switch (cardFrame)
        {
            case "road":
                return "cardFrame_04";
            case "room":
                return "cardFrame_05";
            case "roomPart":
                return "cardFrame_05";
            case "environment":
                return "cardFrame_03";
        }
        return null;
    }

    private void SetCardUI(Card targetCard)
    {
        Sprite frame1 = SpriteList.Instance.LoadSprite(GetFrameName(targetCard.cardFrame));
        card_Frame.sprite = frame1;
        //Sprite frame2 = SpriteList.Instance.LoadSprite("cardFrame2_" + targetCard.cardFrame);
        //card_Frame2.sprite = frame2;
        //card_Frame_Mask.sprite = frame2;

        Sprite cardRank = SpriteList.Instance.LoadSprite("cardRank_" + targetCard.cardGrade.ToString());
        card_Rank.sprite = cardRank;
        card_Rank.gameObject.SetActive(targetCard.cardType != CardType.MapTile && targetCard.cardType != CardType.Environment);

        card_Name.ChangeLangauge(SettingManager.Instance.language, targetCard.cardName);
        card_Description.ChangeLangauge(SettingManager.Instance.language, targetCard.cardDescription);

        Sprite illur = SpriteList.Instance.LoadSprite(targetCard.cardPrefabName);
        card_illust.sprite = illur;
    }

    public void OnPointerEnter()
    {
        if (GameManager.Instance.cardLock)
            return;

        if (!drawEnd)
            return;

        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        UtilHelper.IScaleEffect(card_Frame.transform, card_Frame.transform.localScale, Vector3.one * 1.5f, mouseOverTime, _scaleToken.Token).Forget();
        UtilHelper.IMoveEffect(card_Frame.transform, card_Frame.transform.position, new Vector3(originPos.x, 180, originPos.z), mouseOverTime, _moveToken.Token).Forget();

        card_Frame.transform.rotation = Quaternion.identity;
        transform.SetAsLastSibling();

        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }

    public void OnPointerExit()
    {
        if (!drawEnd)
            return;

        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        UtilHelper.IScaleEffect(card_Frame.transform, card_Frame.transform.localScale, Vector3.one, mouseOverTime, _scaleToken.Token).Forget();
        UtilHelper.IMoveEffect(card_Frame.transform, card_Frame.transform.position, originPos, mouseOverTime, _moveToken.Token).Forget();

        card_Frame.transform.rotation = originRot;
        if (originSiblingIndex != -1)
            transform.SetSiblingIndex(originSiblingIndex);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        _moveToken.Dispose();
        _scaleToken.Dispose();
    }

    private void OnDisable()
    {
        disposables.Dispose();
        _moveToken.Dispose();
        _scaleToken.Dispose();
    }

    public async UniTaskVoid DiscardEffect(bool isRecycle)
    {
        drawEnd = false;
        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        float lerpTime = 0.4f;
        UtilHelper.IColorEffect(transform, Color.white, new Color(1, 1, 1, 0), lerpTime).Forget();
        if (isRecycle)
        {
            UtilHelper.IMoveEffect(transform, originPos, GameManager.Instance.cardDeckController.transform.position, lerpTime, _moveToken.Token).Forget();
            await UtilHelper.IScaleEffect(transform, Vector3.one, Vector3.zero, lerpTime, _scaleToken.Token);
        }
        else
            await StartCoroutine(UtilHelper.IMoveEffect(transform, originPos, originPos + new Vector3(0, 150, 0), lerpTime));

        await UniTask.Yield();
        Destroy(gameObject);
    }

    public async UniTaskVoid DrawEffect()
    {
        drawEnd = false;

        _scaleToken.Cancel();
        _scaleToken = new();

        float lerpTime = 0.5f;
        await UtilHelper.IScaleEffect(transform, Vector3.zero, Vector3.one, lerpTime, _scaleToken.Token);

        drawEnd = true;
    }

    private void Start()
    {
        CardFramework card = GetComponent<CardFramework>();
        if (card != null)
            card._cardInfo.Select(_ => card._cardInfo.Value).Subscribe(_ => SetCardUI(_)).AddTo(disposables);

        Image image = GetComponent<Image>();
        if(image != null)
        {
            image.OnPointerEnterAsObservable().Where(_ => !GameManager.Instance.cardLock && drawEnd).Subscribe(_ => OnPointerEnter());
            image.OnPointerExitAsObservable().Where(_ => drawEnd).Subscribe(_ => OnPointerExit());
        }
    }
}
