using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveManager : IngameSingleton<PassiveManager>
{
    public int monsterHp_Weight = 0;
    public float enemySlow_Weight = 0;
    public int income_Weight = 0;
    public Dictionary<TileNode, float> slowedTile = new Dictionary<TileNode, float>();

    public float GetSlowRate(TileNode curNode)
    {
        float slowRate = 0;

        foreach(TileNode neighborNode in curNode.neighborNodeDic.Values)
        {
            if (slowedTile.ContainsKey(neighborNode))
            {
                if (slowedTile[neighborNode] > slowRate)
                    slowRate = slowedTile[neighborNode];
            }
        }

        return slowRate;
    }

    public void Init()
    {
        //세이브파일에서 수치 받아오는 함수 추가예정
    }
}
