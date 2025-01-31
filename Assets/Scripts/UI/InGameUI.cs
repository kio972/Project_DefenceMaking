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
    [SerializeField]
    private RectTransform uiManageMentBtns;

    [SerializeField]
    private RectTransform uiRight2;
    [SerializeField]
    private BloodEffect bloodEffect;

    private Vector2 originPos_uiTop;
    private Vector2 originPos_uiRight;
    private Vector2 originPos_uiDown;
    private Vector2 originPos_uiManage;

    private Vector2 hidePos_uiTop;
    private Vector2 hidePos_uiRight;
    private Vector2 hidePos_uiDown;
    private Vector2 hidePos_uiManage;

    private Coroutine topCoroutine;
    private Coroutine rightCoroutine;
    private Coroutine downCoroutine;

    private Coroutine right2Coroutine;

    private Coroutine manageCoroutine;

    public bool rightUILock = false;

    public void StartBloodEffect()
    {
        bloodEffect?.StartBloodEffect();
    }

    public void HideRightBtns(bool value = true)
    {
        uiRight.gameObject.SetActive(!value);
    }

    public void SwitchRightToTileUI(bool value, float lerpTime = 0.2f)
    {
        if(value)
            SetRightUI(false, lerpTime, () => { SetTileUI(true, lerpTime); });
        else
            SetTileUI(false, lerpTime, () => { if (rightUILock) return; SetRightUI(true, lerpTime); });
    }

    public void SetTileUI(bool value, float lerpTime = 0.5f, System.Action callBack = null)
    {
        if (right2Coroutine != null)
            StopCoroutine(right2Coroutine);

        Vector2 targetPos = value ? originPos_uiRight : hidePos_uiRight;
        right2Coroutine = StartCoroutine(UtilHelper.IMoveEffect(uiRight2, targetPos, lerpTime, callBack));
    }

    public void SetTopUI(bool value, float lerpTime = 0.5f)
    {
        if (topCoroutine != null)
            StopCoroutine(topCoroutine);

        Vector2 targetPos = value ? originPos_uiTop : hidePos_uiTop;
        topCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiTop, targetPos, lerpTime));
    }

    public void SetRightUI(bool value, float lerpTime = 0.5f, System.Action callBack = null)
    {
        if (rightCoroutine != null)
            StopCoroutine(rightCoroutine);

        Vector2 targetPos = value ? originPos_uiRight : hidePos_uiRight;
        rightCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiRight, targetPos, lerpTime, callBack));
    }

    public void SetDownUI(bool value, float lerpTime = 0.5f)
    {
        if (downCoroutine != null)
            StopCoroutine(downCoroutine);

        Vector2 targetPos = value ? originPos_uiDown : hidePos_uiDown;
        downCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiDown, targetPos, lerpTime));
    }

    public void SetManageUI(bool value, float lerpTime = 0.5f, System.Action callBack = null)
    {
        if (manageCoroutine != null)
            StopCoroutine(manageCoroutine);

        Vector2 targetPos = value ? originPos_uiManage : hidePos_uiManage;
        manageCoroutine = StartCoroutine(UtilHelper.IMoveEffect(uiManageMentBtns, targetPos, lerpTime, callBack));
    }

    public void Init(bool playAnim = true)
    {
        if (playAnim)
        {
            SetTopUI(true, 1f);
            SetRightUI(true, 1f);
            SetDownUI(true, 1f);
            SetManageUI(true, 1f);
        }
        else
        {
            uiTop.anchoredPosition = originPos_uiTop;
            uiRight.anchoredPosition = originPos_uiRight;
            uiDown.anchoredPosition = originPos_uiDown;
        }
    }
    
    private void Awake()
    {
        originPos_uiTop = uiTop.anchoredPosition;
        hidePos_uiTop = originPos_uiTop + new Vector2(0, uiTop.sizeDelta.y + 50);

        originPos_uiRight = uiRight.anchoredPosition;
        hidePos_uiRight = originPos_uiRight + new Vector2(uiRight.sizeDelta.x + 50, 0);

        originPos_uiDown = uiDown.anchoredPosition;
        hidePos_uiDown = originPos_uiDown + new Vector2(0, -uiRight.sizeDelta.y - 20);

        originPos_uiManage = uiManageMentBtns.anchoredPosition;
        hidePos_uiManage = originPos_uiManage + new Vector2(uiRight.sizeDelta.x + 50, 0);

        uiTop.anchoredPosition = hidePos_uiTop;
        uiRight.anchoredPosition = hidePos_uiRight;
        uiDown.anchoredPosition = hidePos_uiDown;

        uiRight2.anchoredPosition = hidePos_uiRight;

        uiManageMentBtns.anchoredPosition = hidePos_uiManage;
    }
}
