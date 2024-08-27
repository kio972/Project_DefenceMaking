using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHide : FSMSingleton<FSMHide>, CharState<Battler>
{
    public void Enter(Battler e)
    {
        e._Animator.SetBool("Hide", true);
        //e._Animator.SetBool("Activated", false);
    }

    public void Excute(Battler e)
    {
        if (GameManager.Instance.timeScale == 0)
            return;

        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return;
        }

        if (e is IHide hideable)
            hideable.HideAction();
    }

    public void Exit(Battler e)
    {
        e._Animator.SetBool("Hide", false);
        //e._Animator.SetBool("Activated", true);
    }
}
