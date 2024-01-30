using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FluctTimer : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private int curTime = 0;
    [SerializeField]
    private int fluctTime = 10;

    private int curWave;
    [SerializeField]
    private TextMeshProUGUI text;

    private void Start()
    {
        curWave = GameManager.Instance.CurWave;
    }

    private void FluctPrice()
    {
        if (target == null)
            return;

        target.BroadcastMessage("FluctPrice", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        if (curWave == GameManager.Instance.CurWave)
            return;

        curTime += GameManager.Instance.CurWave - curWave;
        curWave = GameManager.Instance.CurWave;

        if(curTime >= fluctTime)
        {
            FluctPrice();
            curTime = 0;
        }

        if(text != null)
            text.text = (fluctTime - curTime).ToString();
    }
}