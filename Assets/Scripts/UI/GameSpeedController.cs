using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [SerializeField]
    private Button speed_0;
    [SerializeField]
    private Button speed_1;
    [SerializeField]
    private Button speed_2;

    public Image zeroImg;
    public Image normalImg;
    public Image fastImg;

    

    private int prevState = 0;

    public void SetSpeedPrev(bool showPopUp = true, bool setZero = true)
    {
        switch (prevState)
        {
            case 0:
                if (setZero)
                    SetSpeedZero();
                else
                    SetSpeedNormal(showPopUp);
                break;
            case 1:
                SetSpeedNormal(showPopUp); break;
            case 2:
                SetSpeedFast(showPopUp); break;
            default:
                SetSpeedNormal(showPopUp); break;
        }
    }

    private void SetButtonState()
    {
        zeroImg.color = Color.clear;
        normalImg.color = Color.clear;
        fastImg.color = Color.clear;

        if (GameManager.Instance.timeScale == 0)
            zeroImg.color = Color.white;
        if (GameManager.Instance.timeScale == 1)
            normalImg.color = Color.white;
        if (GameManager.Instance.timeScale == 2)
            fastImg.color = Color.white;
    }

    public bool Is_Tile_Connected(TileNode tile)
    {
        if (NodeManager.Instance.FindPath(tile) == null)
            return false;
        else
            return true;
    }

    public bool Is_Game_Continuable()
    {
        if (NodeManager.Instance.FindPath(NodeManager.Instance.startPoint, NodeManager.Instance.endPoint) == null)
            return false;

        //foreach(Battler target in GameManager.Instance.monsterList)
        //{
        //    if (target.CurTile == NodeManager.Instance.endPoint)
        //        continue;

        //    if (PathFinder.FindPath(target.CurTile, NodeManager.Instance.endPoint) == null)
        //        return false;
        //}

        //foreach(Battler target in GameManager.Instance.adventurersList)
        //{
        //    if (target.CurTile == NodeManager.Instance.endPoint)
        //        continue;

        //    if (PathFinder.FindPath(target.CurTile, NodeManager.Instance.endPoint) == null)
        //        return false;
        //}

        return true;
    }

    public bool Is_All_Tile_Connected()
    {
        foreach(TileNode tile in NodeManager.Instance._ActiveNodes)
        {
            if (tile == NodeManager.Instance.endPoint || tile.curTile == null)
                continue;

            if (tile.curTile.IsDormant)
                continue;

            if (NodeManager.Instance.FindPath(tile) == null)
                return false;
        }

        return true;
    }

    public void SetSpeedZero()
    {
        prevState = (int)GameManager.Instance.timeScale;

        GameManager.Instance.timeScale = 0;
        SetButtonState();
    }

    public void SetSpeedNormal()
    {
        if (!Is_Game_Continuable() && GameManager.Instance.IsInit)
        {
            string desc = DataManager.Instance.GetDescription("announce_ingame_tileconnect");
            GameManager.Instance.popUpMessage?.ToastMsg(desc);
            return;
        }
        prevState = (int)GameManager.Instance.timeScale;
        GameManager.Instance.timeScale = 1;
        SetButtonState();
    }

    public void SetSpeedNormal(bool showPopUp)
    {
        if (!Is_Game_Continuable())
        {
            if (showPopUp && GameManager.Instance.IsInit)
            {
                string desc = DataManager.Instance.GetDescription("announce_ingame_tileconnect");
                GameManager.Instance.popUpMessage?.ToastMsg(desc);
            }
            return;
        }
        prevState = (int)GameManager.Instance.timeScale;
        GameManager.Instance.timeScale = 1;
        SetButtonState();
    }

    public void SetSpeedFast()
    {
        if (!Is_Game_Continuable() && GameManager.Instance.IsInit)
        {
            string desc = DataManager.Instance.GetDescription("announce_ingame_tileconnect");
            GameManager.Instance.popUpMessage?.ToastMsg(desc);
            return;
        }

        GameManager.Instance.timeScale = 2;
        SetButtonState();
    }

    public void SetSpeedFast(bool showPopUp)
    {
        if (!Is_Game_Continuable())
        {
            if(showPopUp && GameManager.Instance.IsInit)
            {
                string desc = DataManager.Instance.GetDescription("announce_ingame_tileconnect");
                GameManager.Instance.popUpMessage?.ToastMsg(desc);
            }
            return;
        }

        GameManager.Instance.timeScale = 2;
        SetButtonState();
    }

    public void Awake()
    {
        speed_0.onClick.AddListener(SetSpeedZero);
        speed_1.onClick.AddListener(SetSpeedNormal);
        speed_2.onClick.AddListener(SetSpeedFast);
    }
}
