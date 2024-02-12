using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject imgGroup;

    [SerializeField]
    private ResearchSlot targetSlot;
    [SerializeField]
    private Image outlineImg;
    [SerializeField]
    private Image frameImg;
    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private GameObject checkBox;

    [SerializeField]
    private Image inProgressImg;

    private bool isClicked = false;
    
    private ResearchUI researchUI;
    private ResearchUI _ResearchUI
    {
        get
        {
            if (researchUI == null)
                researchUI = GetComponentInParent<ResearchUI>();
            return researchUI;
        }
    }

    private ResearchData researchData;

    [SerializeField]
    protected ResearchState curState;
    protected bool isInProgress = false;

    public ResearchState _CurState { get => curState; }

    [SerializeField]
    private ResearchSlot[] prevResearch = null;

    private readonly Color selectedColor = new Color(0.14f, 1, 0);
    private readonly Color mouseOverColor = new Color(0.4f, 0.4f, 0.4f);
    private readonly Color impossibleColor = new Color(0.15f, 0.15f, 0.15f);

    public void CheckResearchUnlock()
    {
        bool unlock = true;
        foreach(ResearchSlot prevSlot in prevResearch)
        {
            if (prevSlot._CurState == ResearchState.Complete)
                continue;
            unlock = false;
        }

        if (unlock)
            curState = ResearchState.Incomplete;
        else
            curState = ResearchState.Impossible;
    }

    protected override void CallPopUpUI()
    {
        base.CallPopUpUI();

        if (targetSlot != null)
            _ResearchUI._ResearchPopup?.SetPopUp(researchData, iconImg.sprite, curState);
    }

    public void SetResearchgState(ResearchState state)
    {
        curState = state;
    }

    public void DeActiveClick()
    {
        isClicked = false;

        if (curState == ResearchState.Incomplete)
            SetDefault();
        if (curState == ResearchState.Impossible)
            SetImPossible();
    }

    private void CallResearchUI(ResearchSlot targetSlot)
    {
        if (_ResearchUI == null) return;

        _ResearchUI.SetClickedSlot(targetSlot);
    }

    private void SetImPossible()
    {
        if (frameImg == null || outlineImg == null) return;

        outlineImg.color = impossibleColor;
        frameImg.color = impossibleColor;
        iconImg.color = Color.gray;
    }

    private void SetDefault()
    {
        if (frameImg == null || outlineImg == null) return;

        outlineImg.color = Color.black;
        frameImg.color = Color.black;
    }

    private void OnClick()
    {
        if (isClicked) return;

        if(targetSlot != null)
        {
            isClicked = true;

            outlineImg.gameObject.SetActive(true);
            outlineImg.color = selectedColor;
            if(curState == ResearchState.Incomplete)
                frameImg.color = Color.black;
            if (curState == ResearchState.Impossible)
                frameImg.color = impossibleColor;
        }
        
        CallResearchUI(targetSlot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) return;

        if (frameImg == null || outlineImg == null) return;
        
        if (isClicked) return;

        if (curState != ResearchState.Incomplete && curState != ResearchState.InProgress && curState != ResearchState.Impossible) return;

        frameImg.color = mouseOverColor;
        outlineImg.color = mouseOverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null) return;
        
        if (isClicked) return;

        if (curState != ResearchState.Incomplete && curState != ResearchState.InProgress && curState != ResearchState.Impossible) return;

        if (curState != ResearchState.Impossible)
            SetDefault();
        else
            SetImPossible();
    }

    private void OnEnable()
    {
        if (outlineImg != null)
            outlineImg.gameObject.SetActive(!isInProgress);

        if (inProgressImg != null)
            inProgressImg.gameObject.SetActive(isInProgress);
    }

    protected override void Awake()
    {
        base.Awake();

        if (button != null)
            button.onClick.AddListener(OnClick);

        if (!string.IsNullOrEmpty(researchId))
            researchData = new ResearchData(researchId);
    }

    private void Start()
    {
        if (curState == ResearchState.None && imgGroup != null)
            imgGroup.SetActive(false);
        else
        {
            CheckResearchUnlock();
            if (curState == ResearchState.Impossible)
                SetImPossible();
        }
    }
}
