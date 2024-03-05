using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMReturn : FSMSingleton<FSMReturn>, CharState<Battler>
{
    public void Enter(Battler e)
    {
        e._Animator?.SetTrigger("Return");
    }

    public void Excute(Battler e)
    {
        
    }

    public void Exit(Battler e)
    {
        
    }
}
