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

    [SerializeField]
    private IllustrateUI shopIllur;
    [SerializeField]
    private PopUpMessage malpoongsun;

    public void PlayScript(string message, string anim, string anim2 = "")
    {
        malpoongsun.ToastMsg(message);
        if (anim == "idle")
            shopIllur.SetAnim(anim, true, 0);
        else
            shopIllur.SetAnim(anim, false, 0, "idle");

        if (!string.IsNullOrEmpty(anim2))
            shopIllur.SetAnim(anim2, false, 1);
    }

    public void PlayScript(string scriptID)
    {
        Dictionary<string, object> script = DataManager.Instance.GetMalpoongsunScript(scriptID);
        if (script == null)
            return;

        string conver = script["script"].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();

        PlayScript(conver, track0, track1);
    }

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);

        if(value)
            PlayScript("Shop000");
    }

    private void Awake()
    {
        HerbSlot[] herbSlots = GetComponentsInChildren<HerbSlot>(true);
        foreach (HerbSlot slot in herbSlots)
            slot.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                SetActive(true);
            else if (uiPage.activeSelf)
                SetActive(false);
        }

        if (curWave == GameManager.Instance.CurWave)
            return;

        curWave = GameManager.Instance.CurWave;
        foreach (FluctTimer timer in timerList)
            timer.IncreaseTime();
    }
}
