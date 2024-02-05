using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface FluctItem
{
    public void FluctPrice();
}

public class HurbSlot : MonoBehaviour, FluctItem
{
    [SerializeField]
    Image icon;
    [SerializeField]
    Image fluctImg;
    [SerializeField]
    TextMeshProUGUI priceText;
    [SerializeField]
    private Button buyBtn;
    [SerializeField]
    private Button sellBtn;

    [SerializeField]
    private int originPrice;
    private int curPrice;

    [SerializeField]
    private float increaseMin;
    [SerializeField]
    private float increaseMax;
    [SerializeField]
    private float decreaseMin;
    [SerializeField]
    private float decreaseMax;

    [SerializeField]
    private int coolTime = 3;

    private bool tradeState = true;

    private int coolStartWave = -1;

    private void BuyHurb()
    {
        if (!tradeState)
            return;

    }

    private void SellHurb()
    {
        if (!tradeState)
            return;

    }

    private void DeListHurb()
    {
        curPrice = 0;
        tradeState = false;
        buyBtn.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        coolStartWave = GameManager.Instance.CurWave;
    }

    private void OnListHurb()
    {
        curPrice = originPrice;
        tradeState = true;
        buyBtn.gameObject.SetActive(true);
        sellBtn.gameObject.SetActive(true);
        coolStartWave = -1;

        priceText.text = curPrice.ToString();
    }

    private int DecreasePrice()
    {
        float changeVal = Random.Range(decreaseMin, decreaseMax);
        return Mathf.RoundToInt(curPrice * changeVal / 100);
    }

    private int IncreasePrice()
    {
        float changeVal = Random.Range(increaseMin, increaseMax);
        return Mathf.RoundToInt(curPrice * changeVal / 100);
    }

    public void FluctPrice()
    {
        int token = Random.Range(0, 2);
        int fluctVal = 0;
        if (token == 0)
            fluctVal -= DecreasePrice();
        else
            fluctVal += IncreasePrice();

        curPrice += fluctVal;
        if(curPrice <= 0)
            DeListHurb();
        priceText.text = curPrice.ToString();
    }

    private void OnEnable()
    {
        if (coolStartWave == -1)
            return;

        int curWave = GameManager.Instance.CurWave;
        if (coolStartWave + coolTime >= curWave)
            OnListHurb();
    }

    private void Start()
    {
        OnListHurb();
    }
}
