using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuideState
{
    None,
    Monster,
    Trap,
    Tile,
    Environment,
    Movable,
}

public class NodeManager : IngameSingleton<NodeManager>
{
    //활성화 노드들의 이웃노드 중 비활성화 노드
    public List<TileNode> virtualNodes = new List<TileNode>();
    //활성화 상태의 노드
    private List<TileNode> activeNodes = new List<TileNode>();
    public List<TileNode> _ActiveNodes { get => activeNodes; }

    public List<TileNode> emptyNodes = new List<TileNode>();

    public List<TileNode> allNodes = new List<TileNode>();

    public List<CompleteRoom> roomTiles = new List<CompleteRoom>();

    public List<Tile> dormantTile = new List<Tile>();

    public TileNode startPoint;
    public TileNode endPoint;

    private int minRow;
    private int maxRow;
    private int minCol;
    private int maxCol;
    public int MinRow { get => minRow; }
    public int MaxRow { get => maxRow; }
    public int MinCol { get => minCol; }
    public int MaxCol { get => maxCol; }

    public bool HaveSingleRoom
    {
        get
        {
            foreach (TileNode node in activeNodes)
            {
                if (node.curTile == null) continue;
                if (node.curTile._TileType == TileType.Room_Single)
                    return true;
            }
            return false;
        }
    }

    public CompleteRoom GetRoomByNode(TileNode targetNode)
    {
        foreach(CompleteRoom room in roomTiles)
        {
            foreach(Tile tile in room._IncludeRooms)
            {
                if (tile.curNode == targetNode)
                    return room;
            }
        }
        return null;
    }

    public void ResetNode()
    {
        activeNodes = new List<TileNode>();
        emptyNodes = new List<TileNode>();
        allNodes = new List<TileNode>();
        roomTiles = new List<CompleteRoom>();
        dormantTile = new List<Tile>();
    }


    #region GuidePart
    private GuideState guideState = GuideState.None;

    public GuideState _GuideState { get => guideState; }

    private bool manaGuideState = false;

    public void SetManaGuide(bool value)
    {
        if (manaGuideState == value)
            return;

        manaGuideState = value;
        RoomManaPool.Instance.SetManaUI(value, roomTiles);
    }

    public void SetGuideState(GuideState guideState, Tile tile = null)
    {
        //if (this.guideState == guideState)
        //    return;

        this.guideState = guideState;
        ResetAvail();

        switch (guideState)
        {
            case GuideState.None:
                //ShowMovableTiles();
                break;
            case GuideState.Movable:
                ShowSelectedTile(tile);
                break;
            case GuideState.Tile:
                if (tile == null)
                    return;
                SetTileAvail(tile);
                break;
            case GuideState.Trap:
                SetTrapAvail();
                break;
            case GuideState.Monster:
                SetMonsterAvail();
                break;
            case GuideState.Environment:
                SetEnvironmentAvail();
                break;
        }
    }

    public void LockMovableTiles()
    {
        foreach (TileNode node in activeNodes)
        {
            if (node.curTile != null && node.curTile.movable && node != endPoint)
                node.curTile.movable = false;
        }

        if (guideState == GuideState.None)
            SetGuideState(GuideState.None);
    }

    private void ShowSelectedTile(Tile targetTile)
    {
        targetTile.curNode.SetGuideColor(Color.yellow);
    }

    private void ShowMovableTiles()
    {
        //가이드 노란색으로 설정
        foreach (TileNode node in activeNodes)
        {
            if (node.curTile != null && node.curTile.movable)
            {
                node.SetGuideColor(Color.yellow);
            }
        }
    }

    private bool IsMonsterSetable(TileNode node)
    {
        if (node.curTile != null)
        {
            if (node.curTile._TileType == TileType.Room || node.curTile._TileType == TileType.Room_Single)
            {
                if (!node.curTile.haveMonster)
                    return true;
            }
        }

        return false;
    }

    private void SetMonsterAvail()
    {
        foreach (TileNode node in activeNodes)
        {
            if (IsMonsterSetable(node))
            {
                List<TileNode> path = PathFinder.Instance.FindPath(node);
                node.SetAvail(path != null);
            }
            else
                node.SetAvail(false);
        }
    }

    private void SetTrapAvail()
    {
        foreach (TileNode node in activeNodes)
        {
            if (node.curTile != null && node.curTile._TileType == TileType.Path && node.curTile.trap == null)
                node.SetAvail(true);
            else
                node.SetAvail(false);
        }
    }

    

    private void SetEnvironmentAvail()
    {
        SetVirtualNode();

        foreach (TileNode node in virtualNodes)
        {
            if (!HaveConnectedTile(node))
                node.SetAvail(true);
            else
                node.SetAvail(false);
        }

    }

    private void SetTileAvail(Tile targetTile)
    {
        SetVirtualNode(targetTile._TileType == TileType.End);

        List<Direction> targetNode_PathDirection = targetTile.PathDirection;
        List<Direction> targetNode_RoomDirection = targetTile.RoomDirection;

        List<TileNode> availTile = new List<TileNode>();

        ResetAvail();
        foreach (TileNode node in virtualNodes)
        {
            if (node == null)
                continue;

            bool isConnected = node.IsConnected(targetNode_PathDirection, targetNode_RoomDirection, targetTile._TileType != TileType.End);
            node.SetAvail(isConnected);
        }
    }

    private void ResetAvail()
    {
        foreach (TileNode node in allNodes)
        {
            node.DeActiveGuide();
            node.setAvail = false;
        }
    }

    #endregion

    public void DormantTileCheck()
    {
        List<Tile> dormantTiles = new List<Tile>(dormantTile);
        foreach(Tile tile in dormantTiles)
        {
            tile.DormantAwakeCheck();
        }
    }

    private List<TileNode> GetConnectedRoom(Tile tile)
    {
        List<TileNode> nodes = new List<TileNode>();
        foreach(Direction dir in tile.RoomDirection)
        {
            TileNode node = tile.curNode.DirectionalNode(dir);
            if (node == null && node.curTile == null)
                continue;

            if (node.curTile.RoomDirection.Contains(UtilHelper.ReverseDirection(dir)))
                nodes.Add(node);
        }

        return nodes;
    }

    private List<Tile> BFSRoom(Tile startTile)
    {
        List<Tile> visited = new List<Tile> { startTile };
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);
        
        while(queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            foreach(Direction dir in next.RoomDirection)
            {
                TileNode targetNode = next.curNode.DirectionalNode(dir);
                // 방 방향으로 타일이 모두 닫혀있지 않음
                if (targetNode == null || targetNode.curTile == null)
                    return null;

                if (visited.Contains(targetNode.curTile))
                    continue;

                // 방 방향으로 방타일이 연결되어있지 않음
                if (!targetNode.curTile.RoomDirection.Contains(UtilHelper.ReverseDirection(dir)))
                    return null;

                queue.Enqueue(targetNode.curTile);
                visited.Add(targetNode.curTile);
            }
        }

        return visited;
    }

    public void RoomCheck(Tile tile)
    {
        CompleteRoom newRoom;
        if (tile._TileType == TileType.Room_Single)
        {
            List<Tile> completeRoom = new List<Tile>() { tile };
            newRoom = new CompleteRoom(completeRoom, true);
        }
        else
        {
            //BFS 수행, 방 완성조건에 부합 시 해당방들에 대해 방완성 = true
            List<Tile> completeRoom = BFSRoom(tile);
            if (completeRoom == null || completeRoom.Count <= 1) return;
            newRoom = new CompleteRoom(completeRoom);
        }
        
        // 해당 방들 완성으로 변경코드 추가 예정
        roomTiles.Add(newRoom);
    }

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
    }

    public TileNode InstanceTile(GameObject tilePrefab, Vector3 targetPosition)
    {
        TileNode tile = tilePrefab.GetComponent<TileNode>();
        if (tile == null) return null;

        tile = Instantiate(tile);
        tile.transform.position = targetPosition;
        return tile;
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

    public void AddVirtualNode(TileNode node, Direction direction)
    {
        if(node.neighborNodeDic.ContainsKey(direction))
        {
            TileNode curNode = node.neighborNodeDic[direction];
            if(!activeNodes.Contains(curNode) && !virtualNodes.Contains(node))
            {
                virtualNodes.Add(curNode);
            }
        }
    }

    public void SetVirtualNode(bool withoutDormant = false)
    {
        virtualNodes = new List<TileNode>();

        //activeNodes의 이웃타일들 전부 가상노드에 추가
        //이웃타일이 이미 activeNode에있다면 추가하지않음
        foreach (TileNode node in activeNodes)
        {
            if (withoutDormant && node.curTile != null && node.curTile.IsDormant)
                continue;

            AddVirtualNode(node, Direction.Left);
            AddVirtualNode(node, Direction.LeftUp);
            AddVirtualNode(node, Direction.LeftDown);
            AddVirtualNode(node, Direction.Right);
            AddVirtualNode(node, Direction.RightUp);
            AddVirtualNode(node, Direction.RightDown);
        }
    }

    public void SetEmptyNode()
    {
        emptyNodes = new List<TileNode>();
        TileNode[] nodes = FindObjectsOfType<TileNode>();
        foreach (TileNode node in nodes)
            emptyNodes.Add(node);
    }

    private void UpdateMinMaxRowCol(int row, int col)
    {
        if (row < minRow)
            minRow = row;
        if (row > maxRow)
            maxRow = row;
        if (col < minCol)
            minCol = col;
        if (col > maxCol)
            maxCol = col;
    }

    public TileNode InstanceNewNode(TileNode node, Direction direction, int[] index ,Transform parent = null)
    {
        Vector3 position = UtilHelper.GetGridPosition(node.transform.position, direction, 1f);
        TileNode newNode = Resources.Load<TileNode>("Prefab/Tile/EmptyTile");
        newNode = Instantiate(newNode);
        newNode.Init(index[0], index[1]);
        UpdateMinMaxRowCol(index[0], index[1]);
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
}
