using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTileSpeed : MonoBehaviour, Research
{
    [SerializeField]
    private TileType targetType;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeTileSpeed(targetType, value);
    }
}