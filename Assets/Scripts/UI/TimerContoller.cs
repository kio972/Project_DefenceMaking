using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerContoller : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timer;

    private string ConvertTimeToString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string timeString = minutes.ToString("00") + ":" + seconds.ToString("00");
        return timeString;
    }

    private void SetTimer()
    {
        float time = GameManager.Instance.Timer;
        timer.text = ConvertTimeToString(time);
    }

    // Update is called once per frame
    void Update()
    {
        SetTimer();
    }
}
