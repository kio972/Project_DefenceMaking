using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugTool : MonoBehaviour
{
    public int waveIndex = 1;

    public string targetCardId;

    int goldIndex = 0;
    int gold { get { goldIndex = goldIndex % 3; if (goldIndex == 0) return 100; else if (goldIndex == 1) return 1000; else return 10000; } }

    readonly int[] herbAmount = { 1, 10, 100 };

    [SerializeField]
    private TextMeshProUGUI waveText;

    public void DrawCard()
    {
        int index = DataManager.Instance.deckListIndex[targetCardId];
        if (index == -1)
            return;
        GameManager.Instance.cardDeckController?.AddCard(index);
        GameManager.Instance.cardDeckController?.DrawCard(index);
    }

    public void RemoveAlly()
    {
        List<Monster> monsterList = new List<Monster>(GameManager.Instance.monsterList);
        foreach (var item in monsterList)
        {
            item.ChangeState(FSMDead.Instance);
            item.Dead(null);
        }
    }

    public void RemoveEnemy()
    {
        List<Adventurer> adventurersList = new List<Adventurer>(GameManager.Instance.adventurersList);
        foreach (var item in adventurersList)
        {
            item.ChangeState(FSMDead.Instance);
            item.Dead(null);
        }
    }

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
        GameManager.Instance.SetWave(waveIndex - 1);
    }

    public void GetGold(int index)
    {
        goldIndex = index;
        GameManager.Instance.gold += gold;
    }

    public void GetHerb1(int index) { GameManager.Instance.herbDic[HerbType.BlackHerb] += herbAmount[index]; }
    public void GetHerb2(int index) { GameManager.Instance.herbDic[HerbType.PurpleHerb] += herbAmount[index]; }
    public void GetHerb3(int index) { GameManager.Instance.herbDic[HerbType.WhiteHerb] += herbAmount[index]; }

    public void IncreaseWave()
    {
        waveIndex++;
        waveIndex = Mathf.Min(waveIndex, DataManager.Instance.waveLevelTable.Count);
        if (waveText != null) waveText.text = waveIndex.ToString();
    }

    public void DecreaseWave()
    {
        waveIndex--;
        waveIndex = Mathf.Max(1, waveIndex);
        if (waveText != null) waveText.text = waveIndex.ToString();
    }
}
