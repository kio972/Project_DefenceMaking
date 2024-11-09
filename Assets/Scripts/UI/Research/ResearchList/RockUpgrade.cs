using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RockUpgrade : MonoBehaviour, Research
{
    [SerializeField]
    int value = 5;

    private bool ApplyRockUpgrade(ITileKind tileKind)
    {
        if (tileKind is not Rock)
            return false;

        PassiveManager.Instance.monsterHp_Weight += (int)value;
        return true;
    }

    public void ActiveResearch()
    {
        foreach (var item in NodeManager.Instance.environments)
            ApplyRockUpgrade(item);

        NodeManager.Instance.AddSetTileEvent(ApplyRockUpgrade);
    }
}
