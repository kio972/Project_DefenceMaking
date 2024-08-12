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
            ChangeState(FSMPatrol.Instance);
            FreakEnemy();
        }
    }

    public override void Init()
    {
        base.Init();
        curSeduceCount = 0;
    }

    private void FreakEnemy()
    {
        List<Battler> rangedTargets = GetRangedTargets(transform.position, freakRange, false);
        foreach (Battler target in rangedTargets)
        {
            target.GetCC(this, 30f);
            target.ChangeState(FSMCC.Instance);
        }
    }
}
