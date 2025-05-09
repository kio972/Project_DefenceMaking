using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

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
                return CardType.PathTile;
            case "room":
                return CardType.RoomTile;
            case "roomPart":
                return CardType.RoomTile;
            case "roomDoor":
                return CardType.RoomTile;
            case "trap":
                return CardType.Trap;
            case "environment":
                return CardType.Environment;
            case "monster":
                return CardType.Monster;
            case "herb":
                return CardType.Environment;
            case "objectForPath":
                return CardType.ObjectForPath;
            case "magic":
                return CardType.Magic;
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
    private Image cardCountImg;
    [SerializeField]
    private Sprite cardCountSprite;
    [SerializeField]
    private Sprite emptycardCountSprite;
    
    [SerializeField]
    private Transform cardZone;
    [SerializeField]
    private Transform cardZoom;

    public Transform CardZone { get => cardZone; }

    private List<int> _cardDeck;
    public List<int> cardDeck { get => _cardDeck; }

    public int cardDeckCount { get => _cardDeck.Count; }

    private List<int> _handCards = new List<int>();
    public List<int> handCards { get => _handCards; }
    
    private bool initState = false;

    [SerializeField]
    private int pointPadding = 2;
    [SerializeField]
    private float handHeight = 1;

    private List<Transform> _cards = new List<Transform>();
    public List<Transform> cards { get => _cards; }
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

    private CancellationTokenSource cancellationTokenSource;

    [SerializeField]
    private TextMeshProUGUI deckCountText;

    private int cardPrice = 5;

    [SerializeField]
    private MouseOverEffect2 recycle;

    private int freeCount = 0;
    private int curFreeCount = 0;

    public ReactiveProperty<GameObject> curHandlingObject { get; private set; } = new ReactiveProperty<GameObject>();

    private int _CardPrice
    {
        get
        {
            if (freeCount == 0) return cardPrice;
            if (curFreeCount == freeCount - 1) return 0;
            return cardPrice;
        }
    }

    public bool IsRecycle { get { return recycle.IsMouseOver; } }

    private Queue<int> nextCardQueue = new Queue<int>();

    public void EnqueueCard(int cardId)
    {
        nextCardQueue.Enqueue(cardId);
    }

    public void SetFreeCount(int value)
    {
        freeCount = value;
    }

    public void IncreaseMaxCount(int value)
    {
        maxCardNumber += value;
        //SetCardPosition();
    }

    public void RemoveCard(int index)
    {
        _cardDeck.Remove(index);

    }

    public void AddCard(int index)
    {
        _cardDeck.Add(index);

    }

    private void UpdateDeckCount()
    {
        //if (deckCountText == null)
        //    return;

        //deckCountText.text = (_cardDeck.Count).ToString();
        //if (_cardDeck.Count < 10)
        //{
        //    cardCountImg.sprite = emptycardCountSprite;
        //    deckCountText.color = Color.red;
        //}
        //else
        //{
        //    cardCountImg.sprite = cardCountSprite;
        //    deckCountText.color = Color.white;
        //}

        //if (_cardDeck.Count <= 0)
        //    deckBtnImg.sprite = emptyDeckImg;
        //else
        //    deckBtnImg.sprite = originDeckImg;
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

        float magintude = (startPos - endPos).magnitude;
        points = Mathf.CeilToInt(magintude / 50f);

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
        if (hand_CardNumber >= 0 && hand_CardNumber <= maxCardNumber)
        {
            // 카드 개수가 0에서 10 사이일 때
            float startX = Mathf.Lerp(minPos_Y, maxPos_Y, (float)hand_CardNumber / maxCardNumber);
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
        if (hand_CardNumber >= 0 && hand_CardNumber <= maxCardNumber)
        {
            // 카드 개수가 0에서 10 사이일 때
            float startX = Mathf.Lerp(minPos_X, maxPos_X, (float)hand_CardNumber / maxCardNumber);
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
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        ISetCardPosition().Forget();
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
        //_cardDeck.Remove(target);
        Card card = new Card(DataManager.Instance.deck_Table[target], target);
        return card;
    }

    private Card ReturnDeck()
    {
        int random = _cardDeck[UnityEngine.Random.Range(0, _cardDeck.Count)];
        return ReturnDeck(random);
    }

    [SerializeField]
    FMODUnity.EventReference refusedSound;
    [SerializeField]
    FMODUnity.EventReference excutedSound;
    public FMODUnity.EventReference drawSound { get => excutedSound; }

    public void DrawDeck()
    {
        if (GameManager.Instance.drawLock)
            return;

        //if (hand_CardNumber >= maxCardNumber)
        //{
        //    GameManager.Instance.popUpMessage?.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_handfull"));
        //    refusedSound?.Post(gameObject);
        //    //AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
        //    return;
        //}

        if (GameManager.Instance.gold < _CardPrice)
        {
            GameManager.Instance.popUpMessage?.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_requireGold"));
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
            //AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            return;
        }

        if (_cardDeck.Count < 1)
        {
            //GameManager.Instance.popUpMessage?.ToastMsg("덱에 카드를 보충하십시오!");
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
            //AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            return;
        }

        GameManager.Instance.gold -= _CardPrice;
        if(nextCardQueue.Count > 0)
        {
            DrawCard(nextCardQueue.Dequeue());
        }
        else
            DrawCard();

        curFreeCount = curFreeCount != freeCount - 1 ? Mathf.Min(curFreeCount + 1, freeCount - 1) : 0;
        FMODUnity.RuntimeManager.PlayOneShot(excutedSound);
        //AudioManager.Instance.Play2DSound("Click_card_01", SettingManager.Instance._UIVolume);
    }

    private async UniTaskVoid ISetCardPosition()
    {
        //모든 카드들의 위치를 lerpTime에 걸쳐 조정
        //모든 카드들의 회전값을 lerpTime에 걸쳐 조정

        float elapsedTime = 0f;
        float lerpTime = 0.5f;
        List<Vector3> cardPos = GetCardPosition();
        List<Vector3> cardRot = CalculateRotation();
        Vector3[] startPositions = new Vector3[_cards.Count];
        Quaternion[] startRotations = new Quaternion[_cards.Count];
        for (int i = 0; i < _cards.Count; i++)
        {
            CardFramework card = _cards[i].GetComponent<CardFramework>();
            card.handIndex = i;
            startPositions[i] = _cards[i].position;
            startRotations[i] = _cards[i].rotation;
            CardUIEffect temp = _cards[i].GetComponent<CardUIEffect>();
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
                _cards[i].transform.position = Vector3.Lerp(startPositions[i], cardPos[i], t);

            for (int i = 0; i < _cards.Count; i++)
            {
                Vector3 targetDirection = cardRot[i];
                Quaternion targetRotation = UtilHelper.AlignUpWithVector(targetDirection);

                Quaternion currentRotation = Quaternion.Lerp(startRotations[i], targetRotation, t);
                _cards[i].transform.rotation = currentRotation;
            }

            await UniTask.Yield(cancellationTokenSource.Token);
        }
    }

    private void DrawTypeCard(List<int> cardPool, int drawNumber = 1)
    {
        if (cardPool == null)
            return;

        List<int> targetCards = new List<int>(cardPool);
        for (int i = 0; i < drawNumber; i++)
        {
            int randomIndex = targetCards[UnityEngine.Random.Range(0, targetCards.Count)];
            while (targetCards.Count != 0)
            {
                randomIndex = targetCards[UnityEngine.Random.Range(0, targetCards.Count)];
                if (_cardDeck.Contains(randomIndex))
                    break;
                targetCards.Remove(randomIndex);
            }

            DrawCard(randomIndex);
        }
    }

    private readonly List<string> mulliganList = new List<string>()
    {
        "c10006", "c10009", "c10011", "c11002", "c13002"
    };

    public void MulliganFixed()
    {
        foreach(var card in mulliganList)
        {
            //AddCard(DataManager.Instance.deckListIndex[card]);
            DrawCard(DataManager.Instance.deckListIndex[card]);
        }
    }

    public void Mulligan()
    {
        // 길타일 3
        // 방타일 1
        // 방조각 2
        List<int> pathPool = DataManager.Instance.pathCard_Indexs;
        DrawTypeCard(pathPool, 3);
        List<int> roomPool = new List<int>(DataManager.Instance.roomCard_Indexs);
        roomPool.RemoveAt(0);
        DrawTypeCard(roomPool);
        //List<int> trapPool = DataManager.Instance.TrapCard_Indexs;
        //DrawTypeCard(trapPool);
        //List<int> monsterPool = DataManager.Instance.MonsterCard_Indexs;
        //DrawTypeCard(monsterPool);
        DrawTypeCard(DataManager.Instance.roomPartCard_Index);
        DrawTypeCard(DataManager.Instance.environmentCard_Indexs);

    }

    public void DrawCard(int cardIndex, Transform startPos)
    {
        //if (_cardDeck.Count < 1) return;
        //if (!_cardDeck.Contains(cardIndex)) return;
        if (hand_CardNumber >= maxCardNumber)
        {
            CardController card = cards[0].GetComponentInChildren<CardController>();
            card?.RemoveCard(false).Forget();
        }

        InstantiateCard(ReturnDeck(cardIndex), startPos);
    }

    public void DrawCard(int cardIndex)
    {
        DrawCard(cardIndex, transform);
    }

    public void DrawCard()
    {
        if(_cardDeck.Count < 1) return;

        if(hand_CardNumber >= maxCardNumber)
        {
            CardController card = cards[0].GetComponentInChildren<CardController>();
            card?.RemoveCard(false).Forget();
        }

        InstantiateCard(ReturnDeck());
    }

    public void DiscardCard(Transform cardTransform, int cardId)
    {
        _cards.Remove(cardTransform);
        _handCards.Remove(cardId);
    }

    private void InstantiateCard(Card targetCard, Transform startPos = null)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Prefab/UI/Card_Frame");

        hand_CardNumber++;
        CardFramework card = Instantiate(cardPrefab, cardZone).GetComponent<CardFramework>();
        CardUIEffect cardUI = card.GetComponent<CardUIEffect>();
        card.Init(targetCard);
        cardUI?.DrawEffect();
        if (startPos == null)
            startPos = transform;
        card.transform.position = startPos.position;
        _cards.Add(card.transform);
        _handCards.Add(targetCard.cardIndex);
        SetCardPosition();
        UpdateDeckCount();
    }

    private void SetDeck()
    {
        _cardDeck = new List<int>();
        foreach(var data in DataManager.Instance.start_deckTable)
        {
            int cardNumber = Convert.ToInt32(data["startNumber"]);
            if (cardNumber == 0)
                continue;

            int index = DataManager.Instance.deckListIndex[data["id"].ToString()];

            Card cardCheck = new Card(DataManager.Instance.deck_Table[index], index);
            GameObject cardPrefab = UtilHelper.GetCardPrefab(cardCheck.cardType, cardCheck.cardPrefabName);
            if (cardPrefab == null)
                continue;

            for (int j = 0; j < cardNumber; j++)
                _cardDeck.Add(index);
        }
    }

    public void LoadData(List<int> cardIdes, List<int> deckLists)
    {
        if (!initState)
            Init();

        _cardDeck = new List<int>(deckLists);
        foreach(int id in cardIdes)
        {
            //AddCard(id);
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
        if (Input.GetKeyDown(SettingManager.Instance.key_Draw._CurKey))
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                DrawDeck();
        }

        if (curScreenSize != SettingManager.Instance.screenSize)
        {
            curScreenSize = SettingManager.Instance.screenSize;
            SetCardPosition();
        }
    }
}
