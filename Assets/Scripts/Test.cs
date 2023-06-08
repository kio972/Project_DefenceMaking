using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public TileNode node;
    public NodeManager nodeManager;
    public Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPosition);
            Debug.Log("Cell center position: " + cellCenter);
        }
    }
}
