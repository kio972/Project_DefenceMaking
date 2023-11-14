using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControlUI : MonoBehaviour
{
    [SerializeField]
    private GameObject tileMoveBtn;
    [SerializeField]
    private GameObject tileRemoveBtn;

    public void MoveTile()
    {
        Tile curTile = InputManager.Instance._CurTile;
        if (curTile == null)
            return;
        if (curTile.Movable)
        {
            curTile.ReadyForMove();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("타일 위에 캐릭터가 있어 움직일 수 없습니다!");
    }

    public void RemoveTile()
    {
        Tile curTile = InputManager.Instance._CurTile;
        if (curTile == null)
            return;
        if(curTile.IsRemovable)
        {
            curTile.RemoveTile();
            InputManager.Instance.ResetTileClick();
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("두개 이상의 타일과 연결된 타일은 제거할 수 없습니다!");
    }

    public void SetButton(bool movable, bool removable)
    {
        tileMoveBtn.SetActive(movable);
        tileRemoveBtn.SetActive(removable);
    }

    public void Update()
    {
        if (!tileMoveBtn.gameObject.activeSelf && !tileRemoveBtn.gameObject.activeSelf)
            return;

    }
}
