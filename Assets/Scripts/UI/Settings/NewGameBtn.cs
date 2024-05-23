using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameBtn : MoveSceneBtn
{
    protected override void MoveScene()
    {
        SaveManager.Instance.playerData = null;
        base.MoveScene();
    }

    private void Awake()
    {
        SaveManager.Instance.LoadPlayerData();
        if (SaveManager.Instance.playerData == new PlayerData())
            gameObject.SetActive(false);
    }

}
