using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStone : Environment, IManaSupply
{
    public int manaValue { get => (int)value; }

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
