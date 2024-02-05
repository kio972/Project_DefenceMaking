using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HurbUpdater3 : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI targetText;


    // Update is called once per frame
    void Update()
    {
        targetText.text = GameManager.Instance.hurb3.ToString();
    }
}
