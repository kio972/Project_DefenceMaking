using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GoldMineUpgrade : MonoBehaviour, Research
{
    [SerializeField]
    int value = 5;

    private bool ApplyGoldMineUpgrade(ITileKind tileKind)
    {
        if (tileKind is not GoldMine)
            return false;

        PassiveManager.Instance.income_Weight += (int)value;
        return true;
    }

    public void ActiveResearch()
    {
        foreach (var item in NodeManager.Instance.environments)
            ApplyGoldMineUpgrade(item);

        NodeManager.Instance.AddSetTileEvent(ApplyGoldMineUpgrade);
    }
}
