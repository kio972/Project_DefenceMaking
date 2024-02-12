using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchMainUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    private Research curResearch;
    private float curProgressTime;

    public float CurProgressTime { get => curProgressTime; }

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        //if (value)
        //    SetItem();
    }
}
