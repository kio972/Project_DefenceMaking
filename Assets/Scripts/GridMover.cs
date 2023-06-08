using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    public Grid grid;
    public Transform targetObject;


    private void MoveTarget(Direction direction)
    {
        targetObject.position = UtilHelper.GetGridPosition(targetObject.position, direction, grid.cellSize.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            MoveTarget(Direction.LeftUp);
        if (Input.GetKeyDown(KeyCode.E))
            MoveTarget(Direction.RightUp);
        if (Input.GetKeyDown(KeyCode.A))
            MoveTarget(Direction.Left);
        if (Input.GetKeyDown(KeyCode.D))
            MoveTarget(Direction.Right);
        if (Input.GetKeyDown(KeyCode.Z))
            MoveTarget(Direction.LeftDown);
        if (Input.GetKeyDown(KeyCode.X))
            MoveTarget(Direction.RightDown);
    }
}
