using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSpring : Environment, ISpawnTimeModifier
{
    public float spawnTimeWeight => value;

    protected override void CustomFunc()
    {
        foreach(var tileNode in curNode.neighborNodeDic.Values)
        {
            if (tileNode.curTile == null)
                continue;
            if (tileNode.curTile.spawner == null)
                continue;

            tileNode.curTile.spawner.SetSpawntimeModifier(this, true);
        }
    }
}
