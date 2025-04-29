using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSpring : Environment, IBattlerEnterEffect
{
    public float spawnTimeWeight => value;

    private float curCoolTime = 0;
    private float coolTime = 1440;

    public void Effect(Battler target)
    {
        if (curCoolTime > 0)
            return;

        if (target.unitType == UnitType.Enemy || target is PlayerBattleMain)
            return;

        int healAmount = Mathf.FloorToInt(target.maxHp * 0.1f);
        target.GetHeal(healAmount, null);
        curCoolTime = coolTime;
    }

    protected override void CustomFunc()
    {
        foreach(var tileNode in curNode.neighborNodeDic.Values)
        {
            tileNode.curNodeEffects.Add(this);
        }
    }

    private void Update()
    {
        if (curCoolTime <= 0)
            return;

        curCoolTime -= GameManager.Instance.InGameDeltaTime;
    }
}
