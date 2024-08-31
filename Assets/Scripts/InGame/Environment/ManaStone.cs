using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStone : Environment
{
    protected override void CustomFunc()
    {
        PassiveManager.Instance.AddManaStone(_CurNode, (int)value);
    }
}
