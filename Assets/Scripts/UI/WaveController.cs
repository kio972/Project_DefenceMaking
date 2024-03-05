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

    private List<string> delayedTargets = new List<string>();

    public bool HaveDelayedTarget { get { return delayedTargets.Count != 0; } }

    public float WaveProgress { get { return waveFill.WaveRate; } }

    public void AddDelayedTarget(string adventurerName)
    {
        delayedTargets.Add(adventurerName);
    }

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
        //List<Dictionary<string, object>> wave_Table����
        //key���� "level"�� �ε����� ã��
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
        waveFill?.SetWaveGauge(waveIndex, curEnemyNumber, maxEnemyNumber);
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

                //���谡 ����
                BattlerPooling.Instance.SpawnAdventurer(waveData.adventurerName);

                curEnemyNumber++;

                waveFill?.SetWaveGauge(waveIndex, curEnemyNumber, maxEnemyNumber);
            }
        }

        callback?.Invoke();
        yield return null;
    }

    public void SpawnWave(int curWave)
    {
        List<WaveData> waveData = new List<WaveData>();
        if (curWave >= 0 && curWave < DataManager.Instance.WaveLevelTable.Count)
            waveData = new List<WaveData>(DataManager.Instance.WaveLevelTable[curWave]);
        else
            curWave--;
        if(delayedTargets.Count > 0)
        {
            foreach(string targetName in delayedTargets)
                waveData.Add(new WaveData(targetName, 1));
            delayedTargets.Clear();
        }

        if (waveData.Count == 0)
            return;

        if (curWave + 1 >= DataManager.Instance.WaveLevelTable.Count)
            StartCoroutine(ISpawnWave(curWave, waveData, () => { GameManager.Instance.AllWaveSpawned = true; }));
        else
            StartCoroutine(ISpawnWave(curWave, waveData));
    }

    public void Init()
    {

    }
}
