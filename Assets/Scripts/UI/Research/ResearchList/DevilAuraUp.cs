using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAuraUp : MonoBehaviour, Research
{
    [SerializeField]
    private int range = 1;
    [SerializeField]
    private int value = 20;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeDevilAura(range, value);
    }
}