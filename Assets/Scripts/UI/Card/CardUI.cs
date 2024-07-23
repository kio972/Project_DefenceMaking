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

    private CompositeDisposable disposables = new CompositeDisposable();

    private void OnDisable()
    {
        disposables.Dispose();
    }

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
            case "herb":
                return "cardFrame1_monster";
        }
        return "";
    }

    public void SetCardUI(Card targetCard)
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

    private void Start()
    {
        CardFramework card = GetComponent<CardFramework>();
        if (card != null)
            card._cardInfo.Select(_ => card._cardInfo.Value).Subscribe(_ => SetCardUI(_)).AddTo(disposables);
    }
}
