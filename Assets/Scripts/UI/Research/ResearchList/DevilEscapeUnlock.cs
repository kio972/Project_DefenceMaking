using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilEscapeUnlock : MonoBehaviour, Research
{
    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        EmergencyEscape escape = new EmergencyEscape();
        king.skills.Add(escape);
        escape.SkillInit();
    }
}
