using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBuff : MonoBehaviour, IResearch
{
    public void ActiveResearch()
    {
        PassiveManager.Instance.MimicBuffActive();
    }
}