using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : IngameSingleton<InputManager>
{
    public TileNode testTile;

    public bool settingCard = false;


    private GameObject endPointPrefab = null;
    public TileNode curMoveObject = null;

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


    public void Call()
    {

    }

    private void InputCheck()
    {

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
                cameraController.ResetCamPos(Input.GetKey(KeyCode.LeftShift));
        }

        if (settingCard)
            return;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.curTile != null && node.curTile.Movable)
            {
                node.curTile.ReadyForMove();
            }
        }
    }
}
