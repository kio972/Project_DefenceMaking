using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GoldMine : Environment
{
    protected override void CustomFunc()
    {
        GameManager.Instance.curWave.Skip(1).Subscribe(_ =>
        {
            DamageTextPooling.Instance.TextEffect(transform.position - (Vector3.down * 0.2f), $"+{value}<sprite name=Gold>", 27f, Color.yellow, true);
        }).AddTo(gameObject);
    }
}
