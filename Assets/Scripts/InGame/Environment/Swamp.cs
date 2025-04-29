using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Swamp : Environment, IStatModifier, IBattlerEnterEffect
{
    private UnitType _targetUnit = UnitType.Enemy;
    public UnitType targetUnit { get => _targetUnit; }
    public float modifyValue { get => base.value; }

    //public StatType statType => StatType.MoveSpeed;

    public void Effect(Battler target)
    {
        if (target.unitType != UnitType.Enemy)
            return;

        TileNode curNode = target.curNode;
        target.AddStatusEffect<SlowCondition>(new SlowCondition(target, 0, modifyValue, () => curNode == target.curNode));
    }

    protected override void CustomFunc()
    {
        var targetNodes = new HashSet<TileNode>(curNode.neighborNodeDic.Values);
        foreach (var node in curNode.neighborNodeDic.Values)
        {
            node.curNodeEffects.Add(this);
        }

        foreach(var target in GameManager.Instance.adventurersList.Where(target => targetNodes.Any(node => node == target.curNode)))
            Effect(target);
    }
}
