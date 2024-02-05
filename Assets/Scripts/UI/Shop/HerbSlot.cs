using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface FluctItem
{
    public void FluctPrice();
    public void UpdateCoolTime();
}

public class HerbSlot : MonoBehaviour, FluctItem
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

    [SerializeField]
    private int targetHurb = 0;

    private bool tradeState = true;

    private int coolStartWave = -1;

    [SerializeField]
    private Sprite increaseSprite;
    [SerializeField]
    private Sprite decreaseSprite;

    private void ModifyHurb(int count)
    {
        switch (targetHurb)
        {
            case 1:
                GameManager.Instance.hurb1 += count;
                return;
            case 2:
                GameManager.Instance.hurb2 += count;
                return;
            case 3:
                GameManager.Instance.hurb3 += count;
                return;
        }
    }
    private bool HaveHurb()
    {
        switch (targetHurb)
        {
            case 1:
                return GameManager.Instance.hurb1 > 0 ? true : false;
            case 2:
                return GameManager.Instance.hurb2 > 0 ? true : false;
            case 3:
                return GameManager.Instance.hurb3 > 0 ? true : false;
        }

        return false;
    }

    public void BuyHurb()
    {
        if (!tradeState)
            return;

        if (GameManager.Instance.gold < curPrice)
            return;

        ModifyHurb(1);
        GameManager.Instance.gold -= curPrice;
    }

    public void SellHurb()
    {
        if (!tradeState)
            return;

        if (!HaveHurb())
            return;

        ModifyHurb(-1);
        GameManager.Instance.gold += curPrice;
    }

    private void DeListHurb()
    {
        if (!tradeState)
            return;

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

    private void SetFluctImg(int val)
    {
        if (fluctImg == null)
            return;

        fluctImg.gameObject.SetActive(true);

        if (val == 0)
            fluctImg.gameObject.SetActive(false);
        else if (val > 0)
            fluctImg.sprite = increaseSprite;
        else
            fluctImg.sprite = decreaseSprite;
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
        SetFluctImg(fluctVal);
    }

    public void UpdateCoolTime()
    {
        if (coolStartWave == -1)
            return;

        int curWave = GameManager.Instance.CurWave;
        if (curWave >= coolStartWave + coolTime)
            OnListHurb();
    }

    private void OnEnable()
    {
        priceText.text = curPrice.ToString();
    }

    private void Start()
    {
        OnListHurb();
    }
}
