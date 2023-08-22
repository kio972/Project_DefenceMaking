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
        if (e._Animator != null)
            e._Animator.SetBool("Move", true);
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
        if (e._Animator != null)
            e._Animator.SetBool("Move", false);
    }
}