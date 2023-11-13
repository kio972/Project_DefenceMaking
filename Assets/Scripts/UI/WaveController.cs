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

public class WaveController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI waveText;
    [SerializeField]
    private WaveGauge waveFill;

    public float WaveProgress { get { return waveFill.WaveRate; } }

    private float CalSpawnWaitTime(int allAmount, float restrictTime = 720f)
    {
        float spawnTime = 2f;
        restrictTime = 720 / (GameManager.Instance.DefaultSpeed);
        if (allAmount * spawnTime > restrictTime)
        {
            spawnTime = restrictTime / allAmount;
        }
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

    public IEnumerator ISpawnWave(int waveIndex, List<WaveData> curWave, System.Action callback = null)
    {
        List<WaveData> waves = new List<WaveData>(PassiveManager.Instance.adventurerRaiseTable);
        foreach(WaveData wave in curWave)
            waves.Add(wave);

        waveText.text = (waveIndex + (GameManager.Instance.loop * DataManager.Instance.WaveLevelTable.Count) + 1).ToString("D2");
        int maxEnemyNumber = 0;
        int curEnemyNumber = 0;
        foreach(WaveData waveData in waves)
            maxEnemyNumber += waveData.number;
        waveFill.SetWaveGauge(waveIndex, curEnemyNumber, maxEnemyNumber);
        float spawnWaitTime = CalSpawnWaitTime(maxEnemyNumber);
        foreach (WaveData waveData in waves)
        {
            for(int i = 0; i < waveData.number; i++)
            {
                float elapsedTime = 0f;
                while (elapsedTime < spawnWaitTime)
                {
                    elapsedTime += Time.deltaTime * GameManager.Instance.timeScale;
                    yield return null;
                }

                //모험가 스폰
                AdventurerPooling.Instance.SpawnAdventurer(waveData.adventurerName);

                curEnemyNumber++;

                waveFill.SetWaveGauge(waveIndex, curEnemyNumber, maxEnemyNumber);
            }
        }

        callback?.Invoke();
        yield return null;
    }

    public void SpawnWave(int curWave)
    {
        List<WaveData> waveData = SetWaveData(curWave);
        if (curWave < 0 || curWave >= DataManager.Instance.WaveLevelTable.Count)
            return;

        if (curWave + 1 >= DataManager.Instance.WaveLevelTable.Count)
            StartCoroutine(ISpawnWave(curWave, DataManager.Instance.WaveLevelTable[curWave], () => { GameManager.Instance.AllWaveSpawned = true; }));
        else
            StartCoroutine(ISpawnWave(curWave, DataManager.Instance.WaveLevelTable[curWave]));
    }

    public void Init()
    {

    }
}
