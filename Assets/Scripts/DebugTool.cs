using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugTool : MonoBehaviour
{
    [SerializeField]
    CardDeckController cardDeckController;
    public int cardIndex;

    Button getCardBtn;

    private int prevIndex;

    [SerializeField]
    TextMeshProUGUI text;

    public void GetCard()
    {
        cardDeckController.DrawCard(cardIndex);
    }

    public void DecreaseIndex()
    {
        cardIndex--;
    }

    public void IncreaseIndex()
    {
        cardIndex++;
    }

    private void Awake()
    {
        if (text != null)
            text.text = DataManager.Instance.Deck_Table[cardIndex]["prefab"].ToString();
    }

    public void Update()
    {
        if(prevIndex != cardIndex)
        {
            if (cardIndex >= DataManager.Instance.Deck_Table.Count)
                cardIndex = 0;
            else if (cardIndex < 0)
                cardIndex = DataManager.Instance.Deck_Table.Count - 1;

            prevIndex = cardIndex;
            if (text != null)
                text.text = DataManager.Instance.Deck_Table[cardIndex]["prefab"].ToString();

        }
    }

}
