using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearSetting : MonoBehaviour
{
    [SerializeField]
    private int endWave;

    async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.CurWave >= endWave, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitUntil(() => GameManager.Instance.Timer >= 720, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitUntil(() => GameManager.Instance.adventurersList.Count == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        GameManager.Instance.WinGame();
    }
}