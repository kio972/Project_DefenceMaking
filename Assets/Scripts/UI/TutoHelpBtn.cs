using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoHelpBtn : MonoBehaviour
{
    [SerializeField]
    DeployUI tutoDeploy;

    [SerializeField]
    ResearchMainUI researchMain;

    public int stage = 0;

    public void OnClick()
    {
        if (stage == 0)
            tutoDeploy.SetActive(true, false);
        else if (stage == 1)
            researchMain.SetActive(true);
        else if(stage == 2)
        {
            List<int> roomPool = new List<int>(DataManager.Instance.roomCard_Indexs);
            roomPool.RemoveAt(0);
            GameManager.Instance.cardDeckController.DrawCard(roomPool[Random.Range(0, roomPool.Count)]);
            gameObject.SetActive(false);
        }
    }
}
