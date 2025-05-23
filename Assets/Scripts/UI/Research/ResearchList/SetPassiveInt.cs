using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPassiveInt : MonoBehaviour, IResearch
{
    [SerializeField]
    private string targetPassiveKey;
    [SerializeField]
    private int value;

    public void ActiveResearch()
    {
        if (string.IsNullOrEmpty(targetPassiveKey))
            return;
        PassiveManager.Instance.SetCount(targetPassiveKey, value);
    }
}
