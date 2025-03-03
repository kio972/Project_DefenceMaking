using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSale : MonoBehaviour, IResearch
{
    [SerializeField]
    private int value = 10;

    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeShopSale(value);
    }
}