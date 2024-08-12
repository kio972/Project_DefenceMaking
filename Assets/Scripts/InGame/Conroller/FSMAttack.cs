using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMAttack : FSMSingleton<FSMAttack>, CharState<Battler>
{
    /*excute : curTarget���� ���� ����
    out case
    0. hp�� 0���Ϸ� ����������� -> Dead��
    1. curTarget�� ����Ѱ�� -> Patrol��
    2. curTarget�� ��Ÿ����� �־������ -> Patrol��
    3. Attack������ ��������� -> Patrol�� */

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

        if (e.curTarget != null)
            e.RotateCharacter(e.curTarget.transform.position);
    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        e.UpdateAttackSpeed();

        if (!e._Animator.GetCurrentAnimatorStateInfo(0).IsTag("ATTACK") && !e._Animator.IsInTransition(0))
        {
            e._Animator.ResetTrigger("Attack");
            e.ChangeState(e.PrevState);
        }
    }

    public void Exit(Battler e)
    {
        e.curTarget = null;
        e.curAttackCoolTime = e.attackCoolTime;
    }
}
