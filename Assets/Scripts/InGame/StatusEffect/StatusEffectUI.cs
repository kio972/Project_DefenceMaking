using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField]
    private GameObject imgGroup;
    [SerializeField]
    private int index = 1;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image coolDownImg;
    [SerializeField]
    private TextMeshProUGUI stackText;

    private CompositeDisposable disposables = new CompositeDisposable();
    private CompositeDisposable durationUpdateStream = new CompositeDisposable();

    public void DisableUI()
    {
        disposables.Dispose();
        durationUpdateStream.Dispose();
    }

    public void UpdateEffect(StatusEffect effect)
    {
        //icon변경함수 추가 필요

        effect._duration.Subscribe(_ =>
        {
            coolDownImg.fillAmount = effect._originDuration  == 0 ? 0 : (effect._originDuration - _) / effect._originDuration;
            if (effect is IStackable stackable && stackable.stackCount > 1)
            {
                stackText.gameObject.SetActive(true);
                stackText.text = stackable.stackCount.ToString();
            }
            else
                stackText.gameObject.SetActive(false);

        }).AddTo(durationUpdateStream);

        Sprite sprite = SpriteList.Instance.LoadSprite(effect.GetType().Name);
        if(sprite != null)
        {
            icon.sprite = sprite;
            coolDownImg.sprite = sprite;
        }
    }

    private void SetOverIndex(int count)
    {
        coolDownImg.fillAmount = 0;
        stackText.gameObject.SetActive(true);
        stackText.text = (count - 3).ToString();
    }

    private void SetEffect(Battler battler, int count)
    {
        bool isActive = count >= index;
        imgGroup.SetActive(isActive);
        durationUpdateStream?.Dispose();
        if (isActive && count <= 4)
            UpdateEffect(battler._effects[index - 1]);
        else if (isActive && index == 4)
            SetOverIndex(count);
    }

    private Battler _battler;

    public void Init(Battler battler)
    {
        _battler = battler;
        //battler._effects.ObserveCountChanged(true).Subscribe(_ => SetEffect(battler, _)).AddTo(disposables);
        SetEffect(_battler, _battler._effects.Count);
    }

    public void Update()
    {
        if (_battler == null) return;
        SetEffect(_battler, _battler._effects.Count);
    }
}