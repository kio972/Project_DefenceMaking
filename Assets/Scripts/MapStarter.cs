using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStarter : MonoBehaviour
{
    void Start()
    {
        if(SaveManager.Instance.playerData == null)
        {
            GameManager.Instance.Init();
            QuestManager.Instance.InitQuest();
        }
        else
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
    }
}
