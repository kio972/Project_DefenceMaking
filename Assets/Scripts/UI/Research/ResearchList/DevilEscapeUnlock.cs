using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilEscapeUnlock : MonoBehaviour, IResearch
{
    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        EmergencyEscape escape = new EmergencyEscape();
        king.AddSkill(escape);
    }
}
