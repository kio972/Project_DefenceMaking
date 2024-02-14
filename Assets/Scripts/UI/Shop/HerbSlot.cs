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
    private int targetherb = 0;

    private bool tradeState = true;

    private int coolStartWave = -1;

    [SerializeField]
    private Sprite increaseSprite;
    [SerializeField]
    private Sprite decreaseSprite;

    private void Modifyherb(int count)
    {
        switch (targetherb)
        {
            case 1:
                GameManager.Instance.herb1 += count;
                return;
            case 2:
                GameManager.Instance.herb2 += count;
                return;
            case 3:
                GameManager.Instance.herb3 += count;
                return;
        }
    }
    private bool Haveherb()
    {
        switch (targetherb)
        {
            case 1:
                return GameManager.Instance.herb1 > 0 ? true : false;
            case 2:
                return GameManager.Instance.herb2 > 0 ? true : false;
            case 3:
                return GameManager.Instance.herb3 > 0 ? true : false;
        }

        return false;
    }

    public void Buyherb()
    {
        if (!tradeState)
            return;

        if (GameManager.Instance.gold < curPrice)
            return;

        Modifyherb(1);
        GameManager.Instance.gold -= curPrice;
    }

    public void Sellherb()
    {
        if (!tradeState)
            return;

        if (!Haveherb())
            return;

        Modifyherb(-1);
        GameManager.Instance.gold += curPrice;
    }

    private void DeListherb()
    {
        if (!tradeState)
            return;

        curPrice = 0;
        tradeState = false;
        buyBtn.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        coolStartWave = GameManager.Instance.CurWave;
    }

    private void OnListherb()
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
            DeListherb();
        priceText.text = curPrice.ToString();
        SetFluctImg(fluctVal);
    }

    public void UpdateCoolTime()
    {
        if (coolStartWave == -1)
            return;

        int curWave = GameManager.Instance.CurWave;
        if (curWave >= coolStartWave + coolTime)
            OnListherb();
    }

    private void OnEnable()
    {
        priceText.text = curPrice.ToString();
    }

    private void Start()
    {
        OnListherb();
    }
}
