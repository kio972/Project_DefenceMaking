using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHide : FSMSingleton<FSMHide>, CharState<Battler>
{
    /*excute : 가만히있기
    out case
    0. hp가 0이하로 떨어졌을경우 -> Dead로
    1. 사거리 안에 적이 들어온 경우 -> Attack으로
    1.1 사거리 안의 적에게 CC(놀람) 부여 */

    private bool NeedChange(Battler e)
    {
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }

        return false;
    }

    private bool AttackCheck(Battler e)
    {
        //1.1
        Battler curTarget = e.BattleCheck();
        if (curTarget != null)
        {
            e.ChangeState(FSMPatrol.Instance);
            return true;
        }

        //1.2
        if (e.chaseTarget != null)
        {
            e.ChangeState(FSMChase.Instance);
            return true;
        }
        return false;
    }

    public void Enter(Battler e)
    {
        
    }

    public void Excute(Battler e)
    {
        if (GameManager.Instance.timeScale == 0)
            return;

        if (NeedChange(e))
            return;

        if (AttackCheck(e))
            return;
    }

    public void Exit(Battler e)
    {
        e._Animator.SetTrigger("Activated");
        List<Battler> rangedTargets = e.GetRangedTargets(e.transform.position, e.attackRange + 0.2f);
        foreach(Battler target in rangedTargets)
        {
            target.GetCC(e, 1f);
            target.ChangeState(FSMCC.Instance);
        }
    }
}
