using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ResearchState
{
    Complete,
    InProgress,
    
}

public class ResearchSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private ResearchSlot targetSlot;
    [SerializeField]
    private Image outlineImg;
    [SerializeField]
    private Image frameImg;
    [SerializeField]
    private Image iconImg;

    private bool isClicked = false;

    private ResearchUI researchUI;

    public void DeActiveClick()
    {
        isClicked = false;

        outlineImg.color = Color.black;
        frameImg.color = Color.black;
    }

    private void CallResearchUI(ResearchSlot targetSlot)
    {
        if(researchUI == null)
            researchUI = GetComponentInParent<ResearchUI>();
        if (researchUI == null) return;

        researchUI.SetClickedSlot(targetSlot);
    }

    private void OnClick()
    {
        if (isClicked) return;

        if(targetSlot != null)
        {
            isClicked = true;

            outlineImg.color = Color.green;
            frameImg.color = Color.black;
        }
        
        CallResearchUI(targetSlot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) return;

        if (isClicked) return;

        if (frameImg == null || outlineImg == null) return;
        frameImg.color = Color.gray;
        outlineImg.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null) return;

        if (isClicked) return;

        if (frameImg == null || outlineImg == null) return;
        frameImg.color = Color.black;
        outlineImg.color = Color.black;
    }

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
    }
}
