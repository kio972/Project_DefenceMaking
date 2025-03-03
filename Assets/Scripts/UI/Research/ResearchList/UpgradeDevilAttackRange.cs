using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilAttackRange : MonoBehaviour, IResearch
{
    [SerializeField]
    private float value;

    public void ActiveResearch()
    {
        GameManager.Instance.king.attackRange += value;
    }
}
