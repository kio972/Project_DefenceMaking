using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    private ISkill _skill;

    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private Image _coolTimeFill;

    public void Init(ISkill skill)
    {
        _skill = skill;
        _skill.SkillInit();
        _skill.isReady.Subscribe(_ => _coolTimeFill.gameObject.SetActive(_));
        _skill.coolRate.Subscribe(_ => _coolTimeFill.fillAmount = _).AddTo(gameObject);
    }

    public void UseSkillBtn()
    {
        if(_skill == null) return;

        if (!(_skill.isReady.Value)) return;

        _skill.UseSkill();
    }
}
