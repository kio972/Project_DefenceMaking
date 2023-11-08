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


    private Button infoBtn;

    [SerializeField]
    private Button deployBtn;



    public void Init(Dictionary<string,object> data)
    {

    }
}
