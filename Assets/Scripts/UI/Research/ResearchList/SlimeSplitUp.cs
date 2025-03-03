using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitUp : MonoBehaviour, IResearch
{
    [SerializeField]
    private int value = 1;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeSlimeSplit(value);
    }
}