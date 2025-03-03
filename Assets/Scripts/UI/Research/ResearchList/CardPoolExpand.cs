using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPoolExpand : MonoBehaviour, IResearch
{
    [SerializeField]
    private string targetCardId;

    [SerializeField]
    private CardPackType targetPack;
    [SerializeField]
    private CardType targetCardType;

    public void ActiveResearch()
    {
        if (string.IsNullOrEmpty(targetCardId))
            return;

        ShopUI shop = GameManager.Instance.shop;

        foreach(var itemSlot in shop.itemSlots)
        {
            Item item = itemSlot.GetComponent<Item>();
            if(item == null) continue;

            if (item is CardPack pack && pack.cardType == targetPack)
                pack.AddCardPool(targetCardType, targetCardId);
        }
    }
}
