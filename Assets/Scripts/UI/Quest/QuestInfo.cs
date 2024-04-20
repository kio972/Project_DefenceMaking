using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questName;
    [SerializeField]
    private Image questTimer;
    [SerializeField]
    private RectTransform self;
    public float baseScale;
    public float conditionScale;

    private float curTime;
    private float limitTime;

    private bool isTimerOn = false;

    private Quest curQuest = null;

    private readonly Color idleColor = Color.white;
    private readonly Color midColor = new Color(1, 0.5f, 0);
    private readonly Color alertColor = Color.red;

    private Animator animator;
    private Animator _Animator { get { if (animator == null) animator = GetComponentInChildren<Animator>(); return animator; } }
    [SerializeField]
    List<QuestCondition> conditions;

    private void DeActive()
    {
        gameObject.SetActive(false);
    }

    public void EndQuest()
    {
        isTimerOn = false;
        _Animator.SetBool("IsClear", curQuest._IsClear);
        _Animator.SetTrigger("End");
        Invoke("DeActive", 1f);
        curQuest = null;
        QuestController questController = transform.parent.GetComponent<QuestController>();
        questController?.EndQuest(curQuest);
    }

    public void SetQuestText(Quest quest)
    {
        questName.text = quest._QuestName;
        foreach (QuestCondition condition in conditions)
            condition.gameObject.SetActive(false);

        for(int i = 0; i < quest._ClearInfo.Count; i++)
        {
            conditions[i].SetQuest(quest, i);
            conditions[i].gameObject.SetActive(true);
        }
    }

    public void SetQuest(Quest quest)
    {
        curQuest = quest;
        
        SetQuestText(quest);
        self.sizeDelta = new Vector2(self.sizeDelta.x, baseScale + (conditionScale * quest._ClearInfo.Count));
        gameObject.SetActive(true);
        _Animator.Rebind();
        isTimerOn = true;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (!isTimerOn)
            return;

        if (questTimer != null)
            questTimer.fillAmount = curQuest._TimeRemain;

        if (curQuest._TimeRemain > 0.4f)
            questTimer.color = idleColor;
        else if (curQuest._TimeRemain > 0.15f)
            questTimer.color = midColor;
        else
            questTimer.color = alertColor;

        if (curQuest._TimeRemain <= 0)
            isTimerOn = false;
    }

    private void Update()
    {
        if (curQuest == null)
            return;

        UpdateTimer();
        if (curQuest._IsEnd)
            EndQuest();
    }
}
