using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilDetect : MonoBehaviour, Research
{
    public void ActiveResearch()
    {
        PassiveManager.Instance.UpgradeDevilDetection();
    }
}
