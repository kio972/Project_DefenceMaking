using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemIron : Golem
{
    public override void Init()
    {
        base.Init();
        shield += Mathf.RoundToInt(maxHp * 0.2f);
    }
}
