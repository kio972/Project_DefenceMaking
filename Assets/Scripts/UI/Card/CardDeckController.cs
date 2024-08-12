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
    //�ʱ� ī�� ����
    public int hand_CardNumber = 0;
    //�ִ� ī�� ����
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

    private int freeCount = 0;
    private int curFreeCount = 0;

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

    public void SetFreeCount(int value)
    {
        freeCount = value;
    }

    public void IncreaseMaxCount(int value)
    {
        maxCardNumber += value;
        //SetCardPosition();
    }

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
        // rotDir�� ���̿� ���� amplitude �����ϴ� �ڵ� �ʿ�
        float minAmp = 0;
        float maxAmp = 300;
        float minDist = 0;
        float maxDist = 2000;
        float distance = rotDir.magnitude;
        float normalizedDist = Mathf.InverseLerp(minDist, maxDist, distance);
        // ����ȭ�� �Ÿ� ���� �ּ������� �ִ����� ������ ������ ����
        amplitude = Mathf.Lerp(minAmp, maxAmp, normalizedDist);
        // �ּ����� ~ �ִ�����
        // �ּҰŸ� ~ �ִ�Ÿ�
        float invert = 1f;
        if (endPos.x > startPos.x)
            invert *= -1f;

        for (int i = 0; i < points; ++i)
        {
            // i�� 0.0-1.0 ������ ������ ����ȭ
            float t = (float)i / (points - 1);

            Vector2 point = Vector2.Lerp(startPos, endPos, t);

            //modifyValue��ŭ point�� ��ǥ���� ����
            Vector2 modifyValue = new Vector2(0, amplitude * Mathf.Sin(2 * Mathf.PI * t * 0.5f) * invert);
            //modifyValue�� rotDir��ŭ ȸ��
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
        //ȸ�������
        //������Ʈ -> ��������Ʈ�� ���⺤�� / 2�� ������Ʈ�� ���� ��ġ
        //�� ��ġ -> ����ġ�� ���� ���⺤�Ϳ� �� transform.up�� �����ϰ� �ϵ��� roation ����
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
            // ī�� ������ 0���� 10 ������ ��
            float startX = Mathf.Lerp(minPos_Y, maxPos_Y, (float)hand_CardNumber / maxCardNumber);
            return startX;
        }
        else
        {
            // ī�� ������ ������ ��� �� �⺻���� 100�� ��ȯ�ϰų� ���� ó���� �����մϴ�.
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
        //���� ī�� ������ 0���� 10�ӿ� ���� 100 ~ 450���� ��ȯ
        if (hand_CardNumber >= 0 && hand_CardNumber <= maxCardNumber)
        {
            // ī�� ������ 0���� 10 ������ ��
            float startX = Mathf.Lerp(minPos_X, maxPos_X, (float)hand_CardNumber / maxCardNumber);
            return startX;
        }
        else
        {
            // ī�� ������ ������ ��� �� �⺻���� 100�� ��ȯ�ϰų� ���� ó���� �����մϴ�.
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
            // i�� 0.0-1.0 ������ ������ ����ȭ
            float t = (float)i / (points - 1);
            // start���� end ��ġ���� points ������ ���� �����ϰ� ��ġ
            float x = Mathf.Lerp(ReturnStartX(), ReturnStartY(), t);
            // 2*Mathf.PI = 360�̰�, t�� 0.0~1.0 ������ ���̱� ������ �� ���� ���ϸ� 1 ������ ���� �׷����� �ϼ��ǰ�,
            // frequency�� ���ϱ� ������ frequency ���� ���� �������� �����ȴ�.
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
            GameManager.Instance.popUpMessage?.ToastMsg("�� �а� ���� á���ϴ�");
            return;
        }

        if (GameManager.Instance.gold < _CardPrice)
        {
            GameManager.Instance.popUpMessage?.ToastMsg("��尡 �����մϴ�");
            return;
        }

        GameManager.Instance.gold -= _CardPrice;
        DrawCard();
        curFreeCount = curFreeCount != freeCount - 1 ? Mathf.Min(curFreeCount + 1, freeCount - 1) : 0;
        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }

    private IEnumerator ISetCardPosition()
    {
        //��� ī����� ��ġ�� lerpTime�� ���� ����
        //��� ī����� ȸ������ lerpTime�� ���� ����

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
            CardUIEffect temp = cards[i].GetComponent<CardUIEffect>();
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
        // ��Ÿ�� 3
        // ��Ÿ�� 1
        // ������ 2
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
        GameObject cardPrefab = Resources.Load<GameObject>("Prefab/UI/Card_Frame");

        hand_CardNumber++;
        CardFramework card = Instantiate(cardPrefab, cardZone).GetComponent<CardFramework>();
        CardUIEffect cardUI = card.GetComponent<CardUIEffect>();
        card.Init(targetCard);
        cardUI?.DrawEffect();
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
