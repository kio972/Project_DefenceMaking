using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameBtn : MoveSceneBtn
{
    private void Awake()
    {
        SaveManager.Instance.LoadPlayerData();
        if (SaveManager.Instance.playerData == new PlayerData())
            gameObject.SetActive(false);
    }

}
