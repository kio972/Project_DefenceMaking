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

    private TooltipObject curToolTipObject;
    private Tile curToolTipTile;

    //[SerializeField]
    //AK.Wwise.Event refusedSound;
    //[SerializeField]
    //AK.Wwise.Event tileMoveSound;
    //[SerializeField]
    //AK.Wwise.Event tileRemoveSound;

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
        curToolTipObject = null;
    }

    public void MoveTile()
    {
        MoveTile(curToolTipTile);
    }

    public void MoveTile(Tile curTile)
    {
        if (curTile == null)
            return;
        if (curTile.IsCharacterOnIt)
        {
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_moveFailEnemy"));
            //refusedSound?.Post(gameObject);
        }
        else
        {
            CloseAllBtn();
            curTile.ReadyForMove().Forget();
            InputManager.Instance.settingCard = true;
            //tileMoveSound?.Post(gameObject);
        }
    }

    public void RemoveSpawner()
    {
        if (curToolTipObject == null)
            return;

        IDestructableObjectKind target = curToolTipObject.GetComponentInParent<IDestructableObjectKind>();
        if (target == null)
            return;

        target.DestroyObject();

        CloseAllBtn();
        //InputManager.Instance.ClickTile(curTile);
    }

    public void RemoveTile()
    {
        Tile curTile = curToolTipTile;
        if (curTile == null)
            return;

        if (GameManager.Instance.isBossOnMap)
        {
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_removeFailLockdown"));
            //refusedSound?.Post(gameObject);
        }
        else if (curTile.objectKind != null)
        { 
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_removeFailObject"));
            //refusedSound?.Post(gameObject);
        }
        else if (curTile.IsCharacterOnIt)
        {
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_moveFailEnemy"));
            //refusedSound?.Post(gameObject);
        }
        else
        {
            curTile.RemoveTile();
            //tileRemoveSound?.Post(gameObject);
        }

        InputManager.Instance.ResetTileClick();
    }

    public void SetButton(TooltipObject tooltipObject)
    {
        curToolTipObject = tooltipObject;

        tileMoveBtn.SetActive(false);
        tileRemoveBtn.SetActive(false);
        spawnerRemoveBtn.SetActive(curToolTipObject.GetComponentInParent<IDestructableObjectKind>() != null);
        exitBtn.SetActive(true);

        toolTip_header?.ChangeLangauge(SettingManager.Instance.language, tooltipObject.toolTipKey_header);
        toolTip_desc?.ChangeLangauge(SettingManager.Instance.language, tooltipObject.toolTipKey_descs);

        if (tooltipObject.toolTipType == ToolTipType.Tile)
            SetButton(tooltipObject.GetComponentInParent<ITileKind>());
        else if(tooltipObject.toolTipType == ToolTipType.Devil)
            SetButton(NodeManager.Instance.endPoint.curTile);
        else
        {
            if (inGameUI == null)
                inGameUI = GetComponentInParent<InGameUI>();
            inGameUI?.SwitchRightToTileUI(true);
        }
    }

    public void SetButton(ITileKind targetTile)
    {
        if (targetTile == null)
            return;

        if(targetTile is Tile realTile)
        {
            tileMoveBtn.SetActive(realTile.Movable);
            spawnerRemoveBtn.SetActive(false);
            tileRemoveBtn.SetActive(realTile.IsRemovable);
            curToolTipTile = realTile;
        }
        
        exitBtn.SetActive(true);
        if (inGameUI == null)
            inGameUI = GetComponentInParent<InGameUI>();
        inGameUI?.SwitchRightToTileUI(true);
    }

    //public void Update()
    //{
    //    if (!tileMoveBtn.gameObject.activeSelf && !tileRemoveBtn.gameObject.activeSelf)
    //        return;

    //}
}
