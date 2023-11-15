using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform uiTop;
    [SerializeField]
    private RectTransform uiRight;
    [SerializeField]
    private RectTransform uiDown;

    private Vector2 originPos_uiTop;
    private Vector2 originPos_uiRight;
    private Vector2 originPos_uiDown;

    private Vector2 hidePos_uiTop;
    private Vector2 hidePos_uiRight;
    private Vector2 hidePos_uiDown;

    private Coroutine topCoroutine;
    private Coroutine rightCoroutine;
    private Coroutine downCoroutine;

    public void SetTopUI(bool value, float lerpTime = 0.5f)
    {
        if (topCoroutine != null)
            StopCoroutine(topCoroutine);

        Vector2 targetPos = value ? originPos_uiTop : hidePos_uiTop;
        topCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiTop.transform, targetPos, lerpTime));
    }

    public void SetRightUI(bool value, float lerpTime = 0.5f)
    {
        if (rightCoroutine != null)
            StopCoroutine(rightCoroutine);

        Vector2 targetPos = value ? originPos_uiRight : hidePos_uiRight;
        rightCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiRight.transform, targetPos, lerpTime));
    }

    public void SetDownUI(bool value, float lerpTime = 0.5f)
    {
        if (downCoroutine != null)
            StopCoroutine(downCoroutine);

        Vector2 targetPos = value ? originPos_uiDown : hidePos_uiDown;
        downCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiDown.transform, targetPos, lerpTime));
    }

    public void Init()
    {
        SetTopUI(true, 1f);
        SetRightUI(true, 1f);
        SetDownUI(true, 1f);
    }
    
    private void Awake()
    {
        originPos_uiTop = uiTop.position;
        hidePos_uiTop = originPos_uiTop + new Vector2(0, uiTop.sizeDelta.y + 20);

        originPos_uiRight = uiRight.position;
        hidePos_uiRight = originPos_uiRight + new Vector2(uiRight.sizeDelta.x + 50, 0);

        originPos_uiDown = uiDown.position;
        hidePos_uiDown = originPos_uiDown + new Vector2(0, -uiRight.sizeDelta.y - 20);

        uiTop.position = hidePos_uiTop;
        uiRight.position = hidePos_uiRight;
        uiDown.position = hidePos_uiDown;
    }
}
