using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType
{
    custom,
    monsterHp,
    income,
    slow,
}

public class Environment : MonoBehaviour
{
    [SerializeField]
    private EnvironmentType environmentType;

    [SerializeField]
    private float value;

    private TileNode curNode;
    public TileNode _CurNode { get => curNode; }

    protected virtual void CustomFunc() { }

    public void Init(TileNode node)
    {
        transform.position = node.transform.position;
        NodeManager.Instance.SetTile(this);
        curNode = node;
        switch(environmentType)
        {
            case EnvironmentType.monsterHp:
                PassiveManager.Instance.monsterHp_Weight += (int)value;
                break;
            case EnvironmentType.income:
                PassiveManager.Instance.income_Weight += (int)value;
                break;
            case EnvironmentType.slow:
                PassiveManager.Instance.slowedTile.Add(node, value);
                break;
        }
    }
}
