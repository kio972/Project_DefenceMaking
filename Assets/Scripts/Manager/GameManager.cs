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

    public int startCardNumber = 6;
    public CardDeckController cardDeckController;
    public GameSpeedController speedController;
    public CameraController cameraController;
    public WaveController waveController;
    public MapBuilder mapBuilder;
    public Canvas cameraCanvas;

    public int king_Hp = 20;
    private bool dailyIncome = true;

    private int curWave = 0; 
    public int CurWave { get => curWave; }

    public bool isInBattle = false;

    public List<Adventurer> adventurersList = new List<Adventurer>();

    public List<Battler> adventurer_entered_BossRoom = new List<Battler>();

    public List<Monster> monsterList = new List<Monster>();

    public PlayerBattleMain king;

    public float timeScale;

    private bool updateNeed = true;

    private bool allWaveSpawned = false;

    private bool isInit = false;
    public bool IsInit { get => isInit; }


    public PopUpMessage popUpMessage;

    public bool isPause = false;

    public void SetCharAnimPause()
    {
        foreach(Battler battler in adventurersList)
        {
            if (battler._Animator.GetBool("Move"))
                battler._Animator.SetBool("Move", false);
        }

        foreach (Battler battler in monsterList)
        {
            if (battler._Animator.GetBool("Move"))
                battler._Animator.SetBool("Move", false);
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

    private void WinGame()
    {
        updateNeed = false;
        speedController.SetSpeedZero();

        ResultController result = FindObjectOfType<ResultController>(true);
        if (result != null)
            result.GameWin();
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
        king.CurTile.curTile.movable = bossTileMove;
    }

    public void SkipDay()
    {
        if(timer < 1350f)
            timer = 1350f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updateNeed) return;

        if(king.isDead)
        {
            LoseGame();
            return;
        }

        if (allWaveSpawned && adventurersList.Count == 0)
        {
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

            curWave++;
            //몬스터 웨이브 스폰
            if (!waveController.SpawnWave(curWave))
                allWaveSpawned = true;

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

    public void SetStroyMode(bool value)
    {
        if (value)
            speedController.SetSpeedZero();
        else
            speedController.SetSpeedPrev();

        isPause = value;
    }

    private void Start()
    {
        mapBuilder.Init();
        SpawnKing();
        SetWaveSpeed();

        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);

        cardDeckController.Mulligan();

        speedController.SetSpeedZero();
        waveController.SpawnWave(curWave);
        NodeManager.Instance.SetGuideState(GuideState.None);

        if (popUpMessage == null)
            popUpMessage = FindObjectOfType<PopUpMessage>(true);


        StoryManager.Instance.EnqueueScript("Dan000");

        isInit = true;
    }
}
