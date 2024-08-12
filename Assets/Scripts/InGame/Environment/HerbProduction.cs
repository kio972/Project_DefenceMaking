using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HerbProduction : Environment
{
    [SerializeField]
    private int targetHerb;

    CancellationTokenSource cancellationToken = new CancellationTokenSource();

    private void OnDestroy()
    {
        cancellationToken.Cancel();
        cancellationToken.Dispose();
    }

    private async UniTaskVoid GetHerb(int targetHerb, int value)
    {
        int prevWave = GameManager.Instance.CurWave;
        while (true)
        {
            if(GameManager.Instance.CurWave > prevWave)
            {
                int waveCount = GameManager.Instance.CurWave - prevWave;
                if (targetHerb == 1)
                    GameManager.Instance.herb1 += value * waveCount;
                else if(targetHerb == 2)
                    GameManager.Instance.herb2 += value * waveCount;
                else if (targetHerb == 3)
                    GameManager.Instance.herb3 += value * waveCount;

                prevWave = GameManager.Instance.CurWave;
            }

            await UniTask.Yield(cancellationToken.Token);
        }
    }

    protected override void CustomFunc()
    {
        GetHerb(targetHerb, (int)value).Forget();
    }
}
