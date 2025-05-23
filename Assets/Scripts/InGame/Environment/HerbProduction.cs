using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HerbProduction : Environment
{
    [SerializeField]
    private HerbType targetHerb;

    protected override void CustomFunc()
    {
        PassiveManager.Instance.UpgradeHerb(targetHerb, (int)value);
    }
}
