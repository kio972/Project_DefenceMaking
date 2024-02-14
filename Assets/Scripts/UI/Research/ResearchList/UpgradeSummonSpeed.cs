using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSummonSpeed : MonoBehaviour, Research
{
    [SerializeField]
    private MonsterType targetType;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeSummonSpeed(targetType, value);
    }
}
