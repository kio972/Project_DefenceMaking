using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    private ISkill _skill;

    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private Image _coolTimeFill;

    private Animator _animator;

    private readonly float mouseOverTime = 0.2f;
    private CancellationTokenSource _scaleToken;

    [SerializeField]
    private SkillCostText costText;

    private void ResetScaleToken()
    {
        _scaleToken?.Cancel();
        _scaleToken?.Dispose();
        _scaleToken = new CancellationTokenSource();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    ResetScaleToken();
                    UtilHelper.IScaleEffect(image.transform, image.transform.localScale, Vector3.one * 1.2f, mouseOverTime, _scaleToken.Token).Forget();
                });
            image.OnPointerExitAsObservable()
                .Subscribe(_ =>
                {
                    ResetScaleToken();
                    UtilHelper.IScaleEffect(image.transform, image.transform.localScale, Vector3.one, mouseOverTime, _scaleToken.Token).Forget();
                });
        }
    }

    public void Init(ISkill skill)
    {
        _skill = skill;
        _skill.SkillInit();
        _skill.isReady.Subscribe(_ => _coolTimeFill.gameObject.SetActive(_));
        _skill.coolRate.Subscribe(_ => _coolTimeFill.fillAmount = _).AddTo(gameObject);
        if (_skill is IHaveCost haveCost)
            costText.SetCost(haveCost);
        else
            costText.gameObject.SetActive(false);
    }

    public void UseSkillBtn()
    {
        if(_skill == null) return;

        if (!(_skill.isReady.Value)) return;
        if (_skill.coolRate.Value > 0)
        {
            _animator?.SetTrigger("Invalid");
            return;
        } 

        _skill.UseSkill();
    }
}
