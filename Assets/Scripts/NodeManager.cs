using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    //비활성화 상태의 노드
    public List<TileNode> virtualNodes;
    //활성화 상태의 노드
    public List<TileNode> activeNodes;

    public List<TileNode> emptyNodes;

    public TileNode startPoint;


    public void SetEmptyNode()
    {
        emptyNodes = new List<TileNode>();
        TileNode[] nodes = FindObjectsOfType<TileNode>();
        foreach (TileNode node in nodes)
            emptyNodes.Add(node);
    }

    public TileNode InstanceTile(GameObject tilePrefab, Vector3 targetPosition)
    {
        TileNode tile = tilePrefab.GetComponent<TileNode>();
        if(tile == null) return null;

        tile = Instantiate(tile);
        tile.transform.position = targetPosition;
        return tile;
    }

    public TileNode InstanceNewNode(TileNode node, Direction direction)
    {
        Vector3 position = UtilHelper.GetGridPosition(node.transform.position, direction, 1f);
        TileNode newNode = Resources.Load<TileNode>("Prefab/Tile/EmptyTile");
        newNode = Instantiate(newNode);
        newNode.transform.position = position;

        return newNode;
    }

    public void SetAvailTile(TileNode targetNode)
    {
        List<Direction> targetNode_PathDirection = targetNode.PathDirection;
        List<Direction> targetNode_RoomDirection = targetNode.RoomDirection;
        foreach (TileNode node in virtualNodes)
        {
            if (node.IsConnected(targetNode_PathDirection, targetNode_RoomDirection))
                node.SetAvail();
        }
    }
    
    public void SetNewNode(TileNode curNode)
    {
        curNode.AddNode(Direction.Left);
        curNode.AddNode(Direction.LeftUp);
        curNode.AddNode(Direction.LeftDown);
        curNode.AddNode(Direction.Right);
        curNode.AddNode(Direction.RightUp);
        curNode.AddNode(Direction.RightDown);

        curNode.DirectionalNode(Direction.Left).SetNeighborNode(curNode, Direction.Left);
        curNode.DirectionalNode(Direction.LeftUp).SetNeighborNode(curNode, Direction.LeftUp);
        curNode.DirectionalNode(Direction.LeftDown).SetNeighborNode(curNode, Direction.LeftDown);
        curNode.DirectionalNode(Direction.Right).SetNeighborNode(curNode, Direction.Right);
        curNode.DirectionalNode(Direction.RightUp).SetNeighborNode(curNode, Direction.RightUp);
        curNode.DirectionalNode(Direction.RightDown).SetNeighborNode(curNode, Direction.RightDown);
    }

    public bool IsNodeInstance()
    {

        return false;
    }

    public void Init()
    {

    }
}
