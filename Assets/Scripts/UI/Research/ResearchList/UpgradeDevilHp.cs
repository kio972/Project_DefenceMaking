using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilHp : MonoBehaviour, Research
{
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        GameManager.Instance.king.maxHp += value;
        GameManager.Instance.king.curHp += value;
    }
}
