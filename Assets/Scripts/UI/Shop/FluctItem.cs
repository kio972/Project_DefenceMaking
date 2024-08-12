using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum FluctType
{
    Both,
    IncreaseOnly,
    DecreaseOnly,
    Fixed,
}

public abstract class FluctItem : MonoBehaviour
{
    [SerializeField]
    protected float increaseMin;
    [SerializeField]
    protected float increaseMax;
    [SerializeField]
    protected float decreaseMin;
    [SerializeField]
    protected float decreaseMax;
    [SerializeField]
    protected int originPrice;
    protected ReactiveProperty<int> curPrice = new ReactiveProperty<int>();

    public int _CurPrice { get => curPrice.Value; set => curPrice.Value = value; }

    [SerializeField]
    protected int coolTime = 3;

    protected int coolStartWave = -1;

    public abstract void FluctPrice();
    public abstract void UpdateCoolTime();
}
