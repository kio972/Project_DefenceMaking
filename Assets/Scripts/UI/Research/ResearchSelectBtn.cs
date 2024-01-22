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

    bool isClicked = false;

    Coroutine position_Modify_Coroutine;

    Vector3 originPos;

    public float mouseOverTime = 0.2f;

    private ResearchSelectControl controller;

    [SerializeField]
    ResearchUI targetPage;

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
        transform.position = originPos;
        if(targetPage != null)
        {
            targetPage.ResetPopUp();
            targetPage.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (isClicked)
            return;
        isClicked = true;

        transform.position = originPos + new Vector3(-110, 0 , 0);
        if (targetPage != null)
            targetPage.gameObject.SetActive(true);
        SetController();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked)
            return;

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, new Vector3(originPos.x - 110, originPos.y, originPos.z), mouseOverTime));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked)
            return;

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, originPos, mouseOverTime));
    }

    public void Init()
    {
        originPos = transform.position;
        if (researchBtn != null)
            researchBtn.onClick.AddListener(OnClick);
    }
}
