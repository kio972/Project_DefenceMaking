using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public enum ResearchState
{
    None,
    Incomplete,
    Complete,
    InProgress,
    Impossible,
}



public class ResearchSlot : PopUICallBtn, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string researchId;
    public string _ResearchId { get => researchId; }

    [SerializeField]
    private GameObject imgGroup;

    [SerializeField]
    private GameObject completedFrame;
    [SerializeField]
    private GameObject selectedFrame;
    [SerializeField]
    private GameObject outLineFrame;
    [SerializeField]
    private GameObject inProgressFrame;

    [SerializeField]
    private ResearchSlot targetSlot;
    //[SerializeField]
    //private Image outlineImg;
    [SerializeField]
    private Image frameImg;
    [SerializeField]
    private Image iconImg;
    //[SerializeField]
    //private GameObject checkBox;

    //[SerializeField]
    //private Image inProgressImg;

    private bool isClicked = false;
    
    private ResearchUI researchUI;
    private ResearchUI _ResearchUI
    {
        get
        {
            if (researchUI == null)
                researchUI = GetComponentInParent<ResearchUI>(true);
            return researchUI;
        }
    }

    private ResearchData researchData;
    private bool initState = false;
    public ResearchData _ResearchData
    {
        get
        {
            if (!initState) Init();
            return researchData;
        }
    }

    public ReactiveProperty<ResearchState> _curState { get; protected set; } = new ReactiveProperty<ResearchState>();

    public ResearchState _CurState { get => _curState.Value; }

    [SerializeField]
    private ResearchSlot[] prevResearch = null;

    private readonly Color selectedColor = new Color(0.14f, 1, 0);
    private readonly Color mouseOverColor = Color.black;
    private readonly Color impossibleColor = new Color(0.494f, 0.470f, 0.439f);
    private readonly Color completeColor = new Color(1, 0.4f, 0);
    private readonly Color defaultColor = new Color(0.305f, 0.274f, 0.254f);

    private void OnDisable()
    {
        DeActiveClick();
        OnPointerExit(null);
    }

    private void OnEnable()
    {
        if (outLineFrame != null)
            outLineFrame.SetActive(_curState.Value is ResearchState.Complete or ResearchState.InProgress);

        if (inProgressFrame != null)
            inProgressFrame.SetActive(_curState.Value == ResearchState.InProgress);

        CheckResearchUnlock();

        if (_curState.Value == ResearchState.Impossible)
            SetImPossible();
        else if (_curState.Value == ResearchState.Complete)
            SetComplete();
        else if (_curState.Value == ResearchState.Incomplete)
            SetDefault();
        else
            iconImg.color = Color.white;

        if(isClicked)
            SetClicked();
    }

    public void CheckResearchUnlock()
    {
        if (_curState.Value == ResearchState.Complete || _curState.Value == ResearchState.InProgress)
            return;

        bool unlock = true;
        foreach(ResearchSlot prevSlot in prevResearch)
        {
            if (prevSlot._CurState == ResearchState.Complete)
                continue;
            unlock = false;
        }

        if (unlock)
            _curState.Value = ResearchState.Incomplete;
        else
            _curState.Value = ResearchState.Impossible;
    }

    public override void CallPopUpUI()
    {
        base.CallPopUpUI();

        if (targetSlot != null)
            _ResearchUI._ResearchPopup?.SetPopUp(this, iconImg.sprite, _curState.Value);
    }

    public void SetResearchState(ResearchState state)
    {
        _curState.Value = state;
        inProgressFrame.SetActive(state == ResearchState.InProgress);
    }

    public void DeActiveClick()
    {
        isClicked = false;

        selectedFrame.SetActive(false);

        if (_curState.Value == ResearchState.Impossible)
            SetImPossible();
        else if (_curState.Value == ResearchState.Incomplete)
            SetDefault();
        else if (_curState.Value == ResearchState.Complete)
            SetComplete();
        else if (_curState.Value == ResearchState.InProgress)
            inProgressFrame.SetActive(true);
    }

    private void CallResearchUI(ResearchSlot targetSlot)
    {
        if (_ResearchUI == null) return;

        _ResearchUI.SetClickedSlot(targetSlot);
    }

    private void SetComplete()
    {
        outLineFrame.SetActive(true);
        completedFrame.SetActive(true);
        iconImg.color = Color.white;
    }

    private void SetImPossible()
    {
        outLineFrame.SetActive(false);
        selectedFrame.SetActive(false);
        frameImg.color = impossibleColor;
        iconImg.color = impossibleColor;
    }

    private void SetDefault()
    {
        outLineFrame.SetActive(false);
        selectedFrame.SetActive(false);
        frameImg.color = defaultColor;
        iconImg.color = defaultColor;
    }

    private void SetClicked()
    {
        outLineFrame.SetActive(true);
        selectedFrame.SetActive(true);
        iconImg.color = _curState.Value == ResearchState.Impossible ? impossibleColor : Color.white;
    }

    public void OnClick()
    {
        if (isClicked) return;

        //if (curState.Value == ResearchState.Complete)
        //    return;

        if (targetSlot != null)
        {
            isClicked = true;

            SetClicked();
        }
        
        CallResearchUI(targetSlot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) return;

        if (frameImg == null || iconImg == null) return;
        
        if (isClicked) return;

        if(_curState.Value is ResearchState.Incomplete or ResearchState.Impossible)
        {
            frameImg.color = mouseOverColor;
            iconImg.color = mouseOverColor;
        }
        else
            iconImg.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null) return;
        
        if (isClicked) return;

        if (_curState.Value == ResearchState.Incomplete)
            SetDefault();
        else if (_curState.Value == ResearchState.Impossible)
            SetImPossible();
        else
            iconImg.color = Color.white;
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        if (initState)
            return;

        if (button != null)
            button.onClick.AddListener(OnClick);

        if (!string.IsNullOrEmpty(researchId))
            researchData = new ResearchData(researchId);

        initState = true;
    }

    private void Start()
    {
        if (_curState.Value == ResearchState.None && imgGroup != null)
            imgGroup.SetActive(false);
        else
        {
            CheckResearchUnlock();
        }
    }
}
