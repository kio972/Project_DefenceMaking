using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeCondition : MonoBehaviour
{
    private bool isActived = false;

    [SerializeField]
    private GameObject targetPrefab;

    private bool IsConditionPassed()
    {
        int spawnerCount = 0;
        bool slime_normal = false;
        bool slime_poison = false;
        bool slime_explosion = false;

        foreach(var spawner in GameManager.Instance.monsterSpawner)
        {
            if(spawner._TargetName == "slime_mucus")
            {
                spawnerCount++;
                slime_normal = true;
            }
            else if(spawner._TargetName == "slime_poison")
            {
                spawnerCount++;
                slime_poison = true;
            }
            else if(spawner._TargetName == "slime_explosion")
            {
                spawnerCount++;
                slime_explosion = true;
            }
        }

        if (spawnerCount < 5)
            return false;

        bool haveAllSpawner = slime_normal && slime_poison && slime_explosion;
        return haveAllSpawner;
    }

    async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsInit, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        
        foreach(var monster in GameManager.Instance.monsterList)
        {
            if (monster.BattlerID == "s_m10004")
            {
                isActived = true;
                return;
            }
        }

        await UniTask.WaitUntil(() => IsConditionPassed(), cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        NodeManager.Instance.hiddenPrioritys.Enqueue(targetPrefab);
    }
}
