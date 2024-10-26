using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMDirectMove : FSMSingleton<FSMDirectMove>, CharState<Battler>
{
    public void Enter(Battler e)
    {
        e.NextTile = null;
    }

    public void Excute(Battler e)
    {
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return;
        }

        if(e.CurTile == e.directPassNode)
        {
            e.ChangeState(FSMPatrol.Instance);
            return;
        }

        e.DirectPass();
    }

    public void Exit(Battler e)
    {
        
    }
}
