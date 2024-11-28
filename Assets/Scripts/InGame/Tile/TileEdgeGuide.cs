using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileEdgeDirection
{
    LeftUp,
    LeftDown,
    Down,
    RightDown,
    RightUp,
    Up,
    None,
}

public class TileEdgeGuide : MonoBehaviour
{
    [SerializeField]
    private Transform leftUp;
    [SerializeField]
    private Transform leftDown;
    [SerializeField]
    private Transform down;
    [SerializeField]
    private Transform rightDown;
    [SerializeField]
    private Transform rightUp;
    [SerializeField]
    private Transform up;

    private Dictionary<TileEdgeDirection, Transform> _tileDirectionPos;
    public Dictionary<TileEdgeDirection, Transform> tileDirectionPos { get => _tileDirectionPos; }

    private void Awake()
    {
        _tileDirectionPos = new Dictionary<TileEdgeDirection, Transform>()
                {
                    { TileEdgeDirection.LeftUp, leftUp },
                    { TileEdgeDirection.LeftDown, leftDown },
                    { TileEdgeDirection.Down, down },
                    { TileEdgeDirection.RightDown, rightDown },
                    { TileEdgeDirection.RightUp, rightUp },
                    { TileEdgeDirection.Up, up },
                };
    }
}
