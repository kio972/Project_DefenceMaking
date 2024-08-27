using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
public class TrapDurationBar : MonoBehaviour
{
    [SerializeField]
    private Trap battler;
    [SerializeField]
    private Image hp_Bar;
    [SerializeField]
    private GameObject imgGroup;

    private float battlerCurHp;
    private float battlerCurSheild;

    private bool deadBar = false;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void HPBarEnd()
    {
        gameObject.SetActive(false);
        disposables.Clear();
    }

    public void Init(Trap trap)
    {
        hp_Bar.fillAmount = 1f;
        deadBar = false;

        UpdatePosition(trap.transform.position);
        imgGroup.SetActive(false);
        var maxhpStream = trap.maxDuration.CombineLatest(trap.curDuration, (maxDuration, curDuration) => new { maxDuration, curDuration });
        var curhpStream = trap.curDuration.AsObservable();
        Observable.CombineLatest(trap.curDuration, trap.maxDuration, (curDuration, maxDuration) => curDuration == maxDuration || maxDuration == 0 ? 1 : (float)curDuration / maxDuration)
            .Subscribe(_ => UpdateHp(_)).AddTo(disposables);
    }


    private void UpdatePosition(Vector3 position)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.position = position;
    }

    public void UpdateHp(float fillRate)
    {
        if(deadBar) return;

        transform.SetAsLastSibling();

        imgGroup.SetActive(fillRate != 1);
        hp_Bar.fillAmount = fillRate;

        if(fillRate <= 0)
        {
            deadBar = true;
            HPBarEnd();
        }
    }
}
