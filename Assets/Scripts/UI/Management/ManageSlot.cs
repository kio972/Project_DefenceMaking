using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageSlot : MonoBehaviour
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

    private ManagementUI managementUI;

    private string prefabName;

    [SerializeField]
    private Image icon;

    public Sprite illur { get => icon.sprite; }

    [SerializeField]
    private LanguageText nameText;
    [SerializeField]
    private TextMeshProUGUI costText;

    [SerializeField]
    private SlotInfo info;

    public void SendInfo()
    {
        info.UpdateInfo(this);
    }

    public void Deploy()
    {
        if(managementUI == null)
            managementUI = GetComponentInParent<ManagementUI>();

        managementUI.DeployReady(cardType, _name, prefabName, cost);
    }

    public void Init(Dictionary<string,object> data)
    {
        id = data["id"].ToString();
        _name = data["name"].ToString();
        cardType = (id[2] == 't' ? CardType.Trap : CardType.Monster);
        minDamage = Convert.ToInt32(data["attackPowerMin"]);
        maxDamage = Convert.ToInt32(data["attackPowerMax"]);

        rate = data["rate"].ToString();

        if (cardType == CardType.Monster)
        {
            hp = Convert.ToInt32(data["hp"]);
            defense = Convert.ToInt32(data["armor"]);
            float.TryParse(data["requiredMagicpower"].ToString(), out mana);
        }
        

        if(cardType == CardType.Trap)
        {
            duration = Convert.ToInt32(data["duration"]);
            maxTarget = Convert.ToInt32(data["targetCount"]);
        }
        
        cost = Convert.ToInt32(data["cost"]);
        prefabName = data["prefab"].ToString();

        Sprite illur = SpriteList.Instance.LoadSprite(prefabName);
        icon.sprite = illur;
        nameText.ChangeLangauge(SettingManager.Instance.language, _name);
        costText.text = cost.ToString() + "G";
    }
}
