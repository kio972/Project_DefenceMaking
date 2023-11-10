using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSlot : MonoBehaviour
{
    private string id = "";
    private bool isTrap = false;

    private int minDamage;
    private int maxDamage;

    private int hp;
    private int defense;

    private int duration;
    private int maxTarget;

    private int cost;

    [SerializeField]
    private CardType cardType;
    private Button infoBtn;

    [SerializeField]
    private Button deployBtn;

    private ManagementUI managementUI;

    [SerializeField]
    private string prefabName;

    public void Deploy()
    {
        if(managementUI == null)
            managementUI = GetComponentInParent<ManagementUI>();

        managementUI.DeployReady(cardType, prefabName);
    }

    public void Init(Dictionary<string,object> data)
    {

    }
}
