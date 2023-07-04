using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSupply : MonoBehaviour
{
    [SerializeField]
    private Button monsterSupplyBtn;
    [SerializeField]
    private Button tileSupplyBtn;
    [SerializeField]
    private Button trapSupplyBtn;

    [SerializeField]
    CardDeckController deckController;

    private int price = 5;

    private void SupplyMonsterCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;
        deckController.DeckSupply(CardType.Monster);
    }

    private void SupplyTileCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;
        deckController.DeckSupply(CardType.MapTile);
    }

    private void SupplyTrapCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;
        deckController.DeckSupply(CardType.Trap);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(monsterSupplyBtn != null)
            monsterSupplyBtn.onClick.AddListener(SupplyMonsterCard);
        if (tileSupplyBtn != null)
            tileSupplyBtn.onClick.AddListener(SupplyTileCard);
        if (trapSupplyBtn != null)
            trapSupplyBtn.onClick.AddListener(SupplyTrapCard);
    }
}
