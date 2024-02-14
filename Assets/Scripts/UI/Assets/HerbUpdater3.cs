using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HerbUpdater3 : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI targetText;


    // Update is called once per frame
    void Update()
    {
        targetText.text = GameManager.Instance.herb3.ToString();
    }
}
