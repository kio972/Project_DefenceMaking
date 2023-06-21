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

    public void SetSpeedZero()
    {
        GameManager.Instance.timeScale = 0;
        SetButtonState();
    }

    public void SetSpeedNormal()
    {
        if (PathFinder.Instance.FindPath(NodeManager.Instance.startPoint) == null)
            return;

        GameManager.Instance.timeScale = 1;
        SetButtonState();
    }

    public void SetSpeedFast()
    {
        if (PathFinder.Instance.FindPath(NodeManager.Instance.startPoint) == null)
            return;

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
