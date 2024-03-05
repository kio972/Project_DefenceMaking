using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    [SerializeField]
    private Image card_Frame;
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
    private TextMeshProUGUI card_Damage;
    [SerializeField]
    private TextMeshProUGUI card_Hp;
    [SerializeField]
    private TextMeshProUGUI card_Defense;
    [SerializeField]
    private TextMeshProUGUI card_Duration;
    [SerializeField]
    private TextMeshProUGUI card_maxTarget;

    [SerializeField]
    private TextMeshProUGUI card_Cost;
    [SerializeField]
    private TextMeshProUGUI card_RequiredMana;

    [SerializeField]
    private GameObject monsterInfo;
    [SerializeField]
    private GameObject trapInfo;


    public void UpdateInfo(DeploySlot data)
    {
        string type = "monster";
        if (data.cardType == CardType.Trap)
            type = "trap";
        Sprite frame = SpriteList.Instance.LoadSprite("cardFrame2_" + type);
        card_Frame.sprite = frame;
        card_Frame_Mask.sprite = frame;

        Sprite cardRank = SpriteList.Instance.LoadSprite("cardRank_" + data.rate);
        card_Rank.sprite = cardRank;

        card_Name.ChangeLangauge(SettingManager.Instance.language, data._name);
        card_Description.ChangeLangauge(SettingManager.Instance.language, data._name + "_desc");

        card_illust.sprite = data.illur;

        card_Cost.text = data.cost.ToString();
        card_RequiredMana.text = data.mana.ToString();

        bool isMonster = type == "monster" ? true : false;
        monsterInfo.SetActive(isMonster);
        trapInfo.SetActive(!isMonster);

        card_RequiredMana.gameObject.SetActive(isMonster);

        if(data.minDamage == data.maxDamage)
            card_Damage.text = data.maxDamage.ToString();
        else
            card_Damage.text = data.minDamage.ToString() + " ~ " + data.maxDamage.ToString();

        if (isMonster)
        {
            card_Hp.text = data.hp.ToString();
            card_Defense.text = data.defense.ToString();
        }
        else
        {
            card_Duration.text = data.duration.ToString();
            card_maxTarget.text = data.maxTarget.ToString();
        }
    }

    public void UpdateInfo(string id)
    {
        int index = UtilHelper.Find_Data_Index(id, DataManager.Instance.Battler_Table, "id");
        
    }
}
