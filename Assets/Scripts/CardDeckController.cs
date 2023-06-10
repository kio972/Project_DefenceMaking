using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDeckController : MonoBehaviour
{
    [SerializeField]
    private Button deckDrawBtn;

    [SerializeField]
    private List<GameObject> deckPrefab;

    [SerializeField]
    private Transform cardZone;


    //초기 카드 숫자
    public int hand_CardNumber = 0;
    //최대 카드 숫자
    public int maxCardNumber = 10;

    private int ReturnDeck()
    {
        // 차후 덱상황에 따라 적절한 index를 반환하는 함수 추가예정
        int random = Random.Range(0, deckPrefab.Count);

        return random;
    }

    public void DrawDeck()
    {
        if (GameManager.Instance.gold < 200 || hand_CardNumber >= maxCardNumber)
            return;

        int targetPrefabNumer = ReturnDeck();
        hand_CardNumber++;
        GameManager.Instance.gold -= 200;

        GameObject temp = Instantiate(deckPrefab[targetPrefabNumer], cardZone);
        temp.transform.position = cardZone.transform.position;
    }

    public void FreeDrawCard()
    {
        int targetPrefabNumer = ReturnDeck();
        hand_CardNumber++;

        GameObject temp = Instantiate(deckPrefab[targetPrefabNumer], cardZone);
        temp.transform.position = cardZone.transform.position;
    }

    // Start is called before the first frame update
    void Awake()
    {
        deckDrawBtn.onClick.AddListener(DrawDeck);
    }
}
