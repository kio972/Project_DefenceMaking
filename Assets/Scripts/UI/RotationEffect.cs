using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotationEffect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image targetImg;
    [SerializeField]
    private Sprite originImg;
    [SerializeField]
    private Sprite selectedImg;

    private bool isOn = false;
    private bool mouseOver = false;

    private RectTransform imgRect;
    public float rotationSpeed = 60f;

    private void Start()
    {
        if (targetImg == null)
            return;

        imgRect = targetImg.GetComponent<RectTransform>();
    }

    public void SetDefault()
    {
        isOn = false;
        targetImg.sprite = originImg;
        imgRect.rotation = Quaternion.identity;
    }

    private void RotateImg()
    {
        float rotationAngle = rotationSpeed * Time.deltaTime;
        imgRect.Rotate(0f, 0f, rotationAngle);
    }

    protected void SetSelected()
    {
        if (isOn)
            return;

        isOn = true;
        targetImg.sprite = selectedImg;
        imgRect.rotation = Quaternion.identity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetImg == null)
            return;
        
        SetSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImg == null)
            return;

        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImg == null)
            return;

        if (!isOn)
            SetDefault();
        mouseOver = false;
    }

    public virtual void Update()
    {
        if (targetImg == null)
            return;

        if (!mouseOver || isOn)
            return;

        RotateImg();
    }
}
