using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilEscapeReduceCoolTime : MonoBehaviour, IResearch
{
    [SerializeField]
    float value = 0.25f;

    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        if (king.HaveSkill(out EmergencyEscape escape))
        {
            escape.ReduceCoolTime(value);
        };
    }
}
