using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    [SerializeField]
    List<FluctTimer> timerList;

    private int curWave;

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
    }

    void Update()
    {
        if (curWave == GameManager.Instance.CurWave)
            return;

        curWave = GameManager.Instance.CurWave;
        foreach (FluctTimer timer in timerList)
            timer.IncreaseTime();
    }
}
