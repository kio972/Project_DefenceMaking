using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public interface ISpawnTimeModifier
{
    float spawnTimeWeight { get; }

}

public class MonsterSpawner : MonoBehaviour, IDestructableObjectKind
{
    [SerializeField]
    private Image bgImg;

    public ReactiveProperty<float> _spawnRate { get; private set; } = new ReactiveProperty<float>();

    private float _spawnCoolTime;
    private float curSpawnCoolTime;

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

    public bool isSpawnLocked = false;

    public bool spawnerActivatedState { get => !isSpawnLocked && isUpdate; }

    private HashSet<ISpawnTimeModifier> _spawntimeModifiers;

    private void CalculateSpawntime()
    {
        curSpawnCoolTime = _spawnCoolTime;

        if (_spawntimeModifiers.Count == 0)
            return;

        Dictionary<Type, float> modifierMinValues = new Dictionary<Type, float>();
        foreach (ISpawnTimeModifier modifier in _spawntimeModifiers)
        {
            Type modifierType = modifier.GetType();
            float weight = modifier.spawnTimeWeight;

            // 해당 타입이 이미 존재하면, 더 작은 값만 유지
            if (modifierMinValues.TryGetValue(modifierType, out float existingWeight))
            {
                if (weight < existingWeight)
                    modifierMinValues[modifierType] = weight;
            }
            else
            {
                modifierMinValues[modifierType] = weight;
            }
        }

        // 가장 낮은 값들만 연산
        foreach (float minWeight in modifierMinValues.Values)
        {
            curSpawnCoolTime *= minWeight;
        }
    }

    public void SetSpawntimeModifier(ISpawnTimeModifier modifier, bool value)
    {
        if (value)
            _spawntimeModifiers.Add(modifier);
        else
            _spawntimeModifiers.Remove(modifier);

        CalculateSpawntime();
    }

    //public void ModifyCoolTime(float rate)
    //{
    //    _spawnCoolTime *= rate;
    //}

    public void CheckTargetCollapsed()
    {
        isUpdate = NodeManager.Instance.FindPath(_tile.curNode, NodeManager.Instance.endPoint) != null;
    }

    public void UpdatePassive()
    {
        Dictionary<string, object> data = DataManager.Instance.battler_Table[monsterIndex];
        this.requiredMana = Convert.ToInt32(data["requiredMagicpower"]);
        this._spawnCoolTime = Convert.ToInt32(data["duration"]);
        
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        this.requiredMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        this._spawnCoolTime *= ((100 - PassiveManager.Instance._MonsterTypeSummonSpeed_Weight[(int)monsterType]) / 100);
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
        this._spawnCoolTime = Convert.ToInt32(data["spawnTime"]);
        curCoolTime = _spawnCoolTime;
        
        isUpdate = true;
        curRoom?.SetSpawner(this, true);
        _spawntimeModifiers = new HashSet<ISpawnTimeModifier>();
        CalculateSpawntime();
        GameManager.Instance.monsterSpawner.Add(this);
    }

    private void Update()
    {
        if (!isUpdate || isEmpty || isSpawnLocked)
            return;

        if(curCoolTime > curSpawnCoolTime)
        {
            _spawnRate.Value = 1;
            if (GameManager.Instance._CurMana + requiredMana > GameManager.Instance._TotalMana)
                return;

            BattlerPooling.Instance.SpawnMonster(targetName, _tile.curNode);
            curCoolTime = 0f;
        }

        curCoolTime += GameManager.Instance.InGameDeltaTime;
        _spawnRate.Value = curCoolTime / curSpawnCoolTime;
    }
}
