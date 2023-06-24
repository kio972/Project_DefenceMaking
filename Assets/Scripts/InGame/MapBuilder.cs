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

    [SerializeField]
    private int emptyNodeSize = 4;

    private void SetBasicTile()
    {
        //스타트포인트부터 스타트포인트 - 직선길 - 직선길  직선길 - 직선길 - 마왕방
        //print(NodeManager.Instance.startPoint.transform.position);
        GameObject startPointPrefab = Resources.Load<GameObject>("Prefab/Tile/StartTile");
        GameObject pathPrefab = Resources.Load<GameObject>("Prefab/Tile/RoadTile0");
        GameObject endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");

        Tile startTile = Instantiate(startPointPrefab)?.GetComponent<Tile>();
        startTile.MoveTile(NodeManager.Instance.startPoint);

        NodeManager.Instance.startPoint = startTile.curNode;
        NodeManager.Instance.activeNodes = new List<TileNode>();

        NodeManager.Instance.activeNodes.Add(startTile.curNode);
        TileNode nextNode = startTile.curNode.neighborNodeDic[Direction.Right];


        for (int i = 0; i < startPathSize; i++)
        {
            Tile pathTile = Instantiate(pathPrefab)?.GetComponent<Tile>();
            pathTile.MoveTile(nextNode);

            NodeManager.Instance.activeNodes.Add(pathTile.curNode);
            nextNode = nextNode.neighborNodeDic[Direction.Right];
        }

        Tile endTile = Instantiate(endPointPrefab)?.GetComponent<Tile>();
        endTile.MoveTile(nextNode);

        NodeManager.Instance.activeNodes.Add(endTile.curNode);
        NodeManager.Instance.endPoint = endTile.curNode;
        endTile.movable = true;

        PlayerBattleMain king = endTile.GetComponentInChildren<PlayerBattleMain>();
        king.Init();
        GameManager.Instance.king = king;
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
        //NodeManager.Instance.allNodes.Add(curNode);
        //for (int i = 0; i < emptyNodeSize; i++)
        //{
        //    NodeManager.Instance.BuildNewNodes();
        //}


        //순환회수 : x - 2회
        //최초노드 생성
        NodeManager.Instance.SetNewNode(curNode);

        //1.최초노드 생성
        //2.최초노드에서 왼쪽타일 + 순환횟수의 타일 불러오기
        //3.해당노드에서 SetNewNode
        //4.해당노드에서 오른쪽위타일을 순환횟수만큼 이동하며 SetNewNode
        //5.끝나면 오른쪽, 오른쪽아래, 왼쪽아래, 왼쪽순으로 이동하며 SetNewNode
        //6.

        //1. 최초노드의 Left에서 SetNewNode
        //RightUp, Right, RightDown, LeftDown, Left, LeftUp
        Direction[] directions = new Direction[]
        { Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown, Direction.Left, Direction.LeftUp};
        
        TileNode node = curNode;
        for (int i = 1; i < emptyNodeSize; i++)
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.Left]);
            node = node.neighborNodeDic[Direction.Left];

            foreach (Direction direction in directions)
            {
                for (int j = 0; j < i; j++)
                {
                    NodeManager.Instance.SetNewNode(node.neighborNodeDic[direction]);
                    node = node.neighborNodeDic[direction];
                }
            }
        }
    }

    public void Init()
    {
        SetNewMap();
        NodeManager.Instance.SetEmptyNode();
        SetStartPoint();
        SetBasicTile();

        InputManager.Instance.Call();
        NodeManager.Instance.endPointPosition = NodeManager.Instance.endPoint.transform.position;
    }
}
