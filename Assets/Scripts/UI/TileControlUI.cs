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

    private InGameUI inGameUI;

    public void CloseAllBtn()
    {
        NodeManager.Instance.SetGuideState(GuideState.None);
        CloseAll();
    }

    public void CloseAll()
    {
        tileMoveBtn.SetActive(false);
        tileRemoveBtn.SetActive(false);
        exitBtn.SetActive(false);
        inGameUI?.SwitchRightToTileUI(false);
    }

    public void MoveTile()
    {
        Tile curTile = InputManager.Instance._CurTile;
        if (curTile == null)
            return;
        if (curTile.MovableNow)
        {
            curTile.ReadyForMove();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("Ÿ�� ���� ĳ���Ͱ� �־� ������ �� �����ϴ�!");
    }

    public void RemoveSpawner()
    {
        Tile curTile = InputManager.Instance._CurTile;
        if (curTile == null)
            return;

        curTile.RemoveSpawner();
        InputManager.Instance.ClickTile(curTile);
    }

    public void RemoveTile()
    {
        Tile curTile = InputManager.Instance._CurTile;
        if (curTile == null)
            return;
        if(curTile.IsRemovableNow)
        {
            curTile.RemoveTile();
            InputManager.Instance.ResetTileClick();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("Ÿ�� ���� ĳ���Ͱ� �־� ���� �� �� �����ϴ�!");
    }

    public void SetButton(Tile targetTile)
    {
        tileMoveBtn.SetActive(targetTile.Movable);
        spawnerRemoveBtn.SetActive(targetTile.HaveSpawner);
        tileRemoveBtn.SetActive(!targetTile.HaveSpawner && targetTile.IsRemovable);
        exitBtn.SetActive(true);
        if (inGameUI == null)
            inGameUI = GetComponentInParent<InGameUI>();
        inGameUI?.SwitchRightToTileUI(true);
    }

    public void Update()
    {
        if (!tileMoveBtn.gameObject.activeSelf && !tileRemoveBtn.gameObject.activeSelf)
            return;

    }
}
