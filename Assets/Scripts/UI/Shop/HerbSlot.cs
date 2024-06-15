using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;

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

    private ReactiveProperty<bool> tradeState = new ReactiveProperty<bool>(true);


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
                GameManager.Instance.herb1 = Mathf.Clamp(GameManager.Instance.herb1, 0, GameManager.Instance.herb1Max);
                GameManager.Instance.notificationBar?.SetMesseage("흑색 허브의 가치가 떨어져 폐기처분되었습니다.", NotificationType.Shop);
                return;
            case 2:
                GameManager.Instance.herb2 = Mathf.Clamp(GameManager.Instance.herb1, 0, GameManager.Instance.herb2Max);
                GameManager.Instance.notificationBar?.SetMesseage("자색 허브의 가치가 떨어져 폐기처분되었습니다.", NotificationType.Shop);
                return;
            case 3:
                GameManager.Instance.herb3 = Mathf.Clamp(GameManager.Instance.herb1, 0, GameManager.Instance.herb3Max);
                GameManager.Instance.notificationBar?.SetMesseage("백색 허브의 가치가 떨어져 폐기처분되었습니다.", NotificationType.Shop);
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
        if (!tradeState.Value)
            return;

        if (GameManager.Instance.gold < curPrice.Value)
        {
            _ShopUI?.PlayScript("Shop034");
            return;
        }

        Modifyherb(1);
        GameManager.Instance.gold -= curPrice.Value;

        if (!string.IsNullOrEmpty(buyScript))
            _ShopUI?.PlayScript(buyScript);
    }

    public void Sellherb()
    {
        if (!tradeState.Value)
            return;

        if (!Haveherb())
            return;

        Modifyherb(-1);
        GameManager.Instance.gold += curPrice.Value;

        if(!string.IsNullOrEmpty(sellScript))
            _ShopUI?.PlayScript(sellScript);
    }

    private void DeListherb()
    {
        if (!tradeState.Value)
            return;

        curPrice.Value = 0;
        tradeState.Value = false;
        
        coolStartWave = GameManager.Instance.CurWave;
        Removeherb();
    }

    private void OnListherb()
    {
        curPrice.Value = originPrice;
        tradeState.Value = true;
        coolStartWave = -1;
    }

    private int DecreasePrice()
    {
        float changeVal = Random.Range(decreaseMin, decreaseMax);
        return Mathf.RoundToInt(curPrice.Value * changeVal / 100);
    }

    private int IncreasePrice()
    {
        float changeVal = Random.Range(increaseMin, increaseMax);
        return Mathf.RoundToInt(curPrice.Value * changeVal / 100);
    }

    public override void FluctPrice()
    {
        if (curPrice.Value == 0)
            return;

        int token = Random.Range(0, 2);
        int fluctVal = 0;
        if (token == 0)
            fluctVal -= DecreasePrice();
        else
            fluctVal += IncreasePrice();

        fluctNoti?.SetNoti(fluctVal, curPrice.Value);
        
        curPrice.Value += fluctVal;
        if(curPrice.Value <= 0)
        {
            DeListherb();
            fluctNoti?.SetCoolTime(coolTime);
        }
        priceText.text = curPrice.ToString();

    }

    public override void UpdateCoolTime()
    {
        if (coolStartWave == -1)
            return;

        int curWave = GameManager.Instance.CurWave;
        fluctNoti?.DecreaseCoolTime();
        if (curWave >= coolStartWave + coolTime)
        {
            OnListherb();
            fluctNoti?.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        priceText.text = curPrice.ToString();
    }

    public void Init()
    {
        OnListherb();
        curPrice.Subscribe(_ => priceText.text = _.ToString());
        tradeState.Subscribe(_ =>
        {
            buyBtn.gameObject.SetActive(_);
            sellBtn.gameObject.SetActive(_);
        });
    }
}
