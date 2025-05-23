using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MouseOverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CancellationTokenSource cts = new CancellationTokenSource();

    [SerializeField]
    private float mouseOverTime = 0.5f;

    private bool isMouseEntered = false;

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isMouseEntered)
            return;

        cts?.Cancel();
        cts = new CancellationTokenSource();
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(false);
        isMouseEntered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1�� �� ���� ǥ��
        ShowTooltipAfterDelay(cts.Token).Forget();
        isMouseEntered = true;
    }

    private async UniTaskVoid ShowTooltipAfterDelay(CancellationToken token)
    {
        await UniTask.Delay((int)(mouseOverTime * 1000), cancellationToken: token);
        if (!token.IsCancellationRequested)
        {
            SetText(); // ������ �ؽ�Ʈ ����
            GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(true);
        }
    }

    public abstract void SetText();

    protected virtual void OnDisable()
    {
        OnPointerExit(null);
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
