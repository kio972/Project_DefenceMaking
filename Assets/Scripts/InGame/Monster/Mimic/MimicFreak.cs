using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class MimicFreak : MimicRecover
{
    private readonly float freakRange = 1f;

    public override void HideAction()
    {
        Battler curTarget = BattleCheck();
        if (curTarget != null)
        {
            chaseTarget = curTarget;
            FreakEnemy().Forget();
        }
    }

    public override void Init()
    {
        base.Init();
        curSeduceCount = 0;
    }

    private async UniTaskVoid FreakEnemy()
    {
        ChangeState(FSMPatrol.Instance);
        float curMovespeed = moveSpeed;
        moveSpeed = 0;

        List<Battler> rangedTargets = GetRangedTargets(transform.position, freakRange, false);
        foreach (Battler target in rangedTargets)
        {
            target.KnockBack(this, UtilHelper.CheckClosestDirection(target.transform.position - transform.position), 0.5f, 60);
        }
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Reveal"));
        await UniTask.WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag("Reveal"));
        moveSpeed = curMovespeed;
        ChangeState(FSMChase.Instance);
    }
}
