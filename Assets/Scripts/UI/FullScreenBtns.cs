using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenBtns : MonoBehaviour
{
    [SerializeField]
    private Button fullScreen_On_Btn;
    [SerializeField]
    private Button fullScreen_Off_Btn;

    private void Call_FullScreen_On()
    {
        SettingManager.Instance.Set_FullScreen(true);
    }

    private void Call_FullScreen_Off()
    {
        SettingManager.Instance.Set_FullScreen(false);
    }

    private void Awake()
    {
        if(fullScreen_On_Btn != null)
            fullScreen_On_Btn.onClick.AddListener(Call_FullScreen_On);
        if (fullScreen_Off_Btn != null)
            fullScreen_Off_Btn.onClick.AddListener(Call_FullScreen_Off);
    }
}
