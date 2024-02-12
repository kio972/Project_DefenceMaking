using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResearchData
{
    public string researchName;
    public string researchDesc;
    public float requiredTime;
    public int requiredMoney;
    public int requiredHurb1;
    public int requiredHurb2;
    public int requiredHurb3;

    public ResearchData(string id)
    {
        int index = UtilHelper.Find_Data_Index(id, DataManager.Instance.Research_Table, "id");
        Dictionary<string, object> data = DataManager.Instance.Research_Table[index];

        researchName = data["Name"].ToString();
        researchDesc = data["Content"].ToString();
        requiredTime = float.Parse(data["RequiredTime"].ToString());
        requiredMoney = Convert.ToInt32(data["RequiredMoney"]);
        requiredHurb1 = Convert.ToInt32(data["RequiredHerb1"]);
        requiredHurb2 = Convert.ToInt32(data["RequiredHerb2"]);
        requiredHurb3 = Convert.ToInt32(data["RequiredHerb3"]);
    }
}

public interface Research
{
    public void ActiveResearch();
}
