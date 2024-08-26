using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
public class SpawnerGauge : MonoBehaviour
{
    [SerializeField]
    private MonsterSpawner spanwer;
    [SerializeField]
    private Image fillImg;

    private float battlerCurHp;
    private float battlerCurSheild;

    private bool deadBar = false;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void HPBarEnd()
    {
        gameObject.SetActive(false);
        disposables.Clear();
    }

    public void Init(MonsterSpawner spanwer)
    {
        this.spanwer = spanwer;
        fillImg.fillAmount = 1f;
        deadBar = false;

        UpdatePosition(spanwer.transform.position);
        spanwer._spawnRate.Subscribe(_ => UpdateHp(_)).AddTo(disposables);
    }


    private void UpdatePosition(Vector3 position)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.position = position;
    }

    public void UpdateHp(float fillRate)
    {
        if(deadBar) return;

        fillImg.fillAmount = fillRate;
    }

    private void Update()
    {
        if (deadBar) return;

        if (!GameManager.Instance.monsterSpawner.Contains(spanwer))
            HPBarEnd();
    }
}
