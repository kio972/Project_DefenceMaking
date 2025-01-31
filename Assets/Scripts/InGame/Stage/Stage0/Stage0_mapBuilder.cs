using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_mapBuilder : MapBuilder
{
    public TileNode SetExampleRoom()
    {
        GameObject roomTile = Resources.Load<GameObject>("Prefab/Tile/RoomTile8");
        GameObject doorTile = Resources.Load<GameObject>("Prefab/Tile/RoomTile22");
        
        TileNode node = NodeManager.Instance.FindNode(-3, 5);
        Tile targetTile = Instantiate(roomTile)?.GetComponent<Tile>();
        targetTile.Init(node, false, false, true);

        TileNode nextNode = node.neighborNodeDic[Direction.RightDown];
        targetTile = Instantiate(roomTile)?.GetComponent<Tile>();
        targetTile.Init(nextNode, false, false, true);
        targetTile.RotateTile(true);

        nextNode = nextNode.neighborNodeDic[Direction.Right];
        targetTile = Instantiate(doorTile)?.GetComponent<Tile>();
        targetTile.Init(nextNode, false, false, true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);
        TileNode doorNode = nextNode;

        nextNode = nextNode.neighborNodeDic[Direction.RightUp];
        targetTile = Instantiate(roomTile)?.GetComponent<Tile>();
        targetTile.Init(nextNode, false, false, true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);

        nextNode = nextNode.neighborNodeDic[Direction.LeftUp];
        targetTile = Instantiate(roomTile)?.GetComponent<Tile>();
        targetTile.Init(nextNode, false, false, true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);
        targetTile.RotateTile(true);

        return doorNode;
    }

    protected override void SetBasicTile()
    {
        //스타트포인트부터 스타트포인트 - 직선길 - 직선길  직선길 - 직선길 - 마왕방
        //print(NodeManager.Instance.startPoint.transform.position);
        GameObject startPointPrefab = Resources.Load<GameObject>("Prefab/Tile/StartTile");
        //GameObject pathPrefab = Resources.Load<GameObject>("Prefab/Tile/RoadTile0");
        GameObject pathPrefab2 = Resources.Load<GameObject>("Prefab/Tile/RoadTile6");
        GameObject endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");
        GameObject roomPrefab = Resources.Load<GameObject>("Prefab/Tile/RoomTile1");

        //NodeManager.Instance.ResetNode();

        Tile startTile = Instantiate(startPointPrefab)?.GetComponent<Tile>();
        startTile.Init(NodeManager.Instance.startPoint, false, false, false);

        NodeManager.Instance.startPoint = startTile.curNode;
        NodeManager.Instance.SetActiveNode(startTile.curNode, true);
        NodeManager.Instance.ExpandEmptyNode(startTile.curNode, emptyNodeSize);

        TileNode nextNode = startTile.curNode.neighborNodeDic[Direction.Right];
        NodeManager.Instance.ExpandEmptyNode(nextNode, emptyNodeSize);

        for (int i = 0; i < 3; i++)
        {
            nextNode = nextNode.neighborNodeDic[Direction.Right];
            NodeManager.Instance.ExpandEmptyNode(nextNode, emptyNodeSize);
        }

        nextNode = nextNode.neighborNodeDic[Direction.RightUp];
        NodeManager.Instance.ExpandEmptyNode(nextNode, emptyNodeSize);

        Tile endTile = Instantiate(endPointPrefab)?.GetComponent<Tile>();
        endTile.Init(nextNode, false, false, false);
        endTile.RotateTile(true);
        NodeManager.Instance.endPoint = endTile.curNode;
        endTile.Movable = true;
    }
}
