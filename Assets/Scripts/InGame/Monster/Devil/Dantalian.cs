using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UniRx.Triggers;

public class Dantalian : PlayerBattleMain
{
    protected override void RemoveOutCaseTargets(List<Battler> targets)
    {
        if (PassiveManager.Instance.devilDetection)
        {
            List<Battler> removeList = new List<Battler>();
            foreach (Battler battler in rangedTargets)
            {
                if (battler.isDead || !targets.Contains(battler))
                    removeList.Add(battler);
            }

            foreach (Battler battler in removeList)
                rangedTargets.Remove(battler);
        }
        else
            base.RemoveOutCaseTargets(targets);
    }

    private async UniTaskVoid DetectCheck()
    {
        await UniTask.WaitUntil(() => PassiveManager.Instance.devilDetection, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        AddStatusEffect<Detect>(new Detect(this, 0));
    }

    private async UniTaskVoid DevilAuraEffect()
    {
        while(!isDead)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f), false, cancellationToken : gameObject.GetCancellationTokenOnDestroy());

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
        DetectCheck().Forget();
    }

}
