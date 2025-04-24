using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Swamp : Environment, IModifier, IBattlerEnterEffect, IStatModifier
{
    private UnitType _targetUnit = UnitType.Enemy;
    public UnitType targetUnit { get => _targetUnit; }
    public float modifyValue { get => base.value; }

    public StatType statType => StatType.MoveSpeed;

    private HashSet<TileNode> targetNodes;

    public void Effect(Battler target)
    {
        if (target.unitType != UnitType.Enemy)
            return;

        TileNode curNode = target.curNode;
        target.AddStatusEffect<SlowCondition>(new SlowCondition(target, 0, modifyValue, () => targetNodes.Any(node => node == target.curNode)));
    }

    protected override void CustomFunc()
    {
        targetNodes = new HashSet<TileNode>(curNode.neighborNodeDic.Values);
        foreach (var node in curNode.neighborNodeDic.Values)
        {
            if (node.curTile == null)
                continue;

            node.curNodeEffects.Add(this);
        }

        foreach(var target in GameManager.Instance.adventurersList.Where(target => targetNodes.Any(node => node == target.curNode)))
            Effect(target);
    }
}
