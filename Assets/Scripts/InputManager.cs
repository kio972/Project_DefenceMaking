using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public TileNode testTile;

    public bool settingCard = false;

    private bool needUpdate = true;

    private GameObject endPointPrefab = null;
    public TileNode curMoveObject = null;

    public void UpdateTile()
    {
        UtilHelper.SetCollider(true, NodeManager.Instance.activeNodes);
    }

    private void ReadySetTile(TileNode curTile)
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
                print(hitObject.name);
            }

        }

        
    }

    void Init()
    {
        if (endPointPrefab == null)
            endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");
    }

    // Update is called once per frame
    void Update()
    {
        if (settingCard)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            UpdateTile();
            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.movable)
            {
                UtilHelper.SetCollider(true, NodeManager.Instance.emptyNodes);
                UtilHelper.SetAvail(true, NodeManager.Instance.emptyNodes);
                UtilHelper.SetCollider(false, NodeManager.Instance.activeNodes);
                node.waitToMove = true;
                settingCard = true;
            }
        }
    }
}
