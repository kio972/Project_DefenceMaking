using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : ToggleGameobject
{
    public void SetActiveMenu(bool value)
    {
        float time = 1f;
        

        if (value)
        {
            time = 0f;
            //SetToggleState(false);
        }

        GameManager.Instance.isPause = value;
        Time.timeScale = time;
    }
}
