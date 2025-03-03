using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilEscapeReduceHpCost : MonoBehaviour, IResearch
{
    [SerializeField]
    int value = 5;

    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        if (king.HaveSkill(out EmergencyEscape escape))
        {
            escape.ReduceHpCost(value);
        };
    }
}
