using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum FogState
{
    Closed,
    Half,
    Opened,
}

public class Fog : MonoBehaviour
{
    private FogState _state;

    private CancellationTokenSource cacelTokensource = new CancellationTokenSource();

    public void SetFog(FogState state)
    {
        if ((int)state <= (int)_state)
            return;

        cacelTokensource.Cancel();
        cacelTokensource = new CancellationTokenSource();
        float curAlpha = _state == FogState.Closed ? 1f : _state == FogState.Half ? 0.8f : 0f;
        if (state == FogState.Closed)
            OpenFog(1, curAlpha).Forget();
        else if (state == FogState.Half)
            OpenFog(0.8f, curAlpha).Forget();
        else
            OpenFog(0f, curAlpha).Forget();

        _state = state;
    }

    public async UniTaskVoid OpenFog(float targetAlpha, float curAlpha)
    {
        gameObject.SetActive(true);
        const float lerpTime = 0.5f;
        float elapsedTime = 0f;
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float nextAlpha = Mathf.Lerp(curAlpha, targetAlpha, elapsedTime / lerpTime);
            SetFogAlpha(nextAlpha);
            await UniTask.Yield(cacelTokensource.Token);
        }

        SetFogAlpha(targetAlpha);
        if(targetAlpha == 0f)
            gameObject.SetActive(false);
    }

    public void SetFogAlpha(float alpha)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Color color = renderer.material.GetColor("_BaseColor");
        renderer.material.SetColor("_BaseColor", new Color(color.r, color.g, color.b, alpha));
    }
}
