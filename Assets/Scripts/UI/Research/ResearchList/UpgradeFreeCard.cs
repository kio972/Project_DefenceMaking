using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFreeCard : MonoBehaviour, IResearch
{
    [SerializeField]
    private int value = 5;

    public void ActiveResearch()
    {
        GameManager.Instance.cardDeckController.SetFreeCount(value);
    }
}