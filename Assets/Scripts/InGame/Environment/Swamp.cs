using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Swamp : Environment, IModifier
{
    private UnitType _targetUnit = UnitType.Enemy;
    public UnitType targetUnit { get => _targetUnit; }
    public float modifyValue { get => base.value; }

    protected override void CustomFunc()
    {
        foreach(var node in curNode.neighborNodeDic.Values)
        {
            if (node.curTile == null)
                continue;

            if (!node.curTile.enemySpeedMults.Any(target => target is Swamp))
            {
                node.curTile.AddEnemySpeedMult(this);
            }
        }
    }
}
