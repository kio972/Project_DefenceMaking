using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMRoom : FSMSingleton<FSMRoom>, CharState<Battler>
{
    /*excute : curTarget���� ���� ����
    out case
    0. hp�� 0���Ϸ� ����������� -> Dead��
    1. curTarget�� ����Ѱ�� -> Patrol��
    2. curTarget�� ��Ÿ����� �־������ -> Patrol��
    3. Attack������ ��������� -> Patrol�� -> Attack���� */

    private bool NeedChange(Battler e)
    {
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }

        //if()

        return false;
    }

    public void Enter(Battler e)
    {

    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;
    }

    public void Exit(Battler e)
    {

    }
}