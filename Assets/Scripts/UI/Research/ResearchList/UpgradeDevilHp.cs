using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilHp : MonoBehaviour, IResearch
{
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        GameManager.Instance.king.curMaxHp += value;
        GameManager.Instance.king.curHp += value;
    }
}
