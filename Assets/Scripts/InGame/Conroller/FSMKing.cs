using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMKing : FSMSingleton<FSMKing>, CharState<Battler>
{
    /*excute : 마왕방을 향해 탐색진행
    out case
    0. hp가 0이하로 떨어졌을경우 -> Dead로
    1.사거리 안에 적이 들어온 경우 -> Attack으로, curTarget에 적 저장 */

    private bool NeedChange(Battler e)
    {
        //0
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
            e.curTarget = curTarget;
            e.ChangeState(FSMAttack.Instance);
            return true;
        }

        return false;
    }

    public void Enter(Battler e)
    {

    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        if (AttackCheck(e))
            return;

    }

    public void Exit(Battler e)
    {
        if (e._Animator != null)
            e._Animator.SetBool("Move", false);
    }
}
