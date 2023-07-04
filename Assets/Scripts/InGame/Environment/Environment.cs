using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType
{
    custom,
    monsterHp,
    income,
}

public class Environment : MonoBehaviour
{
    [SerializeField]
    private EnvironmentType environmentType;

    [SerializeField]
    private float value;

    private TileNode curNode;

    public void Init(TileNode node)
    {
        curNode = node;
        switch(environmentType)
        {
            case EnvironmentType.monsterHp:
                PassiveManager.Instance.monsterHp_Weight += (int)value;
                break;
            case EnvironmentType.income:
                PassiveManager.Instance.income_Weight += (int)value;
                break;
        }
    }
}
