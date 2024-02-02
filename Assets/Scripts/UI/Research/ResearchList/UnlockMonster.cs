using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockMonster : MonoBehaviour, Research
{
    [SerializeField]
    private string targetId;

    public void ActiveResearch()
    {
        PassiveManager.Instance.AddDeployData(targetId);
    }
}
