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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 충돌된 오브젝트 처리
                GameObject hitObject = hit.collider.gameObject;
                //Debug.Log("Hit object: " + hitObject.name);
                TileNode tile = hitObject.GetComponentInParent<TileNode>();
                if (tile != null)
                {
                    TileNode neighborTile = null;
                    if (tile.neighborNodeDic.TryGetValue(Direction.Left, out neighborTile))
                        print(neighborTile.transform.position);
                    if (tile.neighborNodeDic.TryGetValue(Direction.LeftUp, out neighborTile))
                        print(neighborTile.transform.position);
                    if (tile.neighborNodeDic.TryGetValue(Direction.LeftDown, out neighborTile))
                        print(neighborTile.transform.position);
                    if (tile.neighborNodeDic.TryGetValue(Direction.Right, out neighborTile))
                        print(neighborTile.transform.position);
                    if (tile.neighborNodeDic.TryGetValue(Direction.RightUp, out neighborTile))
                        print(neighborTile.transform.position);
                    if (tile.neighborNodeDic.TryGetValue(Direction.RightDown, out neighborTile))
                        print(neighborTile.transform.position);
                }
            }
        }
    }
}
