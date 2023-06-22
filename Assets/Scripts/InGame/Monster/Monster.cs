using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Battler
{
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

    protected override void DeadLock_Logic_Move()
    {
        //교착상태일경우 리스트 초기화 후 이동로직 다시시작
        crossedNodes = new List<TileNode>();
        afterCrossPath = new List<TileNode>();
        lastCrossRoad = null;
        StartCoroutine(MoveLogic());
    }

    public void SetStartPoint(TileNode tile)
    {
        curTile = tile;
    }

    public override void Init()
    {
        base.Init();
        StartCoroutine(MoveLogic());
    }

    public override void Update()
    {
        base.Update();

        if (battleState)
            ExcuteBattle();
    }
}
