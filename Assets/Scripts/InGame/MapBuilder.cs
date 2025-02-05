using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;



public class MapBuilder : MonoBehaviour
{
    public TileNode curNode;

    public int x_Size = 10;
    public int y_Size = 9;

    private int startPathSize = 3;

    [SerializeField]
    protected int emptyNodeSize = 4;

    private int _curTileSetCount = 0;
    private int forHiddenTileCount = 10;
    
    public int curTileSetCount { get => _curTileSetCount; set => _curTileSetCount = value; }

    private bool SetHiddenTileContinuous(ITileKind tileKind)
    {
        if (!GameManager.Instance.IsInit)
            return false;

        if (tileKind is TileHidden || tileKind.curNode == NodeManager.Instance.endPoint)
            return false;
        if (tileKind is Tile tile && tile.IsDormant)
            return false;

        _curTileSetCount++;
        if (_curTileSetCount < forHiddenTileCount)
            return false;

        _curTileSetCount = 0;
        SetHiddenTile(3).Forget();
        return true;
    }

    private bool CheckNearHiddenTile(TileNode targetNode, HashSet<TileNode> hiddenTileList, int dist)
    {
        foreach(TileNode node in hiddenTileList)
        {
            if (UtilHelper.GetTileDistance(targetNode.row, targetNode.col, node.row, node.col) <= dist)
                return true;
        }

        return false;
    }

    private async UniTaskVoid SetHiddenTile(int dist)
    {
        await UniTask.Yield();
        List<TileNode> nodes = NodeManager.Instance.GetDistanceNodes(dist).ToList();
        TileNode targetNode = null;
        while (nodes.Count > 0)
        {
            targetNode = nodes[Random.Range(0, nodes.Count)];
            bool isNearHiddenTile = CheckNearHiddenTile(targetNode, NodeManager.Instance.hiddenTiles, 3);
            if (NodeManager.Instance.hiddenTiles.Contains(targetNode) || isNearHiddenTile)
                nodes.Remove(targetNode);
            else
                break;
        }

        if (targetNode == null)
            return;

        TileHidden hidden = Resources.Load<TileHidden>("Prefab/Tile/HiddenTile");
        hidden = Instantiate(hidden);
        hidden.Init(targetNode);
    }

    public async UniTaskVoid SetHiddenTile(TileData tile)
    {
        await UniTask.Yield();
        TileNode targetNode = NodeManager.Instance.FindNode(tile.row, tile.col);
        if (targetNode == null)
            return;

        TileHidden hidden = Resources.Load<TileHidden>("Prefab/Tile/HiddenTile");
        hidden = Instantiate(hidden);
        hidden.Init(targetNode);
    }

    private void SetRamdomTileToRandomNode(TileNode tileNode, Tile targetTilePrefab, int range)
    {
        //타겟 노드로부터 range거리내의 유효한 타일 중 랜덤한 한 타일에 targetTile을 비활성 상태로 설치
        //차후 문제 생길 시 랜덤리스트 작성 후 하나씩 빼도록 변경해야함
        int randomX = Random.Range(-range, range + 1);
        int randomY = Random.Range(-range, range + 1);
        TileNode targetNode = NodeManager.Instance.FindNode(tileNode.row + randomX, tileNode.col + randomY);
        if(targetNode == null || targetNode.curTile != null)
        {
            SetRamdomTileToRandomNode(tileNode, targetTilePrefab, range);
            return;
        }

        bool isTileAlone = true;
        foreach(TileNode checkNode in targetNode.neighborNodeDic.Values)
        {
            if(checkNode != null && checkNode.curTile != null)
            {
                isTileAlone = false;
                break;
            }
        }

        if(!isTileAlone)
        {
            SetRamdomTileToRandomNode(tileNode, targetTilePrefab, range);
            return;
        }

        int dist = PathFinder.GetNodeDistance(tileNode, targetNode);
        if(dist == -1 || dist > range)
        {
            SetRamdomTileToRandomNode(tileNode, targetTilePrefab, range);
            return;
        }

        Tile newTile = Instantiate(targetTilePrefab);
        newTile.transform.SetParent(tileNode.transform, false);
        newTile.Init(targetNode, true);
    }

    protected virtual void SetBasicTile()
    {
        //스타트포인트부터 스타트포인트 - 직선길 - 직선길  직선길 - 직선길 - 마왕방
        //print(NodeManager.Instance.startPoint.transform.position);
        GameObject startPointPrefab = Resources.Load<GameObject>("Prefab/Tile/StartTile");
        GameObject pathPrefab = Resources.Load<GameObject>("Prefab/Tile/RoadTile0");
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
        

        for (int i = 0; i < startPathSize; i++)
        {
            Tile pathTile = Instantiate(pathPrefab)?.GetComponent<Tile>();
            pathTile.Init(nextNode, false, false, false);

            nextNode = nextNode.neighborNodeDic[Direction.Right];
            NodeManager.Instance.ExpandEmptyNode(nextNode, emptyNodeSize);
        }

        Tile crossTile = Instantiate(pathPrefab2)?.GetComponent<Tile>();
        crossTile.Init(nextNode, false, false, false);
        crossTile.RotateTile();

        nextNode = nextNode.neighborNodeDic[Direction.RightUp];
        NodeManager.Instance.ExpandEmptyNode(nextNode, emptyNodeSize);

        Tile endTile = Instantiate(endPointPrefab)?.GetComponent<Tile>();
        endTile.Init(nextNode, false, false, false);
        endTile.RotateTile(true);
        NodeManager.Instance.endPoint = endTile.curNode;
        endTile.Movable = true;

        nextNode = nextNode.neighborNodeDic[Direction.LeftDown];
        nextNode = nextNode.neighborNodeDic[Direction.RightDown];

        Tile roomTile = Instantiate(roomPrefab)?.GetComponent<Tile>();
        roomTile.Init(nextNode, false, false, false);
        roomTile.RotateTile(true);

        Tutorial tutorial = FindObjectOfType<Tutorial>();
        if (tutorial == null)
        {
            SetHiddenTile(3).Forget();
            SetHiddenTile(3).Forget();
            SetHiddenTile(3).Forget();
        }

        StartHiddenTileCounter();
    }

    public virtual void StartHiddenTileCounter()
    {
        NodeManager.Instance.AddSetTileEvent(SetHiddenTileContinuous);
    }

    private bool IsStartPointValid(TileNode node)
    {
        TileNode traceNode = node;
        for(int i = 0; i < startPathSize + 1; i++)
        {
            if (traceNode.neighborNodeDic.ContainsKey(Direction.Right))
                traceNode = traceNode.neighborNodeDic[Direction.Right];
            else
                return false;
        }

        return true;
    }

    private void SetStartPoint()
    {
        NodeManager.Instance.startPoint = NodeManager.Instance.FindNode(0, 0);
    }

    private void SetNewMap()
    {
        NodeManager.Instance.allNodes.Add(curNode);
        for (int i = 0; i < emptyNodeSize; i++)
        {
            NodeManager.Instance.BuildNewNodes();
        }
    }

    private GameObject GetRandomPrefab()
    {
        int index = DataManager.Instance.pathCard_Indexs[Random.Range(0, DataManager.Instance.pathCard_Indexs.Count)];
        int isRoom = Random.Range(0, 2);
        if (isRoom == 1)
            index = DataManager.Instance.roomCard_Indexs[Random.Range(0, DataManager.Instance.roomCard_Indexs.Count)];

        string prefabName = DataManager.Instance.deck_Table[index]["prefab"].ToString();

        GameObject targetTilePrefab = Resources.Load<GameObject>("Prefab/Tile/" + prefabName);
        if (targetTilePrefab != null)
            return targetTilePrefab;
        else
            return GetRandomPrefab();
    }

    public void SetRandomTile(int range)
    {
        int index = DataManager.Instance.pathCard_Indexs[Random.Range(0, DataManager.Instance.pathCard_Indexs.Count)];
        int isRoom = Random.Range(0, 2);
        if(isRoom == 1)
            index = DataManager.Instance.roomCard_Indexs[Random.Range(0, DataManager.Instance.roomCard_Indexs.Count)];

        string prefabName = DataManager.Instance.deck_Table[index]["prefab"].ToString();

        Tile targetTilePrefab = Resources.Load<Tile>("Prefab/Tile/" + prefabName);
        if(targetTilePrefab == null)
        {
            SetRandomTile(range);
            return;
        }

        //SetRamdomTileToRandomNode(NodeManager.Instance.endPoint, targetTilePrefab, range);
    }

    public void SetRamdomTile(int count, int range)
    {
        for (int i = 0; i < count; i++)
            SetRandomTile(range);
    }

    public void SetTile(TileData tile, bool isEnvironment = false)
    {
        if (!initState)
            Init(false);

        TileNode node = NodeManager.Instance.FindNode(tile.row, tile.col);
        if(node == null)
        {
            Debug.Log($"No EmptyTile/ Row : { tile.row } Col : { tile.col }");
            return;
        }

        if(isEnvironment)
        {
            Environment prefab = Resources.Load<Environment>("Prefab/Environment/" + tile.id);
            Environment instance = Instantiate(prefab);
            instance.Init(node);
        }
        else
        {
            Tile prefab = Resources.Load<Tile>("Prefab/Tile/" + tile.id);
            Tile instance = Instantiate(prefab);
            for (int i = 0; i < tile.rotation; i++)
                instance.RotateTile();
            instance.Init(node, tile.isDormant, tile.isRemovable, false);
            if (!string.IsNullOrEmpty(tile.trapId))
                BattlerPooling.Instance.SpawnTrap(tile.trapId, node, "id", tile.trapDuration);
            if(tile.isUpgraded)
            {
                TileUpgrader upgrader = instance.GetComponent<TileUpgrader>();
                upgrader?.UpgradeTile();
            }
        }
    }

    private bool initState = false;

    public void Init(bool setmap = true)
    {
        initState = true;

        SetNewMap();
        NodeManager.Instance.SetEmptyNode();
        NodeManager.Instance.startPoint = NodeManager.Instance.FindNode(0, 0);

        if(setmap)
            SetBasicTile();

        InputManager input = InputManager.Instance;
    }
}
