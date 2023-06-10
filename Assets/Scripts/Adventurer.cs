using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    private List<TileNode> myPath = new List<TileNode>();
    private TileNode prevTile;
    private TileNode curTile;

    private TileNode lastCrossRoad;
    private List<TileNode> afterCrossPath;

    private TileNode FindNextNode(TileNode curNode)
    {
        //다음 노드 찾는 로직
        TileNode nextNode = null;

        //갈림길일경우 저장

        return nextNode;
    }

    private IEnumerator Move(Vector3 nextPos, System.Action callback = null)
    {

        //몬스터 조우 시 잠시 대기

        yield return null;
        callback?.Invoke();
    }

    private IEnumerator MoveLogic()
    {
        curTile = NodeManager.Instance.startPoint;
        bool reverseLogic = false;
        while(true)
        {
            TileNode nextNode = FindNextNode(curTile);

            if(nextNode != null) // 막다른길일 경우
            {
                //갈림길에 도달할때까지 되돌아감
                reverseLogic = true;
                afterCrossPath.Reverse();
                for(int i = 0; i < afterCrossPath.Count; i++)
                {
                    nextNode = afterCrossPath[i];
                    yield return StartCoroutine(Move(nextNode.transform.position, () => { }));
                }
            }
            else
            {

            }

            yield return StartCoroutine(Move(nextNode.transform.position, () => { }));



            yield return null;
        }
    }

    public void Init()
    {
        
    }
}
