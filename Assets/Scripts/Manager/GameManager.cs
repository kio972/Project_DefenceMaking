using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum HerbType
{
    None,
    BlackHerb,
    PurpleHerb,
    WhiteHerb,
}

public class GameManager : IngameSingleton<GameManager>
{
    [SerializeField]
    private int stageNumber;
    [SerializeField]
    private float defaultSpeed = 100f;
    public float DefaultSpeed { get => defaultSpeed; }

    public float TotalTime { get => curWave.Value * 1440f + Timer; }

    public float InGameDeltaTime { get => Time.deltaTime * defaultSpeed * timeScale; }

    public float gameSpeed = 100f;

    public ReactiveProperty<float> timer { get; private set; } = new ReactiveProperty<float>(0f);
    public float Timer { get => timer.Value; }

    public int gold = 0;
    public ReactiveDictionary<HerbType, int> herbDic { get; private set; } = new ReactiveDictionary<HerbType, int>()
    {
        {HerbType.BlackHerb, 0},
        {HerbType.PurpleHerb, 0},
        {HerbType.WhiteHerb, 0}
    };
    //public int herb1 = 0;
    //public int herb2 = 0;
    //public int herb3 = 0;

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

    public CardSelection cardSelector;
    [SerializeField]
    private int selectInterval = 3;


    //public int king_Hp = 20;
    private bool dailyIncome = true;

    public ReactiveProperty<int> curWave { get; private set; } = new ReactiveProperty<int>(0); 
    public int CurWave
    {
        get
        {
            int loopVal = curWave.Value == -1 ? 1 : 0;
            return (loop * DataManager.Instance.waveLevelTable.Count) + curWave.Value + loopVal;
        }
    }

    public bool isInBattle = false;

    [SerializeField]
    FMODUnity.EventReference midBellSound;


    public ReactiveCollection<Adventurer> adventurersList = new ReactiveCollection<Adventurer>();

    public List<Battler> adventurer_entered_BossRoom = new List<Battler>();

    public ReactiveCollection<Monster> monsterList { get; private set; } = new ReactiveCollection<Monster>();
    public ReactiveCollection<MonsterSpawner> monsterSpawner { get; private set; } = new ReactiveCollection<MonsterSpawner>();

    public List<Trap> trapList = new List<Trap>();

    public PlayerBattleMain king;

    public float timeScale { get => _timeScale.Value; set => _timeScale.Value = value; }
    public ReactiveProperty<float> _timeScale = new ReactiveProperty<float>(1);

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
    public bool drawLock = false;
    public bool tileLock = false;
    public bool timerLock = false;
    public bool cameraLock = false;
    public bool saveLock = false;
    public bool rotateLock = false;
    public bool moveLock = false;

    public int loop = 0;

    [SerializeField]
    InGameUI ingameUI;
    public InGameUI _InGameUI { get => ingameUI; }

    private int totalMana;
    private int curMana;

    public int _TotalMana { get => totalMana; }
    public int _CurMana { get => curMana; set { curMana = value; } }

    public Adventurer LastSpawnedAdventurer;

    private HashSet<Battler> _holdBackedBattlers = new HashSet<Battler>();
    public HashSet<Battler> holdBackedABattlers { get => _holdBackedBattlers; }

    private List<System.Action<Battler, Battler>> battlerDeadEvent = new List<System.Action<Battler, Battler>>();

    public bool isBossOnMap
    {
        get
        {
            foreach(var enemy in adventurersList)
            {
                if(enemy.isBoss)
                    return true;
            }

            return false;
        }
    }

    public void InvokeBattlerDeadEvents(Battler battler, Battler attecker)
    {
        foreach (var curEvent in battlerDeadEvent)
            curEvent.Invoke(battler, attecker);
    }

    public void AddBattlerDeadEvent(System.Action<Battler, Battler> addEvent) => battlerDeadEvent.Add(addEvent);

    public void RemoveBattlerDeadEvent(System.Action<Battler, Battler> removeEvent) => battlerDeadEvent.Remove(removeEvent);

    public void CheckBattlerCollapsed()
    {
        foreach(Adventurer adventurer in adventurersList)
            adventurer.CheckTargetCollapsed();
        foreach(Monster monster in monsterList)
            monster.CheckTargetCollapsed();
        foreach (MonsterSpawner spawner in monsterSpawner)
            spawner.CheckTargetCollapsed();
    }

    public void UpdateTotalMana()
    {
        int totalMana = 0;
        foreach(TileNode node in NodeManager.Instance._ActiveNodes)
        {
            if (node.curTile == null || node.curTile.IsDormant)
                continue;

            totalMana += node.curTile.SupplyMana;
        }

        this.totalMana = totalMana;
    }

    public void IncreaseWave()
    {
        curWave.Value++;
    }

    public void SetWave(int val)
    {
        curWave.Value = val - 1;
        SkipDay();
    }

    //private void LoopWave()
    //{
    //    curWave = -1;
    //    isPause = true;
    //    StoryManager.Instance.EnqueueScript("Dan100");
    //}

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
            if (monster.curNode == tile)
                return true;
        }

        return false;
    }

    public bool IsAdventurererOnTile(TileNode tile)
    {
        foreach(Adventurer adventurer in adventurersList)
        {
            if (adventurer.curNode == tile)
                return true;
        }

        return false;
    }

    public void WinGame()
    {
        updateNeed = false;
        speedController.SetSpeedZero();

        ResultController result = FindObjectOfType<ResultController>(true);
        if (result != null)
            result.GameWin().Forget();
    }

    public void LoseGame()
    {
        updateNeed = false;
        speedController.SetSpeedZero();

        ResultController result = FindObjectOfType<ResultController>(true);
        if (result != null)
            result.GameDefeat().Forget();
    }

    private void UpdateBossRoom()
    {
        bool bossTileMove = true;
        if (adventurer_entered_BossRoom.Count > 0)
            bossTileMove = false;
        king.curNode.curTile.Movable = bossTileMove;
    }

    public void SkipDay()
    {
        if(timer.Value < 1350f)
            timer.Value = 1350f;
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
        if(timerLock)
            return;

        timer.Value += InGameDeltaTime;
        if (timer.Value > 720f && dailyIncome)
        {
            //gold += 50 + PassiveManager.Instance.income_Weight;
            //AudioManager.Instance.Play2DSound("Time_Over_CruchBell-01", SettingManager.Instance.fxVolume);
            dailyIncome = false;
        }

        if(timer.Value > 1440f)
        {
            //재화수급
            gold += 50 + PassiveManager.Instance.income_Weight;
            var keys = new List<HerbType>(herbDic.Keys);
            foreach (var herb in keys)
                herbDic[herb] += PassiveManager.Instance.herbSupplyDic[herb];

            timer.Value = 0f;
            dailyIncome = true;
            curWave.Value++;
            SetWaveSpeed(curWave.Value);
            waveController?.UpdateWaveText();
            if (!spawnLock && !allWaveSpawned)
            {
                //몬스터 웨이브 스폰
                waveController.SpawnWave(curWave.Value);
            }

            //이동가능타일 잠금
            //NodeManager.Instance.LockMovableTiles();

            FMODUnity.RuntimeManager.PlayOneShot(midBellSound);
            //AudioManager.Instance.Play2DSound("Time_Over_CruchBell-01", SettingManager.Instance._FxVolume * 0.5f);

            if(curWave.Value % selectInterval == 0)
                cardSelector?.StartCardSelect();
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

        if(float.TryParse(DataManager.Instance.timeRate_Table[wave]["time magnification"].ToString(), out defaultSpeed))
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
            monster.curHp = Mathf.Max(monster.curHp + healAmount, monster.curMaxHp);
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

        NodeManager.Instance.AddSetTileEvent((_) =>
        {
            UpdateTotalMana();
            return true;
        });
    }    

    public void Init()
    {
        if (mapBuilder == null)
            return;

        mapBuilder.Init();
        SpawnKing();
        SetWaveSpeed();

        ingameUI?.Init();
        //AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);

        cardDeckController.Init();
        //cardDeckController.Invoke("Mulligan", 1f);
        //cardDeckController.Invoke("MulliganFixed", 1f);
        speedController.SetSpeedNormal(false);
        waveController.SpawnWave(curWave.Value);
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

        curWave.Value = data.curWave;
        timer.Value = data.curTime;
        gold = data.gold;
        herbDic[HerbType.BlackHerb] = data.herb1;
        herbDic[HerbType.PurpleHerb] = data.herb2;
        herbDic[HerbType.WhiteHerb] = data.herb3;

        foreach (TileData tile in data.tiles)
            mapBuilder.SetTile(tile);
        foreach (TileData tile in data.environments)
            mapBuilder.SetTile(tile, true);
        foreach (TileData tile in data.hiddenTiles)
            mapBuilder.SetHiddenTile(tile).Forget();
        mapBuilder.curTileSetCount = data.nextHiddenTileCount;
        mapBuilder.StartHiddenTileCounter();

        foreach (SpawnerData spawnerData in data.spawners)
        {
            MonsterSpawner spawner = BattlerPooling.Instance.SetSpawner(NodeManager.Instance.FindNode(spawnerData.row, spawnerData.col), spawnerData.spawnerId, NodeManager.Instance.FindRoom(spawnerData.row, spawnerData.col));
            spawner._CurCoolTime = spawnerData.spawnerCool;
        }

        NodeManager.Instance.SetGuideState(GuideState.None);

        BattlerData king = data.devil;
        NodeManager.Instance.endPoint = NodeManager.Instance.FindNode(king.row, king.col);
        SpawnKing();
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

        SetWaveSpeed(curWave.Value);
        waveController.SpawnWave(curWave.Value);
        waveController.UpdateWaveText();

        research?.LoadData(data);
        shop?.LoadData(data);
        QuestManager.Instance.LoadGame(data);
        SetWaveSpeed(curWave.Value);
        speedController.SetSpeedZero();

        this.king.LoadData(king);
        isInit = true;
    }

    private void OnDestroy()
    {
        EffectPooling.Instance.StopAllEffect();
    }
}
