using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveGauge : MonoBehaviour
{
    [SerializeField]
    private Image fillImg;

    private float waveRate = 0;

    [SerializeField]
    private float lerpTime = 1f;

    private float elapsedTime = 0f;
    private float curFillRate = 0;

    private Coroutine coroutine = null;

    public void SetWaveGauge(int curWave, int curSpawnIndex, int maxSpawnIndex)
    {
        if (curWave != GameManager.Instance.CurWave || maxSpawnIndex == 0)
            return;

        float curIndex = curSpawnIndex;
        float maxIndex = maxSpawnIndex;
        waveRate = curIndex / maxIndex;
        waveRate = Mathf.Clamp(waveRate, 0f, 1f);
        elapsedTime = 0f;

        if(fillImg != null)
            curFillRate = fillImg.fillAmount;
    }


    public void Update()
    {
        if (fillImg == null)
            return;

        if(curFillRate > waveRate)
        {
            fillImg.fillAmount = waveRate;
            curFillRate = fillImg.fillAmount;
            return;
        }
        else
        {
            if (elapsedTime > lerpTime)
            {
                fillImg.fillAmount = waveRate;
                curFillRate = fillImg.fillAmount;
                return;
            }

            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            float currentFillAmount = Mathf.Lerp(curFillRate, waveRate, t);
            fillImg.fillAmount = currentFillAmount;
            elapsedTime += Time.deltaTime * GameManager.Instance.timeScale;
        }
    }
}
