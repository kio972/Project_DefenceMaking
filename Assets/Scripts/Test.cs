using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TileNode node;
    public NodeManager nodeManager;
    public Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        print(nodeManager.GetNewNodePosition(node, direction, 1f));
        print((new Vector3(1, 0, 0) - nodeManager.GetNewNodePosition(node, direction, 1f)).magnitude);
    }
}
