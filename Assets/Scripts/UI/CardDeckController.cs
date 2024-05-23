using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public struct Card
{
    public int cardIndex;
    public string cardFrame;
    public CardType cardType;
    public string cardName;
    public CardGrade cardGrade;
    public string cardDescription;
    public string cardPrefabName;

    private static CardGrade GetCardGrade(string grade)
    {
        switch (grade)
        {
            case "normal":
                return CardGrade.normal;
            case "rare":
                return CardGrade.rare;
            case "epic":
                return CardGrade.epic;
            case "legend":
                return CardGrade.legend;
        }
        return CardGrade.none;
    }

    private static CardType GetCardType(string type)
    {
        switch (type)
        {
            case "road":
                return CardType.MapTile;
            case "room":
                return CardType.MapTile;
            case "roomPart":
                return CardType.MapTile;
            case "trap":
                return CardType.Trap;
            case "environment":
                return CardType.Environment;
            case "monster":
                return CardType.Monster;
        }
        return CardType.None;
    }

    public Card(Dictionary<string, object> cardInfo, int index)
    {
        cardIndex = index;
        cardFrame = cardInfo["type"].ToString();
        cardType = GetCardType(cardInfo["type"].ToString());
        cardGrade = GetCardGrade(cardInfo["grade"].ToString());
        cardName = cardInfo["text_name"].ToString();
        cardDescription = cardInfo["text_description"].ToString();
        cardPrefabName = cardInfo["prefab"].ToString();
    }
}

public class CardDeckController : MonoBehaviour
{
    [SerializeField]
    private Button deckDrawBtn;
    private Image deckBtnImg;
    [SerializeField]
    private Sprite originDeckImg;
    [SerializeField]
    private Sprite emptyDeckImg;

    [SerializeField]
    private Transform cardZone;
    [SerializeField]
    private Transform cardZoom;

    public Transform CardZone { get => cardZone; }

    private List<int> cardDeck;
    public List<int> _CardDeck { get => cardDeck; }

    public int _CardDeckCount { get => cardDeck.Count; }

    private List<int> handCards = new List<int>();
    public List<int> _HandCards { get => handCards; }
    
    private bool initState = false;

    [SerializeField]
    private int pointPadding = 2;
    [SerializeField]
    private float handHeight = 1;

    private List<Transform> cards = new List<Transform>();
    //초기 카드 숫자
    public int hand_CardNumber = 0;
    //최대 카드 숫자
    public int maxCardNumber = 10;

    [SerializeField]
    private UILineRenderer lineRenderer;
    [SerializeField]
    private int lineRendererPoint = 10;
    [SerializeField]
    private float lineAmplitude = 1f;

    private Coroutine set_CardPos_Coroutine = null;

    [SerializeField]
    private TextMeshProUGUI deckCountText;

    private int cardPrice = 5;

    [SerializeField]
    private MouseOverEffect2 recycle;

    public bool IsRecycle { get { return recycle.IsMouseOver; } }

    public void AddCard(int index)
    {
        cardDeck.Add(index);
        UpdateDeckCount();
    }

    private void UpdateDeckCount()
    {
        if (deckCountText == null)
            return;

        deckCountText.text = (cardDeck.Count).ToString();
        if (cardDeck.Count < 10)
            deckCountText.color = Color.red;
        else
            deckCountText.color = Color.black;

        if (cardDeck.Count <= 0)
            deckBtnImg.sprite = emptyDeckImg;
        else
            deckBtnImg.sprite = originDeckImg;
    }

    public void DrawGuide(Vector3 cardPos, bool value)
    {
        lineRenderer.gameObject.SetActive(value);
        if(value)
        {
            List<Vector2> lines = GetLine(cardPos, Input.mousePosition);
            lineRenderer.Points = lines.ToArray();

            float modifyX = SettingManager.Instance.GetScreenSize()[0];
            float modifyY = SettingManager.Instance.GetScreenSize()[1];
            modifyX = 1920f / modifyX;
            modifyY = 1080f / modifyY;
            lineRenderer.transform.localScale = new Vector3(modifyX, modifyY, 1);
        }
    }

    private List<Vector2> GetLine(Vector3 startPos, Vector3 endPos)
    {
        int points = lineRendererPoint;
        List<Vector2> linePosition = new List<Vector2>();

        Vector2 rotDir = startPos - endPos;
        float amplitude = lineAmplitude;
        // rotDir의 길이에 따라 amplitude 조정하는 코드 필요
        float minAmp = 0;
        float maxAmp = 300;
        float minDist = 0;
        float maxDist = 2000;
        float distance = rotDir.magnitude;
        float normalizedDist = Mathf.InverseLerp(minDist, maxDist, distance);
        // 정규화된 거리 값을 최소진폭과 최대진폭 사이의 값으로 매핑
        amplitude = Mathf.Lerp(minAmp, maxAmp, normalizedDist);
        // 최소진폭 ~ 최대진폭
        // 최소거리 ~ 최대거리
        float invert = 1f;
        if (endPos.x > startPos.x)
            invert *= -1f;

        for (int i = 0; i < points; ++i)
        {
            // i를 0.0-1.0 사이의 값으로 정규화
            float t = (float)i / (points - 1);

            Vector2 point = Vector2.Lerp(startPos, endPos, t);

            //modifyValue만큼 point의 좌표값을 조정
            Vector2 modifyValue = new Vector2(0, amplitude * Mathf.Sin(2 * Mathf.PI * t * 0.5f) * invert);
            //modifyValue를 rotDir만큼 회전
            float angle = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, rotDir.normalized);

            Vector2 rotatedModifyValue = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle) * modifyValue;

            point.x += rotatedModifyValue.x;
            point.y += rotatedModifyValue.y;

            linePosition.Add(point);
        }
        return linePosition;
    }

    public List<Vector3> CalculateRotation()
    {
        //회전값계산
        //전포인트 -> 다음포인트로 방향벡터 / 2를 전포인트에 더한 위치
        //위 위치 -> 내위치로 가는 방향벡터와 내 transform.up을 동일하게 하도록 roation 조정
        int points = pointPadding + hand_CardNumber + pointPadding;
        List<Vector3> cardPos = new List<Vector3>();
        List<Vector3> cardDirVecs = new List<Vector3>();
        for (int i = pointPadding - 1; i < points - pointPadding + 1; ++i)
        {
            float t = (float)i / (points - 1);
            float x = Mathf.Lerp(ReturnStartX(), ReturnStartY(), t);
            float y = handHeight * Mathf.Sin(2 * Mathf.PI * t * 0.5f);
            cardPos.Add(new Vector3(x, y));
        }

        for(int i = 1; i < cardPos.Count - 1; i++)
        {
            Vector3 prevPos = cardPos[i - 1] * 1000;
            Vector3 nextPos = cardPos[i + 1] * 1000;
            Vector3 dirVec = (nextPos - prevPos) / 2 * 1000;
            cardDirVecs.Add((cardPos[i] - (prevPos + dirVec)).normalized);
        }

        return cardDirVecs;
    }

    public float ReturnStartY()
    {
        float screenSizeX = SettingManager.Instance.GetScreenSize()[0];
        //float minPos_Y = 1370;
        //float maxPos_Y = 1820;
        float minPos_Y = (screenSizeX / 2) + (screenSizeX * 0.2f);
        float maxPos_Y = (screenSizeX / 2) + (screenSizeX * 0.45f);
        if (hand_CardNumber >= 0 && hand_CardNumber <= 10)
        {
            // 카드 개수가 0에서 10 사이일 때
            float startX = Mathf.Lerp(minPos_Y, maxPos_Y, hand_CardNumber / 10f);
            return startX;
        }
        else
        {
            // 카드 개수가 범위를 벗어날 때 기본값인 100을 반환하거나 예외 처리를 수행합니다.
            return minPos_Y;
        }
    }

    public float ReturnStartX()
    {
        float screenSizeX = SettingManager.Instance.GetScreenSize()[0];
        //float minPos_X = 550;
        //float maxPos_X = 100;
        float minPos_X = (screenSizeX / 2) - (screenSizeX * 0.2f);
        float maxPos_X = (screenSizeX / 2) - (screenSizeX * 0.45f);
        //현재 카드 개수가 0에서 10임에 따라 100 ~ 450값을 반환
        if (hand_CardNumber >= 0 && hand_CardNumber <= 10)
        {
            // 카드 개수가 0에서 10 사이일 때
            float startX = Mathf.Lerp(minPos_X, maxPos_X, hand_CardNumber / 10f);
            return startX;
        }
        else
        {
            // 카드 개수가 범위를 벗어날 때 기본값인 100을 반환하거나 예외 처리를 수행합니다.
            return minPos_X;
        }
    }

    public void SetCardPosition()
    {
        if (set_CardPos_Coroutine != null)
            StopCoroutine(set_CardPos_Coroutine);
        set_CardPos_Coroutine = StartCoroutine(ISetCardPosition());
    }

    private List<Vector3> GetCardPosition()
    {
        int points = pointPadding + hand_CardNumber + pointPadding;
        List<Vector3> cardPos = new List<Vector3>();
        for (int i = pointPadding; i < points - pointPadding; ++i)
        {
            // i를 0.0-1.0 사이의 값으로 정규화
            float t = (float)i / (points - 1);
            // start부터 end 위치까지 points 개수의 점을 일정하게 배치
            float x = Mathf.Lerp(ReturnStartX(), ReturnStartY(), t);
            // 2*Mathf.PI = 360이고, t는 0.0~1.0 사이의 값이기 때문에 이 값을 곱하면 1 진동의 사인 그래프가 완성되고,
            // frequency를 곱하기 때문에 frequency 값에 따라 진동수가 결정된다.
            float y = handHeight * Mathf.Sin(2 * Mathf.PI * t * 0.5f);
            cardPos.Add(new Vector3(x, y));
        }
        return cardPos;
    }

    

    private Card ReturnDeck(int target)
    {
        cardDeck.Remove(target);
        Card card = new Card(DataManager.Instance.Deck_Table[target], target);
        return card;
    }

    private Card ReturnDeck()
    {
        int random = cardDeck[UnityEngine.Random.Range(0, cardDeck.Count)];
        return ReturnDeck(random);
    }

    public void DrawDeck()
    {
        if (hand_CardNumber >= maxCardNumber)
        {
            GameManager.Instance.popUpMessage?.ToastMsg("손 패가 가득 찼습니다");
            return;
        }

        if (GameManager.Instance.gold < cardPrice)
        {
            GameManager.Instance.popUpMessage?.ToastMsg("골드가 부족합니다");
            return;
        }

        GameManager.Instance.gold -= cardPrice;
        DrawCard();
        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }

    private IEnumerator ISetCardPosition()
    {
        //모든 카드들의 위치를 lerpTime에 걸쳐 조정
        //모든 카드들의 회전값을 lerpTime에 걸쳐 조정

        float elapsedTime = 0f;
        float lerpTime = 0.5f;
        List<Vector3> cardPos = GetCardPosition();
        List<Vector3> cardRot = CalculateRotation();
        Vector3[] startPositions = new Vector3[cards.Count];
        Quaternion[] startRotations = new Quaternion[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            startPositions[i] = cards[i].position;
            startRotations[i] = cards[i].rotation;
            CardController temp = cards[i].GetComponent<CardController>();
            temp.originPos = cardPos[i];
            temp.originRot = UtilHelper.AlignUpWithVector(cardRot[i]);
            temp.originSiblingIndex = i;
        }

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            for (int i = 0; i < hand_CardNumber; i++)
                cards[i].transform.position = Vector3.Lerp(startPositions[i], cardPos[i], t);

            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 targetDirection = cardRot[i];
                Quaternion targetRotation = UtilHelper.AlignUpWithVector(targetDirection);

                Quaternion currentRotation = Quaternion.Lerp(startRotations[i], targetRotation, t);
                cards[i].transform.rotation = currentRotation;
            }

            yield return null;
        }
    }

    private void DrawTypeCard(List<int> cardPool, int drawNumber = 1)
    {
        if (cardPool == null)
            return;

        for (int i = 0; i < drawNumber; i++)
        {
            int randomIndex = cardPool[UnityEngine.Random.Range(0, cardPool.Count)];
            while (!cardDeck.Contains(randomIndex))
                randomIndex = cardPool[UnityEngine.Random.Range(0, cardPool.Count)];
            DrawCard(randomIndex);
        }
    }

    

    public void Mulligan()
    {
        // 길타일 3
        // 방타일 1
        // 방조각 2
        List<int> pathPool = DataManager.Instance.PathCard_Indexs;
        DrawTypeCard(pathPool, 3);
        List<int> roomPool = new List<int>(DataManager.Instance.RoomCard_Indexs);
        roomPool.RemoveAt(0);
        DrawTypeCard(roomPool);
        //List<int> trapPool = DataManager.Instance.TrapCard_Indexs;
        //DrawTypeCard(trapPool);
        //List<int> monsterPool = DataManager.Instance.MonsterCard_Indexs;
        //DrawTypeCard(monsterPool);
        DrawTypeCard(DataManager.Instance.RoomPartCard_Indexs);
        DrawTypeCard(DataManager.Instance.EnvironmentCard_Indexs);

    }

    public void DrawCard(int cardIndex)
    {
        if (cardDeck.Count < 1) return;
        if (!cardDeck.Contains(cardIndex)) return;

        InstantiateCard(ReturnDeck(cardIndex));
    }

    public void DrawCard()
    {
        if(cardDeck.Count < 1) return;

        InstantiateCard(ReturnDeck());
    }

    public void DiscardCard(Transform cardTransform, int cardId)
    {
        cards.Remove(cardTransform);
        handCards.Remove(cardId);
    }

    private void InstantiateCard(Card targetCard)
    {
        CardController cardPrefab = Resources.Load<CardController>("Prefab/UI/Card_Frame");

        hand_CardNumber++;
        CardController card = Instantiate(cardPrefab, cardZone);
        card.Init(targetCard);
        card?.DrawEffect();
        card.transform.position = transform.position;
        cards.Add(card.transform);
        handCards.Add(targetCard.cardIndex);
        SetCardPosition();
        UpdateDeckCount();
    }

    private void SetDeck()
    {
        cardDeck = new List<int>();
        for(int i = 0; i < DataManager.Instance.Deck_Table.Count; i++)
        {
            Card cardCheck = new Card(DataManager.Instance.Deck_Table[i], i);
            GameObject cardPrefab = UtilHelper.GetCardPrefab(cardCheck.cardType, cardCheck.cardPrefabName);
            if (cardPrefab == null)
                continue;

            int cardNumber = Convert.ToInt32(DataManager.Instance.Deck_Table[i]["startNumber"]);
            for (int j = 0; j < cardNumber; j++)
                cardDeck.Add(i);
        }
    }

    public void LoadData(List<int> cardIdes, List<int> deckLists)
    {
        if (!initState)
            Init();

        cardDeck = new List<int>(deckLists);
        foreach(int id in cardIdes)
        {
            AddCard(id);
            DrawCard(id);
        }
    }

    public void Init()
    {
        if (initState) return;

        deckDrawBtn.onClick.AddListener(DrawDeck);
        deckBtnImg = deckDrawBtn.GetComponent<Image>();
        SetDeck();
        UpdateDeckCount();
        initState = true;
    }

    private ScreenSize curScreenSize;
    private void Update()
    {
        if(curScreenSize != SettingManager.Instance.screenSize)
        {
            curScreenSize = SettingManager.Instance.screenSize;
            SetCardPosition();
        }
    }
}
