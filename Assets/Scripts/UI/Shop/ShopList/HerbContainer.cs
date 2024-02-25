using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HerbContainer : MonoBehaviour, Item
{
    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;

    [SerializeField]
    private int targetHerb = 0;
    [SerializeField]
    private int value = 100;

    ItemSlot itemSlot;
    ItemSlot _ItemSlot
    {
        get
        {
            if (itemSlot == null)
                itemSlot = GetComponent<ItemSlot>();
            return itemSlot;
        }
    }

    public void OnClick()
    {
        if (_ItemSlot.IsSoldOut)
            return;

        shopUI?.PlayScript(descScript);
    }

    public void UseItem()
    {
        switch(targetHerb)
        {
            case 1:
                GameManager.Instance.herb1Max += value;
                break;
            case 2:
                GameManager.Instance.herb2Max += value;
                break;
            case 3:
                GameManager.Instance.herb3Max += value;
                break;
        }
    }
}
