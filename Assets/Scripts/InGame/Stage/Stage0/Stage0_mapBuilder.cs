using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_mapBuilder : MapBuilder
{
    protected override void SetBasicTile()
    {
        //��ŸƮ����Ʈ���� ��ŸƮ����Ʈ - ������ - ������  ������ - ������ - ���չ�
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