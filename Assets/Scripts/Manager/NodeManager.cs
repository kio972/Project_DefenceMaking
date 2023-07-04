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

    //public Vector3 endPointPosition = Vector3.zero;

    public void SetTile(TileNode curNode, string prefabPath)
    {
        
    }

    public void ExpandEmptyNode(TileNode curNode, int emptyNodeSize)
    {
        //1. 최초노드의 Left에서 SetNewNode
        //RightUp, Right, RightDown, LeftDown, Left, LeftUp
        //Direction[] directions = new Direction[]
        //{ Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown, Direction.Left, Direction.LeftUp};

        SetNewNode(curNode);
        Direction[] directions = new Direction[]
        { Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown, Direction.Left, Direction.LeftUp};

        TileNode node = curNode;
        for (int i = 1; i < emptyNodeSize; i++)
        {
            SetNewNode(node.neighborNodeDic[Direction.Left]);
            node = node.neighborNodeDic[Direction.Left];

            foreach (Direction direction in directions)
            {
                for (int j = 0; j < i; j++)
                {
                    SetNewNode(node.neighborNodeDic[direction]);
                    node = node.neighborNodeDic[direction];
                }
            }
        }

    }

    public List<Direction> GetAllDirection()
    {
        List<Direction> directions = new List<Direction>();
        directions.Add(Direction.Left);
        directions.Add(Direction.LeftUp);
        directions.Add(Direction.RightUp);
        directions.Add(Direction.Right);
        directions.Add(Direction.RightDown);
        directions.Add(Direction.LeftDown);
        return directions;
    }

    public void SetNeighborNode(TileNode targetNode)
    {
        int[] index = new int[2] { targetNode.row, targetNode.col };
        List<Direction> directions = GetAllDirection();
        foreach(Direction direction in directions)
        {
            int[] neighborIndex = NextIndex(index, direction);
            TileNode node = FindNode(neighborIndex[0], neighborIndex[1]);
            if(node != null)
            {
                targetNode.PushNeighborNode(direction, node);
                node.PushNeighborNode(UtilHelper.ReverseDirection(direction), targetNode);
            }
        }
    }

    public int[] NextIndex(int[] originIndex, Direction direction)
    {
        int row = originIndex[0];
        int col = originIndex[1];

        switch (direction)
        {
            case Direction.Left:
                col--;
                break;
            case Direction.Right:
                col++;
                break;
            case Direction.LeftUp:
                row++;
                if (row % 2 == 0)
                    col--;
                break;
            case Direction.LeftDown:
                row--;
                if (row % 2 == 0)
                    col--;
                break;
            case Direction.RightUp:
                row++;
                if (row % 2 != 0)
                    col++;
                break;
            case Direction.RightDown:
                row--;
                if (row % 2 != 0)
                    col++;
                break;
        }

        return new int[2] { row, col };
    }

    public TileNode FindNode(int row, int col)
    {
        foreach(TileNode node in allNodes)
        {
            if (node.row == row && node.col == col)
                return node;
        }

        return null;
    }

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

    public void BuildNewNodes(Direction direction = Direction.None)
    {
        List<TileNode> tempNodes = new List<TileNode>(allNodes);
        //Dictionary<TileNode, List<TileNode>> newNodeDictionary = new Dictionary<TileNode, List<TileNode>>();
        List<Direction> directions = new List<Direction>();
        if (direction == Direction.None)
            directions = GetAllDirection();
        else
            directions.Add(direction);

        foreach (TileNode node in tempNodes)
        {
            //node의 이웃타일이 없는타일의 방향에 노드추가
            foreach(Direction dir in directions)
            {
                TileNode newNode = node.AddNode(node.row, node.col, dir);
            }
        }

        //newNodeDictionary에 있는 key TileNode를 받아오는 함수
        //List<TileNode> keyNodes = UtilHelper.GetKeyValues(newNodeDictionary);
        //foreach (TileNode keyNode in keyNodes)
        //{
        //    foreach (TileNode newNode in newNodeDictionary[keyNode])
        //    {
        //        Direction dir = keyNode.GetNodeDirection(newNode);
        //        newNode.SetNeighborNode(keyNode, dir);
        //    }
        //}
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
            node.DeActiveGuide();
            node.setAvail = false;
        }
    }

    private bool HaveConnectedTile(TileNode node)
    {
        List<Direction> allDirection = GetAllDirection();
        foreach(Direction direction in allDirection)
        {
            if (!node.neighborNodeDic.ContainsKey(direction))
                continue;

            if (node.neighborNodeDic[direction].curTile == null)
                continue;

            Direction reversedDirection = UtilHelper.ReverseDirection(direction);
            if (node.neighborNodeDic[direction].curTile.PathDirection.Contains(reversedDirection))
                return true;
            if (node.neighborNodeDic[direction].curTile.RoomDirection.Contains(reversedDirection))
                return true;
        }

        return false;
    }

    public void SetEnvironmentAvail()
    {
        SetVirtualNode();

        foreach(TileNode node in virtualNodes)
        {
            if (!HaveConnectedTile(node))
                node.SetAvail(true);
            else
                node.SetAvail(false);
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



    public TileNode InstanceNewNode(TileNode node, Direction direction, int[] index ,Transform parent = null)
    {
        Vector3 position = UtilHelper.GetGridPosition(node.transform.position, direction, 1f);
        TileNode newNode = Resources.Load<TileNode>("Prefab/Tile/EmptyTile");
        newNode = Instantiate(newNode);
        newNode.Init(index[0], index[1]);
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
        curNode.AddNode(curNode.row, curNode.col, Direction.Left);
        curNode.AddNode(curNode.row, curNode.col, Direction.LeftUp);
        curNode.AddNode(curNode.row, curNode.col, Direction.LeftDown);
        curNode.AddNode(curNode.row, curNode.col, Direction.Right);
        curNode.AddNode(curNode.row, curNode.col, Direction.RightUp);
        curNode.AddNode(curNode.row, curNode.col, Direction.RightDown);
    }

    //private void Update()
    //{
        
    //}
}
