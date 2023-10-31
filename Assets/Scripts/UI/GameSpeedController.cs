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

    public Sprite zeroSprite;
    public Sprite zeroSprite_Select;
    public Sprite normalSprite;
    public Sprite normalSprite_Select;
    public Sprite fastSprite;
    public Sprite fastSprite_Select;

    public Image zeroImg;
    public Image normalImg;
    public Image fastImg;

    

    private int prevState;

    public void SetSpeedPrev(bool showPopUp = true)
    {
        switch(prevState)
        {
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
        zeroImg.sprite = zeroSprite;
        normalImg.sprite = normalSprite;
        fastImg.sprite = fastSprite;

        if (GameManager.Instance.timeScale == 0)
            zeroImg.sprite = zeroSprite_Select;
        if (GameManager.Instance.timeScale == 1)
            normalImg.sprite = normalSprite_Select;
        if (GameManager.Instance.timeScale == 2)
            fastImg.sprite = fastSprite_Select;
    }

    public bool Is_Tile_Connected(TileNode tile)
    {
        if (PathFinder.Instance.FindPath(tile) == null)
            return false;
        else
            return true;
    }

    public bool Is_Game_Continuable()
    {
        if (PathFinder.Instance.FindPath(NodeManager.Instance.startPoint, NodeManager.Instance.endPoint) == null)
            return false;

        foreach(Battler target in GameManager.Instance.monsterList)
        {
            if (target.CurTile == NodeManager.Instance.endPoint)
                continue;

            if (PathFinder.Instance.FindPath(target.CurTile, NodeManager.Instance.endPoint) == null)
                return false;
        }

        foreach(Battler target in GameManager.Instance.adventurersList)
        {
            if (target.CurTile == NodeManager.Instance.endPoint)
                continue;

            if (PathFinder.Instance.FindPath(target.CurTile, NodeManager.Instance.endPoint) == null)
                return false;
        }

        return true;
    }

    public bool Is_All_Tile_Connected()
    {
        foreach(TileNode tile in NodeManager.Instance.activeNodes)
        {
            if (tile == NodeManager.Instance.endPoint || tile.curTile == null)
                continue;

            if (tile.curTile.IsDormant)
                continue;

            if (PathFinder.Instance.FindPath(tile) == null)
                return false;
        }

        return true;
    }

    public void SetSpeedZero()
    {
        if (GameManager.Instance.timeScale != 0)
            prevState = (int)GameManager.Instance.timeScale;

        GameManager.Instance.timeScale = 0;
        GameManager.Instance.SetCharAnimPause();
        SetButtonState();
    }

    public void SetSpeedNormal()
    {
        if (!Is_Game_Continuable())
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
            if (showPopUp)
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
        if (!Is_Game_Continuable())
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
            if(showPopUp)
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
