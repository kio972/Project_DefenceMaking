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

    public TileNode startPoint;

    

    private void SetNewNode(TileNode curNode)
    {
        curNode.AddNode(Direction.Left);
        curNode.AddNode(Direction.LeftUp);
        curNode.AddNode(Direction.LeftDown);
        curNode.AddNode(Direction.Right);
        curNode.AddNode(Direction.RightUp);
        curNode.AddNode(Direction.RightDown);
    }

    public bool IsNodeInstance()
    {

        return false;
    }
}
