using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using Cysharp.Threading.Tasks;


public class Test : MonoBehaviour
{
    public int endWave = 20;
    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsInit);

        await UniTask.WaitUntil(() => GameManager.Instance.CurWave >= endWave);
        int curCount = GameManager.Instance.adventurersList.Count;
        while(true)
        {
            if (GameManager.Instance.adventurersList.Count > curCount)
                break;
            curCount = GameManager.Instance.adventurersList.Count;
            await UniTask.Yield();
        }

        await UniTask.WaitUntil(() => GameManager.Instance.adventurersList.Count <= 0);
        GameManager.Instance.WinGame();
    }
}
