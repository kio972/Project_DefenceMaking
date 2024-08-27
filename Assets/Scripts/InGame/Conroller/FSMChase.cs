using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMChase : FSMSingleton<FSMChase>, CharState<Battler>
{
    /*enter : ���� Ÿ���� chaseStartTile ����
    excute : curTarget�� ���� �̵�
    out case
    0. hp�� 0���Ϸ� ����������� -> Dead��
    1. curTarget�� ����Ѱ��
    2. curTarget�� ��Ÿ� �ȿ� ���� ��� -> Attack���� */
    private bool AttackCheck(Battler e)
    {
        //1.1
        Battler curTarget = e.BattleCheck(false);
        if (curTarget != null)
        {
            e.curTarget = curTarget;
            e.ChangeState(FSMAttack.Instance);
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

        if(e.chaseTarget == null || e.chaseTarget.isDead)
        {
            e.chaseTarget = null;
            e.ChangeState(FSMPatrol.Instance);
            return;
        }

        e.Chase();
    }

    public void Exit(Battler e)
    {
        
    }
}
