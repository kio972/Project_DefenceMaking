using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public enum Direction
{
    None,
    Left,
    LeftUp,
    RightUp,
    Right,
    RightDown,
    LeftDown,
}

public class TileNode : MonoBehaviour
{
    public int row;
    public int col;

    public Tile curTile { get => tileKind is Tile tile ? tile : null; }
    public Environment environment { get => tileKind is Environment environment ? environment : null; }
    public ITileKind tileKind;

    //public List<TileNode> neighborNodes = new List<TileNode>();
    public Dictionary<Direction, TileNode> neighborNodeDic = new Dictionary<Direction, TileNode>();

    [SerializeField]
    private Renderer guideObject;
    public bool GuideActive { get => guideObject.gameObject.activeSelf; }

    public bool setAvail = false;
    public bool isLock = false;
    [SerializeField]
    private RoadEmptyMaterialProperties guideColor;

    [SerializeField]
    private Fog fog;

    public HashSet<TileNode> revealNodes = new HashSet<TileNode>();

    public Dictionary<Direction, int> connectionState { get; private set; } = new Dictionary<Direction, int>();


    public void UpdateNodeConnectionState()
    {
        foreach(var direction in neighborNodeDic.Keys)
        {
            if(!connectionState.ContainsKey(direction))
                connectionState.Add(direction, 0);

            Tile targetTile = neighborNodeDic[direction].curTile;
            if (targetTile == null || targetTile.IsDormant)
            {
                connectionState[direction] = 0;
                continue;
            }

            if(targetTile._TileType == TileType.End)
                connectionState[direction] = 0;
            else if (targetTile.PathDirection.Contains(UtilHelper.ReverseDirection(direction)))
                connectionState[direction] = 1;
            else if(targetTile.RoomDirection.Contains(UtilHelper.ReverseDirection(direction)))
                connectionState[direction] = 2;
            else
                connectionState[direction] = 0;
        }
    }

    public void RestoreFog()
    {
        fog?.RestoreFog();
    }

    public void SetFog(FogState state)
    {
        fog?.SetFog(state);
    }

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

    public void SetGuideColor(Color color)
    {
        if (guideObject == null)
            return;
        guideObject.gameObject.SetActive(true);
        guideColor?.SetColor(color);
    }

    public void SetAvail(bool value)
    {
        if (guideObject == null)
            return;

        guideObject.gameObject.SetActive(true && !isLock);
        setAvail = value && !isLock;

        if(value)
            guideColor?.SetColor(Color.green);
        else
            guideColor?.SetColor(Color.red);
    }



    public bool IsConnected(List<Direction> pathDirection, List<Direction> roomDirection, bool includeDormant = true)
    {
        //현재 노드가 pathDirection, roomDirection의 조건을 충족하는지 확인하는 함수
        int connectedCount = 0;
        foreach(Direction direction in NodeManager.Instance.GetAllDirection())
        {
            if (neighborNodeDic.ContainsKey(direction))
            {
                TileNode neighborNode = neighborNodeDic[direction];
                Tile curTile = neighborNode.curTile;
                if (!NodeManager.Instance._ActiveNodes.Contains(neighborNode) || curTile == null)
                    continue;

                if (!includeDormant)
                {
                    if (curTile.IsDormant)
                        return false;
                    else if (NodeManager.Instance.FindPath(NodeManager.Instance.startPoint, neighborNode) == null)
                        return false;
                }

                Direction reversed = UtilHelper.ReverseDirection(direction);
                if (curTile.PathDirection.Contains(reversed))
                {
                    if (pathDirection.Contains(direction))
                        connectedCount++;
                    else
                        return false;
                }
                else if (curTile.RoomDirection.Contains(reversed))
                {
                    if (roomDirection.Contains(direction))
                        connectedCount++;
                    else
                        return false;
                }
                else if(pathDirection.Contains(direction) || roomDirection.Contains(direction))
                    return false;
            }
        }

        if (connectedCount == 0)
            return false;
        else
            return true;
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
