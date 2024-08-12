using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSkill : FSMSingleton<FSMSkill>, CharState<Battler>
{
    public void Enter(Battler e)
    {
        //if (e._Animator != null && GameManager.Instance.timeScale != 0)
        //    e._Animator.SetBool("Move", true);
    }

    public void Excute(Battler e)
    {
        if (e.curHp <= 0)
            e.ChangeState(FSMDead.Instance);

        e.UseSkill();
    }

    public void Exit(Battler e)
    {
        //if (e._Animator != null)
        //    e._Animator.SetBool("Move", false);
    }
}
