using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : IngameSingleton<GameManager>
{
    [SerializeField]
    private int stageNumber;
    private float defaultSpeed = 100f;
    public float DefaultSpeed { get => defaultSpeed; }

    public float gameSpeed = 100f;

    float timer = 0f;
    public float Timer { get => timer; }

    public int gold = 0;
    public int herb1 = 0;
    public int herb2 = 0;
    public int herb3 = 0;

    public int startCardNumber = 6;
    public CardDeckController cardDeckController;
    public GameSpeedController speedController;
    public CameraController cameraController;
    public WaveController waveController;
    public MapBuilder mapBuilder;
    public Canvas cameraCanvas;
    public Canvas worldCanvas;

    public int king_Hp = 20;
    private bool dailyIncome = true;

    private int curWave = 0; 
    public int CurWave { get => curWave; }

    public bool isInBattle = false;

    public List<Adventurer> adventurersList = new List<Adventurer>();

    public List<Battler> adventurer_entered_BossRoom = new List<Battler>();

    private List<Monster> monsterList = new List<Monster>();

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

    public int loop = 0;

    [SerializeField]
    InGameUI ingameUI;

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

        this.totalMana = totalMana;
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

    private void WinGame()
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

        if (allWaveSpawned && adventurersList.Count == 0)
        {
            allWaveSpawned = false;
            if (waveController.HaveDelayedTarget)
                return;
            
            loop++;
            if (DataManager.Instance.BuffTable.ContainsKey(loop))
                LoopWave();
            else
                WinGame();

            return;
        }

        UpdateBossRoom();

        timer += Time.deltaTime * defaultSpeed * timeScale;
        if(timer > 720f && dailyIncome)
        {
            gold += 100 + PassiveManager.Instance.income_Weight;
            AudioManager.Instance.Play2DSound("Alert_time", SettingManager.Instance._FxVolume);
            dailyIncome = false;
        }

        if(timer > 1440f)
        {
            //재화수급
            gold += 100 + PassiveManager.Instance.income_Weight;
            timer = 0f;
            dailyIncome = true;

            if(!spawnLock)
            {
                curWave++;
                //몬스터 웨이브 스폰
                waveController.SpawnWave(curWave);
            }

            //이동가능타일 잠금
            NodeManager.Instance.LockMovableTiles();

            AudioManager.Instance.Play2DSound("Alert_time", SettingManager.Instance._FxVolume);
        }
    }

    private void SpawnKing()
    {
        PlayerBattleMain king = Resources.Load<PlayerBattleMain>("Prefab/Monster/King");
        if(king != null)
        {
            king = Instantiate(king);
            king.Init();
            this.king = king;
        }
    }

    private void SetWaveSpeed()
    {
        if (stageNumber == 0)
        {
            defaultSpeed = gameSpeed / 60f;
            return;
        }

        float.TryParse(DataManager.Instance.TimeRate_Table[stageNumber - 1]["time magnification"].ToString(), out defaultSpeed);
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

    public void Init()
    {
        mapBuilder.Init();
        SpawnKing();
        SetWaveSpeed();
        if (ingameUI == null)
            ingameUI = FindObjectOfType<InGameUI>();
        ingameUI?.Init();
        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);

        cardDeckController.Invoke("Mulligan", 1f);
        speedController.SetSpeedZero();
        waveController.SpawnWave(curWave);
        NodeManager.Instance.SetGuideState(GuideState.None);

        if (popUpMessage == null)
            popUpMessage = FindObjectOfType<PopUpMessage>(true);

        isInit = true;
    }
}
