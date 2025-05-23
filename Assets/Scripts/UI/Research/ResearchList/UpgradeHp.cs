using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHp : MonoBehaviour, IResearch
{
    [SerializeField]
    private MonsterType targetType;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeHp(targetType, value);
    }
}
