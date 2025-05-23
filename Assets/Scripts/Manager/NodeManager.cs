using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public enum GuideState
{
    None,
    Monster,
    ObjectForPath,
    Tile,
    Environment,
    Selected,
    Spawner,
}

public class NodeManager : IngameSingleton<NodeManager>
{
    //활성화 노드들의 이웃노드 중 비활성화 노드
    public HashSet<TileNode> virtualNodes = new HashSet<TileNode>();
    //활성화 상태의 노드
    private HashSet<TileNode> activeNodes = new HashSet<TileNode>();
    public HashSet<TileNode> _ActiveNodes { get => activeNodes; }

    public HashSet<TileNode> emptyNodes = new HashSet<TileNode>();

    public HashSet<TileNode> allNodes = new HashSet<TileNode>();

    public ReactiveCollection<CompleteRoom> roomTiles = new ReactiveCollection<CompleteRoom>();

    public HashSet<TileNode> hiddenTiles = new HashSet<TileNode>();

    public List<Tile> dormantTile = new List<Tile>();

    public Dictionary<TileType, List<Tile>> tileDictionary = null;
    public List<Environment> environments = new List<Environment>();

    public TileNode startPoint;
    public TileNode endPoint;

    public Queue<GameObject> hiddenPrioritys = new Queue<GameObject>();

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

    int directSight = 2;
    int inDirectSight = 1;

    private HashSet<TileNode> _directSightNodes = new HashSet<TileNode>();
    private HashSet<TileNode> _inDirectSightNodes = new HashSet<TileNode>();

    public ReactiveCollection<TileNode> directSightNodes { get; private set; } = new ReactiveCollection<TileNode>();
    public ReactiveCollection<TileNode> inDirectSightNodes { get; private set; } = new ReactiveCollection<TileNode>();

    private Dictionary<(TileNode start, TileNode end), (List<TileNode> path, int version)> pathCache = new Dictionary<(TileNode start, TileNode end), (List<TileNode> path, int version)>();
    private int nodeVersion = int.MinValue;

    public void IncreaseNodeVersion() => nodeVersion++;

    public float GetBattlerDistance(Battler origin, Battler target)
    {
        if (origin.curNode == target.curNode)
        {
            float modifyOrigin = Vector3.Distance(origin.transform.position, origin.curNode.transform.position);
            float modifyTarget = Vector3.Distance(target.transform.position, target.curNode.transform.position);
            Direction originBattlerDirection = UtilHelper.CheckClosestDirection(origin.transform.position - origin.curNode.transform.position);
            Direction targetBattlerDirection = UtilHelper.CheckClosestDirection(target.transform.position - target.curNode.transform.position);
            if (originBattlerDirection == targetBattlerDirection)
                modifyTarget *= -1f;

            return modifyOrigin + modifyTarget;
        }
        else
        {
            List<TileNode> path = FindPath(origin.curNode, target.curNode);
            if (path == null || path.Count <= 0)
                return Mathf.Infinity;
            float distance = path.Count * 1f;

            float modifyOrigin = Vector3.Distance(origin.transform.position, origin.curNode.transform.position);
            Direction originBattlerDirection = UtilHelper.CheckClosestDirection(origin.transform.position - origin.curNode.transform.position);
            Direction originPathDirection = origin.curNode.GetNodeDirection(path[0]);
            if (originBattlerDirection == originPathDirection)
                modifyOrigin *= -1f;

            float modifyTarget = Vector3.Distance(target.transform.position, target.curNode.transform.position);
            Direction targetBattlerDirection = UtilHelper.CheckClosestDirection(target.curNode.transform.position - target.transform.position);
            TileNode destPoint = origin.curNode;
            if (path.Count > 1)
                destPoint = path[path.Count - 2];
            Direction targetPathDirection = destPoint.GetNodeDirection(target.curNode);
            if (targetBattlerDirection == targetPathDirection)
                modifyTarget *= -1f;

            return distance + modifyOrigin + modifyTarget;
        }
    }

    public List<TileNode> FindPath(TileNode startTile, TileNode endTile = null)
    {
        if (endTile == null)
            endTile = endPoint;

        var key = (startTile, endTile);

        if (pathCache.TryGetValue(key, out var cachedPath) && cachedPath.version == nodeVersion)
            return cachedPath.path;

        List<TileNode> path = PathFinder.FindPath(startTile, endTile);
        pathCache[key] = (path, nodeVersion);

        return path;
    }

    public void RemoveSightNode(TileNode node)
    {
        HashSet<TileNode> inDirect = GetDistanceNodeFromNode(node, directSight + inDirectSight, true);

        foreach (var item in inDirect)
        {
            item.revealNodes.Remove(node);
            if(item.revealNodes.Count == 0)
            {
                item.RestoreFog();
                _directSightNodes.Remove(item);
                directSightNodes.Remove(item);
            }
        }
    }

    public void AddSightNode(TileNode node)
    {
        HashSet<TileNode> inDirect = GetDistanceNodeFromNode(node, directSight + inDirectSight, true);
        foreach(var item in inDirect)
        {
            if (_inDirectSightNodes.Contains(item))
                continue;

            _inDirectSightNodes.Add(item);
            inDirectSightNodes.Add(item);
        }

        HashSet<TileNode> direct = GetDistanceNodeFromNode(node, directSight, true);
        foreach (var item in direct)
        {
            item.revealNodes.Add(node);
            if (_directSightNodes.Contains(item))
                continue;

            _directSightNodes.Add(item);
            directSightNodes.Add(item);
        }
    }

    //public void UpdateSightNode()
    //{
    //    _inDirectSightNodes = GetDistanceNodes(directSight + inDirectSight, true);
    //    _directSightNodes = GetDistanceNodes(directSight, true);
    //}

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
        activeNodes.Clear();
        emptyNodes.Clear();
        allNodes.Clear();
        roomTiles.Clear();
        dormantTile.Clear();
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
        RoomManaPool.Instance.SetManaUI(value, roomTiles.ToList());
    }

    public void SetGuideSpawner(int requiredMana)
    {
        this.guideState = GuideState.Spawner;
        ResetAvail();
        SetSpawnerAvail(requiredMana);
    }

    public void SetGuideState(GuideState guideState, ITileKind tile = null)
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
            case GuideState.Selected:
                ShowSelectedTile(tile);
                break;
            case GuideState.Tile:
                if (tile == null)
                    return;
                if(tile is Tile realTile)
                    SetTileAvail(realTile);
                break;
            case GuideState.ObjectForPath:
                SetObjectForPathAvail();
                break;
            case GuideState.Spawner:
                SetSpawnerAvail(0);
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
            if (node.curTile != null && node.curTile.Movable && node != endPoint)
                node.curTile.Movable = false;
        }

        if (guideState == GuideState.None)
            SetGuideState(GuideState.None);
    }

    private void ShowSelectedTile(ITileKind targetTile)
    {
        if (targetTile == null) return;
        targetTile.curNode.SetGuideColor(Color.yellow);
    }

    private void ShowMovableTiles()
    {
        //가이드 노란색으로 설정
        foreach (TileNode node in activeNodes)
        {
            if (node.curTile != null && node.curTile.Movable)
            {
                node.SetGuideColor(Color.yellow);
            }
        }
    }

    private bool IsSpawnerSetable(TileNode node)
    {
        if (node.curTile != null && node.curTile.objectKind == null)
        {
            if (node.curTile._TileType == TileType.Room || node.curTile._TileType == TileType.Room_Single)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMonsterSetable(TileNode node)
    {
        return node.curTile != null && node != startPoint && node != endPoint;
    }

    private void SetSpawnerAvail(int requiredMana)
    {
        foreach (CompleteRoom room in roomTiles)
        {
            if (room.IsDormant)
                continue;

            bool haveMana = room._RemainingMana >= requiredMana;
            foreach (Tile tile in room._IncludeRooms)
            {
                if (tile.spawnLock)
                    continue;

                TileNode node = tile.curNode;
                node.SetAvail(false);
                if (IsSpawnerSetable(node) && haveMana)
                {
                    List<TileNode> path = FindPath(node);
                    node.SetAvail(path != null);
                }
            }
        }
    }

    private void SetMonsterAvail()
    {
        foreach (TileNode node in activeNodes)
        {
            if (IsMonsterSetable(node))
            {
                List<TileNode> path = FindPath(node);
                node.SetAvail(path != null);
            }
            else
                node.SetAvail(false);
        }
    }

    private void SetObjectForPathAvail()
    {
        foreach (TileNode node in activeNodes)
        {
            if (node.curTile != null && node.curTile._TileType == TileType.Path && node.curTile.objectKind == null)
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

    public List<TileNode> GetEndTileMovableNodes()
    {
        List<TileNode> nodes = new List<TileNode>();
        SetVirtualNode(true);
        virtualNodes.Add(endPoint);
        foreach (TileNode node in virtualNodes)
        {
            if (node == null)
                continue;
            node.UpdateNodeConnectionState();

            int connectedPathCount = 0;
            int connectedRoomCount = 0;
            Direction connectedPathDir = Direction.None;
            foreach (var kvp in node.connectionState)
            {
                if (kvp.Value == 1)
                {
                    connectedPathCount++;
                    connectedPathDir = kvp.Key;
                }
                else if (kvp.Value == 2)
                    connectedRoomCount++;
            }

            bool isAvail = connectedPathCount == 1 && connectedRoomCount == 0 ? true : false;
            if (isAvail)
            {
                if(FindPath(startPoint, node.neighborNodeDic[connectedPathDir]) != null)
                    nodes.Add(node);
            }
        }
        return nodes;
    }

    private void SetEndTileGuide()
    {
        List<TileNode> movableNodes = GetEndTileMovableNodes();
        ResetAvail();
        foreach (TileNode node in virtualNodes)
            node?.SetAvail(false);

        foreach (TileNode node in movableNodes)
            node.SetAvail(true);
    }

    private void SetTileAvail(Tile targetTile)
    {
        bool isEndtile = targetTile._TileType == TileType.End;
        SetVirtualNode(isEndtile);
        if (isEndtile)
            SetEndTileGuide();
        else
        {
            targetTile.UpdateAvailableNode(virtualNodes, !isEndtile);
            ResetAvail();
            foreach (TileNode node in virtualNodes)
            {
                if (node == null)
                    continue;

                node.SetAvail(targetTile.availableNodes.Contains(node));
            }
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

    private void SearchDistanceNode(int dist, HashSet<TileNode> targetNodes, HashSet<TileNode> searchedNodes)
    {
        for (int i = 0; i < dist; i++)
        {
            foreach (TileNode node in targetNodes)
                searchedNodes.Add(node);
            HashSet<TileNode> prevNodes = new HashSet<TileNode>(targetNodes);

            targetNodes.Clear();
            foreach (TileNode node in prevNodes)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (!node.neighborNodeDic.ContainsKey(direction))
                        continue;

                    TileNode next = node.neighborNodeDic[direction];
                    if (searchedNodes.Contains(next))
                        continue;
                    targetNodes.Add(next);
                }
            }
        }

        foreach (TileNode node in targetNodes)
            searchedNodes.Add(node);
    }

    public HashSet<TileNode> GetDistanceNodeFromNode(TileNode originNode, int dist, bool containAll = false)
    {
        HashSet<TileNode> targetTiles = new HashSet<TileNode>() { originNode };
        if (dist == 0 || originNode == null)
            return targetTiles;

        HashSet<TileNode> searchedNode = new HashSet<TileNode>();
        SearchDistanceNode(dist, targetTiles, searchedNode);

        if (containAll)
            return searchedNode;
        else
            return targetTiles;
    }

    public HashSet<TileNode> GetDistanceNodes(int dist, bool containAll = false)
    {
        HashSet<TileNode> targetTiles = new HashSet<TileNode>(activeNodes);
        if (dist == 0 || targetTiles.Count == 0)
            return targetTiles;

        HashSet<TileNode> searchedNode = new HashSet<TileNode>();
        SearchDistanceNode(dist, targetTiles, searchedNode);

        if (containAll)
            return searchedNode;
        else
            return targetTiles;
    }

    private void TileDictionary_Init()
    {
        tileDictionary = new Dictionary<TileType, List<Tile>>();
        var item = System.Enum.GetValues(typeof(TileType));
        foreach (TileType type in item)
            tileDictionary.Add(type, new List<Tile>());
    }

    private List<System.Func<ITileKind, bool>> setTileEvents = new List<System.Func<ITileKind, bool>>();
    public void AddSetTileEvent(System.Func<ITileKind, bool> newEvent) => setTileEvents.Add(newEvent);
    public void RemoveSetTileEvent(System.Func<ITileKind, bool> newEvent) => setTileEvents.Remove(newEvent);

    public void SetTile(Environment environment)
    {
        environments.Add(environment);
        foreach (var events in setTileEvents)
            events?.Invoke(environment);
    }

    public void SetTile(Tile tile)
    {
        if (tileDictionary == null)
            TileDictionary_Init();

        tileDictionary[tile._TileType].Add(tile);
        
        foreach (var events in setTileEvents)
            events?.Invoke(tile);

        nodeVersion++;
    }
    
    private List<System.Func<ITileKind, bool>> removeTileEvents = new List<System.Func<ITileKind, bool>>();
    public void AddRemoveTileEvent(System.Func<ITileKind, bool> newEvent) => removeTileEvents.Add(newEvent);
    public void RemoveRemoveTileEvent(System.Func<ITileKind, bool> newEvent) => removeTileEvents.Remove(newEvent);

    public void RemoveTile(Tile tile)
    {
        if (tileDictionary == null)
            TileDictionary_Init();

        tileDictionary[tile._TileType].Remove(tile);

        foreach (var events in removeTileEvents)
            events?.Invoke(tile);
        RemoveSightNode(tile.curNode);
        if (tile._TileType == TileType.Room_Single)
        {
            var item = FindRoom(tile.curNode.row, tile.curNode.col);
            roomTiles.Remove(item);
        }

        nodeVersion++;
    }

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

    public List<Tile> BFSRoom(Tile startTile, out bool isCompleted)
    {
        List<Tile> visited = new List<Tile> { startTile };
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);
        isCompleted = true;
        while(queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            foreach(Direction dir in next.RoomDirection)
            {
                TileNode targetNode = next.curNode.DirectionalNode(dir);
                // 방 방향으로 타일이 모두 닫혀있지 않음
                if (targetNode == null || targetNode.curTile == null)
                {
                    isCompleted = false;
                    continue;
                }

                if (visited.Contains(targetNode.curTile))
                    continue;

                // 방 방향으로 방타일이 연결되어있지 않음
                if (!targetNode.curTile.RoomDirection.Contains(UtilHelper.ReverseDirection(dir)))
                {
                    isCompleted = false;
                    continue;
                }

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
            bool isCompleted = false;
            List<Tile> completeRoom = BFSRoom(tile, out isCompleted);
            if (!isCompleted || completeRoom.Count <= 1) return;
            newRoom = new CompleteRoom(completeRoom);
            foreach(Tile target in completeRoom)
            {
                target.SetRoom(newRoom);
            }
        }
        
        // 해당 방들 완성으로 변경코드 추가 예정
        roomTiles.Add(newRoom);
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

    public CompleteRoom FindRoom(int row, int col)
    {
        TileNode point = FindNode(row, col);
        foreach(CompleteRoom room in roomTiles)
        {
            foreach(Tile tile in room._IncludeRooms)
            {
                if (tile.curNode == point)
                    return room;
            }
        }

        return null;
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
        virtualNodes = new HashSet<TileNode>();

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
        emptyNodes = new HashSet<TileNode>();
        TileNode[] nodes = FindObjectsOfType<TileNode>();
        foreach (TileNode node in nodes)
            emptyNodes.Add(node);
    }

    public void UpdateMinMaxRowCol(int row, int col)
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
        //UpdateMinMaxRowCol(index[0], index[1]);
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
