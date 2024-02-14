using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDestuctIncome : MonoBehaviour, Research
{
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeTileDestructInCome(value);
    }
}
