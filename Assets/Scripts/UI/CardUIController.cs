using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIController : MonoBehaviour
{
    [SerializeField]
    private Image card_illust;
    [SerializeField]
    private Image card_Frame;
    [SerializeField]
    private Image card_Rank;
    [SerializeField]
    private TextMeshProUGUI card_Name;
    [SerializeField]
    private TextMeshProUGUI card_Description;

    public void Init(TileType tileType, string targetPrefabName)
    {

    }
}
