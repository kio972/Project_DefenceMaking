using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilAttackSpeed : MonoBehaviour, Research
{
    [SerializeField]
    private float targetSpeed;

    public void ActiveResearch()
    {
        GameManager.Instance.king.attackCoolTime = targetSpeed;
    }
}
