using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private TextMeshProUGUI select1;
    [SerializeField]
    private TextMeshProUGUI select2;

    private int curIndex = 0;
    private List<string> targetQuest;

    private void StartQuest(string id)
    {
        QuestManager.Instance.StartQuest(id);
        GameManager.Instance.SetPause(false);
        gameObject.SetActive(false);
    }

    public void SelectFirst()
    {
        StartQuest(targetQuest[curIndex]);
    }

    public void SelectSecond()
    {
        if (targetQuest.Count > 2)
        {
            curIndex++;
            if (curIndex > targetQuest.Count - 1)
                curIndex = 0;
        }
        else
            StartQuest(targetQuest[1]);
    }

    public void SetMessage(Dictionary<string, object> data)
    {
        gameObject.SetActive(true);
        GameManager.Instance.SetPause(true);

        if (data == null)
            return;

        titleText.text = data["MessageTitle"].ToString();
        messageText.text = data["MessageText"].ToString();
        string[] buttonTexts = data["MessageButtonText"].ToString().Split('/');
        select1.transform.parent.gameObject.SetActive(true);
        select1.text = buttonTexts[0];
        targetQuest = data["TargetQuest"].ToString().Split('/').ToList();
        if (buttonTexts.Length <= 1)
            select2.transform.parent.gameObject.SetActive(false);
        else if(buttonTexts.Length >= 2)
        {
            if (buttonTexts.Length == 2)
                select2.text = buttonTexts[1];
            else
                select2.text = "다른 선택지를 고려한다.";
            select2.transform.parent.gameObject.SetActive(true);
        }

        titleText.text = data["MessageTitle"].ToString();
    }
}
