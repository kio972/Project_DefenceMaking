using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUIControl : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;
    [SerializeField]
    RectTransform contentRect;
    [SerializeField]
    RectTransform popUpRect;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    Transform guideTransform;

    public void Set()
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x - 10f);
        contentRect.anchoredPosition = contentRect.anchoredPosition + new Vector2(-10, 0);

        //1. rect는 popUprect의 넓이만큼 조정
        //2. contentRect는 guideTransform.Position 까지 시간에 따라 이동
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Set();
    }
}
