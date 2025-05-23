using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

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

    [SerializeField]
    private TooltipStats toolTip_stats;

    private InGameUI inGameUI;

    private TooltipObject curToolTipObject;
    private Tile curToolTipTile;

    [SerializeField]
    FMODUnity.EventReference refusedSound;
    [SerializeField]
    FMODUnity.EventReference tileMoveSound;
    [SerializeField]
    FMODUnity.EventReference tileRemoveSound;

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
        inGameUI?.tileArrowUI?.SetOFF();
        RoomManaPool.Instance.HideAllManaText();
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
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
        }
        else
        {
            CloseAllBtn();
            curTile.ReadyForMove().Forget();
            InputManager.Instance.settingCard = true;
            FMODUnity.RuntimeManager.PlayOneShot(tileMoveSound);
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
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
        }
        else if (curTile.objectKind != null)
        { 
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_removeFailObject"));
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
        }
        else if (curTile.IsCharacterOnIt)
        {
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_moveFailEnemy"));
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
        }
        else
        {
            curTile.RemoveTile();
            FMODUnity.RuntimeManager.PlayOneShot(tileRemoveSound);
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
        toolTip_stats?.SetStatZone(tooltipObject);

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

        UpdateLayouts();
    }

    private void UpdateLayouts()
    {
        LayoutGroup[] layouts = GetComponentsInChildren<LayoutGroup>();
        foreach (LayoutGroup layout in layouts)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
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
        inGameUI?.tileArrowUI?.SetOFF();
        inGameUI?.tileArrowUI?.SetArrow(targetTile, targetTile.curNode);

        if(targetTile is ITileManaEffect effect)
        {
            RoomManaPool.Instance.HideAllManaText();
            DrawManaUI(targetTile.curNode, effect);
        }
    }

    private void DrawManaUI(TileNode curNode, ITileManaEffect manaEffect)
    {
        if (manaEffect != null && curNode != null)
        {
            foreach (var node in curNode.neighborNodeDic.Values)
            {
                if (node.tileKind == null)
                    continue;
                string text = manaEffect.GetManaText(node.tileKind);
                if (text == null)
                    continue;
                RoomManaPool.Instance.ShowManaText(node.transform, text);
            }
        }
    }

    public void Update()
    {
        UpdateLayouts();
    }
}
