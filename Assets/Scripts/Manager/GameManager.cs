using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : IngameSingleton<GameManager>
{
    [SerializeField]
    private int stageNumber;
    [SerializeField]
    private float defaultSpeed = 100f;
    public float DefaultSpeed { get => defaultSpeed; }

    public float InGameDeltaTime { get => Time.deltaTime * defaultSpeed * timeScale; }

    public float gameSpeed = 100f;

    float timer = 0f;
    public float Timer { get => timer; }

    public int gold = 0;
    public int herb1 = 0;
    public int herb2 = 0;
    public int herb3 = 0;

    public int herb1Max = 0;
    public int herb2Max = 0;
    public int herb3Max = 0;

    public int startCardNumber = 6;
    public CardDeckController cardDeckController;
    public GameSpeedController speedController;
    public CameraController cameraController;
    public WaveController waveController;
    public MapBuilder mapBuilder;
    public Canvas cameraCanvas;
    public Canvas worldCanvas;
    public NotificationControl notificationBar;

    public ResearchMainUI research;
    public ShopUI shop;

    //public int king_Hp = 20;
    private bool dailyIncome = true;

    private int curWave = 0; 
    public int CurWave
    {
        get
        {
            int loopVal = curWave == -1 ? 1 : 0;
            return (loop * DataManager.Instance.WaveLevelTable.Count) + curWave + loopVal;
        }
    }

    public bool isInBattle = false;


    public ReactiveCollection<Adventurer> adventurersList = new ReactiveCollection<Adventurer>();

    public List<Battler> adventurer_entered_BossRoom = new List<Battler>();

    private List<Monster> monsterList = new List<Monster>();
    public List<Monster> _MonsterList { get => monsterList; }
    public List<MonsterSpawner> monsterSpawner = new List<MonsterSpawner>();

    public List<Trap> trapList = new List<Trap>();

    public PlayerBattleMain king;

    public float timeScale;

    private bool updateNeed = true;

    private bool allWaveSpawned = false;
    public bool AllWaveSpawned { set { allWaveSpawned = value; } }

    private bool isInit = false;
    public bool IsInit { get => isInit; }

    public int researchLevel = 1;

    public PopUpMessage popUpMessage;

    public bool isPause = false;
    public bool speedLock = false;
    public bool spawnLock = false;
    public bool cardLock = false;
    public bool tileLock = false;

    public int loop = 0;

    [SerializeField]
    InGameUI ingameUI;
    public InGameUI _InGameUI { get => ingameUI; }

    private int totalMana;
    private int curMana;

    public int _TotalMana { get => totalMana; }
    public int _CurMana { get => curMana; set { curMana = value; } }

    public void UpdateTotalMana()
    {
        int totalMana = 0;
        foreach(TileNode node in NodeManager.Instance._ActiveNodes)
        {
            if (node.curTile == null || node.curTile.IsDormant)
                continue;

            totalMana += node.curTile.RoomMana;
        }

        this.totalMana = totalMana + PassiveManager.Instance.GetAdditionalMana();
    }

    public void IncreaseWave()
    {
        curWave++;
    }

    public void SetWave(int val)
    {
        curWave = val - 1;
        SkipDay();
    }

    private void LoopWave()
    {
        curWave = -1;
        isPause = true;
        StoryManager.Instance.EnqueueScript("Dan100");
    }

    public void SetMonseter(Monster monster, bool value)
    {
        if (monster == null)
            return;

        if (value)
        {
            monsterList.Add(monster);
            curMana += monster._RequiredMana;
        }
        else
        {
            monsterList.Remove(monster);
            curMana -= monster._RequiredMana;
        }

    }

    public bool IsMonsterOnTile(TileNode tile)
    {
        foreach (Monster monster in monsterList)
        {
            if (monster.CurTile == tile)
                return true;
        }

        return false;
    }

    public bool IsAdventurererOnTile(TileNode tile)
    {
        foreach(Adventurer adventurer in adventurersList)
        {
            if (adventurer.CurTile == tile)
                return true;
        }

        return false;
    }

    private IEnumerator WinTextWait()
    {
        StoryManager.Instance.EnqueueScript("Dan900");

        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        ResultController result = FindObjectOfType<ResultController>(true);
        if (result != null)
            result.GameWin();
    }

    public void WinGame()
    {
        updateNeed = false;
        speedController.SetSpeedZero();
        StartCoroutine(WinTextWait());
        
        
    }

    public void LoseGame()
    {
        updateNeed = false;
        speedController.SetSpeedZero();

        ResultController result = FindObjectOfType<ResultController>(true);
        if (result != null)
            result.GameDefeat();
    }

    private void UpdateBossRoom()
    {
        bool bossTileMove = true;
        if (adventurer_entered_BossRoom.Count > 0)
            bossTileMove = false;
        king.CurTile.curTile.Movable = bossTileMove;
    }

    public void SkipDay()
    {
        if(timer < 1350f)
            timer = 1350f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit) return;

        if (!updateNeed) return;

        if (isPause) return;

        if(king.isDead)
        {
            LoseGame();
            return;
        }

        //if (allWaveSpawned && adventurersList.Count == 0)
        //{
        //    allWaveSpawned = false;
        //    if (waveController.HaveDelayedTarget)
        //        return;
            
        //    loop++;
        //    if (DataManager.Instance.BuffTable.ContainsKey(loop))
        //        LoopWave();
        //    else
        //        WinGame();

        //    return;
        //}

        UpdateBossRoom();

        timer += InGameDeltaTime;
        if (timer > 720f && dailyIncome)
        {
            gold += 50 + PassiveManager.Instance.income_Weight;
            AudioManager.Instance.Play2DSound("Alert_time", SettingManager.Instance._FxVolume);
            dailyIncome = false;
        }

        if(timer > 1440f)
        {
            //��ȭ����
            gold += 50 + PassiveManager.Instance.income_Weight;
            timer = 0f;
            dailyIncome = true;
            curWave++;
            SetWaveSpeed(curWave);
            waveController?.UpdateWaveText();
            if (!spawnLock && !allWaveSpawned)
            {
                //���� ���̺� ����
                waveController.SpawnWave(curWave);
            }

            //�̵�����Ÿ�� ���
            //NodeManager.Instance.LockMovableTiles();

            AudioManager.Instance.Play2DSound("Alert_time", SettingManager.Instance._FxVolume);
        }
    }

    public void SpawnKing()
    {
        PlayerBattleMain king = Resources.Load<PlayerBattleMain>("Prefab/Monster/King");
        if(king != null)
        {
            king = Instantiate(king);
            king.Init();
            this.king = king;
        }
    }

    private void SetWaveSpeed(int wave = 0)
    {
        //if (stageNumber == 0)
        //{
        //    defaultSpeed = gameSpeed / 60f;
        //    return;
        //}

        float.TryParse(DataManager.Instance.TimeRate_Table[wave]["time magnification"].ToString(), out defaultSpeed);
        defaultSpeed = defaultSpeed / 60f;
    }

    public void SetPause(bool value)
    {
        if (value)
            speedController.SetSpeedZero();
        else
            speedController.SetSpeedPrev();

        isPause = value;
    }

    public void HealAlly(int healAmount)
    {
        foreach(Monster monster in monsterList)
        {
            monster.curHp = Mathf.Max(monster.curHp + healAmount, monster.maxHp);
        }
    }

    public void Awake()
    {
        if (ingameUI == null)
            ingameUI = FindObjectOfType<InGameUI>();

        if (popUpMessage == null)
            popUpMessage = FindObjectOfType<PopUpMessage>(true);

        if (research == null)
            research = FindObjectOfType<ResearchMainUI>(true);

        if (shop == null)
            shop = FindObjectOfType<ShopUI>(true);
    }    

    public void Init()
    {
        mapBuilder.Init();
        SpawnKing();
        SetWaveSpeed();

        ingameUI?.Init();
        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);

        cardDeckController.Init();
        cardDeckController.Invoke("Mulligan", 1f);
        speedController.SetSpeedZero();
        waveController.SpawnWave(curWave);
        NodeManager.Instance.SetGuideState(GuideState.None);

        if (popUpMessage == null)
            popUpMessage = FindObjectOfType<PopUpMessage>(true);

        isInit = true;
    }

    public void LoadGame(PlayerData data)
    {
        if(data == null)
        {
            Init();
            return;
        }

        ingameUI?.Init(false);

        curWave = data.curWave;
        timer = data.curTime;
        gold = data.gold;
        herb1 = data.herb1;
        herb2 = data.herb2;
        herb3 = data.herb3;

        foreach (TileData tile in data.tiles)
            mapBuilder.SetTile(tile);
        foreach (TileData tile in data.environments)
            mapBuilder.SetTile(tile, true);

        NodeManager.Instance.SetGuideState(GuideState.None);

        BattlerData king = data.devil;
        NodeManager.Instance.endPoint = NodeManager.Instance.FindNode(king.row, king.col);
        SpawnKing();
        this.king.curHp = king.curHp;

        cardDeckController.LoadData(data.cardIdes, data.deckLists);

        foreach(BattlerData enemy in data.enemys)
        {
            Adventurer target = BattlerPooling.Instance.SpawnAdventurer(enemy.id, "id");
            target.LoadData(enemy);
        }

        foreach (BattlerData ally in data.allies)
        {
            Monster target = BattlerPooling.Instance.SpawnMonster(ally.id, NodeManager.Instance.FindNode(ally.row, ally.col), "id");
            target.LoadData(ally);
        }

        waveController.SpawnWave(curWave);

        research.LoadData(data);
        shop.LoadData(data);
        QuestManager.Instance.LoadGame(data);
        SetWaveSpeed(curWave);
        isInit = true;
    }
}
