using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gold;

    public bool isShortenOn = false;

    // Update is called once per frame
    void Update()
    {
        if(!isShortenOn)
            gold.text = GameManager.Instance.gold.ToString();
        else
        {
            float curGold = GameManager.Instance.gold;
            if (curGold >= 1000000)
                gold.text = (Mathf.Floor(curGold / 100000) / 10).ToString() + "M";
            else if(curGold >= 10000)
                gold.text = (Mathf.Floor(curGold / 100) / 10).ToString() + "K";
            else
                gold.text = curGold.ToString();
        }
    }
}
