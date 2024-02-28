using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeploySlot : MonoBehaviour
{
    private string id = "";

    public string _name;

    public int minDamage;
    public int maxDamage;

    public int hp;
    public int defense;

    public int duration;
    public int maxTarget;

    public float mana;
    public int cost;

    public string rate;

    public CardType cardType;

    [SerializeField]
    private Button deployBtn;

    private DeployUI delpoyUI;

    [SerializeField]
    private string prefabName;

    [SerializeField]
    private Image icon;

    public Sprite illur { get => icon.sprite; }

    [SerializeField]
    private LanguageText nameText;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private TextMeshProUGUI manaText;

    [SerializeField]
    private SlotInfo info;

    private bool isUnlocked = false;
    public bool IsUnlocked
    {
        get
        {
            if (!isUnlocked && PassiveManager.Instance.deployAvailableTable.ContainsKey(id))
                isUnlocked = true;

            return isUnlocked;
        }
    }

    public void SendInfo()
    {
        info.UpdateInfo(this);
    }

    public void Deploy()
    {
        if(delpoyUI == null)
            delpoyUI = GetComponentInParent<DeployUI>();

        delpoyUI.DeployReady(cardType, _name, prefabName, cost);
    }

    public void Init(Dictionary<string,object> data)
    {
        id = data["id"].ToString();
        _name = data["name"].ToString();
        cardType = (id[2] == 't' ? CardType.Trap : CardType.Monster);
        minDamage = Convert.ToInt32(data["attackPowerMin"]);
        maxDamage = Convert.ToInt32(data["attackPowerMax"]);
        mana = 0;

        rate = data["rate"].ToString();

        if (cardType == CardType.Monster)
        {
            hp = Convert.ToInt32(data["hp"]);
            defense = Convert.ToInt32(data["armor"]);
            float.TryParse(data["requiredMagicpower"].ToString(), out mana);
            MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
            mana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        }
        
        if(cardType == CardType.Trap)
        {
            duration = Convert.ToInt32(data["duration"]);
            maxTarget = Convert.ToInt32(data["targetCount"]);
            isUnlocked = true;
        }

        manaText.gameObject.SetActive(mana == 0 ? false : true);
        manaText.text = mana.ToString();

        cost = Convert.ToInt32(data["cost"]);
        prefabName = data["prefab"].ToString();

        Sprite illur = SpriteList.Instance.LoadSprite(prefabName);
        icon.sprite = illur;
        nameText.ChangeLangauge(SettingManager.Instance.language, _name);
        costText.text = cost.ToString();
    }
}
