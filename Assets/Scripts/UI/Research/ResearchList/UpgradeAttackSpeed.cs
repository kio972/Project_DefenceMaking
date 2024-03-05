using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAttackSpeed : MonoBehaviour, Research
{
    [SerializeField]
    private MonsterType targetType;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeAttackSpeed(targetType, value);
    }
}
