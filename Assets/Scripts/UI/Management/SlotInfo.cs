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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
