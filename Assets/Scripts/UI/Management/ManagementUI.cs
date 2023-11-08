using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementUI : MonoBehaviour
{

    private bool initState = false;



    public void SetItem()
    {
        if (!initState)
            Init();

        if(GameManager.Instance.researchLevel == 0)
        {

        }
    }

    public void Init()
    {
        foreach(Dictionary<string, object> data in DataManager.Instance.Battler_Table)
        {
            string id = data["id"].ToString();
            if (id[2] is not 'm' and 't')
                continue;


        }


        initState = true;
    }
}
