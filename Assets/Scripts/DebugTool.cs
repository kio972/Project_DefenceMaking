using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTool : MonoBehaviour
{
    [SerializeField]
    CardDeckController cardDeckController;
    public int cardIndex;

    Button getCardBtn;

    public void GetCard()
    {
        cardDeckController.DrawCard(cardIndex);
    }
    
}
