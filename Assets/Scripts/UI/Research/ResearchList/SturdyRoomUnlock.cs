using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SturdyRoomUnlock : MonoBehaviour, IResearch
{
    private void ApplySturdyRoom(CompleteRoom completeRoom)
    {
        foreach (var room in completeRoom._IncludeRooms)
        {
            if (room.isUpgraded)
                continue;

            TileUpgrader upgrader = room.GetComponent<TileUpgrader>();
            upgrader?.UpgradeTile();
        }

        
    }

    public void ActiveResearch()
    {
        foreach (var completeRoom in NodeManager.Instance.roomTiles)
            ApplySturdyRoom(completeRoom);

        NodeManager.Instance.roomTiles.ObserveAdd().Subscribe(_ => ApplySturdyRoom(_.Value)).AddTo(NodeManager.Instance.gameObject);
    }
}
