using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollRect 컴포넌트
    public RectTransform content; // ScrollRect의 Content
    public RectTransform viewport; // ScrollRect의 Viewport

    public float upPadding = 80;
    public float downPadding = 50;
    /// <summary>
    /// 특정 컨텐츠를 뷰포트 내로 스크롤합니다.
    /// </summary>
    /// <param name="target">대상 Content의 RectTransform</param>
    public void ScrollToContent(RectTransform target)
    {
        float viewMin = viewport.position.y - upPadding;
        float viewMax = viewport.position.y - viewport.rect.height + downPadding;

        float contentPosY = target.position.y;
        float modifyY = 0;

        if (viewMax > contentPosY)
            modifyY = viewMax - contentPosY;
        else if (viewMin < contentPosY)
            modifyY = viewMin - contentPosY;
        else
            return;

        float modifyValue = modifyY / (content.rect.height - viewport.rect.height);

        scrollRect.inertia = false;
        scrollRect.verticalNormalizedPosition -= modifyValue;
        scrollRect.inertia = true;
    }
}