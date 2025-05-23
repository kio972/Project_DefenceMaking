using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Executor : Adventurer
{
    public void SetBribeBtn(int cost)
    {
        BribeBtn bribe = hpBar.GetComponentInChildren<BribeBtn>(true);
        bribe.Init(this, cost);
    }
}
