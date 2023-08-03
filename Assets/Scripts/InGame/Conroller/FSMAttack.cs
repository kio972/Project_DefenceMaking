using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMAttack : FSMSingleton<FSMAttack>, CharState<Battler>
{
    /*excute : curTarget에게 공격 실행
    out case
    0. hp가 0이하로 떨어졌을경우 -> Dead로
    1. curTarget이 사망한경우 -> Patrol로
    2. curTarget이 사거리보다 멀어진경우 -> Patrol로
    3. Attack수행이 끝났을경우 -> Patrol로 */

    private bool NeedChange(Battler e)
    {
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }

        return false;
    }

    public void Enter(Battler e)
    {
        e.Play_AttackAnimation();
    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        e.UpdateAttackSpeed();

        if (!e._Animator.GetCurrentAnimatorStateInfo(0).IsTag("ATTACK") && !e._Animator.IsInTransition(0))
        {
            e._Animator.ResetTrigger("Attack");
            e.ChangeState(FSMPatrol.Instance);
        }
    }

    public void Exit(Battler e)
    {
        e.curTarget = null;
    }
}
