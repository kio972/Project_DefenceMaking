using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HerbSlot : FluctItem
{
    [SerializeField]
    Image icon;
    [SerializeField]
    FluctNoti fluctNoti;
    [SerializeField]
    TextMeshProUGUI priceText;
    [SerializeField]
    private Button buyBtn;
    [SerializeField]
    private Button sellBtn;

    [SerializeField]
    private int targetherb = 0;

    private bool tradeState = true;


    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string buyScript;
    [SerializeField]
    private string sellScript;

    private ShopUI _ShopUI
    {
        get
        {
            if (shopUI == null)
                shopUI = GetComponentInParent<ShopUI>();
            return shopUI;
        }
    }

    private void Removeherb()
    {
        switch (targetherb)
        {
            case 1:
                GameManager.Instance.herb1 = Mathf.Max(GameManager.Instance.herb1, GameManager.Instance.herb1Max);
                GameManager.Instance.notificationBar?.SetMesseage("흑색 허브의 가치가 떨어져 폐기처분되었습니다.");
                return;
            case 2:
                GameManager.Instance.herb2 = Mathf.Max(GameManager.Instance.herb1, GameManager.Instance.herb2Max);
                GameManager.Instance.notificationBar?.SetMesseage("자색 허브의 가치가 떨어져 폐기처분되었습니다.");
                return;
            case 3:
                GameManager.Instance.herb3 = Mathf.Max(GameManager.Instance.herb1, GameManager.Instance.herb3Max);
                GameManager.Instance.notificationBar?.SetMesseage("백색 허브의 가치가 떨어져 폐기처분되었습니다.");
                return;
        }
    }

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
        {
            _ShopUI?.PlayScript("Shop034");
            return;
        }

        Modifyherb(1);
        GameManager.Instance.gold -= curPrice;

        if (!string.IsNullOrEmpty(buyScript))
            _ShopUI?.PlayScript(buyScript);
    }

    public void Sellherb()
    {
        if (!tradeState)
            return;

        if (!Haveherb())
            return;

        Modifyherb(-1);
        GameManager.Instance.gold += curPrice;

        if(!string.IsNullOrEmpty(sellScript))
            _ShopUI?.PlayScript(sellScript);
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
        Removeherb();
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

    public override void FluctPrice()
    {
        if (curPrice == 0)
            return;

        int token = Random.Range(0, 2);
        int fluctVal = 0;
        if (token == 0)
            fluctVal -= DecreasePrice();
        else
            fluctVal += IncreasePrice();

        fluctNoti?.SetNoti(fluctVal, curPrice);
        
        curPrice += fluctVal;
        if(curPrice <= 0)
            DeListherb();
        priceText.text = curPrice.ToString();

    }

    public override void UpdateCoolTime()
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

    public void Init()
    {
        OnListherb();
    }
}
