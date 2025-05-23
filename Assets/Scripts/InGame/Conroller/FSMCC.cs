using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMCC : FSMSingleton<FSMCC>, CharState<Battler>
{
    /*excute : CC�ð����� �������ֱ�
    out case
    1. ��� -> FSMDead��
    2. �ð� ���� -> prevState�� */

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
        e._Animator.SetTrigger("CC");
    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        if (e.CCEscape())
            e.ChangeState(FSMPatrol.Instance);
    }

    public void Exit(Battler e)
    {
        e._Animator?.SetBool("Move", GameManager.Instance.timeScale != 0);
    }
}
