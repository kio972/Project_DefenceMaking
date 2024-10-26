using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefMaster : ThiefVeteran
{
    private float patrolTime = 0;
    private readonly float recoverTime = 180f;

    private async UniTaskVoid RecoverStealth()
    {
        while(!isDead)
        {
            await UniTask.WaitUntil(() => (object)CurState != FSMHide.Instance, default, unitaskCancelTokenSource.Token);

            while(!isDead && patrolTime < recoverTime)
            {
                if ((object)CurState == FSMPatrol.Instance)
                    patrolTime += GameManager.Instance.InGameDeltaTime;
                else
                    patrolTime = 0;

                await UniTask.Yield(unitaskCancelTokenSource.Token);
            }

            patrolTime = 0;
            ChangeState(FSMHide.Instance);
            AddStatusEffect<Stealth_sup>(new Stealth_sup(this, 0));
        }
    }

    public override void Init()
    {
        base.Init();
        RecoverStealth().Forget();
    }
}
