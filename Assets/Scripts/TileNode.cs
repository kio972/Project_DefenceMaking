using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    None,
    LeftUp,
    RightUp,
    Left,
    Right,
    LeftDown,
    RightDown,
}

public class TileNode : MonoBehaviour
{
    [SerializeField]
    private List<Direction> pathDirection = new List<Direction>();
    [SerializeField]
    private List<Direction> roomDirection = new List<Direction>();

    //public List<TileNode> neighborNodes = new List<TileNode>();
    public Dictionary<Direction, TileNode> neighborNodeDic = new Dictionary<Direction, TileNode>();

    public List<Direction> PathDirection { get => pathDirection; }
    public List<Direction> RoomDirection { get => roomDirection; }

    public bool isActive = false;
    public bool haveTrap = false;

    public void SwapTile(TileNode prevNode)
    {
        Dictionary<Direction, TileNode> newDic = new Dictionary<Direction, TileNode>(prevNode.neighborNodeDic);
        neighborNodeDic = newDic;
        //노드별로 이웃타일 재지정
        DirectionalNode(Direction.Left)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.Left), this);
        DirectionalNode(Direction.LeftDown)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.LeftDown), this);
        DirectionalNode(Direction.LeftUp)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.LeftUp), this);
        DirectionalNode(Direction.Right)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.Right), this);
        DirectionalNode(Direction.RightDown)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.RightDown), this);
        DirectionalNode(Direction.RightUp)?.PushNeighborNode(UtilHelper.ReverseDirection(Direction.RightUp), this);
    }

    public Direction GetNodeDirection(TileNode value)
    {
        foreach (var pair in neighborNodeDic)
        {
            if (EqualityComparer<TileNode>.Default.Equals(pair.Value, value))
            {
                return pair.Key;
            }
        }

        // 값이 없을 경우 기본값 반환 또는 예외 처리
        return Direction.None;
    }

    public void SetAvail()
    {
        //배치가능한 virtual노드에 
    }

    public bool IsConnected(List<Direction> targetNode_PathDirection, List<Direction> targetNode_RoomDirection)
    {
        //현재 노드와 targetNode를 비교하여 연결되어있는지 확인하는 함수
        bool isConnected = false;
        foreach(Direction direction in pathDirection)
        {
            foreach (Direction targetDirection in targetNode_PathDirection)
            {
                if (direction == UtilHelper.ReverseDirection(targetDirection))
                    return true;
            }
        }

        foreach (Direction direction in roomDirection)
        {
            foreach (Direction targetDirection in targetNode_RoomDirection)
            {
                if (direction == UtilHelper.ReverseDirection(targetDirection))
                    return true;
            }
        }

        return isConnected;
    }

    public List<Direction> RotateNode(List<Direction> pathDirection)
    {
        //현재 노드의 pathDirection과 roomDirection을 60도 회전하는 함수
        List<Direction> newDirection = new List<Direction>();
        foreach(Direction dir in pathDirection)
        {
            switch(dir)
            {
                case Direction.Left:
                    newDirection.Add(Direction.LeftUp);
                    break;
                case Direction.LeftUp:
                    newDirection.Add(Direction.RightUp);
                    break;
                case Direction.LeftDown:
                    newDirection.Add(Direction.Left);
                    break;
                case Direction.Right:
                    newDirection.Add(Direction.RightDown);
                    break;
                case Direction.RightUp:
                    newDirection.Add(Direction.Right);
                    break;
                case Direction.RightDown:
                    newDirection.Add(Direction.LeftDown);
                    break;
            }
        }

        return newDirection;
    }

    public List<TileNode> CalculatePathableNode()
    {
        List<TileNode> pathableNodes = new List<TileNode>();
        foreach(Direction direction in pathDirection)
        {
            TileNode node = DirectionalNode(direction);
            if (node != null)
                pathableNodes.Add(node);
        }

        foreach (Direction direction in roomDirection)
        {
            TileNode node = DirectionalNode(direction);
            if (node != null)
                pathableNodes.Add(node);
        }

        return pathableNodes;
    }
    
    public void PushNeighborNode(Direction direction, TileNode node)
    {
        if(node == null) return;

        if (neighborNodeDic.ContainsKey(direction))
        {
            neighborNodeDic[direction] = node;
        }
        else
        {
            neighborNodeDic.Add(direction, node);
        }
    }

    //originNode로부터 가상노드들간에 이웃노드들을 설정하는 함수
    public void SetNeighborNode (TileNode originNode, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                PushNeighborNode(Direction.RightUp, originNode.neighborNodeDic[Direction.LeftUp]);
                PushNeighborNode(Direction.RightDown, originNode.neighborNodeDic[Direction.LeftDown]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.Left), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.LeftUp]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.LeftDown]);
                break;
            case Direction.LeftUp:
                PushNeighborNode(Direction.LeftDown, originNode.neighborNodeDic[Direction.Left]);
                PushNeighborNode(Direction.Right, originNode.neighborNodeDic[Direction.RightUp]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.LeftUp), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.Left]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.RightUp]);
                break;
            case Direction.LeftDown:
                PushNeighborNode(Direction.LeftUp, originNode.neighborNodeDic[Direction.Left]);
                PushNeighborNode(Direction.Right, originNode.neighborNodeDic[Direction.RightDown]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.LeftDown), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.Left]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.RightDown]);
                break;
            case Direction.Right:
                PushNeighborNode(Direction.LeftUp, originNode.neighborNodeDic[Direction.RightUp]);
                PushNeighborNode(Direction.LeftDown, originNode.neighborNodeDic[Direction.RightDown]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.Right), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.RightUp]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.RightDown]);
                break;
            case Direction.RightUp:
                PushNeighborNode(Direction.Left, originNode.neighborNodeDic[Direction.LeftUp]);
                PushNeighborNode(Direction.RightDown, originNode.neighborNodeDic[Direction.Right]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.RightUp), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.LeftUp]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.Right]);
                break;
            case Direction.RightDown:
                PushNeighborNode(Direction.RightUp, originNode.neighborNodeDic[Direction.Right]);
                PushNeighborNode(Direction.Left, originNode.neighborNodeDic[Direction.LeftDown]);
                PushNeighborNode(UtilHelper.ReverseDirection(Direction.RightDown), originNode);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.Right]);
                //neighborNodes.Add(originNode.neighborNodeDic[Direction.LeftDown]);
                break;
        }
    }



    public void AddNode(Direction direction)
    {
        TileNode node = null;
        if (!neighborNodeDic.TryGetValue(direction, out node))
        {
            node = NodeManager.Instance.InstanceNewNode(this, direction);
            //neighborNodes.Add(node);
            neighborNodeDic.Add(direction, node);
        }
    }

    public TileNode DirectionalNode(Direction direction)
    {
        if (neighborNodeDic.ContainsKey(direction))
            return neighborNodeDic[direction];

        return null;
    }

    public void Init()
    {
        
    }
}
