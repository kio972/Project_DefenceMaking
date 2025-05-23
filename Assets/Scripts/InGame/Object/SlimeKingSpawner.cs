using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKingSpawner : TileReward
{
    public override void BuildBaseTile(TileNode targetNode)
    {
        BattlerPooling.Instance.SpawnMonster("slime_king", targetNode);
        base.BuildBaseTile(targetNode);
        targetNode.curTile.spawnLock = true;
    }
}
