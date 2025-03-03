using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPassive : MonoBehaviour, IResearch
{
    [SerializeField]
    private string targetPassiveKey;
    [SerializeField]
    private bool value = true;

    public void ActiveResearch()
    {
        if (string.IsNullOrEmpty(targetPassiveKey))
            return;
        PassiveManager.Instance.SetUnLockState(targetPassiveKey, value);
    }
}
