using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilAttackSpeed : MonoBehaviour, IResearch
{
    [SerializeField]
    private float targetSpeed;

    public void ActiveResearch()
    {
        GameManager.Instance.king.attackCoolTime = targetSpeed;
    }
}
