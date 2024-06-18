using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinHeal : MonoBehaviour, Research
{
    public void ActiveResearch()
    {
        PassiveManager.Instance.GoblinHealActive();
    }
}