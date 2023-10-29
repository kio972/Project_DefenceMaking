using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMKing : FSMSingleton<FSMKing>, CharState<Battler>
{
    /*excute : ���չ��� ���� Ž������
    out case
    0. hp�� 0���Ϸ� ����������� -> Dead��
    1.��Ÿ� �ȿ� ���� ���� ��� -> Attack����, curTarget�� �� ���� */

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