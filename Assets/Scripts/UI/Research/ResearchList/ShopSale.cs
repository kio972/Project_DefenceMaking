using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSale : MonoBehaviour, Research
{
    [SerializeField]
    private int value = 10;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeShopSale(value);
    }
}