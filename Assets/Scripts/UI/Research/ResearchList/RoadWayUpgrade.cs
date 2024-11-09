using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadWayUpgrade : MonoBehaviour, Research
{
    private void ApplyRoadWayUpgrade()
    {
        foreach(var node in NodeManager.Instance._ActiveNodes)
        {
            if (node.curTile == null)
                continue;

            Tile tile = node.curTile;
            if (tile._TileType != TileType.Path || !tile.isUpgraded)
                continue;

            TileUpgrader upgrader = tile.GetComponent<TileUpgrader>();
            if (upgrader != null)
                tile.AddAllyAttackSpeedMult(upgrader);
        }
    }

    public void ActiveResearch()
    {
        ApplyRoadWayUpgrade();
    }
}
