using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollRect ������Ʈ
    public RectTransform content; // ScrollRect�� Content
    public RectTransform viewport; // ScrollRect�� Viewport

    public float upPadding = 80;
    public float downPadding = 50;
    /// <summary>
    /// Ư�� �������� ����Ʈ ���� ��ũ���մϴ�.
    /// </summary>
    /// <param name="target">��� Content�� RectTransform</param>
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