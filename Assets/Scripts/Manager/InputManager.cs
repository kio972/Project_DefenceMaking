using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : IngameSingleton<InputManager>
{

    public TileNode testTile;

    public ReactiveProperty<bool> _settingCard { get; private set; } = new ReactiveProperty<bool>(false);
    public bool settingCard { get => _settingCard.Value; set => _settingCard.Value = value; }

    public TileNode curMoveObject = null;

    TileControlUI tileControlUI = null;

    private Tile curTile = null;
    public Tile _CurTile { get => curTile; }


    List<IInput> _inputs = new List<IInput>();
    public List<IInput> inputs { get => _inputs; }


    private void Awake()
    {
        _inputs.Add(new TooltipInput());
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
        if(this.curTile == curTile && curTile.curNode == NodeManager.Instance.endPoint)
        {
            TileControlUI tileControl = FindObjectOfType<TileControlUI>();
            tileControl?.MoveTile();
            return;
        }

        this.curTile = curTile;
        NodeManager.Instance.SetGuideState(GuideState.Selected, curTile);
        TileControlUI tileControlUI = FindObjectOfType<TileControlUI>(true);
        tileControlUI?.SetButton(curTile);
    }

    public void TileClickCheck()
    {
        if (_settingCard.Value)
            return;

        if (Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (NodeManager.Instance._GuideState != GuideState.None && NodeManager.Instance._GuideState != GuideState.Selected)
                return;

            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.curTile != null)
            {
                ClickTile(node.curTile);
                AudioManager.Instance.Play2DSound("Click_tile_01", SettingManager.Instance._FxVolume);
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

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPause)
            return;

        SpeedControlCheck();
        CameraResetCheck();
        //if(!GameManager.Instance.tileLock)
        //    TileClickCheck();

        foreach(IInput input in _inputs)
        {
            if(input.IsCheckValid)
                input.CheckInput();
        }
    }
}
