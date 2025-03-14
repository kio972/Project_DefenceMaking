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

    public void OnPointerExit(PointerEventData eventData)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1초 후 툴팁 표시
        ShowTooltipAfterDelay(cts.Token).Forget();
    }

    private async UniTaskVoid ShowTooltipAfterDelay(CancellationToken token)
    {
        await UniTask.Delay((int)(mouseOverTime * 1000), cancellationToken: token);
        if (!token.IsCancellationRequested)
        {
            SetText(); // 툴팁에 텍스트 설정
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
