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

    public void Call()
    {
        //InputManager Instance 생성 함수
    }

    private void InputCheck()
    {

    }

    private Tile curTile = null;

    private void TileMoveCheck()
    {
        if (settingCard)
            return;

        if (curTile != null)
        {
            TileNode node = curTile.TileMoveCheck();
            if (Input.GetKeyDown(SettingManager.Instance.key_RotateTile._CurKey))
                curTile.twin.RotateTile();
            if (!curTile.Movable || Input.GetKeyUp(SettingManager.Instance.key_CancelControl._CurKey))
            {
                curTile.EndMoveing();
                curTile = null;
            }
            else if (Input.GetKeyUp(SettingManager.Instance.key_BasicControl._CurKey))
            {
                curTile.EndMove(node);
                curTile = null;
            }
        }
        else if (Input.GetKey(SettingManager.Instance.key_BasicControl._CurKey))
        {
            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.curTile != null && node.curTile.Movable)
            {
                curTile = node.curTile;
                print(curTile);
                node.curTile.ReadyForMove();
            }
        }
    }

    private void CameraResetCheck()
    {
        if (Input.GetKeyDown(SettingManager.Instance.key_Camera_Reset._CurKey))
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
                cameraController.ResetCamPos(Input.GetKey(SettingManager.Instance.key_Camera_ResetAssist._CurKey));
        }
    }

    private void SpeedControlCheck()
    {
        if(Input.GetKeyDown(SettingManager.Instance.key_SpeedControl_Zero._CurKey))
        {
            if (GameManager.Instance.timeScale == 0)
                GameManager.Instance.speedController.SetSpeedPrev(true, false);
            else
                GameManager.Instance.speedController.SetSpeedZero();
        }
        else if(Input.GetKeyDown(SettingManager.Instance.key_SpeedControl_One._CurKey))
        {
            GameManager.Instance.speedController.SetSpeedNormal();
        }
        else if(Input.GetKeyDown(SettingManager.Instance.key_SpeedControl_Double._CurKey))
        {
            GameManager.Instance.speedController.SetSpeedFast();
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
        if (GameManager.Instance.isPause)
            return;

        SpeedControlCheck();
        CameraResetCheck();
        TileMoveCheck();
    }
}
