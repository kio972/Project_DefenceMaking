using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilDetectInject : MonoBehaviour, IResearch
{
    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;

        DetectInject devilAura = new DetectInject();
        king.AddSkill(devilAura);
    }
}
