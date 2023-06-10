using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    private List<TileNode> crossedNodes = new List<TileNode>();
    private TileNode prevTile;
    private TileNode curTile;

    private TileNode lastCrossRoad;
    private List<TileNode> afterCrossPath = new List<TileNode>();

    private bool directPass = false;

    private Coroutine directPassCoroutine = null;
    private Coroutine moveCoroutine = null;
    public void EndPointMoved()
    {
        if (!directPass)
            return;

        if(directPassCoroutine != null)
            StopCoroutine(directPassCoroutine);
        directPassCoroutine = StartCoroutine(DirectPass());
    }

    private IEnumerator DirectPass()
    {
        List<TileNode> path = PathFinder.Instance.FindPath(curTile);
        Vector3 finalPos = NodeManager.Instance.endPoint.transform.position;
        foreach (TileNode node in path)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            yield return moveCoroutine = StartCoroutine(Move(node.transform.position,
            () => { NodeAction(node); }));
        }
    }

    //다음 노드 찾는 로직
    private TileNode FindNextNode(TileNode curNode)
    {
        //startNode에서 roomDirection이나 pathDirection이 있는 방향의 이웃노드를 받아옴
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(prevTile);
        if(crossedNodes != null)
        {
            foreach (TileNode node in crossedNodes)
                nextNodes.Remove(node);
        }

        if(nextNodes.Count == 0)
            return null;

        //갈림길일경우 저장
        if(nextNodes.Count > 1)
        {
            afterCrossPath = new List<TileNode>();
            lastCrossRoad = curNode;
            afterCrossPath.Add(lastCrossRoad);
        }
        
        return nextNodes[UnityEngine.Random.Range(0, nextNodes.Count)];
    }

    private IEnumerator Move(Vector3 nextPos, System.Action callback = null)
    {
        float moveSpeed = 1f;
        float distance = Vector3.Distance(transform.position, nextPos);
        //다음 노드로 이동
        while (distance > 0.001f)
        {
            // 다음 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);

            // 현재 위치와 목표 위치 간의 거리 갱신
            distance = Vector3.Distance(transform.position, nextPos);

            //몬스터 조우 시 전투동안 대기하는 함수 추가예정

            yield return null;
        }

        callback?.Invoke();
    }

    private void NodeAction(TileNode nextNode)
    {
        prevTile = curTile;
        curTile = nextNode;
        if (!crossedNodes.Contains(curTile))
            crossedNodes.Add(curTile);

        if(!afterCrossPath.Contains(curTile))
            afterCrossPath.Add(curTile);
    }

    private IEnumerator MoveLogic()
    {
        curTile = NodeManager.Instance.startPoint;
        transform.position = curTile.transform.position;
        while(true)
        {
            if(curTile == null) break;
            if (curTile == NodeManager.Instance.endPoint) break;

            TileNode nextNode = FindNextNode(curTile);

            if(nextNode == null) // 막다른길일 경우
            {
                //갈림길에 도달할때까지 되돌아감
                if(lastCrossRoad != null)
                {
                    afterCrossPath.Reverse();
                    for (int i = 0; i < afterCrossPath.Count; i++)
                    {
                        nextNode = afterCrossPath[i];
                        if (moveCoroutine != null)
                            StopCoroutine(moveCoroutine);
                        yield return moveCoroutine = StartCoroutine(Move(nextNode.transform.position,
                            () => { NodeAction(nextNode); }));
                    }

                    lastCrossRoad = null;
                    afterCrossPath = new List<TileNode>();
                    continue;
                }
                else
                {
                    //도착지까지 길찾아서 최단루트로 바로이동
                    directPass = true;
                    directPassCoroutine = StartCoroutine(DirectPass());
                    yield break;
                }
            }

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            yield return moveCoroutine = StartCoroutine(Move(nextNode.transform.position,
                () => { NodeAction(nextNode); }));
        }
    }

    public void Init()
    {
        StartCoroutine(MoveLogic());
    }

    private void Start()
    {
        Init();
    }
}
