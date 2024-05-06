using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceManaCost : MonoBehaviour, Research
{
    [SerializeField]
    private MonsterType targetType;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.ReduceManaCost(targetType, value);
        DeployUI deployUI = FindObjectOfType<DeployUI>();
        deployUI?.UpdateMana();

    }
}
