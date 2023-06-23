using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster : Battler
{
    [SerializeField]
    private int monsterIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (battleState) return;

        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        if (!battle.isDead)
        {
            battleState = true;
            curTarget = battle;
            curTarget.battleState = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        rangedTargets.Remove(battle);
        if (curTarget == battle)
            curTarget = null;
    }

    protected override void DeadLock_Logic_Move()
    {
        //교착상태일경우 리스트 초기화 후 이동로직 다시시작
        crossedNodes = new List<TileNode>();
        afterCrossPath = new List<TileNode>();
        lastCrossRoad = null;
        prevTile = null;
        StopAllCoroutines();
        StartCoroutine(MoveLogic());
    }

    public void SetStartPoint(TileNode tile)
    {
        curTile = tile;
    }

    public override void Init()
    {
        base.Init();

        maxHp = Convert.ToInt32(DataManager.Instance.Monster_Table[monsterIndex]["hp"]);
        curHp = maxHp;
        damage = Convert.ToInt32(DataManager.Instance.Monster_Table[monsterIndex]["attackPower"]);
        attackSpeed = Convert.ToInt32(DataManager.Instance.Monster_Table[monsterIndex]["attackSpeed"]);
        armor = Convert.ToInt32(DataManager.Instance.Monster_Table[monsterIndex]["armor"]);
        float.TryParse(DataManager.Instance.Monster_Table[monsterIndex]["moveSpeed"].ToString(), out moveSpeed);

        StartCoroutine(MoveLogic());
    }

    public override void Update()
    {
        base.Update();

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
            col.enabled = true;

        if (battleState)
            ExcuteBattle();
    }
}
