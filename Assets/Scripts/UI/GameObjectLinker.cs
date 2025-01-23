using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectLinker : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> targetObjects;

    private bool IsActive
    {
        get
        {
            foreach (var obj in targetObjects)
            {
                if (obj.activeInHierarchy)
                    return true;
            }
            return false;
        }
    }

    private async UniTaskVoid StartLink()
    {
        while (true)
        {
            gameObject.SetActive(IsActive);

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

    }

    private void Start()
    {
        if (targetObjects == null || targetObjects.Count == 0)
            return;

        StartLink().Forget();
    }
}
