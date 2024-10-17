using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilDetectInject : MonoBehaviour, Research
{
    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        DetectInject devilAura = new DetectInject();
        king.skills.Add(devilAura);
        devilAura.SkillInit();
    }
}
