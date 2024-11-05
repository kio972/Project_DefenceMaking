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

public interface IManaSupply
{
    int manaValue { get; }
}

public interface ISpeedModify
{
    float speedRate { get; }
}

public class Environment : MonoBehaviour
{
    [SerializeField]
    private EnvironmentType environmentType;

    [SerializeField]
    protected float value;

    private TileNode curNode;
    public TileNode _CurNode { get => curNode; }

    protected virtual void CustomFunc() { }

    public void Init(TileNode node)
    {
        transform.SetParent(node.transform, false);
        transform.position = node.transform.position;
        NodeManager.Instance.SetTile(this);
        NodeManager.Instance.SetActiveNode(node, true);
        curNode = node;
        curNode.environment = this;
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
            case EnvironmentType.custom:
                CustomFunc();
                break;
        }

        NodeManager.Instance.AddSightNode(curNode);
    }
}
