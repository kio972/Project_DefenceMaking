using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    //비활성화 상태의 노드
    public List<TileNode> virtualNodes;
    //활성화 상태의 노드
    public List<TileNode> activeNodes;

    //출발지로부터 6개 가상노드(isActive = false) 생성
    //새로생성된 가상노드의 이웃노드 생성
    //맵타일을 가상노드에 배치
    //배치된 가상노드는 활성노드로 이동
    //현재 배치된 노드로부터 현재존재하지않는 노드의 가상노드 생성

    public Vector3 GetNewNodePosition(TileNode curNode, Direction direction, float distance)
    {
        float angle = 0f;
        switch (direction)
        {
            case Direction.LeftUp:
                angle = 120f;
                break;
            case Direction.RightUp:
                angle = 60f;
                break;
            case Direction.Left:
                angle = 180f;
                break;
            case Direction.LeftDown:
                angle = -120f;
                break;
            case Direction.RightDown:
                angle = -60f;
                break;
        }

        float angleInRadians = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        dir *= distance;
        return curNode.transform.position + dir;
    }

    private void SetNewNode(TileNode curNode)
    {

    }

    public bool IsNodeInstance()
    {

        return false;
    }
}
