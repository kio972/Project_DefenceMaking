using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public TileNode curNode;

    public int x_Size = 10;
    public int y_Size = 9;

    public int startPathSize = 4;

    private void SetBasicTile()
    {
        //스타트포인트부터 스타트포인트 - 직선길 - 직선길  직선길 - 직선길 - 마왕방
        //print(NodeManager.Instance.startPoint.transform.position);
        GameObject startPointPrefab = Resources.Load<GameObject>("Prefab/Tile/StartTile");
        GameObject pathPrefab = Resources.Load<GameObject>("Prefab/Tile/RoadTile0");
        GameObject endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");

        Vector3 startPointPos = NodeManager.Instance.startPoint.transform.position;
        TileNode startTile = NodeManager.Instance.InstanceTile(startPointPrefab, startPointPos);
        startTile.SwapTile(NodeManager.Instance.startPoint);
        TileNode nextTile = NodeManager.Instance.startPoint.neighborNodeDic[Direction.Right];

        NodeManager.Instance.emptyNodes.Remove(NodeManager.Instance.startPoint);
        Destroy(NodeManager.Instance.startPoint.gameObject);

        NodeManager.Instance.startPoint = startTile;
        
        NodeManager.Instance.activeNodes = new List<TileNode>();
        NodeManager.Instance.activeNodes.Add(startTile);
        NodeManager.Instance.emptyNodes.Remove(startTile);

        for (int i = 0; i < startPathSize; i++)
        {
            Vector3 tempPos = nextTile.transform.position;
            TileNode tempTile = NodeManager.Instance.InstanceTile(pathPrefab, tempPos);
            tempTile.SwapTile(nextTile);
            TileNode destroyTargetTile = nextTile;
            nextTile = nextTile.neighborNodeDic[Direction.Right];

            NodeManager.Instance.emptyNodes.Remove(destroyTargetTile);
            Destroy(destroyTargetTile.gameObject);

            NodeManager.Instance.activeNodes.Add(tempTile);
            NodeManager.Instance.emptyNodes.Remove(tempTile);
        }

        Vector3 endPos = nextTile.transform.position;
        TileNode endTile = NodeManager.Instance.InstanceTile(endPointPrefab, endPos);
        endTile.SwapTile(nextTile);

        NodeManager.Instance.emptyNodes.Remove(nextTile);
        Destroy(nextTile.gameObject);

        NodeManager.Instance.activeNodes.Add(endTile);
        NodeManager.Instance.emptyNodes.Remove(endTile);
        endTile.movable = true;
        NodeManager.Instance.endPoint = endTile;
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
        int errorStack = 0;
        TileNode startPoint = null;
        while (true)
        {
            if(errorStack > 10000)
            {
                print("Cannot_Find_Valid_StartPoint");
                break;
            }

            int randomIndex = Random.Range(0, NodeManager.Instance.emptyNodes.Count);
            startPoint = NodeManager.Instance.emptyNodes[randomIndex];
            if (!IsStartPointValid(startPoint))
            {
                errorStack++;
                continue;
            }
            else
                break;
        }

        NodeManager.Instance.startPoint = startPoint;
    }

    private void SetNewMap()
    {
        //순환회수 : x - 2회
        //최초노드 생성
        NodeManager.Instance.SetNewNode(curNode);

        for (int i = 0; i < y_Size - 2; i++)
        {
            int cycle = x_Size - 2;
            if (i % 2 != 0)
                cycle--;
            for (int j = 0; j < cycle; j++)
            {
                Direction direction = Direction.Right;
                if (cycle != x_Size - 2)
                    direction = Direction.Left;
                if (!curNode.neighborNodeDic.TryGetValue(direction, out curNode))
                    curNode = NodeManager.Instance.InstanceNewNode(curNode, direction);
                NodeManager.Instance.SetNewNode(curNode);
            }

            if (i == y_Size - 3)
                break;

            if (!curNode.neighborNodeDic.TryGetValue(Direction.LeftUp, out curNode))
                curNode = NodeManager.Instance.InstanceNewNode(curNode, Direction.LeftUp);
            NodeManager.Instance.SetNewNode(curNode);
        }
    }

    private void Init()
    {
        SetNewMap();
        NodeManager.Instance.SetEmptyNode();
        SetStartPoint();
        SetBasicTile();

        InputManager.Instance.UpdateTile();
    }

    void Start()
    {
        Init();
    }

}
