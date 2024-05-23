using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct WaveData
{
    public int number;
    public string adventurerName;

    public WaveData(string adventurerName, int number)
    {
        this.adventurerName = adventurerName;
        this.number = number;
    }
}

public struct SpawnData
{
    public int order;
    public string target;
    public SpawnData(int order, string target)
    {
        this.order = order;
        this.target = target;
    }
}

public class WaveController : MonoBehaviour
{
    private List<SpawnData> curWaves = new List<SpawnData>();

    [SerializeField]
    private TextMeshProUGUI waveText;
    [SerializeField]
    private WaveGauge waveFill;

    public float WaveProgress { get { return waveFill.WaveRate; } }

    public void AddDelayedTarget(string adventurerName)
    {
        curWaves.Add(new SpawnData(curWaves.Count + 1, adventurerName));
    }

    private float CalSpawnWaitTime(int allAmount, float restrictTime = 720f)
    {
        float spawnTime = 2f * GameManager.Instance.DefaultSpeed;
        restrictTime = 720;
        if (allAmount * spawnTime > restrictTime)
            spawnTime = restrictTime / allAmount;

        return spawnTime;
    }

    public List<WaveData> SetWaveData(int waveIndex)
    {
        //List<Dictionary<string, object>> wave_Table에서
        //key값이 "level"인 인덱스를 찾고
        List<int> indexList = new List<int>();

        for(int i = 0; i < DataManager.Instance.Wave_Table.Count; i++)
        {
            if (Convert.ToInt32(DataManager.Instance.Wave_Table[i]["level"]) == waveIndex)
                indexList.Add(i);
        }

        List<WaveData> curWave = new List<WaveData>();
        foreach(int i in indexList)
        {
            string adventurerName = DataManager.Instance.Wave_Table[i]["adventure"].ToString();
            int number = Convert.ToInt32(DataManager.Instance.Wave_Table[i]["num"]);
            WaveData waveData = new WaveData(adventurerName, number);
            curWave.Add(waveData);
        }

        return curWave;
    }

    private void CalculateWaves(List<WaveData> waveData)
    {
        List<string> targets = new List<string>();
        foreach (WaveData data in waveData)
            for (int i = 0; i < data.number; i++)
                targets.Add(data.adventurerName);
        foreach (WaveData data in PassiveManager.Instance.adventurerRaiseTable)
            for (int i = 0; i < data.number; i++)
                targets.Add(data.adventurerName);

        for (int i = 0; i < targets.Count; i++)
            curWaves.Add(new SpawnData(curWaves.Count + 1, targets[i]));
    }

    public IEnumerator ISpawnWave(int waveIndex, System.Action callback = null)
    {
        float spawnWaitTime = CalSpawnWaitTime(curWaves.Count);

        waveText.text = (waveIndex + (GameManager.Instance.loop * DataManager.Instance.WaveLevelTable.Count) + 1).ToString("D2");
        waveFill?.SetWaveGauge(waveIndex, 0, curWaves.Count);
        float curTime = GameManager.Instance.Timer;
        foreach(SpawnData data in curWaves)
        {
            float targetTime = data.order * spawnWaitTime;
            if (targetTime < curTime)
                continue;

            waveFill?.SetWaveGauge(waveIndex, data.order - 1, curWaves.Count);
            while (curTime < targetTime)
            {
                curTime = GameManager.Instance.Timer;
                yield return null;
            }

            BattlerPooling.Instance.SpawnAdventurer(data.target);
        }

        waveFill?.SetWaveGauge(waveIndex, curWaves.Count, curWaves.Count);
        curWaves = new List<SpawnData>();

        callback?.Invoke();
        yield return null;
    }

    public void SpawnWave(int curWave)
    {
        List<WaveData> waveData = new List<WaveData>();
        if (curWave >= 0 && curWave < DataManager.Instance.WaveLevelTable.Count)
            waveData = new List<WaveData>(DataManager.Instance.WaveLevelTable[curWave]);

        CalculateWaves(waveData);
        if (curWaves.Count == 0)
            return;

        if (curWave + 1 >= DataManager.Instance.WaveLevelTable.Count)
            StartCoroutine(ISpawnWave(curWave, () => { GameManager.Instance.AllWaveSpawned = true; }));
        else
            StartCoroutine(ISpawnWave(curWave));
    }
}
