using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

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
    [SerializeField]
    Transform originTransform;

    private Transform curTarget;
    private Coroutine moveCoroutine = null;

    public float moveSpeed = 2f;

    private void SetPopUp(bool value)
    {
        if (popUpRect.gameObject.activeSelf == value)
            return;

        //float modifySize = popUpRect.sizeDelta.x;
        //if (value)
        //    modifySize = -modifySize;
        //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x + modifySize);
        popUpRect.gameObject.SetActive(value);
    }

    private bool IsMoveAvail(Transform target)
    {
        if (target == null)
            return true;

        if (target.position.x <= guideTransform.position.x)
            return false;

        return true;
    }

    public void SetUI(Transform target = null)
    {
        if (curTarget == target)
            return;

        curTarget = target;
        SetPopUp(target != null);

        if(IsMoveAvail(target))
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            Vector3 targetPos = originTransform.position;
            if (target != null)
            {
                float direction = guideTransform.position.x - target.transform.position.x;
                targetPos = contentRect.transform.position;
                targetPos.x += direction;
            }

            moveCoroutine = StartCoroutine(UtilHelper.MoveToTargetPos(contentRect, targetPos, 0.2f));
        }
    }
}
