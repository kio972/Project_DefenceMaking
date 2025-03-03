using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAuraUp : MonoBehaviour, IResearch
{
    [SerializeField]
    private int range = 1;
    [SerializeField]
    private int value = 20;

    public void ActiveResearch()
    {
        var king = GameManager.Instance.king;
        if(king.HaveSkill(out DevilAuraSkill skill))
        {
            skill.SetDevilAuraValue(range, value);
        }
        else
        {
            DevilAuraSkill devilAura = new DevilAuraSkill();
            devilAura.SetDevilAuraValue(range, value);
            king.AddSkill(devilAura);
        }
    }
}