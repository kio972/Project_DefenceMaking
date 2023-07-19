using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    public int row;
    public int col;

    public Tile curTile;

    //public List<TileNode> neighborNodes = new List<TileNode>();
    public Dictionary<Direction, TileNode> neighborNodeDic = new Dictionary<Direction, TileNode>();

    [SerializeField]
    private Renderer guideObject;
    public bool GuideActive { get => guideObject.gameObject.activeSelf; }

    public bool setAvail = false;

    public void DeActiveGuide()
    {
        guideObject.gameObject.SetActive(false);
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

    public void SetAvail(bool value)
    {
        if (guideObject == null)
            return;

        guideObject.gameObject.SetActive(true);
        setAvail = value;

        if(value)
        {
            guideObject.material.SetColor("_FresnelColor", Color.green);
        }
        else
        {
            guideObject.material.SetColor("_FresnelColor", Color.red);
        }
    }



    public bool IsConnected(List<Direction> targetNode_PathDirection, List<Direction> targetNode_RoomDirection)
    {
        //현재 노드와 targetNode를 비교하여 연결되어있는지 확인하는 함수
        bool isConnected = false;
        foreach (Direction direction in targetNode_PathDirection)
        {
            if (neighborNodeDic.ContainsKey(direction))
            {
                TileNode targetNode = neighborNodeDic[direction];
                if (!NodeManager.Instance.activeNodes.Contains(targetNode) || targetNode.curTile == null)
                    continue;

                foreach (Direction targetDirection in targetNode.curTile.PathDirection)
                {
                    if (direction == UtilHelper.ReverseDirection(targetDirection))
                        return true;
                }
            }
        }

        foreach (Direction direction in targetNode_RoomDirection)
        {
            if (neighborNodeDic.ContainsKey(direction))
            {
                TileNode targetNode = neighborNodeDic[direction];
                if (!NodeManager.Instance.activeNodes.Contains(targetNode))
                    continue;

                foreach (Direction targetDirection in targetNode.curTile.RoomDirection)
                {
                    if (direction == UtilHelper.ReverseDirection(targetDirection))
                        return true;
                }
            }
        }

        return isConnected;
    }

    public void PushNeighborNode(Direction direction, TileNode node)
    {
        if(node == null) return;

        if (neighborNodeDic.ContainsKey(direction))
            neighborNodeDic[direction] = node;
        else
            neighborNodeDic.Add(direction, node);
    }

    //originNode로부터 가상노드들간에 이웃노드들을 설정하는 함수
    public void SetNeighborNode (TileNode originNode, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                PushNeighborNode(Direction.RightUp, originNode.DirectionalNode(Direction.LeftUp));
                PushNeighborNode(Direction.RightDown, originNode.DirectionalNode(Direction.LeftDown));
                break;
            case Direction.LeftUp:
                PushNeighborNode(Direction.LeftDown, originNode.DirectionalNode(Direction.Left));
                PushNeighborNode(Direction.Right, originNode.DirectionalNode(Direction.RightUp));
                break;
            case Direction.LeftDown:
                PushNeighborNode(Direction.LeftUp, originNode.DirectionalNode(Direction.Left));
                PushNeighborNode(Direction.Right, originNode.DirectionalNode(Direction.RightDown));
                break;
            case Direction.Right:
                PushNeighborNode(Direction.LeftUp, originNode.DirectionalNode(Direction.RightUp));
                PushNeighborNode(Direction.LeftDown, originNode.DirectionalNode(Direction.RightDown));
                break;
            case Direction.RightUp:
                PushNeighborNode(Direction.Left, originNode.DirectionalNode(Direction.LeftUp));
                PushNeighborNode(Direction.RightDown, originNode.DirectionalNode(Direction.Right));
                break;
            case Direction.RightDown:
                PushNeighborNode(Direction.RightUp, originNode.DirectionalNode(Direction.Right));
                PushNeighborNode(Direction.Left, originNode.DirectionalNode(Direction.LeftDown));
                break;
        }

        PushNeighborNode(UtilHelper.ReverseDirection(direction), originNode);
    }


    public TileNode AddNode(int originRow, int originCol, Direction direction)
    {
        TileNode node = null;
        if (!neighborNodeDic.TryGetValue(direction, out node))
        {
            int[] index = NodeManager.Instance.NextIndex(new int[2] { originRow, originCol }, direction);
            node = NodeManager.Instance.FindNode(index[0], index[1]);
            if (node != null)
                return node;
            else
            {
                node = NodeManager.Instance.InstanceNewNode(this, direction, index);
                //neighborNodeDic.Add(direction, node);
            }
            
            return node;
        }
        return null;
    }

    public TileNode DirectionalNode(Direction direction)
    {
        if (neighborNodeDic.ContainsKey(direction))
            return neighborNodeDic[direction];

        return null;
    }



    public void Init(int row, int col)
    {
        this.row = row;
        this.col = col;
        NodeManager.Instance.allNodes.Add(this);
        NodeManager.Instance.emptyNodes.Add(this);
        NodeManager.Instance.SetNeighborNode(this);
    }
}
