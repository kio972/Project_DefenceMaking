using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMDead : FSMSingleton<FSMDead>, CharState<Battler>
{
    public void Enter(Battler e)
    {
        e._Animator?.SetBool("Die", true);
    }

    public void Excute(Battler e)
    {
        
    }

    public void Exit(Battler e)
    {
        e._Animator?.SetBool("Die", false);
    }
}
