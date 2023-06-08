using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LeftUp,
    RightUp,
    Left,
    Right,
    LeftDown,
    RightDown,
}

public class TileNode : MonoBehaviour
{
    public List<Direction> pathDirection = new List<Direction>();
    public List<Direction> roomDirection = new List<Direction>();

    public List<TileNode> neighborNodes = new List<TileNode>();
    
    public bool isActive = false;

    public void Init()
    {

    }
}
