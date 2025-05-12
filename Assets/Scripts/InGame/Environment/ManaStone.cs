using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStone : Environment, ITileManaEffect, ITileArrowEffect
{
    public int manaValue { get => (int)value; }

    public ArrowColor GetArrowColor(ITileKind target)
    {
        if (target is Tile tile)
        {
            if (tile._TileType is TileType.Start or TileType.End)
                return ArrowColor.None;

            return ArrowColor.Green;
        }

        return ArrowColor.None;
    }

    public string GetManaText(ITileKind target)
    {
        if (target is Tile tile)
        {
            if (tile._TileType is TileType.Start or TileType.End or TileType.Environment)
                return null;

            int curVal = curNode == null ? manaValue : 0;
            if(tile._TileType is TileType.Path)
            {
                return $"<color=green>{tile.SupplyMana + curVal}</color>";
            }

            return $"<color=green>{tile.RoomMana + curVal}</color>";
        }

        return null;
    }

    protected override void CustomFunc()
    {
        foreach(var node in curNode.neighborNodeDic.Values)
        {
            if (node == null || node.curTile == null)
                continue;
            int updateRoomMana = node.curTile.RoomMana;
            GameManager.Instance.UpdateTotalMana();
        }
    }
}
