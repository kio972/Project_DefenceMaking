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

    [SerializeField]
    private GameObject on_Active;
    [SerializeField]
    private GameObject on_DeActive;
    [SerializeField]
    private GameObject off_Active;
    [SerializeField]
    private GameObject off_DeActive;

    private void Call_FullScreen_On()
    {
        SettingManager.Instance.Set_FullScreen(true);
        on_Active.SetActive(true);
        on_DeActive.SetActive(false);
        off_Active.SetActive(false);
        off_DeActive.SetActive(true);
    }

    private void Call_FullScreen_Off()
    {
        SettingManager.Instance.Set_FullScreen(false);
        on_Active.SetActive(false);
        on_DeActive.SetActive(true);
        off_Active.SetActive(true);
        off_DeActive.SetActive(false);
    }

    private void Awake()
    {
        if(fullScreen_On_Btn != null)
            fullScreen_On_Btn.onClick.AddListener(Call_FullScreen_On);
        if (fullScreen_Off_Btn != null)
            fullScreen_Off_Btn.onClick.AddListener(Call_FullScreen_Off);

        if (fullScreen_Off_Btn != null && fullScreen_On_Btn != null)
        {
            if (SettingManager.Instance.screen_FullSize)
                Call_FullScreen_On();
            else
                Call_FullScreen_Off();
        }
    }
}
