using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Civilian : Adventurer
{
    protected override void CollapseLogic()
    {
        if (isCollapsed)
        {
            collapseCool += Time.deltaTime * GameManager.Instance.timeScale;
            if (collapseCool >= 5f)
            {
                collapseCool = 0f;
                GetDamage(curHp + armor + shield, null);
            }
        }
    }
}
