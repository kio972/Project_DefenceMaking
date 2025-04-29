using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater : Environment, ISpawnTimeModifier, IObjectSetEffect
{
    public float spawnTimeWeight => value;

    public void Effect(IObjectKind target)
    {
        if(target is MonsterSpawner spawner)
        {
            spawner.SetSpawntimeModifier(this, true);
        }
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
