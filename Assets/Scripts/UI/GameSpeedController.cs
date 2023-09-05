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

    [SerializeField]
    private PopUpMessage popUpMessage;

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

    public bool Is_All_Tile_Connected()
    {
        foreach(TileNode tile in NodeManager.Instance.activeNodes)
        {
            if (tile == NodeManager.Instance.endPoint || tile.curTile == null)
                continue;

            if (PathFinder.Instance.FindPath(tile) == null)
                return false;
        }

        return true;
    }

    public void SetSpeedZero()
    {
        GameManager.Instance.timeScale = 0;
        GameManager.Instance.SetCharAnimPause();
        SetButtonState();
    }

    public void SetSpeedNormal()
    {
        if (!Is_All_Tile_Connected())
        {
            popUpMessage?.ToastMsg("모든 타일이 마왕방과 연결되어 있어야 합니다!");
            return;
        }

        GameManager.Instance.timeScale = 1;
        SetButtonState();
    }

    public void SetSpeedFast()
    {
        if (!Is_All_Tile_Connected())
        {
            popUpMessage?.ToastMsg("모든 타일이 마왕방과 연결되어 있어야 합니다!");
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
