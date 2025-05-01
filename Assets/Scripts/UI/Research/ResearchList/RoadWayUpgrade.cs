using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class RoadWayUpgrade : MonoBehaviour, IResearch, IStatModifier, IBattlerEnterEffect, IBuffEffectInfo
{
    public float modifyValue { get => 1.2f; }

    public string descKey => "tooltip_buff_3";

    public float effectValue => Mathf.Round((modifyValue - 1) * 100);

    public void Effect(Battler target)
    {
        if (target.unitType != UnitType.Player)
            return;

        TileNode curNode = target.curNode;
        target.AddStatusEffect<AttackSpeedUpCondition>(new AttackSpeedUpCondition(target, 0, modifyValue, () => curNode == target.curNode));
    }

    private bool AddRoadWayUpgrade(ITileKind target)
    {
        if(target is Tile tile && tile._TileType == TileType.Path)
        {
            tile._isUpgraded.Subscribe(_ =>
            {
                if(_)
                    tile.curTileEffects.Add(this);
                else
                    tile.curTileEffects.Remove(this);

            }).AddTo(tile);
            return true;
        }

        return false;
    }

    public void ActiveResearch()
    {
        foreach(var node in NodeManager.Instance._ActiveNodes)
        {
            if(node.curTile == null) continue;
            AddRoadWayUpgrade(node.curTile);
        }

        NodeManager.Instance.AddSetTileEvent(AddRoadWayUpgrade);
    }
}
