using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MonsterSpawner : MonoBehaviour, IDestructableObjectKind
{
    [SerializeField]
    private Image bgImg;

    public ReactiveProperty<float> _spawnRate { get; private set; } = new ReactiveProperty<float>();

    private float spawnCoolTime;

    private float curCoolTime;
    public float _CurCoolTime { get => curCoolTime; set => curCoolTime = value; }

    //private TileNode tile;
    //public TileNode CurTile { get => tile; }

    private Tile _tile;

    public Tile curTile { get => _tile; }

    private bool isUpdate = false;

    private string targetName;
    public string _TargetName { get => targetName; }
    public string _TargetKey
    {
        get
        {
            string key = targetName.Split('_')[0];
            string number = DataManager.Instance.battler_Table[monsterIndex]["id"].ToString();
            number = number[number.Length - 1].ToString();
            return key + number;
        }
    }

    private int requiredMana;
    public int _RequiredMana { get => requiredMana; }

    private int monsterIndex;

    private CompleteRoom curRoom;

    public bool isEmpty = false;

    public void CheckTargetCollapsed()
    {
        isUpdate = PathFinder.FindPath(_tile.curNode, NodeManager.Instance.endPoint) != null;
    }

    public void UpdatePassive()
    {
        Dictionary<string, object> data = DataManager.Instance.battler_Table[monsterIndex];
        this.requiredMana = Convert.ToInt32(data["requiredMagicpower"]);
        this.spawnCoolTime = Convert.ToInt32(data["duration"]);
        
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        this.requiredMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        this.spawnCoolTime *= ((100 - PassiveManager.Instance._MonsterTypeSummonSpeed_Weight[(int)monsterType]) / 100);
    }

    public void DestroyObject()
    {
        isUpdate = false;
        curRoom?.SetSpawner(this, false);
        GameManager.Instance.monsterSpawner.Remove(this);
        curTile.RemoveObject();
        Destroy(this.gameObject);
    }

    public void SetTileInfo(Tile tile) => _tile = tile;

    public void Init(TileNode curNode, string targetName, CompleteRoom room)
    {
        SetTileInfo(curNode.curTile);
        this.curRoom = room;
        transform.position = curNode.transform.position;
        this.targetName = targetName;
        curNode.curTile.SetObject(this);

        monsterIndex = UtilHelper.Find_Data_Index(targetName, DataManager.Instance.battler_Table, "name");
        Dictionary<string, object> data = DataManager.Instance.battler_Table[monsterIndex];
        Sprite illur = SpriteList.Instance.LoadSprite(data["prefab"].ToString() + "_Spawner");
        if(bgImg != null)
            bgImg.sprite = illur;
        //fillImg.sprite = illur;
        this.requiredMana = Convert.ToInt32(data["requiredMagicpower"]);
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        this.requiredMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        this.spawnCoolTime = Convert.ToInt32(data["spawnTime"]);
        curCoolTime = spawnCoolTime;
        
        isUpdate = true;
        GameManager.Instance.monsterSpawner.Add(this);
        curRoom?.SetSpawner(this, true);
    }

    private void Update()
    {
        if (!isUpdate || isEmpty)
            return;

        if(curCoolTime > spawnCoolTime)
        {
            _spawnRate.Value = 1;
            if (GameManager.Instance._CurMana + requiredMana > GameManager.Instance._TotalMana)
                return;

            BattlerPooling.Instance.SpawnMonster(targetName, _tile.curNode);
            curCoolTime = 0f;
        }

        curCoolTime += GameManager.Instance.InGameDeltaTime;
        _spawnRate.Value = curCoolTime / spawnCoolTime;
    }
}
