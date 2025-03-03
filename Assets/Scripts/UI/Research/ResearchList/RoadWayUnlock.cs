using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadWayUnlock : MonoBehaviour, IResearch
{
    private bool IsConnectedWithRoadWay(Tile tile)
    {
        foreach(var node in tile.curNode.neighborNodeDic.Values)
        {
            if(node.curTile == null || node.curTile._TileType != TileType.Path) continue;

            if(node.curTile.isUpgraded)
                return true;
        }

        return false;
    }

    private bool RoadWayCheck(ITileKind tileKind)
    {
        if(tileKind is Tile tile && tile._TileType == TileType.Path)
        {
            List<Tile> paths = UtilHelper.GetPathCount(tile);
            if (paths.Count >= 5)
            {
                foreach (var path in paths)
                {
                    TileUpgrader upgrader = path.GetComponent<TileUpgrader>();
                    upgrader?.UpgradeTile();
                }
            }
        }

        return false;
    }

    private void ApplyRoadWay()
    {
        HashSet<Tile> checkedTiles = new HashSet<Tile>();
        foreach(var node in NodeManager.Instance._ActiveNodes)
        {
            if (node.curTile == null)
                continue;

            Tile tile = node.curTile;
            if (tile._TileType != TileType.Path || tile.isUpgraded)
                continue;

            if (checkedTiles.Contains(tile))
                continue;

            List<Tile> tiles = UtilHelper.GetPathCount(tile);
            foreach (var target in tiles)
            {
                if (tiles.Count >= 5)
                {
                    TileUpgrader upgrader = tile.GetComponent<TileUpgrader>();
                    upgrader?.UpgradeTile();
                }

                checkedTiles.Add(tile);
            }
        }
    }

    public void ActiveResearch()
    {
        ApplyRoadWay();
        NodeManager.Instance.AddSetTileEvent(RoadWayCheck);
    }
}
