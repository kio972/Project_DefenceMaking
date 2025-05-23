using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IStatObject
{
    string GetStat(StatType statType);
}

public enum StatType
{
    Hp,
    Dur,
    Atk,
    AttackSpeed,
    Def,
    AttackRange,
    SpawnCool,
    Mana,
    BuffTexts,
    MoveSpeed,
    AtkMin,
    AtkMax,
}

public class TooltipStats : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI hp;
    [SerializeField]
    TextMeshProUGUI dur;
    [SerializeField]
    TextMeshProUGUI atk;
    [SerializeField]
    TextMeshProUGUI atkSpeed;
    [SerializeField]
    TextMeshProUGUI def;
    [SerializeField]
    TextMeshProUGUI atkRange;
    [SerializeField]
    TextMeshProUGUI spawnCool;
    [SerializeField]
    TextMeshProUGUI mana;
    [SerializeField]
    TextMeshProUGUI buffs;

    private Dictionary<StatType, TextMeshProUGUI> statsDic;

    private bool isInit = false;

    private Dictionary<ToolTipType, List<StatType>> tooltipToStatDef = new Dictionary<ToolTipType, List<StatType>>()
    {
        { ToolTipType.Devil, new List<StatType>(){ StatType.Hp, StatType.Atk, StatType.AttackSpeed, StatType.Def, StatType.AttackRange} },
        { ToolTipType.Ally, new List<StatType>(){ StatType.Hp, StatType.Atk, StatType.AttackSpeed, StatType.Def, StatType.AttackRange} },
        { ToolTipType.Enemy, new List<StatType>(){ StatType.Hp, StatType.Atk, StatType.AttackSpeed, StatType.Def, StatType.AttackRange} },
        { ToolTipType.Special, new List<StatType>(){ StatType.Hp, StatType.Atk, StatType.AttackSpeed, StatType.Def, StatType.AttackRange, StatType.BuffTexts} },
        { ToolTipType.Trap, new List<StatType>(){ StatType.Dur, StatType.Atk, StatType.AttackSpeed} },
        { ToolTipType.Spawner, new List<StatType>(){ StatType.SpawnCool } },
        { ToolTipType.Tile, new List<StatType>(){ StatType.Mana, StatType.BuffTexts } },
    };

    IStatObject curStatObject;

    private void ResetStats()
    {
        foreach(var item in statsDic.Values)
            item.gameObject.SetActive(false);
    }

    public void SetStatZone(TooltipObject tooltipObject)
    {
        if (!isInit)
            Init();

        ResetStats();
        ToolTipType type = tooltipObject.toolTipType;
        foreach(var stat in tooltipToStatDef[type])
        {
            statsDic[stat].gameObject.SetActive(true);
        }

        curStatObject = tooltipObject.GetComponentInParent<IStatObject>();
    }

    private void Init()
    {
        statsDic = new Dictionary<StatType, TextMeshProUGUI>();
        statsDic[StatType.Hp] = hp;
        statsDic[StatType.Dur] = dur;
        statsDic[StatType.Atk] = atk;
        statsDic[StatType.AttackSpeed] = atkSpeed;
        statsDic[StatType.Def] = def;
        statsDic[StatType.AttackRange] = atkRange;
        statsDic[StatType.SpawnCool] = spawnCool;
        statsDic[StatType.Mana] = mana;
        statsDic[StatType.BuffTexts] = buffs;
    }

    private void Update()
    {
        if (curStatObject == null)
            return;

        foreach (var kvp in statsDic)
        {
            if (!kvp.Value.gameObject.activeInHierarchy)
                continue;

            kvp.Value.text = curStatObject.GetStat(kvp.Key);
        }
    }
}
