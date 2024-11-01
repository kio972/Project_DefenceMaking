using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeploySlot))]
public class DeploySlotInjecter : MonoBehaviour
{
    DeploySlot slot;

    [SerializeField]
    SlotInfo slotInfo;

    [SerializeField]
    private string id;

    private void Start()
    {
        slot = GetComponent<DeploySlot>();
        slot.Init(DataManager.Instance.battlerDic[id]);

        if (slotInfo != null && slotInfo.curSlot != null)
            return;

        slot.SendInfo();
    }
}
