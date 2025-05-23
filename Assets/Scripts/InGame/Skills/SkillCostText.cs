using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillCostText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    IHaveCost _skill;

    public void SetCost(IHaveCost skill)
    {
        _skill = skill;
        Update();
    }

    public void Update()
    {
        if (_skill == null)
            return;

        text.text = $"{_skill.cost}<sprite name={_skill.costType}>";
    }
}
