using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    [SerializeField]
    private Image card_Frame;
    [SerializeField]
    private Image card_Frame_Mask;
    [SerializeField]
    private Image card_illust;
    [SerializeField]
    private Image card_Rank;
    [SerializeField]
    private LanguageText card_Name;
    [SerializeField]
    private LanguageText card_Description;

    [SerializeField]
    private TextMeshProUGUI card_Damage;
    [SerializeField]
    private TextMeshProUGUI card_Hp;
    [SerializeField]
    private TextMeshProUGUI card_Defense;
    [SerializeField]
    private TextMeshProUGUI card_Duration;
    [SerializeField]
    private TextMeshProUGUI card_maxTarget;

    [SerializeField]
    private GameObject monsterInfo;
    [SerializeField]
    private GameObject trapInfo;

    private enum UnitType
    {
        Monster,
        Trap,
    }

    public void UpdateInfo(Dictionary<string, object> data)
    {
        string id = data["id"].ToString();


    }

    public void UpdateInfo(string id)
    {
        int index = UtilHelper.Find_Data_Index(id, DataManager.Instance.Battler_Table, "id");
        UpdateInfo(DataManager.Instance.Battler_Table[index]);
    }
}
