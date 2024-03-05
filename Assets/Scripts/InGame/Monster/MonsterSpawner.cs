using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Image fillImg;

    private float spawnCoolTime;

    private float curCoolTime;

    private TileNode tile;

    private bool isUpdate = false;

    private string targetName;

    private int requiredMana;

    private int monsterIndex;

    public void UpdatePassive()
    {
        Dictionary<string, object> data = DataManager.Instance.Battler_Table[monsterIndex];
        this.requiredMana = Convert.ToInt32(data["requiredMagicpower"]);
        this.spawnCoolTime = Convert.ToInt32(data["duration"]);
        
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        this.requiredMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        this.spawnCoolTime *= ((100 - PassiveManager.Instance._MonsterTypeSummonSpeed_Weight[(int)monsterType]) / 100);
    }

    public void Dead()
    {
        isUpdate = false;
        GameManager.Instance.monsterSpawner.Remove(this);

        Destroy(this.gameObject);
    }

    public void Init(TileNode curNode, string targetName)
    {
        this.tile = curNode;
        transform.position = curNode.transform.position;
        this.targetName = targetName;
        curNode.curTile.AddSpawner(this);

        monsterIndex = UtilHelper.Find_Data_Index(targetName, DataManager.Instance.Battler_Table, "name");
        Dictionary<string, object> data = DataManager.Instance.Battler_Table[monsterIndex];
        Sprite illur = SpriteList.Instance.LoadSprite(data["prefab"].ToString());
        bgImg.sprite = illur;
        fillImg.sprite = illur;
        this.requiredMana = Convert.ToInt32(data["requiredMagicpower"]);
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        this.requiredMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];
        this.spawnCoolTime = Convert.ToInt32(data["duration"]);
        curCoolTime = spawnCoolTime;
        
        isUpdate = true;
        GameManager.Instance.monsterSpawner.Add(this);
    }

    private void Update()
    {
        if (!isUpdate)
            return;

        if(curCoolTime > spawnCoolTime)
        {
            fillImg.fillAmount = 0f;
            if (GameManager.Instance._CurMana + requiredMana > GameManager.Instance._TotalMana)
                return;

            BattlerPooling.Instance.SpawnMonster(targetName, tile);
            curCoolTime = 0f;
        }

        curCoolTime += Time.deltaTime * GameManager.Instance.timeScale;
        fillImg.fillAmount = 1 - (curCoolTime / spawnCoolTime);
    }
}
