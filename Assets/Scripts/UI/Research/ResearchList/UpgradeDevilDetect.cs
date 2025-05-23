using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDevilDetect : MonoBehaviour, IResearch
{
    public void ActiveResearch()
    {
        PlayerBattleMain king = GameManager.Instance.king;
        king.AddStatusEffect<Detect>(new Detect(king, 0));
    }
}
