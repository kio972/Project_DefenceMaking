using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : IngameSingleton<InputManager>
{

    public TileNode testTile;

    public bool settingCard = false;

    public bool movingTile = false;

    private GameObject endPointPrefab = null;
    public TileNode curMoveObject = null;

    TileControlUI tileControlUI = null;

    private Tile curTile = null;
    public Tile _CurTile { get => curTile; }

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

    public void ResetTileClick()
    {
        if(tileControlUI == null)
            tileControlUI = FindObjectOfType<TileControlUI>();
        tileControlUI?.CloseAll();
        if (curTile != null && curTile.waitToMove)
            curTile.EndMoveing();
        curTile = null;
        NodeManager.Instance.SetGuideState(GuideState.None);
    }

    public void ClickTile(Tile curTile)
    {
        this.curTile = curTile;
        NodeManager.Instance.SetGuideState(GuideState.Movable, curTile);
        TileControlUI tileControlUI = FindObjectOfType<TileControlUI>(true);
        tileControlUI?.SetButton(curTile);
    }

    public void TileClickCheck()
    {
        if (settingCard)
            return;

        if (Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (NodeManager.Instance._GuideState != GuideState.None && NodeManager.Instance._GuideState != GuideState.Movable)
                return;

            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.curTile != null)
            {
                ClickTile(node.curTile);
            }
            else
                ResetTileClick();
        }
        else if (Input.GetKeyUp(SettingManager.Instance.key_CancelControl._CurKey))
            ResetTileClick();
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
        if (GameManager.Instance.speedLock)
            return;

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
        if(!GameManager.Instance.tileLock)
            TileClickCheck();
    }
}
