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
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f));

            if (PassiveManager.Instance.devilAuraRange == 0 || PassiveManager.Instance.devilAuraPower == 0)
                continue;

            foreach(Monster monster in GameManager.Instance._MonsterList)
            {
                if (monster.HaveEffect<DevilAura>())
                    continue;

                float dist = UtilHelper.CalCulateDistance(transform, monster.transform);
                if (dist > PassiveManager.Instance.devilAuraRange)
                    continue;

                monster.AddStatusEffect<DevilAura>(new DevilAura(monster, 0, transform));
            }
        }
    }


    public override void Init()
    {
        base.Init();

        DevilAuraEffect().Forget();
    }

}
