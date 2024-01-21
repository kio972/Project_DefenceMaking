using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    private Transform curTarget;
    private Coroutine moveCoroutine = null;

    public float moveSpeed = 2f;

    private IEnumerator ModifyPositon(Transform origin, Transform moveTarget, Vector3 targetPos, float moveSpeed = 1f)
    {
        //움직일 방향 도출
        float direction = targetPos.x - origin.transform.position.x;
        Vector3 movedir = Vector3.left;
        if (direction > 0)
            movedir = Vector3.right;
        else if (direction == 0)
            yield break;

        while(true)
        {
            if (direction > 0 && origin.transform.position.x >= targetPos.x)
                break;
            if (direction < 0 && origin.transform.position.x <= targetPos.x)
                break;

            moveTarget.transform.Translate(movedir * Time.deltaTime * 1000 * moveSpeed);
            yield return null;
        }

        yield return null;
    }

    private void SetPopUp(bool value)
    {
        if (popUpRect.gameObject.activeSelf == value)
            return;

        float modifySize = popUpRect.sizeDelta.x;
        if (value)
            modifySize = -modifySize;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x + modifySize);
        popUpRect.gameObject.SetActive(value);
    }

    private bool IsMoveAvail(Transform target)
    {
        if (target == null)
            return false;

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

            StartCoroutine(ModifyPositon(target, contentRect.transform, guideTransform.position, moveSpeed));
        }
    }
}
