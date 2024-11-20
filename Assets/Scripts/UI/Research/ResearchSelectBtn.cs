using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchSelectBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform iconRect;

    [SerializeField]
    Button researchBtn;

    [SerializeField]
    Sprite clickedSprite;
    [SerializeField]
    Sprite unclickedSprite;

    bool isClicked = false;

    Coroutine position_Modify_Coroutine;

    Vector3 originPos;

    public float mouseOverTime = 0.2f;

    private ResearchSelectControl controller;

    [SerializeField]
    ResearchSlot baseSlot;

    [SerializeField]
    ResearchUI targetPage;

    private void OnEnable()
    {
        if(isClicked)
            SetBaseBtn();
    }

    private void SetController()
    {
        if(controller == null)
            controller = GetComponentInParent<ResearchSelectControl>();
        if (controller == null) return;

        controller.SetCurResearch(this);
    }

    public void DeActiveClick()
    {
        isClicked = false;
        researchBtn.image.sprite = unclickedSprite;
        //transform.position = originPos;
        if (targetPage != null)
        {
            //targetPage.ResetPopUp();
            targetPage.gameObject.SetActive(false);
        }
    }

    private void SetBaseBtn()
    {
        baseSlot?.OnClick();
        baseSlot?.CallPopUpUI();
    }

    public void OnClick()
    {
        if (isClicked)
            return;
        isClicked = true;

        researchBtn.image.sprite = clickedSprite;
        researchBtn.image.color = Color.white;
        //transform.position = originPos + new Vector3(-110, 0 , 0);
        if (targetPage != null)
            targetPage.gameObject.SetActive(true);
        SetBaseBtn();
        SetController();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked)
            return;

        researchBtn.image.color = Color.white * 0.96f;

        //if (position_Modify_Coroutine != null)
        //    StopCoroutine(position_Modify_Coroutine);
        //position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, new Vector3(originPos.x - 110, originPos.y, originPos.z), mouseOverTime));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked)
            return;

        researchBtn.image.color = Color.white;
        //if (position_Modify_Coroutine != null)
        //    StopCoroutine(position_Modify_Coroutine);
        //position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, originPos, mouseOverTime));
    }

    public void Init()
    {
        originPos = transform.position;
        if (researchBtn != null)
            researchBtn.onClick.AddListener(OnClick);
    }
}
