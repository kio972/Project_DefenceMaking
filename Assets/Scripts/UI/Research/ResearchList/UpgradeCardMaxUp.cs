using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardMaxUp : MonoBehaviour, IResearch
{
    [SerializeField]
    private int value = 1;

    public void ActiveResearch()
    {
        GameManager.Instance.cardDeckController.IncreaseMaxCount(value);
    }
}