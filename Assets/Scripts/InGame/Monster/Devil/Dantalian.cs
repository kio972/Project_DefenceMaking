using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Dantalian : PlayerBattleMain
{
    private async UniTaskVoid DevilAuraEffect()
    {
        while(!isDead)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

            if (PassiveManager.Instance.devilAuraRange == 0 || PassiveManager.Instance.devilAuraPower == 0)
                continue;

            foreach(Monster monster in GameManager.Instance._MonsterList)
            {
                float dist = UtilHelper.CalCulateDistance(transform, monster.transform);
                if (dist > PassiveManager.Instance.devilAuraRange)
                    continue;
            }
        }
    }


    public override void Init()
    {
        base.Init();

        DevilAuraEffect().Forget();
    }

}
