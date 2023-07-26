using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMChase : FSMSingleton<FSMChase>, CharState<Battler>
{
    /*enter : 현재 타일을 chaseStartTile 저장
    excute : curTarget을 향해 이동
    out case
    0. hp가 0이하로 떨어졌을경우 -> Dead로
    1. curTarget이 사망한경우
    2. curTarget이 사거리 안에 들어온 경우 -> Attack으로 */
    private bool AttackCheck(Battler e)
    {
        //1.1
        Battler curTarget = e.BattleCheck();
        if (curTarget != null)
        {
            e.ChangeState(FSMAttack.Instance);
            e.curTarget = curTarget;
            return true;
        }

        return false;
    }

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

    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        if (AttackCheck(e))
            return;

        e.Chase();
    }

    public void Exit(Battler e)
    {

    }
}
