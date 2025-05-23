using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectUI : CardUI
{
    public void SetCardUI(Card targetCard, bool isAdd)
    {
        SetCardUI(targetCard);
        card_Description?.ChangeLangauge(SettingManager.Instance.language, isAdd ? "card_choose_0" : "card_choose_1");
        if(!isAdd)
        {
            Sprite frame1 = SpriteList.Instance.LoadSprite("cardDel");
            card_Frame.sprite = frame1;
        }
    }
}
