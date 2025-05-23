using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMonster : MonoBehaviour, IResearch
{
    [SerializeField]
    private string[] targetId;

    public void ActiveResearch()
    {
        foreach (string id in targetId)
            PassiveManager.Instance.RemoveDeployData(id);
    }
}
