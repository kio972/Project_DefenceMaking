using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gold;


    // Update is called once per frame
    void Update()
    {
        gold.text = GameManager.Instance.gold.ToString();
    }
}
