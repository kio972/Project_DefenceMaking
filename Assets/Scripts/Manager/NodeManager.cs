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

    public List<TileNode> allNodes = new List<TileNode>();

    public TileNode startPoint;
    public TileNode endPoint;

    public Vector3 endPointPosition = Vector3.zero;

    public void SetActiveNode(TileNode node, bool value)
    {
        if(value)
        {
            activeNodes.Add(node);
            emptyNodes.Remove(node);
        }
        else
        {
            emptyNodes.Add(node);
            activeNodes.Remove(node);
        }
    }
        

    private List<Direction> SetDirection(Direction direction)
    {
        List<Direction> directions = new List<Direction>();
        if (direction == Direction.None)
        {
            directions.Add(Direction.Left);
            directions.Add(Direction.LeftUp);
            directions.Add(Direction.RightUp);
            directions.Add(Direction.Right);
            directions.Add(Direction.RightDown);
            directions.Add(Direction.LeftDown);
        }
        else
            directions.Add(direction);
        return directions;
    }

    private void AddNewNodeToDic(TileNode newNode, TileNode parentNode, Dictionary<TileNode, List<TileNode>> newNodeDictionary)
    {
        if (newNode == null)
            return;

        if (newNodeDictionary.ContainsKey(parentNode))
        {
            if (newNodeDictionary[parentNode] == null)
                newNodeDictionary[parentNode] = new List<TileNode>();
            newNodeDictionary[parentNode].Add(newNode);
        }
        else
        {
            List<TileNode> newNodes = new List<TileNode>();
            newNodes.Add(newNode);
            newNodeDictionary.Add(parentNode, newNodes);
        }
    }

    public void BuildNewNodes(Direction direction)
    {
        List<TileNode> tempNodes = new List<TileNode>(allNodes);
        Dictionary<TileNode, List<TileNode>> newNodeDictionary = new Dictionary<TileNode, List<TileNode>>();

        foreach (TileNode node in tempNodes)
        {
            //node의 이웃타일이 없는타일의 방향에 노드추가
            TileNode newNode = node.AddNode(direction);
            if (newNode != null)
            {
                AddNewNodeToDic(newNode, node, newNodeDictionary);
                allNodes.Add(newNode);
            }
        }

        //newNodeDictionary에 있는 key TileNode를 받아오는 함수
        List<TileNode> keyNodes = UtilHelper.GetKeyValues(newNodeDictionary);
        foreach (TileNode keyNode in keyNodes)
        {
            foreach (TileNode newNode in newNodeDictionary[keyNode])
            {
                Direction dir = keyNode.GetNodeDirection(newNode);
                newNode.SetNeighborNode(keyNode, dir);
            }
        }
    }

    public TileNode InstanceTile(GameObject tilePrefab, Vector3 targetPosition)
    {
        TileNode tile = tilePrefab.GetComponent<TileNode>();
        if (tile == null) return null;

        tile = Instantiate(tile);
        tile.transform.position = targetPosition;
        return tile;
    }

    public void ResetAvail()
    {
        foreach (TileNode node in allNodes)
        {
            node.gameObject.SetActive(false);
            node.setAvail = false;
        }
    }

    public void SetTileAvail(Tile targetTile)
    {
        SetVirtualNode();

        List<Direction> targetNode_PathDirection = targetTile.PathDirection;
        List<Direction> targetNode_RoomDirection = targetTile.RoomDirection;

        List<TileNode> availTile = new List<TileNode>();

        ResetAvail();
        //UtilHelper.SetCollider(true, virtualNodes);
        foreach (TileNode node in virtualNodes)
        {
            bool isConnected = node.IsConnected(targetNode_PathDirection, targetNode_RoomDirection);
            node.SetAvail(isConnected);
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

    public void SetVirtualNode()
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



    public TileNode InstanceNewNode(TileNode node, Direction direction, Transform parent = null)
    {
        Vector3 position = UtilHelper.GetGridPosition(node.transform.position, direction, 1f);
        TileNode newNode = Resources.Load<TileNode>("Prefab/Tile/EmptyTile");
        newNode = Instantiate(newNode);
        newNode.transform.position = position;
        if(parent != null)
            newNode.transform.SetParent(parent);
        else
        {
            MapBuilder map = FindObjectOfType<MapBuilder>();
            if (map != null)
                newNode.transform.SetParent(map.transform);
        }
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

    private void Update()
    {
        //if(endPointPosition != endPoint.transform.position)
        //{
        //    //도착지 변경시 영향 함수 호출

        //    if (PathFinder.Instance.FindPath(startPoint) == null)
        //        GameManager.Instance.speedController.SetSpeedZero();
        //    else
        //        GameManager.Instance.speedController.SetSpeedNormal();

        //    Adventurer[] adventurers = FindObjectsOfType<Adventurer>();
        //    foreach (Adventurer adventurer in adventurers)
        //        adventurer.EndPointMoved();

        //    endPointPosition = endPoint.transform.position;
        //}
    }
}
