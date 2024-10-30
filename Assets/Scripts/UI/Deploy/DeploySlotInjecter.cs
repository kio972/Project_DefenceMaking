using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeploySlot))]
public class DeploySlotInjecter : MonoBehaviour
{
    DeploySlot slot;

    [SerializeField]
    private string id;

    private void Awake()
    {
        slot = GetComponent<DeploySlot>();
        slot.Init(DataManager.Instance.battlerDic[id]);
        slot.SendInfo();
    }
}
