using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : UISetActiveBtn
{
    [SerializeField]
    private GameObject noData;
    [SerializeField]
    private GameObject confirm;

    public override void SetActive(bool value)
    {
        if (value)
        {
            UIManager.Instance.AddTab(targetObject);
            SaveManager.Instance.LoadPlayerData();
            bool haveData = SaveManager.Instance.playerData.tiles != null;
            noData?.SetActive(!haveData);
            confirm.SetActive(haveData);
        }
        else
            UIManager.Instance.CloseTab(targetObject);
    }
}
