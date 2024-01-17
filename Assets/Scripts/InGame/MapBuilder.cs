using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




public class MapBuilder : MonoBehaviour
{
    public TileNode curNode;

    public int x_Size = 10;
    public int y_Size = 9;

    private int startPathSize = 3;

    [SerializeField]
    private int emptyNodeSize = 4;

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

        int dist = PathFinder.Instance.GetNodeDistance(tileNode, targetNode);
        if(dist == -1 || dist > range)
        {
            SetRamdomTileToRandomNode(tileNode, targetTilePrefab, range);
            return;
        }

        Tile newTile = Instantiate(targetTilePrefab);
        newTile.transform.SetParent(tileNode.transform, false);
        newTile.Init(targetNode, true);
    }

    private void SetBasicTile()
    {
        //스타트포인트부터 스타트포인트 - 직선길 - 직선길  직선길 - 직선길 - 마왕방
        //print(NodeManager.Instance.startPoint.transform.position);
        GameObject startPointPrefab = Resources.Load<GameObject>("Prefab/Tile/StartTile");
        GameObject pathPrefab = Resources.Load<GameObject>("Prefab/Tile/RoadTile0");
        GameObject pathPrefab2 = Resources.Load<GameObject>("Prefab/Tile/RoadTile6");
        GameObject endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");

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
        endTile.movable = true;
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

    public void SetRandomTile(int range)
    {
        int index = DataManager.Instance.PathCard_Indexs[Random.Range(0, DataManager.Instance.PathCard_Indexs.Count)];
        int isRoom = Random.Range(0, 2);
        if(isRoom == 1)
            index = DataManager.Instance.RoomCard_Indexs[Random.Range(0, DataManager.Instance.RoomCard_Indexs.Count)];

        string prefabName = DataManager.Instance.Deck_Table[index]["prefab"].ToString();

        Tile targetTilePrefab = Resources.Load<Tile>("Prefab/Tile/" + prefabName);
        if(targetTilePrefab == null)
        {
            SetRandomTile(range);
            return;
        }

        SetRamdomTileToRandomNode(NodeManager.Instance.endPoint, targetTilePrefab, range);
    }

    public void SetRamdomTile(int count, int range)
    {
        for (int i = 0; i < count; i++)
            SetRandomTile(range);
    }

    public void Init()
    {
        SetNewMap();
        NodeManager.Instance.SetEmptyNode();
        NodeManager.Instance.startPoint = NodeManager.Instance.FindNode(0, 0);
        SetBasicTile();

        Tutorial tutorial = FindObjectOfType<Tutorial>();
        if (tutorial == null)
            SetRamdomTile(4, 5);

        InputManager.Instance.Call();
    }
}
