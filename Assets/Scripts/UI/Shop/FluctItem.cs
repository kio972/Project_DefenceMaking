using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected int curPrice;

    [SerializeField]
    protected int coolTime = 3;

    protected int coolStartWave = -1;

    public abstract void FluctPrice();
    public abstract void UpdateCoolTime();
}
