using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResearchData
{
    public int researchIndex;
    public string researchName;
    public string researchDesc;
    public float requiredTime;
    public int requiredMoney;
    public int requiredherb1;
    public int requiredherb2;
    public int requiredherb3;

    public ResearchData(string id)
    {
        researchIndex = UtilHelper.Find_Data_Index(id, DataManager.Instance.Research_Table, "id");
        Dictionary<string, object> data = DataManager.Instance.Research_Table[researchIndex];

        researchName = data["Name"].ToString();
        researchDesc = data["Content"].ToString();
        requiredTime = float.Parse(data["RequiredTime"].ToString());
        requiredMoney = Convert.ToInt32(data["RequiredMoney"]);
        requiredherb1 = Convert.ToInt32(data["RequiredHerb1"]);
        requiredherb2 = Convert.ToInt32(data["RequiredHerb2"]);
        requiredherb3 = Convert.ToInt32(data["RequiredHerb3"]);
    }
}

public interface Research
{
    public void ActiveResearch();
}
