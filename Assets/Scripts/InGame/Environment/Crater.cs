using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater : Environment, ISpawnTimeModifier, IObjectSetEffect, ITileArrowEffect
{
    public float spawnTimeWeight => value;

    public void Effect(IObjectKind target)
    {
        if(target is MonsterSpawner spawner)
        {
            spawner.SetSpawntimeModifier(this, true);
        }
    }

    public ArrowColor GetArrowColor(ITileKind target)
    {
        if (target is Tile tile && tile.spawner != null)
        {
            return ArrowColor.Green;
        }

        return ArrowColor.None;
    }

    protected override void CustomFunc()
    {
        foreach(var tileNode in curNode.neighborNodeDic.Values)
        {
            tileNode.curNodeEffects.Add(this);

            if (tileNode.curTile == null)
                continue;
            if (tileNode.curTile.spawner == null)
                continue;

            Effect(tileNode.curTile.spawner);
        }
    }
}
