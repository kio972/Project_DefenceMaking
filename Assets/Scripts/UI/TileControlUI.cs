using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControlUI : MonoBehaviour
{
    [SerializeField]
    private GameObject tileMoveBtn;
    [SerializeField]
    private GameObject tileRemoveBtn;
    [SerializeField]
    private GameObject exitBtn;
    [SerializeField]
    private GameObject spawnerRemoveBtn;

    [SerializeField]
    private LanguageText toolTip_header;
    [SerializeField]
    private LanguageText toolTip_desc;

    private InGameUI inGameUI;

    private Tile curToolTipTile;

    TooltipInput _tooltipInput;
    TooltipInput tooltipInput
    {
        get
        {
            if (_tooltipInput != null)
                return _tooltipInput;
            foreach (var item in InputManager.Instance.inputs)
            {
                if (item is TooltipInput tooltipInput)
                    _tooltipInput = tooltipInput;
            }

            return _tooltipInput;
        }
    }

    public void CloseAllBtn()
    {
        //NodeManager.Instance.SetGuideState(GuideState.None);
        CloseAll();
    }

    public void CloseAll()
    {
        tooltipInput.ForceReset();
        tileMoveBtn.SetActive(false);
        tileRemoveBtn.SetActive(false);
        exitBtn.SetActive(false);
        inGameUI?.SwitchRightToTileUI(false);
    }

    public void MoveTile()
    {
        MoveTile(curToolTipTile);
    }

    public void MoveTile(Tile curTile)
    {
        InputManager.Instance.settingCard = true;
        if (curTile == null)
            return;
        if (curTile.MovableNow)
        {
            CloseAllBtn();
            curTile.ReadyForMove().Forget();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("타일 위에 캐릭터가 있어 움직일 수 없습니다!");
    }

    public void RemoveSpawner()
    {
        Tile curTile = curToolTipTile;
        if (curTile == null)
            return;

        curTile.RemoveSpawner();
        CloseAllBtn();
        //InputManager.Instance.ClickTile(curTile);
    }

    public void RemoveTile()
    {
        Tile curTile = curToolTipTile;
        if (curTile == null)
            return;
        if(curTile.IsRemovableNow)
        {
            curTile.RemoveTile();
            InputManager.Instance.ResetTileClick();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("타일 위에 캐릭터가 있어 제거 할 수 없습니다!");
    }

    public void SetButton(TooltipObject tooltipObject)
    {
        tileMoveBtn.SetActive(false);
        tileRemoveBtn.SetActive(false);
        exitBtn.SetActive(true);

        toolTip_header?.ChangeLangauge(SettingManager.Instance.language, tooltipObject.toolTipKey_header);
        toolTip_desc?.ChangeLangauge(SettingManager.Instance.language, tooltipObject.toolTipKey_descs);

        if (tooltipObject.toolTipType == ToolTipType.Tile)
            SetButton(tooltipObject.GetComponentInParent<Tile>());
        else if(tooltipObject.toolTipType == ToolTipType.Devil)
            SetButton(NodeManager.Instance.endPoint.curTile);
        else
        {
            if (inGameUI == null)
                inGameUI = GetComponentInParent<InGameUI>();
            inGameUI?.SwitchRightToTileUI(true);
        }
    }

    public void SetButton(Tile targetTile)
    {
        if (targetTile == null)
            return;

        tileMoveBtn.SetActive(targetTile.Movable);
        spawnerRemoveBtn.SetActive(targetTile.HaveSpawner);
        tileRemoveBtn.SetActive(!targetTile.HaveSpawner && targetTile.IsRemovable);
        exitBtn.SetActive(true);
        if (inGameUI == null)
            inGameUI = GetComponentInParent<InGameUI>();
        inGameUI?.SwitchRightToTileUI(true);
        curToolTipTile = targetTile;
    }

    //public void Update()
    //{
    //    if (!tileMoveBtn.gameObject.activeSelf && !tileRemoveBtn.gameObject.activeSelf)
    //        return;

    //}
}
