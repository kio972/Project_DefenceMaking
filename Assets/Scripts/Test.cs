using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public int count = 5;
    public CardPackEffect cardPackEffect;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            List<Card> cards = new List<Card>();
            for(int i = 0; i < count; i++)
                cards.Add(new Card(DataManager.Instance.Deck_Table[i], 0));

            cardPackEffect.ShowEffect(cards);
        }
    }
}
