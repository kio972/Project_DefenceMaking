using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    //활성화 노드들의 이웃노드 중 비활성화 노드
    public List<TileNode> virtualNodes = new List<TileNode>();
    //활성화 상태의 노드
    public List<TileNode> activeNodes = new List<TileNode>();

    public List<TileNode> emptyNodes = new List<TileNode>();

    public TileNode startPoint;
    public TileNode endPoint;

    public Vector3 endPointPosition = Vector3.zero;

    public void ResetAvail()
    {
        foreach (TileNode node in activeNodes)
        {
            node.SetAvail(false);
        }

        foreach (TileNode node in emptyNodes)
        {
            node.SetAvail(false);
        }
    }

    public void SetPathAvailTile(TileNode targetNode)
    {
        ResetAvail();
        List<Direction> targetNode_PathDirection = targetNode.PathDirection;
        List<Direction> targetNode_RoomDirection = targetNode.RoomDirection;
        UtilHelper.SetCollider(true, virtualNodes);
        foreach (TileNode node in virtualNodes)
        {
            if (node.IsConnected(targetNode_PathDirection, targetNode_RoomDirection))
                node.SetAvail(true);
            else
            {
                Collider col = node.GetComponentInChildren<Collider>();
                if(col != null)
                    col.enabled = false;
            }
        }
    }

    public void AddVirtualNode(TileNode node, Direction direction)
    {
        if(node.neighborNodeDic.ContainsKey(direction))
        {
            TileNode curNode = node.neighborNodeDic[direction];
            if(!activeNodes.Contains(curNode))
            {
                virtualNodes.Add(curNode);
            }
        }
    }

    public void SetVirtualTile()
    {
        virtualNodes = new List<TileNode>();

        //activeNodes의 이웃타일들 전부 가상노드에 추가
        //이웃타일이 이미 activeNode에있다면 추가하지않음
        foreach (TileNode node in activeNodes)
        {
            AddVirtualNode(node, Direction.Left);
            AddVirtualNode(node, Direction.LeftUp);
            AddVirtualNode(node, Direction.LeftDown);
            AddVirtualNode(node, Direction.Right);
            AddVirtualNode(node, Direction.RightUp);
            AddVirtualNode(node, Direction.RightDown);
        }
    }

    //public void SetAvailTile(TileNode curTile)
    //{
    //    SetVirtualTile();
    //    UtilHelper.SetCollider(false, emptyNodes);
    //    UtilHelper.SetCollider(false, activeNodes);
        
    //}

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

    private void Update()
    {
        if(endPointPosition != endPoint.transform.position)
        {
            //도착지 변경시 영향 함수 호출

            if (PathFinder.Instance.FindPath(startPoint) == null)
                GameManager.Instance.speedController.SetSpeedZero();
            else
                GameManager.Instance.speedController.SetSpeedNormal();

            Adventurer[] adventurers = FindObjectsOfType<Adventurer>();
            foreach (Adventurer adventurer in adventurers)
                adventurer.EndPointMoved();

            endPointPosition = endPoint.transform.position;
        }
    }
}
