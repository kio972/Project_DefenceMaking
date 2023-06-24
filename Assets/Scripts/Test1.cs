using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test1 : MonoBehaviour
{
    public TileNode node;
    public NodeManager nodeManager;
    public Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            NodeManager.Instance.SetNewNode(node);
        if (Input.GetKeyDown(KeyCode.A))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.Left]);
            node = node.neighborNodeDic[Direction.Left];
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.LeftUp]);
            node = node.neighborNodeDic[Direction.LeftUp];
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.RightUp]);
            node = node.neighborNodeDic[Direction.RightUp];
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.Right]);
            node = node.neighborNodeDic[Direction.Right];
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.RightDown]);
            node = node.neighborNodeDic[Direction.RightDown];
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            NodeManager.Instance.SetNewNode(node.neighborNodeDic[Direction.LeftDown]);
            node = node.neighborNodeDic[Direction.LeftDown];
        }
    }
}
