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

    public int waveIndex;

    Button getCardBtn;

    private int prevIndex;

    [SerializeField]
    TextMeshProUGUI text;

    public void WinGame()
    {
        GameManager.Instance.SendMessage("WinGame");
    }

    public void LoseGame()
    {
        GameManager.Instance.SendMessage("LoseGame");
    }

    public void SetWave()
    {
        GameManager.Instance.SetWave(waveIndex);
    }

    public void GetCard()
    {
        cardDeckController.DrawCard(cardIndex);
    }

    int goldIndex = 0;
    [SerializeField]
    TextMeshProUGUI goldText;
    int gold { get { goldIndex = goldIndex % 3; if (goldIndex == 0) return 100; else if (goldIndex == 1) return 1000; else return 10000; } }
    public void SetGoldIndex(bool isRight)
    {
        if (isRight)
            goldIndex++;
        else
            goldIndex--;

        goldText.text = $"{gold}{"골드"}";
    }

    public void GetGold()
    {
        GameManager.Instance.gold += gold;
    }

    int herbIndex = 0;
    [SerializeField]
    TextMeshProUGUI herbText;
    
    public void SetHerbIndex(bool isRight)
    {
        if (isRight)
            herbIndex++;
        else
            herbIndex--;
        herbIndex = herbIndex % 3;
        if (herbIndex == 0)
            herbText.text = "흑색 허브(1개)";
        else if(herbIndex == 1)
            herbText.text = "자색 허브(1개)";
        else
            herbText.text = "백색 허브(1개)";
    }

    public void GetHerb()
    {
        if (herbIndex == 0)
            GameManager.Instance.herb1++;
        else if (herbIndex == 1)
            GameManager.Instance.herb2++;
        else
            GameManager.Instance.herb3++;
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
