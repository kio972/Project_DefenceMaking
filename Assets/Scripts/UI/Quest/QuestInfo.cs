using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IQuestInteract
{
    void EndQuest();
    void SetQuest(Quest quest);
}

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

    private Quest _curQuest = null;
    public Quest curQuest { get => _curQuest; }

    [SerializeField]
    private Color idleColor = Color.white;
    [SerializeField]
    private Color midColor = new Color(1, 0.5f, 0);
    [SerializeField]
    private Color alertColor = Color.red;

    private Animator animator;
    private Animator _Animator { get { if (animator == null) animator = GetComponentInChildren<Animator>(); return animator; } }
    [SerializeField]
    List<QuestConditionUI> conditions;

    [SerializeField]
    private GameObject alarmObject;

    [SerializeField]
    bool isMainQuest = false;

    //[SerializeField]
    //AK.Wwise.Event clearSound;
    //[SerializeField]
    //AK.Wwise.Event failSound;


    private IQuestInteract _interactBtn;
    private IQuestInteract interactBtn
    {
        get
        {
            if (_interactBtn == null)
            {
                if (isMainQuest)
                    _interactBtn = GameManager.Instance._InGameUI.mainQuestInteractBtn;
                else
                    _interactBtn = GetComponentInChildren<IQuestInteract>(true);
            }
            return _interactBtn;
        }
    }

    private void DeActive()
    {
        gameObject.SetActive(false);
    }

    public void EndQuest()
    {
        isTimerOn = false;
        _Animator.SetBool("IsClear", _curQuest._IsClear);
        _Animator.SetTrigger("End");
        //Invoke("DeActive", 1f);

        //string questEndClip = (curQuest._IsClear ? "Quset_Close_Sussed_" : "Quset_Close_Failed_") + Random.Range(1, 3).ToString();
        //AudioManager.Instance.Play2DSound(questEndClip, SettingManager.Instance._FxVolume);
        //if (_curQuest._IsClear)
        //    clearSound?.Post(gameObject);
        //else
        //    failSound?.Post(gameObject);
        _curQuest = null;
        interactBtn?.EndQuest();
    }

    public void SetQuestText(Quest quest)
    {
        questName.text = DataManager.Instance.GetDescription(quest._QuestName);
        foreach (QuestConditionUI condition in conditions)
            condition.gameObject.SetActive(false);

        for(int i = 0; i < quest._ClearInfo.Count; i++)
        {
            conditions[i].SetQuest(quest, i);
            conditions[i].gameObject.SetActive(true);
        }
    }

    public void SetQuest(Quest quest)
    {
        _curQuest = quest;
        
        SetQuestText(quest);
        self.sizeDelta = new Vector2(self.sizeDelta.x, baseScale + (conditionScale * quest._ClearInfo.Count));
        gameObject.SetActive(true);
        _Animator.Rebind();
        isTimerOn = true;
        alarmObject.SetActive(false);
        UpdateTimer();
        interactBtn?.SetQuest(quest);
    }

    private void UpdateTimer()
    {
        if (!isTimerOn)
            return;

        if (questTimer != null)
            questTimer.fillAmount = _curQuest._TimeRemain;

        if (_curQuest._TimeRemain > 0.34f)
            questTimer.color = idleColor;
        else if (_curQuest._TimeRemain > 0.17f)
            questTimer.color = midColor;
        else
        {
            questTimer.color = alertColor;
            alarmObject.SetActive(true);
        }

        if (_curQuest._TimeRemain <= 0)
        {
            isTimerOn = false;
            alarmObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_curQuest == null)
            return;

        UpdateTimer();
        if (_curQuest._IsEnd)
            EndQuest();
    }

    private void Start()
    {
        LanguageManager.Instance.AddLanguageAction(() =>
        {
            if(_curQuest != null)
                questName.text = DataManager.Instance.GetDescription(_curQuest._QuestName);
        });
    }
}
