using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class PassiveManager : IngameSingleton<PassiveManager>
{
    public int monsterHp_Weight = 0;

    public int monsterDefense_Weight = 0;
    public float monsterAttackSpeed_Weight = 0; 
    public float monsterDamageRate_Weight = 0;
    public float monsterHpRate_Weight = 0;

    public float adventurerAttackSpeed_Weight = 0;
    public float adventurerDamageRate_Weight = 0;

    public float enemySlow_Weight = 0;
    public int income_Weight = 0;
    public Dictionary<TileNode, float> slowedTile = new Dictionary<TileNode, float>();

    public Dictionary<TileNode, int> manaTile = new Dictionary<TileNode, int>();

    public List<WaveData> adventurerRaiseTable = new List<WaveData>();

    public Dictionary<string, bool> deployAvailableTable = new Dictionary<string, bool>();

    private int[] monsterTypeHp_Weight = new int[Enum.GetValues(typeof(MonsterType)).Length];
    private int[] monsterTypeAttackSpeed_Weight = new int[Enum.GetValues(typeof(MonsterType)).Length];
    private int[] monsterTypeReduceMana_Weight = new int[Enum.GetValues(typeof(MonsterType)).Length];
    private int[] monsterTypeResurrect_Weight = new int[Enum.GetValues(typeof(MonsterType)).Length];
    private int[] monsterTypeSummonSpeed_Weight = new int[Enum.GetValues(typeof(MonsterType)).Length];

    public int[] _MonsterTypeHp_Weight { get => monsterTypeHp_Weight; }
    public int[] _MonsterTypeAttackSpeed_Weight { get => monsterTypeAttackSpeed_Weight; }
    public int[] _MonsterTypeReduceMana_Weight { get => monsterTypeReduceMana_Weight; }
    public int[] _MonsterTypeResurrect_Weight { get => monsterTypeResurrect_Weight; }
    public int[] _MonsterTypeSummonSpeed_Weight { get => monsterTypeSummonSpeed_Weight; }

    private int[] tileSpeed_Weight = new int[Enum.GetValues(typeof(TileType)).Length];
    public int[] _TileSpeed_Weight { get => tileSpeed_Weight; }

    private int tileDestructIncome = 0;
    public int _TileDesturctIncome { get => tileDestructIncome; }

    public ReactiveProperty<int> _shopSaleAmount { get; private set; } = new ReactiveProperty<int>(0);

    public int devilAuraRange { get; private set; } = 0;
    public int devilAuraPower { get; private set; } = 0;

    public ReactiveProperty<int> _slimeSplit_Weight { get; private set; } = new ReactiveProperty<int>(0);

    public bool isGoblinHealActive { get; private set; } = false;

    public int golemHoldback_Weight { get; private set; } = 0;

    public bool isMimicBuffActive { get; private set; } = false;

    public void MimicBuffActive()
    {
        isMimicBuffActive = true;
    }

    public void GolemHoldbackUp(int value)
    {
        golemHoldback_Weight += value;
        foreach (Golem golem in GameManager.Instance._MonsterList)
            golem.UpdateHoldBack(value);
    }

    public void GoblinHealActive()
    {
        isGoblinHealActive = true;
    }

    public void UpgradeSlimeSplit(int value)
    {
        _slimeSplit_Weight.Value += value;
        foreach (Slime slime in GameManager.Instance._MonsterList)
            slime.UpgradeSplitCount(value);
    }

    public void UpgradeDevilAura(int range, int value)
    {
        devilAuraRange = range;
        devilAuraPower = value;
    }

    public void UpgradeShopSale(int value)
    {
        _shopSaleAmount.Value += value;
    }

    public void UpgradeTileDestructInCome(int value)
    {
        tileDestructIncome += value;
    }

    public void UpgradeTileSpeed(TileType tileType, int value)
    {
        tileSpeed_Weight[(int)tileType] += value;
    }

    public void UpgradeSummonSpeed(MonsterType monsterType, int value)
    {
        monsterTypeResurrect_Weight[(int)monsterType] += value;
        foreach (MonsterSpawner spawner in GameManager.Instance.monsterSpawner)
            spawner.UpdatePassive();
    }

    public void UpgradeResurrect(MonsterType monsterType, int value)
    {
        monsterTypeResurrect_Weight[(int)monsterType] += value;
    }

    public void ReduceManaCost(MonsterType monsterType, int value)
    {
        monsterTypeReduceMana_Weight[(int)monsterType] += value;
        foreach (MonsterSpawner spawner in GameManager.Instance.monsterSpawner)
            spawner.UpdatePassive();
    }

    public void UpgradeAttackSpeed(MonsterType monsterType, int value)
    {
        monsterTypeAttackSpeed_Weight[(int)monsterType] += value;
    }

    public void UpgradeHp(MonsterType monsterType, int value)
    {
        monsterTypeHp_Weight[(int)monsterType] += value;
    }

    public void AddDeployData(string targetId)
    {
        if(!deployAvailableTable.ContainsKey(targetId))
            deployAvailableTable.Add(targetId, true);
    }

    public void AddBuffTable(Dictionary<string, object> data)
    {
        BuffTable table = new BuffTable(data);
        monsterDefense_Weight += table.ally_defense;
        monsterAttackSpeed_Weight += table.ally_attackSpeed;
        monsterDamageRate_Weight += table.ally_damageRate;
        monsterHpRate_Weight += table.ally_hpRate;
        monsterHp_Weight += table.ally_maxHp;
        if(table.ally_heal != 0)
            GameManager.Instance.HealAlly(table.ally_heal);
        GameManager.Instance.gold += table.gold;

        adventurerAttackSpeed_Weight += table.enemy_attackSpeed;
        adventurerDamageRate_Weight += table.enemy_damageRate;
        if (table.enemy_raise[0] != "")
        {
            foreach (string target in table.enemy_raise)
            {
                string[] split = target.Split('+');
                int number = Convert.ToInt32(split[1]);

                WaveData newWaveData = new WaveData(split[0], number);
                adventurerRaiseTable.Add(newWaveData);
            }
        }
    }

    public float GetSlowRate(TileNode curNode)
    {
        float slowRate = 0;

        foreach(TileNode neighborNode in curNode.neighborNodeDic.Values)
        {
            if (slowedTile.ContainsKey(neighborNode))
            {
                if (slowedTile[neighborNode] > slowRate)
                    slowRate = slowedTile[neighborNode];
            }
        }

        return slowRate;
    }

    public int GetAdditionalMana()
    {
        int value = 0;
        foreach(var node in manaTile.Keys)
        {
            foreach (TileNode neighborNode in node.neighborNodeDic.Values)
            {
                if (neighborNode.curTile == null)
                    continue;
                value += manaTile[node];
            }
        }

        return value;
    }

    public void Init()
    {
        //세이브파일에서 수치 받아오는 함수 추가예정
    }
}
