using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeploySlot : MonoBehaviour, ISlot
{
    private string id = "";

    public string targetName;

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
    public MonsterType monsterType = MonsterType.none;

    [SerializeField]
    private Button deployBtn;

    private DeployUI delpoyUI;

    public string prefabName;

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

    Dictionary<string, object> data;

    [SerializeField]
    private GameObject clickedImg;


    private bool isUnlocked = false;
    public bool IsUnlocked
    {
        get
        {
            if (PassiveManager.Instance.deployAvailableTable.ContainsKey(id))
            {
                isUnlocked = PassiveManager.Instance.deployAvailableTable[id];
            }

            return isUnlocked;
        }
    }
    public void SetClicked(bool value)
    {
        if (clickedImg != null)
            clickedImg.SetActive(value);
    }

    private void OnEnable()
    {
        Color targetColor = GameManager.Instance.gold >= cost ? Color.white : Color.red;
        costText.color = targetColor;
    }

    public void SendInfo()
    {
        info.UpdateInfo(this);
    }

    public void Deploy()
    {
        if(delpoyUI == null)
            delpoyUI = GetComponentInParent<DeployUI>();

        delpoyUI.DeployReady(cardType, targetName, prefabName, cost);
    }

    public void UpdateMana()
    {
        if (cardType != CardType.Monster)
            return;
        float.TryParse(data["requiredMagicpower"].ToString(), out mana);
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        mana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        manaText.text = mana.ToString();
    }

    public void Init(Dictionary<string,object> data)
    {
        id = data["id"].ToString();
        targetName = data["name"].ToString();
        cardType = (id[2] == 't' ? CardType.Trap : CardType.Spawner);
        if (id.Contains("s_m4"))
            cardType = CardType.Monster;

        minDamage = Convert.ToInt32(data["attackPowerMin"]);
        maxDamage = Convert.ToInt32(data["attackPowerMax"]);
        mana = 0;

        rate = data["rate"].ToString();

        if (cardType is CardType.Monster or CardType.Spawner)
        {
            hp = Convert.ToInt32(data["hp"]);
            defense = Convert.ToInt32(data["armor"]);
            float.TryParse(data["requiredMagicpower"].ToString(), out mana);
            monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
            mana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        }
        
        if(cardType == CardType.Trap)
        {
            duration = Convert.ToInt32(data["duration"]);
            maxTarget = Convert.ToInt32(data["targetCount"]);
        }

        manaText.gameObject.SetActive(mana == 0 ? false : true);
        manaText.text = mana.ToString();

        cost = Convert.ToInt32(data["cost"]);
        prefabName = data["prefab"].ToString();

        Sprite illur = SpriteList.Instance.LoadSprite(prefabName);
        icon.sprite = illur;
        nameText?.ChangeLangauge(SettingManager.Instance.language, targetName);
        costText.text = cost.ToString();

        this.data = data;
    }
}
