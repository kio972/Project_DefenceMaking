using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKingSpawner : TileReward
{
    public override void BuildBaseTile(TileNode targetNode)
    {
        base.BuildBaseTile(targetNode);
        BattlerPooling.Instance.SpawnMonster("slime_king", targetNode);
    }
}
