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

public interface IStatModifier
{
    //StatType statType { get; }

    float modifyValue { get; }
}

public class Environment : MonoBehaviour, ITileKind, IStatObject
{
    [SerializeField]
    private EnvironmentType environmentType;

    [SerializeField]
    protected float value;

    private TileNode _curNode;
    public TileNode curNode { get => _curNode; }

    [SerializeField]
    FMODUnity.EventReference buildSound;

    protected virtual void CustomFunc() { }

    public void Init(TileNode node)
    {
        transform.SetParent(node.transform, false);
        transform.position = node.transform.position;
        NodeManager.Instance.SetTile(this);
        NodeManager.Instance.SetActiveNode(node, true);
        _curNode = node;
        _curNode.tileKind = this;
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
                break;
        }
        CustomFunc();
        NodeManager.Instance.AddSightNode(_curNode);
        if(GameManager.Instance.IsInit)
            FMODUnity.RuntimeManager.PlayOneShot(buildSound);
    }

    public string GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Mana:
                return $"{0}";
            case StatType.BuffTexts:
                return null;
            default:
                return null;
        }
    }
}
