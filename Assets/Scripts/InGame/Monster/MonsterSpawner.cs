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

    public void Dead()
    {
        isUpdate = false;
    }

    public void Init(TileNode curNode, string targetName)
    {
        this.tile = curNode;
        transform.position = curNode.transform.position;
        this.targetName = targetName;
        

        Dictionary<string, object> data = DataManager.Instance.Battler_Table[UtilHelper.Find_Data_Index(targetName, DataManager.Instance.Battler_Table, "name")];
        Sprite illur = SpriteList.Instance.LoadSprite(data["prefab"].ToString());
        bgImg.sprite = illur;
        fillImg.sprite = illur;

        this.spawnCoolTime = Convert.ToInt32(data["duration"]);
        curCoolTime = spawnCoolTime;
        
        isUpdate = true;
    }

    private void Update()
    {
        if (!isUpdate)
            return;

        curCoolTime += Time.deltaTime * GameManager.Instance.timeScale;
        fillImg.fillAmount = 1 - (curCoolTime / spawnCoolTime);

        if(curCoolTime > spawnCoolTime)
        {
            BattlerPooling.Instance.SpawnMonster(targetName, tile);
            curCoolTime = 0f;
            fillImg.fillAmount = 1f;
        }
    }
}
